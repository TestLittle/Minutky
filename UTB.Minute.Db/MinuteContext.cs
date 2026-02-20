using Microsoft.EntityFrameworkCore;

namespace UTB.Minute.Db
{
    public class MinuteContext(DbContextOptions<MinuteContext> options) : DbContext(options)
    {
        public DbSet<MinuteMeal> MinuteMeals { get; set; }
    }
}
