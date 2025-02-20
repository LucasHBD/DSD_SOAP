using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Configuração de CORS para permitir requisições do cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

var httpClient = new HttpClient();

// 🔵 Endpoint REST -> Encaminha para a API REST
app.MapPost("/api/guessattempts/submit", async (HttpContext context) =>
{
    var apiUrl = "http://localhost:5041/api/guessattempts/submit"; // URL da API REST - porta 5041
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
    var response = await httpClient.PostAsync(apiUrl, content);

    context.Response.StatusCode = (int)response.StatusCode;
    await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
});

// 🔴 Endpoint SOAP -> Encaminha para o Serviço SOAP (SoapService)
app.MapPost("/api/guessattempts/submit-soap", async (HttpContext context) =>
{
    var soapServiceUrl = "http://localhost:5263/SoapService.svc"; // URL do SoapService - porta 5263
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

    var request = new HttpRequestMessage(HttpMethod.Post, soapServiceUrl)
    {
        Content = new StringContent(requestBody, Encoding.UTF8, "text/xml")
    };

    request.Headers.Add("SOAPAction", "http://tempuri.org/ISoapService/GuessNumber");

    var response = await httpClient.SendAsync(request);
    context.Response.StatusCode = (int)response.StatusCode;
    await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
});

app.Run();
