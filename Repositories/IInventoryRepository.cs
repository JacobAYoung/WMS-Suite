using WMS_Suite.DataContracts;
using WMS_Suite.Models;

namespace WMS_Suite.Repositories
{
    public interface IInventoryRepository
    {
        Task<List<InventoryItem>> GetAllAsync();
        Task AddAsync(InventoryItem item);
        Task UpdateAsync(InventoryItem item);
        // Add Delete if needed

        Task AddSalesHistoryAsync(SalesHistory history);
        Task<List<SalesHistory>> GetSalesForItemAsync(int itemId);

        Task<AppSettings> GetSettingsAsync();
        Task SaveSettingsAsync(AppSettings settings);
    }
}
