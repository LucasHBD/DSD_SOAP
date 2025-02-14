using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GameClient
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            var restApiUrl = "http://localhost:5041/api/guessattempts/submit";  // URL da API

            // Chama a API REST para registrar uma tentativa
            int userGuess = 50;  // Vamos supor que o usuário adivinhe o número 50
            await RegisterAttemptAsync(restApiUrl, userGuess);

            Console.WriteLine("Tentativa registrada na API REST.");
        }

        private static async Task RegisterAttemptAsync(string apiUrl, int guess)
        {
            try
            {
                var attempt = new { Guess = guess }; // Cria o objeto da tentativa
                var content = JsonContent.Create(attempt); // Serializa para JSON

                var response = await _httpClient.PostAsync(apiUrl, content); // Envia a requisição

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Tentativa registrada na API REST.");
                }
                else
                {
                    // Exibe o status code e a razão do erro
                    Console.WriteLine($"Erro ao registrar tentativa. Código: {response.StatusCode}, Razão: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                // Trata erros inesperados, como problemas de conexão
                Console.WriteLine($"Erro ao registrar tentativa: {ex.Message}");
            }
        }
    }
}
