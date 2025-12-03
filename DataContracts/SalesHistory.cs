namespace WMS_Suite.DataContracts
{
    public class SalesHistory
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; } // FK to item
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public int SoldQuantity { get; set; }
    }
}
