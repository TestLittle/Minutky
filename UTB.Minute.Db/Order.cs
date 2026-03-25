namespace UTB.Minute.Db

{
    public class Order
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public int MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}
