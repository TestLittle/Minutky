using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using UTB.Minute.Contracts;

namespace UTB.Minute.WebApi.Tests;

    [Collection("Database collection")]
    public class MealsTests(TestFixture fixture)
    {
        private readonly TestFixture fixture = fixture;

        [Fact]
        public async Task GetAllMinuteMeals_ReturnsAllSeededMinuteMeals()
        {
            var response = await fixture.HttpClient.GetAsync("/minuteMeals", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            MinuteMealDto[]? meals = await response.Content.ReadFromJsonAsync<MinuteMealDto[]>(TestContext.Current.CancellationToken);

            Assert.NotNull(meals);
            Assert.True(meals.Length >= 3);
            Assert.Contains(meals, m => m.Desc == "Testovaci rizek" && m.Price == 145.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci Caesar salat" && m.Price == 109.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci rohlik" && m.Price == 2.90);
        }

        [Fact]
        public async Task GetAllActiveMinuteMeals_ReturnsAllSeededActiveMeals()
        {
            var response = await fixture.HttpClient.GetAsync("/minuteMeals/active", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            MinuteMealDto[]? meals = await response.Content.ReadFromJsonAsync<MinuteMealDto[]>(TestContext.Current.CancellationToken);

            Assert.NotNull(meals);
            Assert.True(meals.Length >= 2);
            Assert.Contains(meals, m => m.Desc == "Testovaci rizek" && m.Price == 145.99);
            Assert.Contains(meals, m => m.Desc == "Testovaci Caesar salat" && m.Price == 109.99);
        }

        [Fact]
        public async Task CreateMinuteMeal_ReturnsCreatedAndMealStays()
        {
            var request = new MinuteMealRequestDto("Testovaci svickova", 148.99, false);
            var response = await fixture.HttpClient.PostAsJsonAsync("/minuteMeals", request, TestContext.Current.CancellationToken);
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
            MinuteMealPatchIsActiveDto changeIsActive = new MinuteMealPatchIsActiveDto(true);
            
        }
}
