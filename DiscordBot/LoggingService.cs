using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    // TODO: Build out logs to a text document
    public class LoggingService
    {

        private static readonly string LogDirectory = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, "logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "logs.txt");
        private static readonly object _lock = new object();

        public LoggingService(DiscordSocketClient client, CommandService? command = null)
        {
            Console.WriteLine(Path.GetFullPath(LogFilePath));

            client.Log += LogAsync;

            if (command != null)
                command.Log += LogAsync;
        }

        public Task LogAsync(LogMessage message)
        {
            var logText = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{message.Severity}] {message.Source}: {message.Message}";

            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
            {
                Console.WriteLine($"[General/{message.Severity}] {message}");
            }

            lock (_lock)
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                File.AppendAllText(LogFilePath, logText + Environment.NewLine);
            }

            return Task.CompletedTask;
        }

        public Task getCompletedTask
        {
            get { return Task.CompletedTask; }
        }

    }

}
