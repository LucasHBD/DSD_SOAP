var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var attempts = new List<GuessAttempt>();

app.MapPost("/api/guessattempts/submit", (GuessAttempt attempt) =>
{
    if (attempt == null)
    {
        return Results.BadRequest("Tentativa inválida.");
    }

    attempts.Add(attempt);
    Console.WriteLine($"Tentativa do usuário: {attempt.Guess}");

    return Results.Ok("Tentativa registrada!");
});

app.MapGet("/api/guessattempts", () => Results.Ok(attempts));

app.Run();

record GuessAttempt(int Guess);
