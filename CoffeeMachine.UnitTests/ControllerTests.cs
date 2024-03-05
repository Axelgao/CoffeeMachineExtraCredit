using CoffeeMachine.Controllers;
using CoffeeMachine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using System;
using Newtonsoft.Json;
using CoffeeMachine.Interfaces;

namespace CoffeeMachine.UnitTests
{
    public class ControllerTests
    {
        private readonly CoffeeMachineController _controller;
        private readonly Mock<ILogger<CoffeeMachineController>> _loggerMock = new Mock<ILogger<CoffeeMachineController>>();
        private readonly Mock<IWeatherService> _weatherServiceMock = new Mock<IWeatherService>();

        public ControllerTests() 
        {
            _weatherServiceMock.Setup(service => service.GetTempAsync()).ReturnsAsync(35);

            _controller = new CoffeeMachineController(_loggerMock.Object, _weatherServiceMock.Object);
        }

        [Fact]
        public async Task TestRequirementOneAsync()
        {
            var res = await _controller.BrewCoffeeAsync();

            var okResult = Assert.IsType<OkObjectResult>(res);
            Assert.NotNull(okResult);

            var coffeeResponse = Assert.IsType<Response>(okResult.Value);
            Assert.Equal("Your refreshing iced coffee is ready", coffeeResponse.Message);
        }

        [Fact]
        public async Task TestRequirementTwoAsync()
        {
            IActionResult res = null;
            for (int i = 0; i < 5; i++)
            {
                res = await _controller.BrewCoffeeAsync();
            }

            var statusCodeRes = Assert.IsType<StatusCodeResult>(res);
            Assert.Equal(503, statusCodeRes.StatusCode);
        }

        [Fact]
        public async Task TestRequirementThreeAsync()
        {
            SystemTime.Now = () => new DateTime(2000, 4, 1);

            var res = await _controller.BrewCoffeeAsync();

            var statusCodeRes = Assert.IsType<StatusCodeResult>(res);
            Assert.Equal(418, statusCodeRes.StatusCode);

            SystemTime.Reset();
        }
    }
}