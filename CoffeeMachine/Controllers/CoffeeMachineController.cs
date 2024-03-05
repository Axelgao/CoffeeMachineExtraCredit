using Microsoft.AspNetCore.Mvc;
using CoffeeMachine.Models;
using CoffeeMachine.Interfaces;
using CoffeeMachine.Services;

namespace CoffeeMachine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeMachineController : ControllerBase
    {
        private readonly ILogger<CoffeeMachineController> _logger;
        private static int _counter = 0;
        private readonly IWeatherService _weatherService;

        public CoffeeMachineController(ILogger<CoffeeMachineController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet("brew-coffee")]
        public async Task<IActionResult> BrewCoffeeAsync()
        {
            try
            {
                _counter++;

                // Requirement #3
                if (SystemTime.Now().Month == 4 && SystemTime.Now().Day == 1) 
                {
                    _logger.LogInformation("April 1st, Teapot.");
                    return StatusCode(418);
                }

                // Requirement #2
                if (_counter % 5 ==  0)
                {
                    _logger.LogInformation("Coffee machine is out of coffee");
                    return StatusCode(503);
                }

                // Requirement #1
                var temp = await _weatherService.GetTempAsync();
                var message = temp > 30 ? "Your refreshing iced coffee is ready" : "Your piping hot coffee is ready";
                var res = new Response
                {
                    Message = message,
                    Prepared = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(500);
            }
        }
    }

    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;

        public static void Reset() => Now = () => DateTime.Now;
    }
}
