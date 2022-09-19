using Microsoft.EntityFrameworkCore;
using Peritus_BE.Context;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    // this defines a CORS policy called "default"
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddDbContext<PeritusDbContext>(opt => opt.UseInMemoryDatabase("Peritus"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("default");

app.MapGet("/", () => "Hello World!");

app.MapGet("/quotes", async (PeritusDbContext db) =>
    await db.Quotes.ToListAsync());

app.MapGet("/quotes/{id}", async (int id, PeritusDbContext db) =>
    await db.Quotes.FindAsync(id)
        is Quote quote
            ? Results.Ok(quote)
            : Results.NotFound());

app.MapPost("/quotes", async (Quote quote, PeritusDbContext db) =>
{
    db.Quotes.Add(quote);
    await db.SaveChangesAsync();

    return Results.Created($"/quotes/{quote.Id}", quote);
});

app.MapPut("/quotes/{id}", async (int id, Quote inputQuote, PeritusDbContext db) =>
{
    var quote = await db.Quotes.FindAsync(id);

    if (quote is null) return Results.NotFound();

    quote.QuoteText = inputQuote.QuoteText;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/quotes/{id}", async (int id, PeritusDbContext db) =>
{
    if (await db.Quotes.FindAsync(id) is Quote quote)
    {
        db.Quotes.Remove(quote);
        await db.SaveChangesAsync();
        return Results.Ok(quote);
    }

    return Results.NotFound();
});

app.Run();

public class Quote
{
    public int Id { get; set; }
    public string? QuoteText { get; set; }
    public DateTime UploadDate { get; set; }
}
