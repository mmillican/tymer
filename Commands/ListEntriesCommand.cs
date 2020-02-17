using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace tymer.Commands
{
    [Command("list", Description = "List time entries")]
    class ListEntriesCommand : TymeCommandBase
    {
        protected override int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return base.OnExecute(app);
        }        

        public override List<string> CreateArgs() 
        {
            var args = new List<string>();
            return args;
        }
    }
}
