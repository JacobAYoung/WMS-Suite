using WMS_Suite.DataContracts;
using WMS_Suite.Repositories;

namespace WMS_Suite.Services
{
    public interface IForecastService
    {
        Task<string> GetForecastAsync(InventoryItem item, IInventoryRepository repo);
    }
}
