using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Test
{
    public class CompleteTodoItemTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeCommandDispatcher _commandDispatcher;

        public CompleteTodoItemTests() 
        {
            _commandDispatcher = new FakeCommandDispatcher();

            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => builder
                    .ConfigureTestServices(services =>
                    {
                        services.AddScoped<ICommandDispatcher>(sp => _commandDispatcher);
                    }));

            _httpClient = application.CreateClient();
        }

        [Fact]
        public async Task CompleteTodoItem_ShouldReturnOK_OnSuccess()
        {
            var response = await _httpClient.PostAsync($"api/CompleteTodoItem/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CompleteTodoItem_ShouldReturnBadRequest_OnValidationError()
        {
            _commandDispatcher.SetException(new ArgumentException());

            var response = await _httpClient.PostAsync($"api/CompleteTodoItem/{Guid.NewGuid()}");

            var jsonResult = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CompleteTodoItem_ShouldReturnInternalServerError_OnUnexpectedError()
        {
            _commandDispatcher.SetException(new ArgumentException());

            var response = await _httpClient.PostAsync($"api/CompleteTodoItem/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
