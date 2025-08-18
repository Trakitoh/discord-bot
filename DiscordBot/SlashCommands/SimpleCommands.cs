using NetCord.Services.ApplicationCommands;

namespace DiscordBot.SlashCommands;

public class SimpleCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    public async Task Ping()
    {
        await Context.Channel.SendMessageAsync("Pong!");
    }
    
    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";
}