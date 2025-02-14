var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Lista de tentativas (escopo local)
var attempts = new List<GuessAttempt>();

// Configura o endpoint POST para registrar tentativas
app.MapPost("/api/guessattempts/submit", (GuessAttempt attempt) =>
{
    if (attempt == null)
    {
        return Results.BadRequest("Tentativa inválida.");
    }

    // Armazena a tentativa
    attempts.Add(attempt);
    Console.WriteLine($"Tentativa do usuário: {attempt.Guess}");

    // Retorna um status de sucesso
    return Results.Ok("Tentativa registrada!");
})
.WithName("SubmitGuessAttempt");

// Configura o endpoint GET para listar todas as tentativas
app.MapGet("/api/guessattempts", () =>
{
    return Results.Ok(attempts);
})
.WithName("GetAllGuessAttempts");

// Roda a aplicação
app.Run();

// Record declarado após as Top-level statements
record GuessAttempt(int Guess);
