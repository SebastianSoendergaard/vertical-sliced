using Microsoft.Extensions.DependencyInjection;

namespace Todo.Test
{
    public class RemoveCompleteTodoItemTests
    {
        ICommandDispatcher _commandDispatcher;
        IQueryDispatcher _queryDispatcher;

        public RemoveCompleteTodoItemTests()
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
        public async Task RemoveCompleteTodoItem_ShouldRemoveACompleteItem_WhenSucceded()
        {
            var addResult = await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));
            await _commandDispatcher.Dispatch(new CompleteTodoItemCommand(addResult.Id));

            await _commandDispatcher.Dispatch(new RemoveCompleteTodoItemCommand(addResult.Id));

            var result = await _queryDispatcher.Dispatch(new GetTodoItemsQuery());
            Assert.Empty(result.TodoItems);
        }

        [Fact]
        public async Task RemoveCompleteTodoItem_ShouldNotRemoveACompleteItem_WhenFailed()
        {
            var addResult = await _commandDispatcher.Dispatch(new AddTodoItemCommand("Pick up milk"));
            await _commandDispatcher.Dispatch(new CompleteTodoItemCommand(addResult.Id));

            var removeCommand = new RemoveCompleteTodoItemCommand(Guid.NewGuid());
            await Assert.ThrowsAsync<ArgumentException>(async () => await _commandDispatcher.Dispatch(removeCommand));

            var result = await _queryDispatcher.Dispatch(new GetTodoItemsQuery());
            Assert.Single(result.TodoItems);
        }
    }
}
