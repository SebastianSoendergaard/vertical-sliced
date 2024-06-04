using Framework;

public class RemoveCompleteTodoItem
{
    public class Command : ICommand
    {
        public Command(Guid id)
        {
            Id = TodoItemId.From(id);
        }

        internal TodoItemId Id { get; set; }
    }

    internal class Handler(IStore store) : ICommandHandler<Command>
    {
        public async Task Handle(Command command, CancellationToken ct)
        {
            var todoItem = await store.GetItem<TodoItem, TodoItemId>(command.Id);
            if (todoItem == null)
            {
                throw new ArgumentException("Given TodoItem does not exist");
            }

            if (!todoItem.IsComplete)
            {
                throw new ArgumentException("The given TodoItem is not complete");
            }

            await store.RemoveItem(todoItem.Id);
        }
    }
}