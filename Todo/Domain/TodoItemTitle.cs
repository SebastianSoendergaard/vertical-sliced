using Framework;
using Todo.Domain;

internal class TodoItemTitle : ValueObject
{
    public string Value { get; private set; }

    private TodoItemTitle(string title)
    {
        Value = title;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<TodoItemTitle> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return TodoErrors.InvalidTitle;
        }

        if (title.Length > 100)
        {
            return TodoErrors.InvalidTitle;
        }

        return new TodoItemTitle(title);
    }
}
