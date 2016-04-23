using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TempBot
{
	class Program
	{
		static void Main(string[] args)
		{
			var bot = new Bot(System.Configuration.ConfigurationSettings.AppSettings["DiscordBotToken"].ToString());
			bot.Connect();

			Console.ReadKey();
			Environment.Exit(0);
		}
	}
}
