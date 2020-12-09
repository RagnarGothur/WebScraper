namespace WebScraperConsoleClient.Commands
{
    public interface IConsoleCommand
    {
        public string[] Args { get; }

        public int Execute();
    }
}
