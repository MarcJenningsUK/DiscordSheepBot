using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordSheepBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            
            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            var listOfCommands = _commands.Commands.Select(c => "!" + c.Name).ToList();
            ServicesCache.ServiceNames = listOfCommands;
            //listOfCommands.ForEach(c => Console.WriteLine(c));
        }

        List<string> badWords = new List<string> { "banana", "concurrent" }; 

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

// routine to catch naughty words.
if(badWords.Any(w => message.Content.ToLower().Contains(w.ToLower())))
{
    doBadWordFilter(message);
    return;
}

            // This value holds the offset where the prefix ends
            var argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            
            // if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;
            if (!message.HasCharPrefix('!', ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await _commands.ExecuteAsync(context, argPos, _services);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

private async void doBadWordFilter(SocketUserMessage message)
{
    var content = message.Content;
    foreach (var b in badWords)
    {
        content = System.Text.RegularExpressions.Regex.Replace(content, b, "x".PadRight(b.Length, 'x'), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    var context = new SocketCommandContext(_discord, message);

    await context.Channel.SendMessageAsync($"The naughty user <@{message.Author.Id}> said : \n> {content}");
    await message.DeleteAsync();
}

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}