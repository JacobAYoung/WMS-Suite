using WMS_Suite.DataContracts;

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
    }
}
