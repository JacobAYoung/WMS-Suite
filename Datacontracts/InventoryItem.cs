namespace WMS_Suite.DataContracts
{
    public class InventoryItem
    {
        public int Id { get; set; } // Primary key
        public string Sku { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }
    }
}
