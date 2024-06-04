using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Test
{
    public class UndoCompleteTodoItemTests
    {
        ICommandDispatcher _commandDispatcher;
        IQueryDispatcher _queryDispatcher;

        public UndoCompleteTodoItemTests()
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
        public async Task UndoCompleteTodoItem_ShouldMarkItemAsIncompleted_WhenSucceded()
        {
            var addResult = await _commandDispatcher.Dispatch(AddTodoItem.Command.Create("Pick up milk").Value);
            await _commandDispatcher.Dispatch(new CompleteTodoItem.Command(addResult.Value.Id));

            await _commandDispatcher.Dispatch(new UndoCompleteTodoItem.Command(addResult.Value.Id));

            var result = await _queryDispatcher.Dispatch(new GetTodoItems.Query());
            Assert.False(result.TodoItems.Single().IsComplete);
        }

        [Fact]
        public async Task UndoCompleteTodoItem_ShouldNotMarkItemAsIncompleted_WhenFailed()
        {
            var addResult = await _commandDispatcher.Dispatch(AddTodoItem.Command.Create("Pick up milk").Value);
            await _commandDispatcher.Dispatch(new CompleteTodoItem.Command(addResult.Value.Id));

            var undoCommand = new UndoCompleteTodoItem.Command(Guid.NewGuid());
            await Assert.ThrowsAsync<ArgumentException>(async () => await _commandDispatcher.Dispatch(undoCommand));

            var result = await _queryDispatcher.Dispatch(new GetTodoItems.Query());
            Assert.True(result.TodoItems.Single().IsComplete);
        }
    }
}
