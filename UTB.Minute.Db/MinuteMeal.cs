namespace UTB.Minute.Db
{
    public class MinuteMeal
    {
        public int Id { get; set; }
        public required string Desc { get; set; }
        public required double Price { get; set; }

        public List<MenuItem> MenuItems { get; set; } = [];
        // Zde bude kolekce menuItems (1:N)
    }
}
