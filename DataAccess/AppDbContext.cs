using Microsoft.EntityFrameworkCore;
using WMS_Suite.DataContracts;

namespace WMS_Suite.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<SalesHistory> SalesHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=inventory.db"); // Local file DB
        }
    }
}
