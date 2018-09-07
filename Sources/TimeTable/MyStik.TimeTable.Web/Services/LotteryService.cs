using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using Postal;

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
