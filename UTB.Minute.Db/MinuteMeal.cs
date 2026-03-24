namespace UTB.Minute.Db
{
    public class MinuteMeal
    {
        public int Id { get; set; }
        public required string Desc { get; set; }
        public required double Price { get; set; }

        // TENTO ŘÁDEK TAM URČITĚ DOPLŇ (Soft-delete):
        public bool IsActive { get; set; } = true;

        public List<MenuItem> MenuItems { get; set; } = [];
    }
}