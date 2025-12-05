using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS_Suite.Models
{
    public class AppSettings
    {
        public int Id { get; set; } = 1; // Singleton
        public string ShopifyStoreUrl { get; set; } // e.g., "mystorename.myshopify.com"
        public string ShopifyAccessToken { get; set; }
    }
}
