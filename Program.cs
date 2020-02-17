using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using tymer.Commands;

namespace tymer
{
    [Command("tymer")]
    [Subcommand(
        typeof(LogTimeCommand),
        typeof(ListEntriesCommand)
    )]
    class Tyme : TymeCommandBase
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Tyme>(args);

        protected override int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 1;
        }

        public override List<string> CreateArgs() 
        {
            var args = new List<string>();
            return args;
        }
    }
}
