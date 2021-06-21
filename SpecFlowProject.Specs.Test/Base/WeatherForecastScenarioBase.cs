using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace SpecFlowProject.Specs.Base
{
    public class WeatherForecastScenarioBase
    {
        private const string ApiUrlBase = "api/v1/weatherforecast";

        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(WeatherForecastScenarioBase))
               .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<WeatherForecastTestsStartup>();

            return new TestServer(hostBuilder);
        }

        public static class Get
        {
            public static string GetForecast()
            {
                return $"{ApiUrlBase}";
            }
        }

        public static class Post
        {
            public static string SaveForeCast = $"{ApiUrlBase}/save";
        }
    }
}
