using System.Diagnostics;
using System.Security.AccessControl;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Initializers;
using MyStik.TimeTable.DataServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data.DefaultData;
using MyStik.TimeTable.GpUntisImport.TestData;
using CommandLine;
using MyStik.TimeTable.DataServices.GpUntis;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace MyStik.TimeTable.GpUntisImport
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

            // Kann auch in der Produktion gemacht werden
            // Dürfte eher nicht vorkommen, da zuerst die Webanwendung ausgeliefert und gestartet wird
            // Die Migration findet dann dort statt
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TimeTableDbContext, TimeTable.Data.Migrations.Configuration>());


            var options = new Options();
            var parser = new Parser();
            var result = parser.ParseArguments(args, options);

            if (!result)
            {
                Console.WriteLine(options.GetUsage());
                Console.WriteLine("Fertig - Bitte Taste drücken");
                Console.ReadLine();
                return;
            }

            if (options.ResetDb)
            {
                Console.WriteLine("Datenbank wird zurückgesetzt");
                Database.SetInitializer<TimeTableDbContext>(new InitDevDatabase());
            }

            Console.WriteLine("Importiere Daten für [{0}] aus [{1}]", options.Semester, options.DataPath);



            


            var reader = new FileReader();
            Console.WriteLine("Lese Textdateien ein ...");
            reader.ReadFiles(options.DataPath);
            Console.WriteLine("Einlesen Textdateien beendet");



            if (reader.Context.ErrorMessages.Any())
            {
                Console.WriteLine("Daten werden nicht importiert! {0} Fehler", reader.Context.ErrorMessages.Count);
            }
            else
            {
                Console.WriteLine("Prüfe Vorbedingungen");

                var importer = new SemesterImport(reader.Context);

                importer.CheckGroupConsistency();
                importer.CheckRooms(options.Faculty);
                importer.CheckLecturers(options.Faculty);

                if (reader.Context.ErrorMessages.Any())
                {
                    Console.WriteLine("Daten werden nicht importiert! {0} Fehler", reader.Context.ErrorMessages.Count);
                }
                else
                {
                    Console.WriteLine("Initialisiere Semester");

                    //importer.InitSemester(options.Semester);
                    //importer.RepairSemester(options.Semester);
                    //importer.InitOrganiser(options.Faculty);

                    if (options.ClearData)
                    {
                        Console.WriteLine("Lösche Semesterdaten");
                        importer.ClearSemester();
                    }

                    Console.WriteLine("Importiere Kurse");
                    importer.ImportCourses();

                    Console.WriteLine("Initialisiere WPMs");
                    importer.InitWPMs();
                    
                    Console.WriteLine("Verschiebe Kurse");
                    // Move Date 10.07.2015 => 01.04.2015
                    // Move Date 09.07.2015 => 03.06.2015
                    importer.MoveDates(new DateTime(2015, 7, 10), new DateTime(2015, 4, 1));
                    importer.MoveDates(new DateTime(2015, 7, 9), new DateTime(2015, 6, 3));


                    // restliche Testdaten einfügen
                    if (options.InsertTestData)
                    {
                        Console.WriteLine("Füge Testdaten ein");

                        var devData = new DevData();
                        devData.InitInternationalActivities();

                        devData.InitUnionActivities();
                        devData.InitUnionNewsletter();

                        devData.InitOfficeHours(options.Semester);
                    }
                }
            }

            Console.WriteLine("Fertig - Bitte Taste drücken");
            Console.ReadLine();
        }
    }
}
