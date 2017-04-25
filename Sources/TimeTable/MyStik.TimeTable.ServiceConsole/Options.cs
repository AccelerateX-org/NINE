using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace MyStik.TimeTable.ServiceConsole
{
    class Options
    {
        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            var usage = new StringBuilder();
            usage.AppendLine("Service Console");
            usage.AppendLine("Parameter...");
            return usage.ToString();
        }

    }
}
