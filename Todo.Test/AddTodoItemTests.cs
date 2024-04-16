using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Test
{
    public class AddTodoItemTests
    {
        ICommandDispatcher _commandDispatcher;
        IQueryDispatcher _queryDispatcher;

        public AddTodoItemTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IStore, InMemoryStore>();
            serviceCollection.AddScoped<ICommandDispatcher, CommandDispatcher>();
            serviceCollection.AddScoped<IQueryDispatcher, QueryDispatcher>();

            serviceCollection.AddTodoModule();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            _queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
        }

        [Fact]
        public async Task AddTodoItem_ShouldSucced_WhenNoExistingItems()
        {
            var command = new AddTodoItemCommand("Pick up milk");

            var result = await _commandDispatcher.Dispatch(command);

            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task AddTodoItem_ShouldSucced_WhenExistingItems()
        {
            await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));
            var command = new AddTodoItemCommand("Pick up sugar");

            var result = await _commandDispatcher.Dispatch(command);

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

        [Fact]
        public async Task AddTodoItem_ShouldAddIncompleteItem_WhenSucceded()
        {
            await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));

            var result = await _queryDispatcher.Dispatch(new GetTodoItemsQuery());
            Assert.NotNull(result);
            Assert.Single(result.TodoItems);
            Assert.False(result.TodoItems.Single().IsComplete);
        }
    }
}