using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.CSV2;
using MyStik.TimeTable.DataServices.Planning;

namespace IOTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dates = new List<ActivityDate>
            {
                new ActivityDate
                {
                    Begin = new DateTime(2026, 3, 24, 17, 45, 0),
                    End = new DateTime(2026, 3, 24, 21, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 3, 31, 17, 45, 0),
                    End = new DateTime(2026, 3, 31, 21, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 4, 14, 17, 45, 0),
                    End = new DateTime(2026, 4, 14, 21, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 4, 28, 17, 45, 0),
                    End = new DateTime(2026, 4, 28, 21, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 5, 12, 17, 45, 0),
                    End = new DateTime(2026, 5, 12, 21, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 6, 2, 17, 45, 0),
                    End = new DateTime(2026, 6, 2, 21, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 6, 4, 9, 0, 0),
                    End = new DateTime(2026, 6, 4, 18, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 6, 5, 9, 0, 0),
                    End = new DateTime(2026, 6, 5, 18, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 6, 6, 9, 0, 0),
                    End = new DateTime(2026, 6, 6, 12, 00, 0),
                },
                new ActivityDate
                {
                    Begin = new DateTime(2026, 6, 16, 17, 45, 0),
                    End = new DateTime(2026, 6, 16, 21, 00, 0),
                },
            };

            var schedule = new CourseSchedule(dates, new DateTime(2026, 3, 18), new DateTime(2026,7,10));

            schedule.Init();

            foreach (var scheduleBlock in schedule.Blocks)
            {
                Console.WriteLine($"{scheduleBlock.OffsetWeekday}: {scheduleBlock.Dates.Count}");
            }

            Console.WriteLine("finished");
            Console.ReadLine();
        }
    }
}
