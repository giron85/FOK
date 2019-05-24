namespace Reti.Lab.FoodOnKontainers.DTO
{
    public class OrderItem
    {
        public int IdOrder { get; set; }
        public int IdMenuItem { get; set; }
        public string MenuItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
