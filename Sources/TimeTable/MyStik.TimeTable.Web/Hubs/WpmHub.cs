using System;
using log4net;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class WpmHub : Hub
    {
        /// <summary>
        /// Ad hoc Ausführung einer Platzverlosung
        /// </summary>
        /// <param name="id">Id der Platzverlosung</param>
        public void ExecuteLottery(Guid id)
        {
            var logger = LogManager.GetLogger("Lottery");

            var report = new LotteryDrawingReportModel();

            var db = new TimeTableDbContext();

            var lotteryService = new LotteryService(db, id);


            var msg = "Vorbereitung";
            var perc1 = 33;
            var perc2 = 0;
            Clients.Caller.updateProgress(msg, perc1, perc2);

            var lottery = lotteryService.GetLottery();

            if (lottery == null)
            {
                logger.FatalFormat("verlosung [{0}] existiert nicht", id);
                return;
            }


            report.Lottery = lottery;

            var wpmList = lotteryService.GetLotteryCourseList();

            var i = 0;
            var n = wpmList.Count;

            msg = string.Format("Verlosung für {0} Kurse", n);
            perc1 = 66;
            perc2 = 0;
            Clients.Caller.updateProgress(msg, perc1, perc2);


            // jede Vorlesung einzeln durchgehen
            logger.InfoFormat("Starte manuelle Verlosung {0} für {1} Kurse", lottery.Name, wpmList.Count);

            foreach (var wpm in wpmList)
            {
                i++;

                msg = string.Format("Verlosung für {0} Kurs", wpm.Name);
                perc1 = 66;
                perc2 = (i * 100) / n;
                Clients.Caller.updateProgress(msg, perc1, perc2);

                lotteryService.RunLotteryForCourse(wpm);

                //report.Courses.Add(courseReport);
            }

            logger.InfoFormat("Manuelle Verlosung {0} beendet", lottery.Name);


            msg = "Verlosung abgeschlossen";
            perc1 = 100;
            perc2 = 100;
            Clients.Caller.updateProgress(msg, perc1, perc2);

            // jetzt noch den Report senden
            var html = report.CreateHtml();
            Clients.Caller.showReport(html);
        }

    }
}

