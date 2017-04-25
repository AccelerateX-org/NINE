using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Test
{
    public static class TestDataService
    {
        private static TimeTableDbContext _ctx;

        public static TimeTableDbContext GetDataContext()
        {
            if (_ctx == null)
            {
                _ctx = new TimeTableDbContext();
                InitTestData();
            }
            return _ctx;
        }


        private static void InitTestData()
        {
            Room r = new Room();
            r.Name = "IT Labor";
            r.Number = "R 1.083";

            _ctx.Rooms.Add(r);



            Course tm = new Course()
            {
                Name = "Technische Mechanik",
                ShortName = "TM",
                Occurrence = new Occurrence()
                {
                    Capacity = 3,
                    IsAvailable = true,
                }
            };

            tm.Occurrence.Subscriptions.Add(new OccurrenceSubscription()
            {
                UserId = "1",
                TimeStamp = DateTime.Now,
            });

            
            _ctx.Activities.Add(tm);

            Course Info = new Course()
            {
                Name = "Infosysteme",
                ShortName = "Info",
                Occurrence = new Occurrence()
                {
                    Capacity = 25,
                    IsAvailable = true

                }
                                
            };

            _ctx.Activities.Add(Info);
                                   
            _ctx.SaveChanges();
        }

    }
}
