using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/minuteMeals", WebApiVersion1.GetAllMinuteMeals);
app.MapGet("/minuteMeals/active", WebApiVersion1.GetAllActiveMinuteMeals);
app.MapPost("/minuteMeals", WebApiVersion1.CreateMinuteMeal);
app.MapPatch("/minuteMeals/{id}/active", WebApiVersion1.ChangeActiveStatusMinuteMeal);
app.MapPut("/minuteMeals/{id}", WebApiVersion1.PutMinuteMeal);
app.MapPatch("/minuteMeals/{id}/desc", WebApiVersion1.PatchDescMinuteMeal);
app.MapPatch("/minuteMeals/{id}/price", WebApiVersion1.PatchPriceMinuteMeal);

app.MapPost("/menuItems", WebApiVersion1.CreateMenuItem);
app.MapGet("/menuItems", WebApiVersion1.GetAllMenuItems);
app.MapDelete("/menuItems/{id}", WebApiVersion1.DeleteMenuItem);
app.MapPatch("/menuItems/{id}/date", WebApiVersion1.PatchMenuItemDate);
app.MapPatch("/menuItems/{id}/portions", WebApiVersion1.PatchMenuItemPortions);
app.MapPut("/menuItems/{id}", WebApiVersion1.PutMenuItem);
app.MapPatch("/menuItems/{id}/portion", WebApiVersion1.DecreaseNumberOfPortions);
app.MapPatch("/menuItems/{id}/meal", WebApiVersion1.ChangeMenuItemMeal);

app.MapGet("/orders", WebApiVersion1.GetAllOrders);
app.MapPost("/orders", WebApiVersion1.CreateOrder);
app.MapPut("/orders/{id}", WebApiVersion1.PutOrder);
app.MapPatch("/orders/{id}/status", WebApiVersion1.OrderChangeStatus);
app.MapPatch("/orders/{id}/menuItem", WebApiVersion1.PatchOrderMenuItem);



app.UseHttpsRedirection();

app.Run();

public static class WebApiVersion1
{
    public static async Task<Ok<MinuteMealDto[]>> GetAllMinuteMeals(MinuteContext context)
    {
        var meals = await context.MinuteMeals.Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price, m.IsActive)).ToArrayAsync();

        return TypedResults.Ok(meals);
    }

    public static async Task<Ok<MinuteMealDto[]>> GetAllActiveMinuteMeals(MinuteContext context)
    {
        var query = context.MinuteMeals.AsQueryable();
        MinuteMealDto[] meals = await query.Where(m => m.IsActive == true).Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price, m.IsActive)).ToArrayAsync();

        return TypedResults.Ok(meals);
    }

    public static async Task<Created<MinuteMealDto>> CreateMinuteMeal(MinuteMealRequestDto request, MinuteContext context)
    {
        var meal = new MinuteMeal { Desc = request.Desc, Price = request.Price, IsActive = request.IsActive};

        context.MinuteMeals.Add(meal);
        await context.SaveChangesAsync();

        MinuteMealDto mealDto = new MinuteMealDto(meal.Id, meal.Desc, meal.Price, meal.IsActive);
        return TypedResults.Created($"/minuteMeals/{meal.Id}", mealDto);
    }

    public static async Task<Results<NoContent, NotFound>> ChangeActiveStatusMinuteMeal(int id, MinuteMealPatchIsActiveDto patch, MinuteContext context)
    {
        if(await context.MinuteMeals.FindAsync(id) is MinuteMeal meal)
        {
            meal.IsActive = patch.IsActive;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> PutMinuteMeal(int id, MinuteMealRequestDto request, MinuteContext context)
    {
        if(await context.MinuteMeals.FindAsync(id) is MinuteMeal meal)
        {
            meal.Desc = request.Desc;
            meal.Price = request.Price;
            meal.IsActive = request.IsActive;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }


    public static async Task<Results<NoContent, NotFound>> PatchDescMinuteMeal(int id, MinuteMealPatchDescDto patch, MinuteContext context)
    {
        if(await context.MinuteMeals.FindAsync(id) is MinuteMeal meal)
        {
            meal.Desc = patch.Desc;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> PatchPriceMinuteMeal(int id, MinuteMealPatchPriceDto patch, MinuteContext context)
    {
        if(await context.MinuteMeals.FindAsync(id) is MinuteMeal meal)
        {
            meal.Price = patch.Price;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<Created<MenuItemDto>, BadRequest<string>>> CreateMenuItem(MenuItemRequestDto request, MinuteContext context)
    {
        var meal = await context.MinuteMeals.FirstOrDefaultAsync(m => m.IsActive);
        if (meal == null) return TypedResults.BadRequest("Zadne aktivni jidlo neexistuje.");

        MenuItem menuItem = new MenuItem() { Date = request.Date, Portions = request.Portions, MinuteMealId = request.MinuteMealId};
        context.MenuItems.Add(menuItem);
        await context.SaveChangesAsync();
        return TypedResults.Created($"/menuItems/{menuItem.Id}", new MenuItemDto(menuItem.Id, menuItem.Date, menuItem.Portions, menuItem.MinuteMealId));
    }

    public static async Task<Ok<MenuItemDto[]>> GetAllMenuItems(MinuteContext context)
    {
        var menuItems = await context.MenuItems.Select(mi => new MenuItemDto(mi.Id, mi.Date, mi.Portions, mi.MinuteMealId)).ToArrayAsync();
        return TypedResults.Ok(menuItems);
    }
    
    public static async Task<Results<NoContent, NotFound>> DeleteMenuItem(int id, MinuteContext context)
    {
        if(await context.MenuItems.FindAsync(id) is MenuItem item)
        {
            context.MenuItems.Remove(item);
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound(); 
        }
    }

    public static async Task<Results<NoContent, NotFound>> PatchMenuItemDate(int id, MenuItemPatchDateDto patch, MinuteContext context)
    {
        if(await context.MenuItems.FindAsync(id) is MenuItem item)
        {
            item.Date = patch.Date;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> PatchMenuItemPortions(int id, MenuItemPatchPortionsDto patch, MinuteContext context)
    {
        if(await context.MenuItems.FindAsync(id) is MenuItem item)
        {
            item.Portions = patch.Portions;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> DecreaseNumberOfPortions(int id, MinuteContext context)
    {
        if(await context.MenuItems.FindAsync(id) is MenuItem item)
        {
            item.Portions = item.Portions - 1;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> ChangeMenuItemMeal(int id, MenuItemPatchMeal patch,  MinuteContext context)
    {
        if(await context.MenuItems.FindAsync(id) is MenuItem item)
        {
            item.MinuteMealId = patch.MinuteMealId;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> PutMenuItem(int id, MenuItemRequestDto request, MinuteContext context)
    {
        if(await context.MenuItems.FindAsync(id) is MenuItem item)
        {
            item.Date = request.Date;
            item.Portions = request.Portions;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Ok<OrderDto[]>> GetAllOrders(MinuteContext context)
    {
        var orders = await context.Orders.Select(o => new OrderDto(o.Id, o.OrderStatus, o.MenuItemId)).ToArrayAsync();
        return TypedResults.Ok(orders);
    }

    public static async Task<Results<Created<OrderDto>, BadRequest<string>, Conflict<string>>> CreateOrder(OrderRequestDto request, MinuteContext context)
    {
        var item = await context.MenuItems.FindAsync(request.MenuItemId);
        if (item == null || item.Portions <= 0) return TypedResults.BadRequest("Sold out.");
        item.Portions--;
        var order = new Order { OrderStatus =  request.status, MenuItemId = request.MenuItemId};
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return TypedResults.Created($"/orders/{order.Id}", new OrderDto(order.Id, order.OrderStatus, order.MenuItemId));
    }

    public static async Task<Results<NoContent, NotFound>> PutOrder(int id, OrderRequestDto request, MinuteContext context)
    {
        if(await context.Orders.FindAsync(id) is Order order)
        {
            order.OrderStatus = request.status;
            order.MenuItemId = request.MenuItemId;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<NoContent, NotFound>> OrderChangeStatus(int id, OrderPatchStatusDto patch, MinuteContext context)
    {
        if(await context.Orders.FindAsync(id) is Order order)
        {
            order.OrderStatus = patch.status;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }
    public static async Task<Results<NoContent, NotFound>> PatchOrderMenuItem(int id, OrderPatchMenuItemDto patch, MinuteContext context)
    {
        if(await context.Orders.FindAsync(id) is Order order)
        {
            order.MenuItemId = patch.MenuItemId;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        else
        {
            return TypedResults.NotFound();
        }
    }
}
