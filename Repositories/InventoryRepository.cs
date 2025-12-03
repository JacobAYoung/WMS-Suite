using Microsoft.EntityFrameworkCore;
using WMS_Suite.DataAccess;
using WMS_Suite.DataContracts;

namespace WMS_Suite.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryItem>> GetAllAsync()
        {
            return await _context.InventoryItems.ToListAsync();
        }

        public async Task AddAsync(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddSalesHistoryAsync(SalesHistory history)
        {
            _context.SalesHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SalesHistory>> GetSalesForItemAsync(int itemId)
        {
            return await _context.SalesHistories.Where(h => h.InventoryItemId == itemId).ToListAsync();
        }
    }
}
