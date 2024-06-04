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
}
