using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace tymer.Commands
{
    [HelpOption("--help")]
    abstract class TymeCommandBase
    {
        public abstract List<string> CreateArgs();

        protected virtual int OnExecute(CommandLineApplication app)
        {
            var args = CreateArgs();
            // Console.WriteLine("Result = tyme " + ArgumentEscaper.EscapeAndConcatenate(args));
            return 0;
        }
    }
}
