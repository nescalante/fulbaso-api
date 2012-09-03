using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fulbaso.Importer
{
    public class InternalConsole : IConsoleOutput
    {
        public void Write(string value)
        {
            Console.WriteLine(value);
        }
    }
}
