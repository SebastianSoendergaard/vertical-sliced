using Microsoft.Extensions.DependencyInjection;

namespace Todo.Test
{
    public class AddTodoItemTests
    {
        ICommandDispatcher _dispatcher;

        public AddTodoItemTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IStore, InMemoryStore>();
            serviceCollection.AddScoped<ICommandDispatcher, CommandDispatcher>();

            serviceCollection.AddTodoModule();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _dispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
        }

        [Fact]
        public async Task AddTodoItem_ShouldSucced_WhenNoExistingItems()
        {
            var command = new AddTodoItemCommand("Pick up milk");

            var result = await _dispatcher.Dispatch(command);

            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task AddTodoItem_ShouldSucced_WhenExistingItems()
        {
            await _dispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));
            var command = new AddTodoItemCommand("Pick up sugar");

            var result = await _dispatcher.Dispatch(command);

            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void AddTodoItem_ShouldFail_WhenGivenEmptyDescription()
        {
            Assert.Throws<ArgumentException>(() => new AddTodoItemCommand(""));
        }

        [Fact]
        public void AddTodoItem_ShouldFail_WhenGivenNullDescription()
        {
            Assert.Throws<ArgumentException>(() => new AddTodoItemCommand(null!));
        }
    }
}