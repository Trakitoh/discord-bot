using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace DiscordBot;

class Program
{
    static async Task Main(string[] args)
    {

        string botToken = GetConfigValue("Discord:Token");
        GatewayClient client = new(new BotToken(botToken), new GatewayClientConfiguration
        {
            Intents = default,
            Logger = new ConsoleLogger(),
        });

        ApplicationCommandService<ApplicationCommandContext> aCS = new();
        // aCS.AddSlashCommand("ping", "Ping!", () => "Pong!");
        aCS.AddModules(typeof(Program).Assembly);

        client.InteractionCreate += async interaction =>
        {
            // Check if the interaction is an application command interaction
            if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
                return;

            // Execute the command
            var result = await aCS.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, client));

            // Check if the execution failed
            if (result is not IFailResult failResult)
                return;

            // Return the error message to the user if the execution failed
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
            }
            catch
            {
            }
        };

        // Register the commands so that you can use them in the Discord client
        await aCS.RegisterCommandsAsync(client.Rest, client.Id);
        await client.StartAsync();
        await Task.Delay(-1);
    }

    // TODO: Extract to separate file (?)
    static string GetConfigValue(string key)
    {
        var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        return configuration[key] ?? throw new ArgumentException("Config value not found or failed to load", key);
    }

    // static async Task<InteractionCallbackResponse?> HandleInteractions (Interaction interaction)
    // {
    //                 // Check if the interaction is an application command interaction
    //         if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
    //             return null;

    //         // Execute the command
    //         var result = await aCS.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, client));

    //         // Check if the execution failed
    //         if (result is not IFailResult failResult)
    //             return;

    //         // Return the error message to the user if the execution failed
    //         try
    //         {
    //             await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
    //         }
    //         catch
    //         {
    //         }
    // }
}