using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord
{
    public class Program
    {
        private const string BotToken = "NjYwNjMzNzY0Mjg4MDA0MTEx.Xgfyrg.mwcIhDZ3tRxWrBqrY1qD-Mv68g0";

        static async Task Main(string[] args)
        {
            await RunBotAsync();
        }

        private static DiscordSocketClient _client;
        private static CommandService _commands;
        private static IServiceProvider _provider;

        private static async Task RunBotAsync()
        {
            _provider = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        
            await _client.LoginAsync(TokenType.Bot, BotToken);
            await _client.StartAsync();
        }
    }
}
