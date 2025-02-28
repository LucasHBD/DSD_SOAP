using StackExchange.Redis;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string redisConnectionString = "localhost"; // Assumindo que o Redis está rodando localmente

        static async Task Main(string[] args)
        {
            var restApiUrl = "http://localhost:5041/api/guessattempts/submit"; // API REST - porta 5041
            var soapServiceUrl = "http://localhost:5263/SoapService.svc"; // API SOAP - porta 5263

            int userGuess = 50;  // Simulando a tentativa do usuário

            // Chama a API REST
            await RegisterAttemptAsync(restApiUrl, userGuess);

            // Chama a API SOAP
            await RegisterAttemptSoapAsync(soapServiceUrl, userGuess);

            // Envia a tentativa para a fila Redis
            await SendToQueueAsync(userGuess); // Envia para a fila Redis

            // Aguardar um momento antes de tentar ler a mensagem da fila
            await Task.Delay(1000);

            // Lê a mensagem da fila Redis
            await ReceiveFromQueueAsync();
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

        // Envia a tentativa para a fila Redis
        private static async Task SendToQueueAsync(int guess)
        {
            try
            {
                var connection = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
                var db = connection.GetDatabase();

                // Usando uma lista do Redis como fila
                await db.ListRightPushAsync("guessQueue", guess.ToString());

                Console.WriteLine($"Tentativa {guess} enviada para a fila Redis.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar para a fila Redis: {ex.Message}");
            }
        }

        // Recebe uma tentativa da fila Redis
        private static async Task ReceiveFromQueueAsync()
        {
            try
            {
                var connection = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
                var db = connection.GetDatabase();

                // Pega a próxima tentativa da fila
                var guess = await db.ListLeftPopAsync("guessQueue");

                if (guess.HasValue)
                {
                    Console.WriteLine($"Tentativa recebida da fila Redis: {guess}");
                }
                else
                {
                    Console.WriteLine("Nenhuma tentativa na fila Redis.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao receber da fila Redis: {ex.Message}");
            }
        }
    }
}
