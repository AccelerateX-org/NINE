using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace MyStik.TimeTable.ServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Das interessiert in der Produktionsumgebung nicht
            // da hier der Connection String ohne DataDictionary auskommt
            // zeigt direkt auf einen SQL-Server
            var tempDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            tempDir = Directory.GetParent(tempDir.FullName);
            tempDir = Directory.GetParent(tempDir.FullName);
            tempDir = Directory.GetParent(tempDir.FullName);

            var dbPath = Path.Combine(tempDir.FullName, @"MyStik.TimeTable.Web\App_Data\");

            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);

            var options = new Options();
            var parser = new Parser();
            var result = parser.ParseArguments(args, options);

            if (!result)
            {
                return;
            }

            var lottery = new Lottery();
            lottery.RunLottery();

        }
    }
}
