namespace UTB.Minute.Db
{
    public class MenuItem
    {
        public int Id { get; set; }
        public required DateTime Date { get; set; }
        public required int Portions { get; set; }
        //minutemealsid
        //objednávka bude mít jeden menuitem

    }
}
