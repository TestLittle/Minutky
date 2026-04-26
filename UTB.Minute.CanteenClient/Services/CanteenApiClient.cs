using System.Net.Http.Json;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

namespace UTB.Minute.CanteenClient.Services;

public class CanteenApiClient(HttpClient http)
{
    public async Task<List<MenuItemDto>> GetMenuItemsAsync()
        => await http.GetFromJsonAsync<List<MenuItemDto>>("menuItems") ?? new();

    public async Task<List<MinuteMealDto>> GetMealsAsync()
        => await http.GetFromJsonAsync<List<MinuteMealDto>>("minuteMeals") ?? new();

    public async Task CreateOrderAsync(OrderRequestDto order)
    {
        // Volá tvůj POST /orders
        await http.PostAsJsonAsync("orders", order);
    }
    public async Task<List<OrderDto>> GetOrdersAsync()
    => await http.GetFromJsonAsync<List<OrderDto>>("orders") ?? new();
    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        // Volá tvůj endpoint PATCH /orders/{id}/status
        await http.PatchAsJsonAsync($"orders/{orderId}/status", new OrderPatchStatusDto(newStatus));
    }
}