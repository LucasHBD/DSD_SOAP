using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            var restApiUrl = "http://localhost:5041/api/guessattempts/submit"; // API REST - porta 5041
            var soapServiceUrl = "http://localhost:5263/SoapService.svc"; // API SOAP - porta 5263

            int userGuess = 50;  // Simulando a tentativa do usuário

            await RegisterAttemptAsync(restApiUrl, userGuess); // Chama a API REST
            await RegisterAttemptSoapAsync(soapServiceUrl, userGuess); // Chama o serviço SOAP

        }

        private static async Task RegisterAttemptAsync(string apiUrl, int guess)
        {
            try
            {
                var attempt = new { Guess = guess };
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(attempt), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Tentativa registrada na API REST.");
                }
                else
                {
                    Console.WriteLine($"Erro ao registrar tentativa REST. Código: {response.StatusCode}, Razão: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar tentativa REST: {ex.Message}");
            }
        }

        private static async Task RegisterAttemptSoapAsync(string apiUrl, int guess)
        {
            try
            {
                var soapRequest = $@"
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:web=""http://tempuri.org/"">
                       <soapenv:Header/>
                       <soapenv:Body>
                          <web:GuessNumber>
                             <guess>{guess}</guess>
                          </web:GuessNumber>
                       </soapenv:Body>
                    </soapenv:Envelope>";

                var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "http://tempuri.org/ISoapService/GuessNumber");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Tentativa registrada na API SOAP.");
                }
                else
                {
                    Console.WriteLine($"Erro ao registrar tentativa SOAP. Código: {response.StatusCode}, Razão: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar tentativa SOAP: {ex.Message}");
            }
        }
    }
}
