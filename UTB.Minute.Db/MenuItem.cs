namespace UTB.Minute.Db
{
    public class MenuItem
    {
        public int Id { get; set; }
        public required DateTime Date { get; set; }
        public required int Portions { get; set; }

        public int MinuteMealId { get; set; }
        public MinuteMeal? MinuteMeal { get; set; }

        public List<Order> Orders { get; set; } = [];
    }
}
