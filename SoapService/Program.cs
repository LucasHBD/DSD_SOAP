using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

app.UseServiceModel(builder =>
{
    builder.AddService<SoapService>();
    builder.AddServiceEndpoint<SoapService, ISoapService>(new BasicHttpBinding(), "/SoapService.svc");

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();

[ServiceContract]
public interface ISoapService
{
    [OperationContract]
    string GuessNumber(int guess);
}

public class SoapService : ISoapService
{
    public string GuessNumber(int guess) => $"Guess received: {guess}";
}
