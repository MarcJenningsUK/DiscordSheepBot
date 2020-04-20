using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordSheepBot
{
    class Program
    {
		private DiscordSocketClient _client;

		public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
			_client = new DiscordSocketClient();

			_client.Log += Log;

			// Remember to keep token private or to read it from an 
			// external source! In this case, we are reading the token 
			// from an environment variable. If you do not know how to set-up
			// environment variables, you may find more information on the 
			// Internet or by using other methods such as reading from 
			// a configuration.

			_client.MessageReceived += MessageReceived;

			var token = System.IO.File.ReadAllText("token.txt");

			await _client.LoginAsync(TokenType.Bot, token.Trim());
			await _client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private async Task MessageReceived(SocketMessage message)
		{
			Console.WriteLine(message.Content);
			
			if (message.Content == "!ping")
			{
				await message.Channel.SendMessageAsync("You need a pong " + message.Author.Username + "?");
			}
			
			if(message.Content.ToLower() == "!summonthewiseone")
			{
				await message.channel.SendMEssageAsync("By the powers granted me, I summon thee, <@!313832264792539142>");
			}
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}

/*
 * https://discord.foxbot.me/stable/guides/getting_started/first-bot.html
 */
