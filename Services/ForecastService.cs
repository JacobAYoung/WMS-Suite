using WMS_Suite.DataContracts;
using WMS_Suite.Repositories;

namespace WMS_Suite.Services
{
    public class ForecastService : IForecastService
    {
        public async Task<string> GetForecastAsync(InventoryItem item, IInventoryRepository repo)
        {
            var history = await repo.GetSalesForItemAsync(item.Id);
            var recent = history.Where(h => h.SaleDate > DateTime.UtcNow.AddDays(-30)).ToList();
            if (!recent.Any()) return "No sales data yet.";

            double avgDaily = recent.Sum(h => h.SoldQuantity) / 30.0; // Approximate
            int daysLeft = avgDaily > 0 ? (int)(item.Quantity / avgDaily) : int.MaxValue;
            return daysLeft < 5 ? $"Reorder soon: {daysLeft} days left at current rate." : $"Stock good: {daysLeft} days left.";
        }
    }
}
