using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpecFlowTest.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SpecFlowTest.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private static Dictionary<DateTime, WeatherForecast> _forecast = new Dictionary<DateTime, WeatherForecast>();

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            var rng = new Random();

            await Task.Yield();

            return Enumerable.Range(1, 5).Select(index => GetForecastForDay(index, rng))
            .ToArray();
        }

        private static WeatherForecast GetForecastForDay(int index, Random rng)
        {
            WeatherForecast forecast = null;
            var date = DateTime.Now.AddDays(index).Date;

            if(!_forecast.TryGetValue(date, out forecast))
            {
                _forecast[date] = forecast = new WeatherForecast
                {
                    Date = date,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
            }
            
            return forecast;
        }

        [Route("save")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> Post(WeatherForecast adjustForecastForDay)
        {
            var user = HttpContext.User;

            var totalDaysDiff = (adjustForecastForDay.Date - DateTime.Today).TotalDays;
            if (totalDaysDiff > 5 || totalDaysDiff < 1)
                throw new WeatherForecastDomainException($"You cannot save a weather forecast beyond 5 days ahead or for today or in the past");

            _forecast[adjustForecastForDay.Date] = adjustForecastForDay;

            await Task.Yield();

            return Ok();
        }
    }
}
