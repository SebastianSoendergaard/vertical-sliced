using Framework;

namespace Todo.Domain
{
    internal static class TodoErrors
    {
        public static readonly Error InvalidTitle = new Error("Todo.InvalidTitle", "Title for a new todo item can not be empty and can not exeed 100 characters in length");
        public static readonly Error InvalidDescription = new Error("Todo.InvalidDescription", "Descripton for a new todo item can not be empty");
    }
}
