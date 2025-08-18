using System.ComponentModel;
using System.Reflection;
using Discord;
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
        var token = GetConfigValue("BOT_TOKEN");

        _client = new DiscordSocketClient();
        _handler = new InteractionService(_client);
        _client.Log += Log;
        _client.Ready += RegisterCommands;
        _client.InteractionCreated += HandleInteractions;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1); // Blocks task until the program is closed.
    }

    // TODO: Extract to separate file (?)
    static string GetConfigValue(string key)
    {
        var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        return configuration[key] ?? throw new ArgumentException("Config value not found or failed to load", key);
    }

    // TODO: read up on https://docs.discordnet.dev/guides/concepts/logging.html
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

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