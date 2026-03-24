using System.Net;
using System.Net.Http.Json;
using UTB.Minute.Contracts;
using Xunit;
using Aspire.Hosting.Testing;

namespace UTB.Minute.WebApi.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task WebApi_Full_Checklist_Test()
    {
        // Vytáhneme si CancellationToken, abychom uspokojili xUnit warningy
        var ct = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UTB_Minute_AppHost>(cancellationToken: ct);
        await using var app = await appHost.BuildAsync(ct);
        await app.StartAsync(ct);
        var httpClient = app.CreateHttpClient("utb-minute-webapi");

        // ==========================================
        // 1. JÍDLA (Meals)
        // ==========================================
        var newMeal = new MinuteMealRequestDto("Testovaci Rizek", 150);
        var postMealRes = await httpClient.PostAsJsonAsync("/minuteMeals", newMeal, cancellationToken: ct);
        Assert.Equal(HttpStatusCode.Created, postMealRes.StatusCode);
        var createdMeal = await postMealRes.Content.ReadFromJsonAsync<MinuteMealDto>(cancellationToken: ct);

        var meals = await httpClient.GetFromJsonAsync<MinuteMealDto[]>("/minuteMeals", cancellationToken: ct);
        Assert.Contains(meals!, m => m.Desc == "Testovaci Rizek");

        var updateMealReq = new MinuteMealPatchPriceDto(160);
        var patchMealRes = await httpClient.PatchAsJsonAsync($"/minuteMeals/{createdMeal!.MinuteMealId}/price", updateMealReq, cancellationToken: ct);
        Assert.Equal(HttpStatusCode.NoContent, patchMealRes.StatusCode);

        // ==========================================
        // 2. MENU
        // ==========================================
        // a) Vytvoření (POST)
        var menuDto = new MenuItemDto(0, DateTime.Today, 10);
        var postMenuRes = await httpClient.PostAsJsonAsync("/menu", menuDto, cancellationToken: ct);
        Assert.Equal(HttpStatusCode.Created, postMenuRes.StatusCode);
        var createdMenu = await postMenuRes.Content.ReadFromJsonAsync<MenuItemDto>(cancellationToken: ct);

        // b) Čtení (GET)
        var menus = await httpClient.GetFromJsonAsync<MenuItemDto[]>("/menu", cancellationToken: ct);
        Assert.NotNull(menus);

        // c) Úprava (PUT) - Změníme počet porcí z 10 na 20
        var updateMenuDto = new MenuItemDto(createdMenu!.MenuItemId, DateTime.Today, 20);
        var putMenuRes = await httpClient.PutAsJsonAsync($"/menu/{createdMenu.MenuItemId}", updateMenuDto, cancellationToken: ct);
        Assert.Equal(HttpStatusCode.NoContent, putMenuRes.StatusCode);

        // ==========================================
        // 3. OBJEDNÁVKY
        // ==========================================
        var postOrderRes = await httpClient.PostAsync($"/orders?menuItemId={createdMenu!.MenuItemId}", null, ct);
        Assert.Equal(HttpStatusCode.Created, postOrderRes.StatusCode);
        var createdOrder = await postOrderRes.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: ct);

        var patchStatusRes = await httpClient.PatchAsync($"/orders/{createdOrder!.OrderId}/status?status=Preparing", null, ct);
        patchStatusRes.EnsureSuccessStatusCode();

        // ==========================================
        // 4. CLEANUP (Smazání/Deaktivace)
        // ==========================================
        var delMenuRes = await httpClient.DeleteAsync($"/menu/{createdMenu.MenuItemId}", ct);
        Assert.Equal(HttpStatusCode.NoContent, delMenuRes.StatusCode);

        var delMealRes = await httpClient.DeleteAsync($"/minuteMeals/{createdMeal.MinuteMealId}", ct);
        Assert.Equal(HttpStatusCode.NoContent, delMealRes.StatusCode);
    }
}