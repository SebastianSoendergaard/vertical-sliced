using Framework;

public class GetTodoItems
{
    public class Query : IQuery<Result>
    {
    }

    public class Result
    {
        internal Result(IReadOnlyList<TodoItem> todoItems)
        {
            TodoItems = todoItems.Select(i => new Item(i.Id.Value, i.Description.Value, i.IsComplete)).ToList();
        }

        public IReadOnlyList<Item> TodoItems { get; private set; }

        public record Item(Guid Id, string Description, bool IsComplete);
    }

    internal class Handler(IStore store) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query query, CancellationToken ct)
        {
            var items = await store.GetItems<TodoItem>();
            return new Result(items.ToList());
        }
    }
}