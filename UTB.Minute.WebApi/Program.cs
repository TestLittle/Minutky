using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();
app.MapDefaultEndpoints();

// --- MAPOVÁNÍ ---
app.MapGet("/minuteMeals", WebApiVersion1.GetAllMinuteMeals);
app.MapPost("/minuteMeals", WebApiVersion1.CreateMinuteMeal);
app.MapDelete("/minuteMeals/{id}", WebApiVersion1.DeleteMinuteMeal);
app.MapPut("/minuteMeals/{id}", WebApiVersion1.PutMinuteMeal);
app.MapPatch("/minuteMeals/{id}/desc", WebApiVersion1.PatchDescMinuteMeal);
app.MapPatch("/minuteMeals/{id}/price", WebApiVersion1.PatchPriceMinuteMeal);

app.MapGet("/menu", WebApiVersion1.GetAllMenuItems);
app.MapPost("/menu", WebApiVersion1.CreateMenuItem);
app.MapDelete("/menu/{id}", WebApiVersion1.DeleteMenuItem);
app.MapPut("/menu/{id}", WebApiVersion1.PutMenuItem);

app.MapGet("/orders", WebApiVersion1.GetAllOrders);
app.MapPost("/orders", WebApiVersion1.CreateOrder);
app.MapPatch("/orders/{id}/status", WebApiVersion1.UpdateOrderStatus);

app.Run();

public static class WebApiVersion1
{
    public static async Task<Ok<MinuteMealDto[]>> GetAllMinuteMeals(MinuteContext context) =>
        TypedResults.Ok(await context.MinuteMeals.Where(m => m.IsActive).Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price)).ToArrayAsync());

    public static async Task<Created<MinuteMealDto>> CreateMinuteMeal(MinuteMealRequestDto req, MinuteContext db)
    {
        var meal = new MinuteMeal { Desc = req.Desc, Price = req.Price, IsActive = true };
        db.MinuteMeals.Add(meal);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/minuteMeals/{meal.Id}", new MinuteMealDto(meal.Id, meal.Desc, meal.Price));
    }

    public static async Task<Results<Created<MenuItemDto>, BadRequest<string>>> CreateMenuItem(MenuItemDto dto, MinuteContext db)
    {
        var meal = await db.MinuteMeals.FirstOrDefaultAsync(m => m.IsActive);
        if (meal == null) return TypedResults.BadRequest("Zadne aktivni jidlo neexistuje.");

        var item = new MenuItem { Date = dto.Date, Portions = dto.Portions, MinuteMealId = meal.Id };
        db.MenuItems.Add(item);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/menu/{item.Id}", new MenuItemDto(item.Id, item.Date, item.Portions));
    }

    public static async Task<Results<NoContent, NotFound>> DeleteMinuteMeal(int id, MinuteContext db)
    {
        var meal = await db.MinuteMeals.FindAsync(id);
        if (meal is null) return TypedResults.NotFound();
        meal.IsActive = false;
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> PutMinuteMeal(int id, MinuteMealRequestDto req, MinuteContext db)
    {
        var meal = await db.MinuteMeals.FindAsync(id);
        if (meal is null) return TypedResults.NotFound();
        meal.Desc = req.Desc; meal.Price = req.Price;
        await db.SaveChangesAsync(); return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> PatchDescMinuteMeal(int id, MinuteMealPatchDescDto patch, MinuteContext db)
    {
        var meal = await db.MinuteMeals.FindAsync(id);
        if (meal is null) return TypedResults.NotFound();
        meal.Desc = patch.Desc; await db.SaveChangesAsync(); return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> PatchPriceMinuteMeal(int id, MinuteMealPatchPriceDto patch, MinuteContext db)
    {
        var meal = await db.MinuteMeals.FindAsync(id);
        if (meal is null) return TypedResults.NotFound();
        meal.Price = patch.Price; await db.SaveChangesAsync(); return TypedResults.NoContent();
    }

    public static async Task<Ok<MenuItemDto[]>> GetAllMenuItems(MinuteContext db) =>
        TypedResults.Ok(await db.MenuItems.Select(m => new MenuItemDto(m.Id, m.Date, m.Portions)).ToArrayAsync());

    public static async Task<Results<NoContent, NotFound>> DeleteMenuItem(int id, MinuteContext db)
    {
        var item = await db.MenuItems.FindAsync(id);
        if (item is null) return TypedResults.NotFound();
        db.MenuItems.Remove(item); await db.SaveChangesAsync(); return TypedResults.NoContent();
    }
    public static async Task<Results<NoContent, NotFound>> PutMenuItem(int id, MenuItemDto req, MinuteContext db)
    {
        var item = await db.MenuItems.FindAsync(id);
        if (item is null) return TypedResults.NotFound();

        // Upravíme datum a porce
        item.Date = req.Date;
        item.Portions = req.Portions;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<Ok<OrderDto[]>> GetAllOrders(MinuteContext db) =>
        TypedResults.Ok(await db.Orders.Select(o => new OrderDto(o.Id)).ToArrayAsync());

    public static async Task<Results<Created<OrderDto>, BadRequest<string>, Conflict<string>>> CreateOrder(int menuItemId, MinuteContext db)
    {
        var item = await db.MenuItems.FindAsync(menuItemId);
        if (item == null || item.Portions <= 0) return TypedResults.BadRequest("Sold out.");
        item.Portions--;
        var order = new Order { MenuItemId = menuItemId, OrderStatus = OrderStatus.Preparing };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/orders/{order.Id}", new OrderDto(order.Id));
    }


    public static async Task<Results<NoContent, NotFound>> UpdateOrderStatus(int id, OrderStatus status, MinuteContext db)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) return TypedResults.NotFound();
        order.OrderStatus = status;
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}