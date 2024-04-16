using Framework;

public class GetTodoItemsQuery : IQuery<GetTodoItemsResult>
{
}

public class GetTodoItemsResult
{
    internal GetTodoItemsResult(IReadOnlyList<TodoItem> todoItems)
    {
        TodoItems = todoItems.Select(i => new Item(i.Id.Value, i.Description.Value, i.IsComplete)).ToList();
    }

    public IReadOnlyList<Item> TodoItems { get; private set; }

    public record Item(Guid Id, string Description, bool IsComplete);
}

internal class GetTodoItemsHandler(IStore store) : IQueryHandler<GetTodoItemsQuery, GetTodoItemsResult>
{
    public async Task<GetTodoItemsResult> Handle(GetTodoItemsQuery query, CancellationToken ct)
    {
        var items = await store.GetItems<TodoItem>();
        return new GetTodoItemsResult(items.ToList());
    }
}