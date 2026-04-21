using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.DataServices.IO.CSV2;

namespace IOTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new FileReader();
            reader.ProcessFile(".\\..\\..\\data\\csv_test_fk05.csv");

            var invalidCourses = reader.Context.Courses.Where(x => !x.Key.IsValid);
            var validCourses = reader.Context.Courses.Where(x => x.Key.IsValid);

            var keyValuePairs = invalidCourses.ToList();

            Console.WriteLine($@"Valid Courses {validCourses.Count()}");
            Console.WriteLine($@"Invalid Courses {keyValuePairs.Count()}");

            foreach (var invalidCourse in keyValuePairs)
            {
                foreach (var error in invalidCourse.Key.Errors)    
                {
                    Console.WriteLine($@"{invalidCourse.Key.Id}: {error}");
                }
            }

            // zuerst das vorhandensein der Objekte prüfen, dann die Daten importieren, damit die Fehlermeldungen nicht durch fehlende Objekte verursacht werden
            // Stammdaten: Org, Semester, Segment, Course, Activity, Room, Lecturer, Modul, Kohorten
            var importer = new SemesterImport(reader.Context);


            foreach (var validCourse in validCourses)
            {
                importer.CheckCourse(validCourse.Key);
                if (!validCourse.Key.IsValid) continue;
                foreach (var dataSet in validCourse.Value)
                {
                    importer.CheckLecturer(validCourse.Key, dataSet.Lecturers);
                    importer.CheckRooms(validCourse.Key, dataSet.Rooms);
                    //importer.CheckCohorts(validCourse.Key, dataSet.Cohorts);
                    //importer.CheckModules(validCourse.Key, dataSet.Modules);
                }
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
