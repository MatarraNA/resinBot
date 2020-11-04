using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace resinBot.Core.Commands
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("die")]
        public async Task DoDie()
        {
            // is user me?
            if (!Context.User.Id.Equals(142068805416255490)) return;

            // it is me poggies, die bot
            await Context.Channel.SendMessageAsync("Exiting...");
            Environment.Exit(-1);
        }
    }
}
