using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using MyStik.TimeTable.Data;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace MyStik.TimeTable.FamosImport
{
    class Program
    {
        static void Main(string[] args)
        {
            // für lokale Datei
            // AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Empty));

            // für Date in Web-App
            var tempDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            tempDir = Directory.GetParent(tempDir.FullName);
            tempDir = Directory.GetParent(tempDir.FullName);
            tempDir = Directory.GetParent(tempDir.FullName);

            var dbPath = Path.Combine(tempDir.FullName, @"MyStik.TimeTable.Web\App_Data\");
            
            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TimeTableDbContext, TimeTable.Data.Migrations.Configuration>());


            /*
            var db = new TimeTableDbContext();
            var fk09 = db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FK 09"));
            fk09.ShortName = "FK 09";
            var fs09 = db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FS09"));
            fs09.ShortName = "FS 09";
            db.SaveChanges();
            */



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

            var import = new FamosImport();

            if (options.ClearData)
            {
                // alle FAMOS-Daten löschen
                import.DeleteImport();
            }

            if (options.UpdateData)
            {
                import.ReadFile(options.DataPath, 0);
                // Räume prüfen und ggf. ergänzen
                import.CheckRooms();

                // Veranstalter prüfen und ggf. ergänzen
                import.CheckOrganiser();


                // Importdaten bündeln und dabei prüfen
                import.CheckActivities();

                // Jetzt die Termine importieren
                import.ImportDates();
            }

            Console.WriteLine("Fertig - Bitte Taste drücken");
            Console.ReadLine();
        }
    }
}
