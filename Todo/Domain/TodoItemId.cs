using Framework;

internal class TodoItemId : ValueObject
{
    public Guid Value { get; private set; }

    private TodoItemId(Guid id) 
    { 
        Value = id; 
    }

    public static TodoItemId NewId() 
    { 
        return new TodoItemId(Guid.NewGuid()); 
    }

    public static TodoItemId From(Guid id)
    {
        return new TodoItemId(id);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
