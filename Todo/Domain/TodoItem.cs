internal class TodoItem
{
    public TodoItemId Id { get; private set; } 
    public TodoItemDescription Description { get; private set; }
    public bool IsComplete { get; private set; }

    public TodoItem(TodoItemId id, TodoItemTitle title, TodoItemDescription description, TodoItemExpiryDate expiryDate)
    {
        Id = id;
        Description = description;
        IsComplete = false;
    }

    public void MarkAsComplete()
    {
        IsComplete = true;
    }

    public void MarkAsIncomplete()
    {
        IsComplete = false;
    }
}
