using System.Windows;
using WMS_Suite.DataAccess;
using WMS_Suite.Repositories;
using WMS_Suite.ViewModels;

namespace WMS_Suite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = new AppDbContext();
            var repo = new InventoryRepository(context);
            DataContext = new InventoryViewModel(repo);
        }
    }
}