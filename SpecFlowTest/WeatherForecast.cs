using System;
using System.ComponentModel.DataAnnotations;

namespace SpecFlowTest
{
    public class WeatherForecast
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(-70, 70)]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
