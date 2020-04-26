using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordSheepBot.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {

        // Ban a user
        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can ban
        [RequireUserPermission(GuildPermission.BanMembers)]
        // make sure the bot itself can ban
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            if (user.Nickname == "Marc") return;
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }

        [Command("swear")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SwearAsync()
        {
            var r = new Random();
            var swears = new List<string> { "Cock", "Dick", "Pillock", "Flange" };
            await ReplyAsync(swears.OrderBy(s => r.Next() ).First() );
        }

        //[Command("clearbot")]
        //[RequireContext(ContextType.Guild)]
        //// make sure the user invoking the command can ban
        //[RequireUserPermission(GuildPermission.Administrator)]
        //// make sure the bot itself can ban
        //[RequireBotPermission(GuildPermission.Administrator)]
        //public async Task ClearBotAsync()
        //{
        //    var chan = Context.Channel;
        //    var all = chan.GetMessagesAsync(100);
        //    var allList = all.Select(m => m.First()).ToListAsync();

        //    var messages = await allList.Where(m => m.First().Content.StartsWith("!")).Select(m => m.First().Id).ToListAsync() ;

        //    messages.ForEach(m => chan.DeleteMessageAsync(m));            
        //}
    }
}

