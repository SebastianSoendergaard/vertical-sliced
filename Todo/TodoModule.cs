using Framework;
using Microsoft.Extensions.DependencyInjection;


namespace Todo
{
    public static class TodoModule
    {
        public static IServiceCollection AddTodoModule(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<AddTodoItem.Command, Result<AddTodoItem.Result>>, AddTodoItem.Handler>();
            services.AddScoped<IQueryHandler<GetTodoItems.Query, GetTodoItems.Result>, GetTodoItems.Handler>();
            services.AddScoped<ICommandHandler<CompleteTodoItem.Command>, CompleteTodoItem.Handler>();
            services.AddScoped<ICommandHandler<UndoCompleteTodoItem.Command>, UndoCompleteTodoItem.Handler>();
            services.AddScoped<ICommandHandler<RemoveCompleteTodoItem.Command>, RemoveCompleteTodoItem.Handler>();

            services.AddScoped<ITimeProvider, Infrastructure.TimeProvider>();

            return services;
        }
    }
}
