using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace MyStik.TimeTable.GpUntisImport
{
    class Options
    {
        [Option('s',"semester", Required=true, HelpText="Angabe des Semester in der Form SSxx oder WSxx")]
        public string Semester { get; set; }

        [Option('f', "faculty", Required = true, DefaultValue = "FK 09", HelpText = "Angabe der Fakultät in der Form FKxx")]
        public string Faculty { get; set; }

        [Option('d', "data", Required = true, HelpText = "Pfad auf das Verzeichnis mit den gpUntis Daten")]
        public string DataPath { get; set; }

        [Option('h', "hard check", Required = false, DefaultValue = false, HelpText = "Strenge Prüfung der Daten")]
        public bool CheckHard { get; set; }

        [Option('t', "testdata", Required = false, DefaultValue = false, HelpText = "Import von Testdaten")]
        public bool InsertTestData { get; set; }

        [Option('r', "reset", Required = false, DefaultValue = false, HelpText = "Setzt die Datenbank zurück")]
        public bool ResetDb { get; set; }

        [Option('c', "clear", Required = false, DefaultValue = false, HelpText = "alte Daten löschen")]
        public bool ClearData { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            var usage = new StringBuilder();
            usage.AppendLine("gpUntis Importer");
            usage.AppendLine("Parameter...");
            return usage.ToString();
        }


    }
}
