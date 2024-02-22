using Microsoft.OpenApi.Models;
using Todo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks", Version = "v1" });
});

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddTodoModule();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

// TODO API
app.MapPost("/AddTodoItem", async (ICommandDispatcher dispatcher, NewTodoItemDto todoItem) => await dispatcher.Dispatch(new AddTodoItemCommand(todoItem.Description)));
app.MapPost("/CompleteTodoItem/{id}", async (ICommandDispatcher dispatcher, Guid id) => await dispatcher.Dispatch(new CompleteTodoItemCommand(id)));
app.MapPost("/UndoCompleteTodoItem/{id}", async (ICommandDispatcher dispatcher, Guid id) => await dispatcher.Dispatch(new UndoCompleteTodoItemCommand(id)));
app.MapPost("/RemoveCompleteTodoItem/{id}", async (ICommandDispatcher dispatcher, Guid id) => await dispatcher.Dispatch(new RemoveCompleteTodoItemCommand(id)));
app.MapGet("/GetTodoItems", async (IQueryDispatcher dispatcher) => await dispatcher.Dispatch(new GetTodoItemsQuery()));

app.Run();

record NewTodoItemDto(string Description);
record TodoItemDto(Guid Id, string Description, bool IsComplete);