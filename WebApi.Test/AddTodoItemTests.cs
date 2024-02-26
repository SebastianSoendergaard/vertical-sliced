using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace WebApi.Test
{
    public class AddTodoItemTests
    {
        private readonly HttpClient _httpClient;

        public AddTodoItemTests() 
        {
            var application = new WebApplicationFactory<Program>();
            _httpClient = application.CreateClient();
        }

        [Fact]
        public async Task AddTodoItem_ShouldSucced()
        {
            var response = await _httpClient.PostAsJsonAsync("api/AddTodoItem", new 
            {
                Description = "Pick up milk"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddTodoItem_ShouldReturnId()
        {
            var response = await _httpClient.PostAsJsonAsync("api/AddTodoItem", new
            {
                Description = "Pick up milk"
            });

            var result = await response.As<AddTodoItemResult>();
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        private record AddTodoItemResult(Guid Id);
    }
}