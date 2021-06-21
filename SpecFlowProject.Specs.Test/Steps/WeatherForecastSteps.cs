using System;
using TechTalk.SpecFlow;
using FluentAssertions;
using SpecFlowTest.Controllers;
using SpecFlowProject.Specs.Base;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpecFlowTest;
using System.Net.Http;
using System.Text;
using System.Linq;
using static SpecFlowTest.Filters.HttpGlobalExceptionFilter;

namespace SpecFlowProject.Specs.Test.Steps
{
    [Binding]
    public class WeatherForecastSteps : WeatherForecastScenarioBase
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestServer _server;

        private WeatherForecast[] _wForecastResult;
        private System.Net.HttpStatusCode _resultCode;
        private string _requestResultString;

        public WeatherForecastSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _server = CreateServer();
        }

        [Given(@"Anonymous User")]
        public void GivenAnonymousUser()
        {
            //AutoAuthorizeMiddleware.UseMiddleWare = false;
        }

        [Given(@"Authorized User")]
        public void GivenAuthorizedUser()
        {
            //For the sake of simplicity we just turn on/off using fake identity claims
            //For more advanced scenarious we should use different users
            //AutoAuthorizeMiddleware.UseMiddleWare = true;
        }

        [Then(@"the result should be NotAuthorized")]
        public void ThenTheResultShouldBeNotAuthorized()
        {
            //Add additional logic for 403 Forbidden if needed
            //_resultCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [When(@"User Gets WeatherForecast")]
        public async Task WhenUserGetsWeatherForecast()
        {
            // Act get new weather forecast
            var response = await _server.CreateClient()
                .GetAsync(Get.GetForecast());
            
            _resultCode = response.StatusCode;

            var responseBody = await response.Content.ReadAsStringAsync();
            _wForecastResult = JsonConvert.DeserializeObject<WeatherForecast[]>(responseBody);
        }

        [Then(@"the result should be OK")]
        public void ThenTheResultShouldBeOK()
        {
            _resultCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Then(@"the result should be BadRequest")]
        public void ThenTheResultShouldBeBadRequest()
        {
            _resultCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Then(@"the result should be the forecast for five days ahead")]
        public void ThenTheResultShouldBeTheForecastForFiveDaysAhead()
        {
            //Assert result
            _wForecastResult.Should().HaveCount(5);
            _wForecastResult[0].Date.Should().Be(DateTime.Now.AddDays(1).Date);

            _wForecastResult[0].TemperatureC.Should().BeGreaterThan(-70);
            _wForecastResult[0].TemperatureC.Should().BeLessThan(70);

            _wForecastResult[1].Date.Should().Be(DateTime.Now.AddDays(2).Date);
            _wForecastResult[2].Date.Should().Be(DateTime.Now.AddDays(3).Date);
            _wForecastResult[3].Date.Should().Be(DateTime.Now.AddDays(4).Date);
            _wForecastResult[4].Date.Should().Be(DateTime.Now.AddDays(5).Date);
        }

        [When(@"User Saves WeatherForecast with temperature (.*) grad celsus for day in (.*) days")]
        public async Task WhenUserSavesWeatherForecastWithTemperatureGradCelsusForDayInDays(int temp, int inDays)
        {
            var content = new StringContent(BuildWeatherForecastRequest(inDays, temp), UTF8Encoding.UTF8, "application/json");

            // Act save new weather forecast
            HttpResponseMessage response = await _server.CreateClient().PostAsync(Post.SaveForeCast, content);
            _resultCode = response.StatusCode;
            _requestResultString = await response.Content.ReadAsStringAsync();
        }


        [Then(@"the result forecast should contain saved temperature (.*) for forecast for day in (.*) days")]
        public async Task ThenTheResultForecastShouldContainSavedTemperatureForForecastForDayInDays(int temp, int inDays)
        {
            // Act get new weather forecast
            var response = await _server.CreateClient()
                .GetAsync(Get.GetForecast());

            var responseBody = await response.Content.ReadAsStringAsync();

            _wForecastResult = JsonConvert.DeserializeObject<WeatherForecast[]>(responseBody);

            _wForecastResult.Should().ContainSingle(f => f.Date == DateTime.Today.AddDays(inDays) && f.TemperatureC == temp);
        }


        [Then(@"the result should contain days validation error")]
        public void ThenTheResultShouldContainDaysValidationError()
        {
            JsonErrorResponse resp = JsonConvert.DeserializeObject<JsonErrorResponse>(_requestResultString);
            resp.Messages.Should().Contain(m => m.Contains("You cannot save a weather forecast beyond 5 days ahead or for today or in the past"));
        }

        [Then(@"the result should contain temperature validation error")]
        public void ThenTheResultShouldContainTemperatureValidationError()
        {
            JsonValidationErrorResponse resp = JsonConvert.DeserializeObject<JsonValidationErrorResponse>(_requestResultString);
            resp.Errors.TemperatureC.Should().Contain(m => m.Contains("The field TemperatureC must be between -70 and 70."));
        }


        string BuildWeatherForecastRequest(int inDays, int temp)
        {
            var forecastForDay = new WeatherForecast()
            {
                Date = DateTime.Now.AddDays(inDays).Date,
                TemperatureC = temp,
                Summary = ""
            };

            return JsonConvert.SerializeObject(forecastForDay);
        }
    }
}
