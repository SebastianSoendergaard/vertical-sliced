using Framework;

public class AddTodoItem
{
    public class Command : ICommand<Result<Result>>
    {
        private Command(TodoItemDescription description)
        {
            Description = description;
        }

        internal TodoItemDescription Description { get; private set; }

        public static Result<Command> Create(string description)
        {
            return TodoItemDescription.Create(description)
                .Bind(d => new Command(d));
        }
    }

    public class Result
    {
        internal Result(TodoItemId id)
        {
            Id = id.Value;
        }

        public Guid Id { get; private set; }
    }

    internal class Handler(IStore store) : ICommandHandler<Command, Result<Result>>
    {
        public async Task<Result<Result>> Handle(Command command, CancellationToken ct)
        {
            var todoItem = new TodoItem(TodoItemId.NewId(), command.Description);
            await store.StoreItem(todoItem.Id, todoItem);
            return new Result(todoItem.Id);
        }
    }
}