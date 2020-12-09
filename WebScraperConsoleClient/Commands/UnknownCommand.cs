using System;

namespace WebScraperConsoleClient.Commands
{
    internal class UnknownCommand : ConsoleCommandBase, IConsoleCommand
    {
        public string[] Args { get; }

        public UnknownCommand(string[] args)
        {
            Args = args;
        }

        public int Execute()
        {
            Console.Error.WriteLine("Unknown command");
            return 0;
        }
    }
}