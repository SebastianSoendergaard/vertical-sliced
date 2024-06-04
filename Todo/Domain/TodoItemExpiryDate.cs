using Framework;
using Todo.Domain;

internal class TodoItemExpiryDate : ValueObject
{
    public static readonly TodoItemExpiryDate None = new TodoItemExpiryDate(DateOnly.MaxValue);

    public DateOnly Value { get; private set; }

    private TodoItemExpiryDate(DateOnly date)
    {
        Value = date;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<TodoItemExpiryDate> Create(DateOnly? date, ITimeProvider timeProvider)
    {
        if (date == null)
        {
            return None;
        }

        if (date <= timeProvider.GetCurrentDate())
        {
            return TodoErrors.InvalidDescription;
        }

        return new TodoItemExpiryDate(date.Value);
    }
}
