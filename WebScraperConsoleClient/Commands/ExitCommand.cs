namespace WebScraperConsoleClient.Commands
{
    public class ExitCommand : ConsoleCommandBase, IConsoleCommand
    {
        public string[] Args { get; }

        public ExitCommand(string[] args)
        {
            Args = args;
        }

        public int Execute()
        {
            return -1;
        }
    }
}
