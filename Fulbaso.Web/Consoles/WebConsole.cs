using Fulbaso.Importer;

namespace Fulbaso.Web.Console
{
    public class WebConsole : IConsoleOutput
    {
        public void Write(string value)
        {
            ConsoleUtil.Call("importer", value);
        }
    }
}