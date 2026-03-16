using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/reset-db", async (MinuteContext context) =>
{
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
    
    MinuteMeal cheese = new() { Desc = "Smazeny syr s hranolkami", Price = 124.99};
    MinuteMeal hamburger = new() { Desc = "Hamburger", Price = 139.99 };
    MinuteMeal hamPizza = new() { Desc = "Sunkova Pizza", Price = 109.99 };

    MenuItem m1 = new MenuItem { Date = new DateTime(new DateOnly(2026, 3, 17), new TimeOnly(12, 25)), Portions = 12, MinuteMeal = cheese };
    MenuItem m2 = new MenuItem { Date = new DateTime(new DateOnly(2026, 3, 17), new TimeOnly(12, 25)), Portions = 15, MinuteMeal = hamburger };
    MenuItem m3 = new MenuItem { Date = new DateTime(new DateOnly(2026, 3, 17), new TimeOnly(12, 25)), Portions = 20, MinuteMeal = hamPizza };

    Order o1 = new Order { MenuItem = m1, OrderStatus = OrderStatus.Cancelled };

    await context.MinuteMeals.AddRangeAsync(cheese, hamburger, hamPizza);
    await context.MenuItems.AddRangeAsync(m1, m2, m3);
    await context.Orders.AddRangeAsync(o1);
    
    await context.SaveChangesAsync();
});


app.UseHttpsRedirection();

app.Run();
