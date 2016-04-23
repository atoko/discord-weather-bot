using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;

namespace TempBot
{
	internal struct WeatherResponse
	{
		public string coordinates;
		public string city;
		public string temperature;
		public string temperatureUnit;
		public string humidity;
		public string pressure;
		public string wind;
		public string windDirection;
		public string clouds;
		public string visibility;
		public string precipitation;
		public string weather;
		public string lastupdate;
	}
	internal class WeatherClient
	{
		static string Host = "http://api.openweathermap.org/data/2.5/";
		static string AppId = "b3ef6fd414ed24d7ccacc4b51d9ff5cb";
		static Dictionary<string, string> UnitSettings = new Dictionary<string, string>();

		private string userId = "";
		private string _unit
		{
			get
			{
				if (UnitSettings.ContainsKey(userId))
				{
					return UnitSettings[userId];
				}
				return "imperial";
			}
			set
			{
				if (UnitSettings.ContainsKey(userId))
				{
					UnitSettings[userId] = value.Trim().ToLower();
				}
				else
				{
					UnitSettings.Add(userId, value.Trim().ToLower());
				}
			}
		}
		private string _humanUnit
		{
			get
			{
				switch (this._unit)
				{
					case "imperial":
						return "F";
					case "kelvin":
						return "K";
					case "metric":
						return "C";
					default:
						return "";
				}
			}
		}
		private string _parameters
		{
			get { return String.Format("&units={0}", this._unit); }
		}

		public static WeatherClient Factory(string userId)
		{
			var client = new WeatherClient();
			client.userId = userId;
			return client;
		}
		public string SetUnits(string unit)
		{
			_unit = unit;
			return _unit;
		}
		public WeatherResponse GetLocation(string city)
		{
			var connectionString = Host + "weather?q=" + city + "&appid=" + AppId + _parameters;
			var web = JObject.Parse(new WebClient().DownloadString(connectionString));

			return new WeatherResponse
			{
				city = web["name"].ToString(),
				weather = String.Format("{0}: {1}", web["weather"][0]["main"].ToString(), web["weather"][0]["description"].ToString()),
				temperature = web["main"]["temp"].ToString(),
				temperatureUnit = this._humanUnit,
				pressure = web["main"]["pressure"].ToString(),
				humidity = web["main"]["humidity"].ToString(),
				wind = web["wind"]["speed"].ToString(),
				//windDirection = web["wind"]["deg"].ToString(),
				clouds = web["clouds"]["all"].ToString() + "%",
				lastupdate = web["dt"].ToString()
			};
		}
	}
}
