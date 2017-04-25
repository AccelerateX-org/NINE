using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.ServiceConsole
{
    class Lottery
    {
        internal void RunLottery()
        {
            /*
            var logger = LogManager.GetLogger("Lottery");

            try
            {
                var db = new TimeTableDbContext();

                var semester = new SemesterService().GetNewestSemester();

                var lotteryService = new LotteryService();

                logger.InfoFormat("Starte Verlosung für Semester {0}", semester.Name);

                var wpmList = lotteryService.GetLottery(semester);

                foreach (var wpm in wpmList)
                {
                    lotteryService.RunLotteryForCourse(wpm);
                }

                logger.InfoFormat("Verlosung für Semester {0} beendet", semester.Name);
            }
            catch (Exception ex)
            {
                logger.FatalFormat("Abbruch wegen Fehler: {0}", ex.Message);
            }
            */
        }
    }
}
