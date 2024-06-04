using Framework;
using Todo.Domain;

internal class TodoItemDescription : ValueObject
{
    public string Value { get; private set;}

    private TodoItemDescription(string description)
    {
        Value = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<TodoItemDescription> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return TodoErrors.InvalidDescription;
        }

        return new TodoItemDescription(description);
    }

    public TodoItemTitle AsTitle()
    {
        var title = Value;
        if (title.Length > 100)
        { 
            title = $"{title.Substring(0, 97)}...";
        }
        return TodoItemTitle.Create(title).Value;
    }
}
