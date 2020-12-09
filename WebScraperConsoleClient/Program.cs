using System;

namespace WebScraperConsoleClient
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                foreach (string arg in args)
                    CommandParser.Parse(arg).Execute();

                int exit = 0;
                do
                {
                    Console.WriteLine("Enter a command:");

                    var input = Console.ReadLine();
                    var command = CommandParser.Parse(input);
                    exit = command.Execute();
                }
                while (exit == 0);

                return exit;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);

                return 1;
            }
        }
    }
}
