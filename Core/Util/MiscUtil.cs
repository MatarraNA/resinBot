using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace resinBot.Core.Util
{
    public static class MiscUtil
    {
        /// <summary>
        /// Send a simple emote msg in chat
        /// </summary>
        /// <param name="text"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task EmbedResponse(string text, ISocketMessageChannel channel)
        {
            var embedBuilder = new EmbedBuilder()
            {
                Description = text
            };
            await channel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}
