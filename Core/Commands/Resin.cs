using Discord;
using Discord.Commands;
using resinBot.Core.Database;
using resinBot.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace resinBot.Core.Commands
{
    public class Resin : ModuleBase<SocketCommandContext>
    {
        private static string RESIN_IMG_URL = "https://i.imgur.com/uEfsS2O.png";

        [Command("resin")]
        public async Task DoResin()
        {
            // gather current resin values
            if( !Data.UserExists(Context.User.Id) )
            {
                await MiscUtil.EmbedResponse($"User <@{Context.User.Id}> was not found in the database. " +
                    $"Use `{Config.Config.GENERAL.CMD_PREFIX}setresin <resin> <minutes> <seconds>` to set your current resin.",
                    Context.Channel);
                return;
            }
            await DisplayResin(Context.User, Context.Channel);
            await Context.Message.DeleteAsync();
        }

        public async Task DisplayResin(IUser user, IMessageChannel channel )
        {
            // user was found, gather data
            int lastResin = Data.GetLastResin(user.Id);
            long lastTimeTicks = Data.GetResinTicks(user.Id);
            DateTime lastDateTime = new DateTime(lastTimeTicks);

            // get offset
            var offset = DateTime.Now - lastDateTime;

            // convert to resin
            int elapsedResin = (int)offset.TotalMinutes / Config.Config.GENERAL.REGEN_RATE_MINS;
            int totalResin = Math.Clamp(lastResin + elapsedResin, 0, Config.Config.GENERAL.MAX_RESIN);

            // time until next resin
            TimeSpan nextResinSpan = TimeSpan.FromMinutes(offset.TotalMinutes % Config.Config.GENERAL.REGEN_RATE_MINS);
            nextResinSpan = TimeSpan.FromMinutes(Config.Config.GENERAL.REGEN_RATE_MINS).Subtract(nextResinSpan);

            // display it pretty
            var embed = new EmbedBuilder();
            embed.WithAuthor("Resin Tracker!", user.GetAvatarUrl());
            embed.WithDescription($"Resin Data for <@{user.Id}>");
            embed.WithThumbnailUrl(RESIN_IMG_URL);
            embed.AddField($"__Resin__", $"**{totalResin}**", true);
            embed.AddField($"__Next Resin__", $"**{nextResinSpan.Minutes}m{nextResinSpan.Seconds}s**", true);

            // display
            await channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("setresin")]
        public async Task DoSetResin(int resinValue, int mins, int seconds )
        {
            // some prereqs
            resinValue = Math.Clamp(resinValue, 0, Config.Config.GENERAL.MAX_RESIN);
            mins = Math.Clamp(mins, 0, Config.Config.GENERAL.REGEN_RATE_MINS-1);
            seconds = Math.Clamp(seconds, 0, 59);

            // inverse values
            int offsetMins = (Config.Config.GENERAL.REGEN_RATE_MINS-1) - mins;
            int offsetSeconds = 60 - seconds;

            // calculate offset
            TimeSpan offset = TimeSpan.FromMinutes(offsetMins);
            offset += TimeSpan.FromSeconds(offsetSeconds);

            // subtract it from current time
            var time = DateTime.Now;
            time = time.Subtract(offset);

            // set values
            Data.SetResinTicks(Context.User.Id, time.Ticks);
            Data.SetLastResin(Context.User.Id, resinValue);

            // display everything
            var embed = new EmbedBuilder();
            embed.WithAuthor("Resin Timestamp!", Context.User.GetAvatarUrl());
            embed.WithDescription($"Updated resin values for <@{Context.User.Id}>");
            embed.WithThumbnailUrl(RESIN_IMG_URL);
            embed.AddField($"__Resin__", $"**{resinValue}**", true);
            embed.AddField($"__Duration Offset__", $"**{offset.Minutes}m{offset.Seconds}s**", true);
            embed.AddField($"__Time until next Resin__", $"**{mins}m{seconds}s**", true);
            await Context.Channel.SendMessageAsync("", false, embed.Build());

            // delete orig
            await Context.Message.DeleteAsync();
        }

        [Command("spend")]
        public async Task DoSpend(int resinValue)
        {
            // TODO CHECK IF EXISTS FIRST
            
            // some prereqs
            resinValue = Math.Clamp(resinValue, 0, Config.Config.GENERAL.MAX_RESIN);

            // get old value
            int oldResin = Data.GetLastResin(Context.User.Id);
            int newResin = oldResin - resinValue;

            // update values
            Data.SetLastResin(Context.User.Id, newResin);

            // calcualte new resin ticks
            long lastTimeTicks = Data.GetResinTicks(Context.User.Id);
            DateTime lastDateTime = new DateTime(lastTimeTicks);
            var offset = DateTime.Now - lastDateTime; // get offset
            TimeSpan nextResinSpan = TimeSpan.FromMinutes(offset.TotalMinutes % Config.Config.GENERAL.REGEN_RATE_MINS); // next resin
            var time = DateTime.Now;
            time = time.Subtract(offset);
            Data.SetResinTicks(Context.User.Id, time.Ticks);

            // display it!
            await DisplayResin(Context.User, Context.Channel);
            await Context.Message.DeleteAsync();
        }
    }
}
