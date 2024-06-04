using Framework;
using Todo.Domain;

internal class TodoItemExpiryDate : ValueObject
{
    public DateOnly Value { get; private set; }

    private TodoItemExpiryDate(DateOnly date)
    {
        Value = date;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<TodoItemExpiryDate> Create(DateOnly date, ITimeProvider timeProvider)
    {
        if (date <= timeProvider.GetCurrentDate())
        {
            return TodoErrors.InvalidDescription;
        }

        return new TodoItemExpiryDate(date);
    }
}
