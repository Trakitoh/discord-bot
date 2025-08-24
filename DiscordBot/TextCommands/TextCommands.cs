using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.TextCommands
{
    public class TextCommands : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Repeats the input text.")]
        public async Task SayAsync([Remainder] string text)
        {
            await ReplyAsync(text);
        }

        [Command("uptime")]
        [Alias("up")]
        [Summary("States the amount of time bot has been up.")]
        public async Task UptimeAsync()
        {
            TimeSpan uptime = DateTime.Now - LoggingService.StartTime;

            string formatted = FormatUptime(uptime);

            await ReplyAsync($"I've been running for: **{formatted}**");
        }

        [Command("info")]
        [Alias("about", "botinfo", "information")]
        public async Task BotInfoAsync()
        {
            TimeSpan uptime = DateTime.Now - LoggingService.StartTime;
            string formatted = FormatUptime(uptime);

            var embed = new EmbedBuilder()
                .WithTitle(Startup.GetConfigValue("BOT_NAME"))
                .WithColor(Color.Magenta)
                .WithDescription(Startup.GetConfigValue("BOT_DESCRIPTION"))
                .AddField("🕒 Uptime", formatted, true)
                .AddField("📦 Github", Startup.GetConfigValue("GITHUB_LINK"), true)
                .AddField("⚙️ Maintainers", Startup.GetConfigValue("MAINTAINERS"), true)
                .WithFooter(footer => footer.Text = $"Requested by {Context.User.Username}")
                .WithCurrentTimestamp();

            await ReplyAsync(embed: embed.Build());
        }

        private string FormatUptime(TimeSpan uptime)
        {
            return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }
    }
}
