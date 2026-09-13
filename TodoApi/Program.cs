using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Data;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var todoGroup = app.MapGroup("/api/todos");

todoGroup.MapGet("/", async (AppDbContext db) =>
{
    var todos = await db.Todos
        .Select(x => new TodoGetDto(x.Id, x.Title, x.IsCompleted))
        .ToListAsync();
    
    return Results.Ok(todos);
});

todoGroup.MapGet("/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null) return Results.NotFound();

        return Results.Ok(new TodoGetDto(todo.Id, todo.Title,  todo.IsCompleted));
    }
    catch(Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

todoGroup.MapPost("/", async (TodoItem dto, AppDbContext db) =>
{
    var todo = new TodoItem
    {
        Title = dto.Title,
        IsCompleted = false,
        CreatedAt = DateTime.UtcNow
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    var result = new TodoGetDto(todo.Id, todo.Title, todo.IsCompleted);
    return Results.Created($"/api/todos/{todo.Id}", result);
});

#region api


// var todos = new List<TodoGetDto>
// {
//     new (1, "string", true),
//     new (2, "smt", false),
//     new (3, "smt2", true)
// };

// todoGroup.MapGet("/", () => Results.Ok(todos));

// todoGroup.MapGet("/{id}", (int id) =>
// {
//     var todo = todos.FirstOrDefault(t => t.id == id);

//     return todo is not null ? Results.Ok(todo) : Results.NotFound();
// });

// todoGroup.MapPost("/", (TodoPostDto dto) =>
// {
//     var nextId = todos.Count == 0 ? 1 : todos.Max(t => t.id) + 1;
//     var todo = new TodoGetDto(nextId, dto.title, false);
//     todos.Add(todo);

//     return Results.Created($"/api/todos/{todo.id}", todo);
// });

// todoGroup.MapPut("/{id}", (int id, TodoPutDto dto) =>
// {
//     try
//     {
//         var index = todos.FindIndex(x => x.id == id);
//         if (index == -1) return Results.NotFound();

//         todos[index] = todos[index] with
//         {
//             Title = dto.Title,
//             IsComplete = dto.IsComplete
//         };

//         return Results.Ok(todos[index]);
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.Message);
//     }
// });

// todoGroup.MapDelete("/{id}", (int id) =>
// {
//     try
//     {
//         var todo = todos.FirstOrDefault(x => x.id == id);
//         if (todo is null) return Results.NotFound();

//         todos.Remove(todo);
//         return Results.NoContent();
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.Message);
//     }
// });

#endregion


app.Run();