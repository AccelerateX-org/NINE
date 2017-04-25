using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace MyStik.TimeTable.FamosImport
{
    class Options
    {
        [Option('d', "data", Required = true, HelpText = "Pfadangabe für die FAMOS Datei.")]
        public string DataPath { get; set; }

        [Option('c', "clear", Required = false, DefaultValue = false, HelpText = "alte Daten löschen")]
        public bool ClearData { get; set; }

        [Option('u', "update", Required = false, DefaultValue = true, HelpText = "Daten aktualisieren")]
        public bool UpdateData { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            var usage = new StringBuilder();
            usage.AppendLine("FAMOS Importer");
            usage.AppendLine("Parameter...");
            return usage.ToString();
        }


    }
}
