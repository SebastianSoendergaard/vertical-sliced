using Microsoft.Extensions.DependencyInjection;

namespace Todo.Test
{
    public class GetTodoItemsTests
    {
        ICommandDispatcher _commandDispatcher;
        IQueryDispatcher _queryDispatcher;

        public GetTodoItemsTests()
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
        public async Task GetTodoItems_ShouldReturnEmptyCollection_WhenNoItems()
        {
            var query = new GetTodoItemsQuery();

            var result = await _queryDispatcher.Dispatch(query);

            Assert.Empty(result.TodoItems);
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturnItem_WhenSingleItem()
        {
            await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));
            var query = new GetTodoItemsQuery();

            var result = await _queryDispatcher.Dispatch(query);

            Assert.Single(result.TodoItems);
        }

        [Fact]
        public async Task GetTodoItems_ShouldReturnItems_WhenMultipleItems()
        {
            await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));
            await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up sugar"));
            var query = new GetTodoItemsQuery();

            var result = await _queryDispatcher.Dispatch(query);

            Assert.Equal(2, result.TodoItems.Count);
        }
    }
}