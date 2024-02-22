using System.IO;
using Framework;

internal class TodoItem
{
    public TodoItemId Id { get; private set; } 
    public TodoItemDescription Description { get; private set; }
    public bool IsComplete { get; private set; }

    public TodoItem(TodoItemId id, TodoItemDescription description)
    {
        Id = id;
        Description = description;
        IsComplete = false;
    }

    public void MarkAsComplete()
    {
        IsComplete = true;
    }
}

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

internal class TodoItemDescription : ValueObject
{
    public string Value { get; private set;}

    public TodoItemDescription(string description)
    {
        if(string.IsNullOrEmpty(description)) throw new ArgumentException("Descripton for a new todo item can not be empty");
        Value = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}