using System.Net;
using Microsoft.OpenApi.Models;
using Todo;
using WebApi;

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

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (InvalidOperationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsJsonAsync(new ApiError($"Invalid operation: {ex.Message}"));
    }
    catch (ArgumentException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsJsonAsync(new ApiError($"Invalid input: {ex.Message}"));
    }
    catch (Exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(new ApiError("Internal server error"));
    }
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

// TODO API
app.MapPost("/api/AddTodoItem", async (ICommandDispatcher dispatcher, NewTodoItemDto todoItem) => await dispatcher.Dispatch(new AddTodoItemCommand(todoItem.Description)));
app.MapPost("/api/CompleteTodoItem/{id}", async (ICommandDispatcher dispatcher, Guid id) => await dispatcher.Dispatch(new CompleteTodoItemCommand(id)));
app.MapPost("/api/UndoCompleteTodoItem/{id}", async (ICommandDispatcher dispatcher, Guid id) => await dispatcher.Dispatch(new UndoCompleteTodoItemCommand(id)));
app.MapPost("/api/RemoveCompleteTodoItem/{id}", async (ICommandDispatcher dispatcher, Guid id) => await dispatcher.Dispatch(new RemoveCompleteTodoItemCommand(id)));
app.MapGet("/api/GetTodoItems", async (IQueryDispatcher dispatcher) => await dispatcher.Dispatch(new GetTodoItemsQuery()));

app.Run();

record NewTodoItemDto(string Description);
record TodoItemDto(Guid Id, string Description, bool IsComplete);

public partial class Program { }