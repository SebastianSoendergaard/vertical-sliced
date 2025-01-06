using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApi.Test
{
    public class GetTodoItemsTests
    {
        private readonly HttpClient _httpClient;

        public GetTodoItemsTests()
        {
            WebApplicationFactory<Program> application = new();
            _httpClient = application.CreateClient();
        }

        [Fact]
        public async Task GetTodoItems_ShouldSucced()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("api/GetTodoItems");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturnTodoItems()
        {
            _ = await _httpClient.PostAsJsonAsync("api/AddTodoItem", new
            {
                Description = "Pick up milk"
            });

            GetTodoItemsResult? result = await _httpClient.GetFromJsonAsync<GetTodoItemsResult>("api/GetTodoItems");

            Assert.NotNull(result);
            _ = Assert.Single(result.TodoItems);
            Assert.NotEqual(Guid.Empty, result.TodoItems.First().Id);
            Assert.Equal("Pick up milk", result.TodoItems.First().Description);
            Assert.False(result.TodoItems.First().IsComplete);
        }

        private record GetTodoItemsResult(IEnumerable<TodoItem> TodoItems);
        private record TodoItem(Guid Id, string Description, bool IsComplete);
    }
}