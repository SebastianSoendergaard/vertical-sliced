using Framework;
using Microsoft.Extensions.DependencyInjection;


namespace Todo
{
    public static class TodoModule
    {
        public static IServiceCollection AddTodoModule(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<AddTodoItemCommand, Result<AddTodoItemResult>>, AddTodoItemHandler>();
            services.AddScoped<IQueryHandler<GetTodoItemsQuery, GetTodoItemsResult>, GetTodoItemsHandler>();
            services.AddScoped<ICommandHandler<CompleteTodoItemCommand>, CompleteTodoItemHandler>();
            services.AddScoped<ICommandHandler<UndoCompleteTodoItemCommand>, UndoCompleteTodoItemHandler>();
            services.AddScoped<ICommandHandler<RemoveCompleteTodoItemCommand>, RemoveCompleteTodoItemHandler>();

            return services;
        }
    }
}
