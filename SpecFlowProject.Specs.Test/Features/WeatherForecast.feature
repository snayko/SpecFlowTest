Feature: WeatherForecast
	WeatherForecast feature(controller) to get weather for upcomming 5 days
	or to save weather for one of the five upcomming days.

@get
Scenario: Get Weather Forecast
	Given Anonymous User
	When User Gets WeatherForecast
	Then the result should be OK
		And the result should be the forecast for five days ahead

#post OK
@post
Scenario Outline: Save Weather Forecast
	Given Authorized User
	When User Saves WeatherForecast with temperature <temp> grad celsus for day in <in_days> days
	Then the result should be OK
		And the result forecast should contain saved temperature <temp> for forecast for day in <in_days> days

		Examples:
			| temp | in_days | 
			|   12 |   1     | 
			|   20 |   2     |
			|   69 |   3     |
			|  -69 |   4     |
			|   25 |   5     |

#post validation error day is incorrect
Scenario Outline: Save Weather Forecast Error Day is incorrect
	Given Authorized User
	When User Saves WeatherForecast with temperature 20 grad celsus for day in <in_days> days 
	Then the result should be BadRequest
		And the result should contain days validation error
		
		Examples:
			| in_days | 
			|   -1    | 
			|   -100  |
			|   6     |
			|   7     |
			|   100   |

#post validation error temperature is incorrect
Scenario Outline: Save Weather Forecast Error Temperature is incorrect
	Given Authorized User
	When User Saves WeatherForecast with temperature <temp> grad celsus for day in 3 days 
	Then the result should be BadRequest
		And the result should contain temperature validation error

		Examples:
			| temp    | 
			|   -71   | 
			|   71    |
			|   -100  |
			|   100   |

#not authorized user error 401
@ignore
Scenario: Save Weather Forecast NotAuthorized
	This scenario needs to be implemented still
	Given Anonymous User
	When User Saves WeatherForecast with temperature 20 grad celsus for day in 4 days
	Then the result should be NotAuthorized