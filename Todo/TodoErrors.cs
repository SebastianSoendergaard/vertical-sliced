using Framework;

namespace Todo
{
    internal static class TodoErrors
    {
        public static readonly Error InvalidDescription = new Error("Todo.InvalidDescription", "Descripton for a new todo item can not be empty");
    }
}
