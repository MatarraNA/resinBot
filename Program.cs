using System.Reflection;
using System.Threading.Tasks;
using System;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using resinBot.Core.Config;

namespace inhouseBot
{
    class Program
    {
        public static DiscordSocketClient Client;
        public static CommandService Commands;

        static void Main(string[] args)
        => new Program().MainAscyn().GetAwaiter().GetResult();

        private async Task MainAscyn()
        {
            // create discord client
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Error,
                MessageCacheSize = 30
            });

            // register commands
            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Error
            });

            // event callbacks
            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            // actually initiate the client
            await Client.LoginAsync(TokenType.Bot, Config.GENERAL.BOT_TOKEN);
            await Client.StartAsync();

            // force to stall forever
            await Task.Delay(-1);
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            // commands
            try
            {
                var Message = arg as SocketUserMessage;
                var Context = new SocketCommandContext(Client, Message);

                if (Context.Message == null || Context.Message.Content == "") return;
                if (Context.User.Id != Client.CurrentUser.Id && Context.User.IsBot) return;

                int ArgPos = 0;

                if (!(Message.HasStringPrefix(Config.GENERAL.CMD_PREFIX, ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

                try
                {
                    var Result = await Commands.ExecuteAsync(Context, ArgPos, null);
                    if (!Result.IsSuccess)
                    {
                        // do nothing for failed msgs
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        await Context.Channel.SendMessageAsync($"{ex.Message}");
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }
    }
}
