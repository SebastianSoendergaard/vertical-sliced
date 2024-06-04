internal class TodoItem
{
    public TodoItemId Id { get; private set; }
    public TodoItemTitle Title { get; }
    public TodoItemDescription Description { get; private set; }
    public TodoItemExpiryDate ExpiryDate { get; }
    public bool IsComplete { get; private set; }

    public TodoItem(TodoItemId id, TodoItemTitle title, TodoItemDescription description, TodoItemExpiryDate expiryDate)
    {
        Id = id;
        Title = title;
        Description = description;
        ExpiryDate = expiryDate;
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
