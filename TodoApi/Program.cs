using TodoApi.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var todos = new List<TodoGetDto>
{
    new (1, "string", true),
    new (2, "smt", false),
    new (3, "smt2", true)
};

app.MapGet("/todos", () => Results.Ok(todos));

app.MapGet("/", () => "Hello Todo API");

app.Run();