using System.Linq;

using WebScraperConsoleClient.Commands;

namespace WebScraperConsoleClient
{
    public class Argument
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public static class CommandParser
    {
        public static IConsoleCommand Parse(string commandString)
        {
            var parts = commandString.Split(' ');
            var name = parts.First();
            var args = parts.Skip(1).ToArray();
            //TODO: make a help command
            switch (name)
            {
                case "q":
                case "Q":
                case "quit":
                case "Quit":
                case "exit":
                case "Exit":
                    return new ExitCommand(args);
                case "getImages":
                case "GetImages":
                    return new GetImagesCommand(args);
                default:
                    return new UnknownCommand(args);
            }
        }
    }
}
