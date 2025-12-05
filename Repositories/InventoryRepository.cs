using Microsoft.EntityFrameworkCore;
using WMS_Suite.DataAccess;
using WMS_Suite.DataContracts;
using WMS_Suite.Models;

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

        public async Task<AppSettings> GetSettingsAsync()
        {
            return await _context.AppSettings.FirstOrDefaultAsync() ?? new AppSettings();
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            var existing = await _context.AppSettings.FindAsync(settings.Id);
            if (existing == null)
            {
                _context.AppSettings.Add(settings);
            }
            else
            {
                // Copy properties to tracked entity
                existing.ShopifyStoreUrl = settings.ShopifyStoreUrl;
                existing.ShopifyAccessToken = settings.ShopifyAccessToken;
                _context.AppSettings.Update(existing);
            }
            await _context.SaveChangesAsync();
        }
    }
}
