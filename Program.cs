using ToDoApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

// using Microsoft.OpenApi.Models;
 
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks", Version = "v1" });
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("*")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                      });
});

builder.Services.AddDbContext<ToDoDbContext>();
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    // options.RoutePrefix = string.Empty;
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/item", (ToDoDbContext ctx) => {
    return ctx.Items.ToList();
});
app.MapGet("/item/{id}", (ToDoDbContext ctx,int id) => {
    return ctx.Items.FindAsync(id);
});
app.MapPost("/item", async(ToDoDbContext ctx,Items item) => {
    ctx.Items.Add(item);
    await ctx.SaveChangesAsync();
    return item;
});
app.MapPut("/item/{id}",async (ToDoDbContext ctx,[FromBody] Items item,int id) => {
     var existItem = await ctx.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();

    existItem.Name = item.Name;
    existItem.IsComplete = item.IsComplete;

    await ctx.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/item/{id}",async (ToDoDbContext ctx,int id) => {
     var existItem = await ctx.Items.FindAsync(id);
     if (existItem is null) return Results.NotFound();

     ctx.Items.Remove(existItem);
     ctx.SaveChangesAsync();

     return Results.NoContent();
});



app.Run();
