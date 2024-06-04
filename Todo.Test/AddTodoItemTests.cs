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
            var command = AddTodoItemCommand.Create("Pick up milk");

            var result = await _commandDispatcher.Dispatch(command.Value);

            Assert.NotEqual(Guid.Empty, result.Value.Id);
        }

        [Fact]
        public async Task AddTodoItem_ShouldSucced_WhenExistingItems()
        {
            await _commandDispatcher.Dispatch(AddTodoItemCommand.Create("Pick up milk").Value);
            var command = AddTodoItemCommand.Create("Pick up sugar");

            var result = await _commandDispatcher.Dispatch(command.Value);

            Assert.NotEqual(Guid.Empty, result.Value.Id);
        }

        [Fact]
        public void AddTodoItem_ShouldFail_WhenGivenEmptyDescription()
        {
            var result = AddTodoItemCommand.Create("");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void AddTodoItem_ShouldFail_WhenGivenNullDescription()
        {
            var result = AddTodoItemCommand.Create(null!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task AddTodoItem_ShouldAddIncompleteItem_WhenSucceded()
        {
            await _commandDispatcher.Dispatch(AddTodoItemCommand.Create("Pick up milk").Value);

            var result = await _queryDispatcher.Dispatch(new GetTodoItemsQuery());
            Assert.NotNull(result);
            Assert.Single(result.TodoItems);
            Assert.False(result.TodoItems.Single().IsComplete);
        }
    }
}