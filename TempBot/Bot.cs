using System;
using System.Threading;
using DiscordSharp;
using DiscordSharp.Objects;
using System.Collections.Generic;

namespace TempBot
{
	internal struct RouteArguments
	{
		public string parameter;
		public DiscordMember author;
	}
	internal class Bot
	{
		DiscordClient _client;
		Dictionary<string, Func<WeatherClient, RouteArguments, string>> _routes = new Dictionary<string, Func<WeatherClient, RouteArguments, string>>();

		public Bot(string token)
		{
			Console.WriteLine("Client created");
			_client = new DiscordClient(token, true);

			_client.Connected += this.OnConnected;
			_client.PrivateMessageReceived += this.OnPrivateMessageReceived;
			_client.MessageReceived += this.OnMessageReceived;

			_routes.Add("temperature", (client, arg) => {
				var data = client.GetLocation(arg.parameter);
				return String.Format("The temperature in {1} is {0}{2}", data.temperature, data.city, data.temperatureUnit);
			});

			_routes.Add("unit", (client, arg) =>
			{
				switch (arg.parameter.ToLower())
				{
					case "imperial":
					case "metric":
					case "kelvin":
						break;
					default:
						return String.Format("Sorry, {0}. Unit not recognized. Available settings are imperial|metric|kelvin.", arg.author.Username);
				}
				var unit = client.SetUnits(arg.parameter);
				return String.Format("Units set to {0} for {1}", unit, arg.author.Username);
			});
		}

		void OnConnected(object sender, DiscordConnectEventArgs e)
		{
			_client.UpdateBotStatus(true);
		}
		void OnPrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e)
		{
			Respond(e.Author,
				e.Message.Substring(1),
				e.Channel,
				(response) => { e.Author.SendMessage(response); });
		}
		void OnMessageReceived(object sender, DiscordSharp.Events.DiscordMessageEventArgs e)
		{

			if (e.MessageText.IndexOf("!") == 0)
			{
				Respond(e.Author,
					e.MessageText.Substring(1),
					e.Channel,
					(response) => { e.Channel.SendMessage(response); });
			}
		}
		void Respond(DiscordMember author, string message, DiscordChannelBase chan, Action<string> response)
		{
			var weather = WeatherClient.Factory(author.ID);
			bool handled = false;

			chan.SimulateTyping();
			foreach (var route in _routes)
			{
				if (message.IndexOf(route.Key) == 0)
				{
					var data = new RouteArguments
					{
						parameter = message.Substring(message.IndexOf(" ") + 1),
						author = author
					};
					response(route.Value(weather, data));
					handled = true;
				}
			}

			if (handled == false)
			{
				var help = "I couldn't understand your request. How can I be of help?";
				response(help);
			}
		}
		public void Connect()
		{
			try
			{
				_client.SendLoginRequest();
				Thread connect = new Thread(_client.Connect);
				connect.Start();
				Console.WriteLine("Client connected");
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
			}
		}
	}
}
