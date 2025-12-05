using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS_Suite.DataContracts;
using WMS_Suite.Repositories;

namespace WMS_Suite.Services
{
    public interface IShopifyService
    {
        public interface IShopifyService
        {
            Task SyncInventoryAsync(List<InventoryItem> localItems, IInventoryRepository repo, Action<string> logMismatch);
            Task<List<SalesHistory>> FetchRecentSalesAsync(int itemId); // Still stub
        }
    }
}
