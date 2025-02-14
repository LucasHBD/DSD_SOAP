using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuessAttemptsController : ControllerBase
    {
        [HttpPost("submit")]
        public IActionResult SubmitAttempt([FromBody] GuessAttempt attempt)
        {
            // Lógica para registrar a tentativa
            Console.WriteLine($"Tentativa do usuário: {attempt.Guess}");
            return Ok("Tentativa registrada!");
        }
    }

    public class GuessAttempt
    {
        public int Guess { get; set; }
    }
}
