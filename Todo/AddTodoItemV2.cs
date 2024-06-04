using Framework;

public class AddTodoItemV2
{
    public class Command : ICommand<Result<Result>>
    {
        private Command(TodoItemTitle title, TodoItemDescription description, TodoItemExpiryDate expiryDate)
        {
            Title = title;
            Description = description;
            ExpiryDate = expiryDate;
        }

        internal TodoItemTitle Title { get; private set; }
        internal TodoItemDescription Description { get; private set; }
        internal TodoItemExpiryDate ExpiryDate { get; private set; }

        public static Result<Command> Create(string title, string description, DateOnly? expiryDate)
        {
            return TodoItemTitle.Create(title)
                .Join(TodoItemDescription.Create(description))
                .Join(TodoItemExpiryDate.Create(expiryDate))
                .Bind(t, d, e => new Command(t, d, e));
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