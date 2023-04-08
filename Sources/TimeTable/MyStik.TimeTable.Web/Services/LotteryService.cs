using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LotteryService
    {
        private TimeTableDbContext db;
        private Lottery lottery;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        public LotteryService(TimeTableDbContext db, Guid id)
        {
            this.db = db;
            lottery = db.Lotteries.SingleOrDefault(l => l.Id == id);
        }



        public Data.Lottery GetLottery()
        {
            return lottery;
        }


        public ICollection<Course> GetLotteryCourseList()
        {
            var courseList = new List<Course>();

            if (lottery != null)
            {
                courseList.AddRange(
                    lottery.Occurrences.Select(
                        occurrence => db.Activities.OfType<Course>().SingleOrDefault(
                            c => c.Occurrence.Id == occurrence.Id)).Where(course => course != null));
            }

            return courseList;
        }
    }
}
