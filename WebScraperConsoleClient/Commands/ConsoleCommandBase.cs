using System.Collections.Generic;
using System.Linq;

namespace WebScraperConsoleClient.Commands
{
    public abstract class ConsoleCommandBase
    {
        public string GetArgumentValue(IEnumerable<string> args, string argName)
        {
            return args.SkipWhile(i => i != argName).Skip(1).Take(1).FirstOrDefault();
        }
    }
}
