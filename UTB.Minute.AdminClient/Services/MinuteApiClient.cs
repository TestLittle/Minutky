using System.Net.Http.Json;
using UTB.Minute.Contracts;

namespace UTB.Minute.AdminClient.Services;

public class MinuteApiClient(HttpClient http)
{
    public async Task<List<MinuteMealDto>> GetMealsAsync()
    {
        return await http.GetFromJsonAsync<List<MinuteMealDto>>("minuteMeals") ?? new();
    }

    public async Task<HttpResponseMessage> ToggleMealActiveAsync(int id, bool isActive)
    {
        return await http.PatchAsJsonAsync($"minuteMeals/{id}/active", new MinuteMealPatchIsActiveDto(isActive));
    }
    public async Task CreateMealAsync(MinuteMealRequestDto meal)
    {
        await http.PostAsJsonAsync("minuteMeals", meal);
    }
    public async Task<List<MinuteMealDto>> GetActiveMealsAsync()
    {
        return await http.GetFromJsonAsync<List<MinuteMealDto>>("minuteMeals/active") ?? new();
    }
    public async Task<List<MenuItemDto>> GetMenuItemsAsync()
    {
        return await http.GetFromJsonAsync<List<MenuItemDto>>("menuItems") ?? new();
    }
    public async Task CreateMenuItemAsync(MenuItemRequestDto item)
    {
        await http.PostAsJsonAsync("menuItems", item);
    }
    public async Task DeleteMenuItemAsync(int id)
    {
        await http.DeleteAsync($"menuItems/{id}");
    }
    public async Task UpdateMenuItemAsync(int id, MenuItemRequestDto item)
    {
        await http.PutAsJsonAsync($"menuItems/{id}", item);
    }
    public async Task UpdateMealAsync(int id, MinuteMealRequestDto meal)
    {
        await http.PutAsJsonAsync($"minuteMeals/{id}", meal);
    }

}