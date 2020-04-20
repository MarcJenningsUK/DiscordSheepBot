using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DiscordSheepBot.Services;
using System.Net.Http;

namespace DiscordSheepBot
{
    class Program
    {
		private DiscordSocketClient _client;

		public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                var token = System.IO.File.ReadAllText("token.txt");
                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hardcoding.
                await client.LoginAsync(TokenType.Bot, token.Trim());
                await client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .BuildServiceProvider();
        }

        //      public async Task MainAsync()
        //      {
        //	_client = new DiscordSocketClient();

        //	_client.Log += Log;

        //	// Remember to keep token private or to read it from an 
        //	// external source! In this case, we are reading the token 
        //	// from an environment variable. If you do not know how to set-up
        //	// environment variables, you may find more information on the 
        //	// Internet or by using other methods such as reading from 
        //	// a configuration.

        //	_client.MessageReceived += MessageReceived;

        //	var token = System.IO.File.ReadAllText("token.txt");

        //	await _client.LoginAsync(TokenType.Bot, token.Trim());
        //	await _client.StartAsync();

        //	// Block this task until the program is closed.
        //	await Task.Delay(-1);
        //}

        //private async Task MessageReceived(SocketMessage message)
        //{
        //	if (message.Content.ToLower().StartsWith("!ping"))
        //	{
        //		await message.Channel.SendMessageAsync("You need a pong " + message.Author.Username + "?");
        //	}
        //	if (message.Content.ToLower().StartsWith("!summonthewiseone"))
        //	{
        //		await message.Channel.SendMessageAsync("By the powers granted me, I summon thee, <@!313832264792539142>");
        //	}
        //}

        //private Task Log(LogMessage msg)
        //{
        //	Console.WriteLine(msg.ToString());
        //	return Task.CompletedTask;
        //}
    }
}

/*
 * https://discord.foxbot.me/stable/guides/getting_started/first-bot.html
 */
