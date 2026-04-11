using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using UTB.Minute.Contracts;
using UTB.Minute.Db;

namespace UTB.Minute.WebApi.Tests;

    [Collection("Database collection")]
    public class MealsTests(TestFixture fixture)
    {
        private readonly TestFixture fixture = fixture;
        [Fact]
        public async Task GetAllMinuteMeals_ReturnsAllSeededMinuteMeals()
        {
            var ct = TestContext.Current.CancellationToken;
            var response = await fixture.HttpClient.GetAsync("/minuteMeals", ct);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            MinuteMealDto[]? meals = await response.Content.ReadFromJsonAsync<MinuteMealDto[]>(ct);

            Assert.NotNull(meals);
            Assert.True(meals.Length >= 3);
            Assert.Contains(meals, m => m.Desc == "Testovaci rizek" && m.Price == 145.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci Caesar salat" && m.Price == 109.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci rohlik" && m.Price == 2.90);
        }

        [Fact]
        public async Task GetAllActiveMinuteMeals_ReturnsAllSeededActiveMeals()
        {
            var ct = TestContext.Current.CancellationToken;
            var response = await fixture.HttpClient.GetAsync("/minuteMeals/active", ct);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            MinuteMealDto[]? meals = await response.Content.ReadFromJsonAsync<MinuteMealDto[]>(ct);

            Assert.NotNull(meals);
            Assert.True(meals.Length >= 2);
            Assert.Contains(meals, m => m.Desc == "Testovaci rizek" && m.Price == 145.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci Caesar salat" && m.Price == 109.99);
        }

        [Fact]
        public async Task CreateMinuteMeal_ReturnsCreatedAndMealStays()
        {
            var ct = TestContext.Current.CancellationToken;
            var request = new MinuteMealRequestDto("Testovaci svickova", 148.99, false);
            var response = await fixture.HttpClient.PostAsJsonAsync("/minuteMeals", request, ct);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            MinuteMealDto? meal = await response.Content.ReadFromJsonAsync<MinuteMealDto>(TestContext.Current.CancellationToken);
            Assert.NotNull(meal);
            Assert.Equal(request.Desc, meal.Desc);
            Assert.Equal(request.Price, meal.Price);
            Assert.False(meal.IsActive);
            Assert.NotNull(response.Headers.Location);
            Assert.EndsWith($"/minuteMeals/{meal.MinuteMealId}", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task ChangeActiveStatusMinuteMeal_ChangesValueToTrue()
        {
            // id je v seedu  = false
            MinuteMealPatchIsActiveDto patch = new(true);
            var response = await fixture.HttpClient.PatchAsJsonAsync("/minuteMeals/3/active", patch);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // overeni
            var getResponse = await fixture.HttpClient.GetAsync("/minuteMeals");
            var meals = await getResponse.Content.ReadFromJsonAsync<MinuteMealDto[]>();
            Assert.True(meals!.First(m => m.MinuteMealId == 3).IsActive);
        }

        [Fact]
        public async Task PutMinuteMeal_InvalidId_ReturnsNotFound()
        {
            var request = new MinuteMealRequestDto("Neexistujici", 100, true);
            var response = await fixture.HttpClient.PutAsJsonAsync("/minuteMeals/9999", request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }




    //MENU
        [Fact]
        public async Task GetAllMenuItems_ReturnsSeededItems()
        {
            var response = await fixture.HttpClient.GetAsync("/menuItems");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var items = await response.Content.ReadFromJsonAsync<MenuItemDto[]>();

            Assert.NotNull(items);
            Assert.NotEmpty(items);
        }

        [Fact]
        public async Task CreateMenuItem_ValidRequest_ReturnsCreated()
        {
            var request = new MenuItemRequestDto(DateTime.Now.AddDays(1), 50, 1);
            var response = await fixture.HttpClient.PostAsJsonAsync("/menuItems", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeleteMenuItem_ValidId_ReturnsNoContent()
        {
            //id 2 pryc 
            var response = await fixture.HttpClient.DeleteAsync("/menuItems/2");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getRes = await fixture.HttpClient.GetAsync("/menuItems");
            var items = await getRes.Content.ReadFromJsonAsync<MenuItemDto[]>();
            Assert.DoesNotContain(items!, i => i.MenuItemId == 2);
        }

        //OBJEDNÁVKY
        [Fact]
        public async Task GetAllOrders_ReturnsSeededOrder()
        {
            var response = await fixture.HttpClient.GetAsync("/orders");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var orders = await response.Content.ReadFromJsonAsync<OrderDto[]>();

            Assert.NotNull(orders);
            Assert.NotEmpty(orders);
        }

        [Fact]
        public async Task CreateOrder_ValidRequest_DecreasesPortions()
        {
            // id 1 ma 12 porci....
            var request = new OrderRequestDto(OrderStatus.Preparing, 1);
            var response = await fixture.HttpClient.PostAsJsonAsync("/orders", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // kontrola ze se kleso
            var menuRes = await fixture.HttpClient.GetAsync("/menuItems");
            var items = await menuRes.Content.ReadFromJsonAsync<MenuItemDto[]>();
            Assert.Equal(11, items!.First(i => i.MenuItemId == 1).Portions);
        }

        [Fact]
        public async Task OrderChangeStatus_ValidId_UpdatesStatus()
        {
            var patch = new OrderPatchStatusDto(OrderStatus.Completed);
            var response = await fixture.HttpClient.PatchAsJsonAsync("/orders/1/status", patch);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        //INVALIDNÍ TESTY
        [Fact]
        public async Task DeleteMenuItem_InvalidId_ReturnsNotFound()
        {
            // smaze se id ktere neni
            var response = await fixture.HttpClient.DeleteAsync("/menuItems/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task OrderChangeStatus_InvalidId_ReturnsNotFound()
        {
            // zmena stavu objednavky (ktera neexistuje)
            var patch = new OrderPatchStatusDto(OrderStatus.Completed);
            var response = await fixture.HttpClient.PatchAsJsonAsync("/orders/9999/status", patch);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task CreateOrder_InvalidMenuItemId_ReturnsBadRequest()
        {
            // kontrola if item = null (jidlo s id ktere neexistuje)
            var request = new OrderRequestDto(OrderStatus.Preparing, 8888);
            var response = await fixture.HttpClient.PostAsJsonAsync("/orders", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

}


