using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using log4net;
using log4net.Core;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Lottery;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Jobs
{
    public class LotteryJob
    {
        private ILog logger = LogManager.GetLogger("Lottery");

        private TimeTableDbContext db = new TimeTableDbContext();
        private Lottery lottery;



        /// <summary>
        /// Aufruf durch JobScheduler
        /// </summary>
        /// <param name="id"></param>
        public void RunLottery(Guid id)
        {
            var lotteryService = new DrawingService(db, id);
            lottery = lotteryService.Lottery;

            // Nachsehen, ob es die Lottery noch gibt
            if (lottery == null)
            {
                logger.ErrorFormat("Lottery with id {0} does not exist.", id);
                RemoveJob(id);
                return;
            }

            // schauen, ob es passt
            var today = DateTime.Today;

            if (lottery.FirstDrawing > today)
            {
                // Vor der ersten Ausführung => nix machen
                logger.InfoFormat("Verlosung {0} startet erst am {1}", lottery.Name, lottery.FirstDrawing.ToShortDateString());
                return;
            }

            if (lottery.LastDrawing < today)
            {
                logger.InfoFormat("Verlosung {0} wurde beendet am {1}", lottery.Name, lottery.LastDrawing.ToShortDateString());
                // Jon löschen, wird nicht mehr benötigt
                RemoveJob(id);
                return;
            }

            logger.InfoFormat("Start Verlosung {0}", lottery.Name);

            lotteryService.InitLotPots();
            var rounds = lotteryService.ExecuteDrawing();
            db.SaveChanges();

            logger.InfoFormat("Ende Verlosung {0} - mit {1} Iterationen", lottery.Name, rounds);
            logger.InfoFormat("Start Mailversand {0}", lottery.Name);

            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;
            drawing.Lottery = lottery;

            var mailService = new LotteryMailService(lotteryService);
            mailService.SendDrawingMails(drawing);
            logger.InfoFormat("Ende Mailversand {0}", lottery.Name);

            if (lottery.LastDrawing.Date == today)
            {
                // nach der letzen Verlosung den Job löschen
                RemoveJob(id);
            }

        }

        private void RemoveJob(Guid id)
        {
            RecurringJob.RemoveIfExists(id.ToString());
            if (lottery != null)
            {
                logger.InfoFormat("Job {0} for lottery {1} removed from schedule", id, lottery.Name);
            }
            else
            {
                logger.InfoFormat("Job {0} for unknown lottery removed from schedule", id);
            }
            return;
        }

    }
}