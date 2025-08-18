
using Discord.Interactions;

namespace DiscordBot.SlashCommands;

public class SimpleCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Pings the bot, so we know its alive")]
    public async Task Ping() => await RespondAsync(text: "pong!");
}