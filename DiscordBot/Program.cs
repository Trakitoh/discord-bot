using System.ComponentModel;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot;

class Program
{
    private static DiscordSocketClient _client;
    private static InteractionService _handler;

    static async Task Main(string[] args)
    {

        var token = Startup.GetConfigValue("BOT_TOKEN");

        _client = new DiscordSocketClient();
        _handler = new InteractionService(_client);

        var loggingService = new LoggingService(_client, new CommandService());

        _client.Ready += RegisterCommands;
        _client.InteractionCreated += HandleInteractions;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1); // Blocks task until the program is closed.
    }

    // TODO: read up on https://docs.discordnet.dev/guides/concepts/logging.html

    private static async Task RegisterCommands()
    {
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        Console.WriteLine("Commands registered successfully!");
        await _handler.RegisterCommandsGloballyAsync();
    }

    private static async Task HandleInteractions(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_client, interaction);
        await _handler.ExecuteCommandAsync(context, null);
    }
}