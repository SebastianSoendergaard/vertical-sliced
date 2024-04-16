using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Test
{
    public class CompleteTodoItemTests
    {
        ICommandDispatcher _commandDispatcher;
        IQueryDispatcher _queryDispatcher;

        public CompleteTodoItemTests()
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
        public async Task CompleteTodoItem_ShouldMarkItemAsCompleted_WhenSucceded()
        {
            var addResult = await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));

            await _commandDispatcher.Dispatch(new CompleteTodoItemCommand(addResult.Id));

            var result = await _queryDispatcher.Dispatch(new GetTodoItemsQuery());
            Assert.True(result.TodoItems.Single().IsComplete);
        }

        [Fact]
        public async Task CompleteTodoItem_ShouldNotMarkItemAsCompleted_WhenFailed()
        {
            await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));

            var completeCommand = new CompleteTodoItemCommand(Guid.NewGuid());
            await Assert.ThrowsAsync<ArgumentException>(async () => await _commandDispatcher.Dispatch(completeCommand));

            var result = await _queryDispatcher.Dispatch(new GetTodoItemsQuery());
            Assert.False(result.TodoItems.Single().IsComplete);
        }
    }
}
