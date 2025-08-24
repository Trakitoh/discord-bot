using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
    private static CommandService _commands;

    private static CommandHandler? commandHandler;

    static async Task Main(string[] args)
    {
        
        var token = Startup.GetConfigValue("BOT_TOKEN");

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            // In Discord Developer Portal, go to Bot -> make sure "Message Content Intent", "Server Members Intent" && "Presence Intent" is turned on..
            // Hours Wasted figuring out this^ was the problem: 2 Hours
            GatewayIntents = GatewayIntents.AllUnprivileged |
                             GatewayIntents.MessageContent
        });
        _handler = new InteractionService(_client);
        _commands = new CommandService();
        commandHandler = new CommandHandler(_client, _commands);

        // This registers the commands, without it, text commands break
        await commandHandler.InstallCommandsAsync();

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

    private static async Task HandleMessageAsync(SocketMessage messageParam)
    {
        if (!(messageParam is SocketUserMessage message)) return;
        if (message.Source != MessageSource.User) return;

        int argPos = 0;

        if (!message.HasCharPrefix('!', ref argPos)) return;

        var context = new SocketCommandContext(_client, message);

        await _commands.ExecuteAsync(context, argPos, services: null);
    }
}