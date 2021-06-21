using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpecFlowTest.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class WeatherForecastDomainException : Exception
    {
        public WeatherForecastDomainException()
        { }

        public WeatherForecastDomainException(string message)
            : base(message)
        { }

        public WeatherForecastDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
