using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MinuteContext>("database");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/minuteMeals", WebApiVersion1.GetAllMinuteMeals);
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

app.MapGet("/orders", WebApiVersion1.GetAllOrders);



app.UseHttpsRedirection();

app.Run();

public static class WebApiVersion1
{
    public static async Task<Ok<MinuteMealDto[]>> GetAllMinuteMeals(MinuteContext context)
    {
        var meals = await context.MinuteMeals.Select(m => new MinuteMealDto(m.Id, m.Desc, m.Price, m.IsDeactivated)).ToArrayAsync();

        return TypedResults.Ok(meals);
    }
    
    public static async Task<Created<MinuteMealDto>> CreateMinuteMeal(MinuteMealRequestDto request, MinuteContext context)
    {
        var meal = new MinuteMeal { Desc = request.Desc, Price = request.Price, IsDeactivated = request.IsDeactivated};

        context.MinuteMeals.Add(meal);
        await context.SaveChangesAsync();

        MinuteMealDto mealDto = new MinuteMealDto(meal.Id, meal.Desc, meal.Price, meal.IsDeactivated);
        return TypedResults.Created($"/minuteMeals/{meal.Id}", mealDto);
    }

    public static async Task<Results<NoContent, NotFound>> ChangeActiveStatusMinuteMeal(int id, MinuteMealPatchIsDeactivatedDto patch, MinuteContext context)
    {
        if(await context.MinuteMeals.FindAsync(id) is MinuteMeal meal)
        {
            meal.IsDeactivated = patch.IsDeactivated;
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
            meal.IsDeactivated = request.IsDeactivated;
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


    public static async Task<Created<MenuItemDto>> CreateMenuItem(MenuItemRequestDto request, MinuteContext context)
    {
        MenuItem menuItem = new MenuItem() { Date = request.Date, Portions = request.Portions };
        context.MenuItems.Add(menuItem);
        await context.SaveChangesAsync();
        return TypedResults.Created($"/menuItems/{menuItem.Id}", new MenuItemDto(menuItem.Id, menuItem.Date, menuItem.Portions));
    }

    public static async Task<Ok<MenuItemDto[]>> GetAllMenuItems(MinuteContext context)
    {
        var menuItems = await context.MenuItems.Select(mi => new MenuItemDto(mi.Id, mi.Date, mi.Portions)).ToArrayAsync();
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
        var orders = await context.Orders.Select(o => new OrderDto(o.Id, o.OrderStatus)).ToArrayAsync();
        return TypedResults.Ok(orders);
    }
}
