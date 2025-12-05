using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WMS_Suite.DataContracts;
using WMS_Suite.Repositories;
using WMS_Suite.Services;
using WMS_Suite.Views;

namespace WMS_Suite.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        private readonly IInventoryRepository _repository;
        private ObservableCollection<InventoryItem> _items;
        private InventoryItem _selectedItem;

        public ObservableCollection<InventoryItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public InventoryItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private string _forecast;
        public string Forecast
        {
            get => _forecast;
            set => SetProperty(ref _forecast, value);
        }
        private string _mismatches;
        public string Mismatches
        {
            get => _mismatches;
            set => SetProperty(ref _mismatches, value);
        }

        public ICommand SyncShopifyCommand { get; }
        public ICommand EditSettingsCommand { get; }
        public ICommand GetForecastCommand { get; }

        public ICommand LoadItemsCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand UpdateItemCommand { get; }
        public ICommand LogSaleCommand { get; }
        public ICommand GenerateBarcodeCommand { get; }

        public InventoryViewModel(IInventoryRepository repository)
        {
            _repository = repository;
            Items = new ObservableCollection<InventoryItem>();

            LoadItemsCommand = new RelayCommand(async _ => await LoadItemsAsync());
            AddItemCommand = new RelayCommand(AddItem);
            UpdateItemCommand = new RelayCommand(UpdateItem, CanUpdateItem);

            GetForecastCommand = new RelayCommand(async _ => await GetForecastAsync(), _ => SelectedItem != null);

            LogSaleCommand = new RelayCommand(async _ => await LogSaleAsync(), _ => SelectedItem != null);

            GenerateBarcodeCommand = new RelayCommand(GenerateBarcode, _ => SelectedItem != null);

            SyncShopifyCommand = new RelayCommand(async _ => await SyncShopifyAsync());
            EditSettingsCommand = new RelayCommand(async _ => await EditSettingsAsync());
        }

        private async Task SyncShopifyAsync()
        {
            var settings = await _repository.GetSettingsAsync();
            if (string.IsNullOrEmpty(settings.ShopifyAccessToken))
            {
                MessageBox.Show("Set Shopify credentials first.");
                return;
            }

            Mismatches = ""; // Clear
            var service = new ShopifyService(settings.ShopifyStoreUrl, settings.ShopifyAccessToken);
            await service.SyncInventoryAsync(Items.ToList(), _repository, mismatch => Mismatches += mismatch + "\n");

            // Refresh list after sync
            await LoadItemsAsync();

            if (SelectedItem != null) await GetForecastAsync();
        }

        private async Task EditSettingsAsync()
        {
            var settings = await _repository.GetSettingsAsync();
            settings.ShopifyStoreUrl = Microsoft.VisualBasic.Interaction.InputBox("Store URL (e.g., mystore.myshopify.com):", DefaultResponse: settings.ShopifyStoreUrl ?? "");
            settings.ShopifyAccessToken = Microsoft.VisualBasic.Interaction.InputBox("Access Token:", DefaultResponse: settings.ShopifyAccessToken ?? "");
            await _repository.SaveSettingsAsync(settings);
        }

        private void GenerateBarcode(object parameter)
        {
            if (SelectedItem == null) return;

            var label = Microsoft.VisualBasic.Interaction.InputBox("Label from (Name/SKU/Desc):", DefaultResponse: "SKU");
            var data = label switch
            {
                "Name" => SelectedItem.Name,
                "Desc" => SelectedItem.Description ?? SelectedItem.Sku,
                _ => SelectedItem.Sku
            };

            var service = new BarcodeService(); // Or inject
            var barcodeImg = service.GenerateBarcode(data, label);

            var barcodeWindow = new BarcodeWindow(barcodeImg);
            barcodeWindow.ShowDialog();
        }

        private async Task LogSaleAsync()
        {
            if (SelectedItem == null) return;

            var soldQty = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("Sold Quantity:") ?? "0");
            var saleDate = DateTime.Parse(Microsoft.VisualBasic.Interaction.InputBox("Sale Date (YYYY-MM-DD):", DefaultResponse: DateTime.UtcNow.ToString("yyyy-MM-dd")) ?? DateTime.UtcNow.ToString("yyyy-MM-dd"));

            var history = new SalesHistory { InventoryItemId = SelectedItem.Id, SoldQuantity = soldQty, SaleDate = saleDate };
            await _repository.AddSalesHistoryAsync(history);

            // Optional: Update quantity if simulating a sale
            SelectedItem.Quantity -= soldQty;
            await _repository.UpdateAsync(SelectedItem);

            await GetForecastAsync(); // Refresh forecast immediately
        }

        private async Task GetForecastAsync()
        {
            if (SelectedItem == null) return;
            var service = new ForecastService(); // Or inject
            Forecast = await service.GetForecastAsync(SelectedItem, _repository);
        }

        private async Task LoadItemsAsync()
        {
            var items = await _repository.GetAllAsync();
            Items.Clear();
            foreach (var item in items) Items.Add(item);
        }

        private void AddItem(object parameter)
        {
            // Simple example: Prompt for input (replace with a dialog later)
            var sku = Microsoft.VisualBasic.Interaction.InputBox("Enter SKU:");
            var name = Microsoft.VisualBasic.Interaction.InputBox("Enter Name:");
            var qty = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("Enter Quantity:") ?? "0");

            var newItem = new InventoryItem { Sku = sku, Name = name, Quantity = qty };
            _repository.AddAsync(newItem).Wait(); // Sync for simplicity; async later
            Items.Add(newItem);
        }

        private void UpdateItem(object parameter)
        {
            if (SelectedItem == null) return;
            var newQty = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("New Quantity:", DefaultResponse: SelectedItem.Quantity.ToString()) ?? "0");
            SelectedItem.Quantity = newQty;
            _repository.UpdateAsync(SelectedItem).Wait();
            OnPropertyChanged(nameof(Items)); // Refresh binding if needed
        }

        private bool CanUpdateItem(object parameter) => SelectedItem != null;
    }

    // Simple ICommand impl (add to a Commands folder or inline)
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
    }
}
