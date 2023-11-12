using log4net;
using MyStik.TimeTable.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class PublicController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Reservation(Guid id)
        {
            var logger = LogManager.GetLogger("Reservation");
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var model = new RoomReservationViewModel();

            if (room != null)
            {
                logger.InfoFormat("Reservation requested for {0}", room.Number);

                model.Room = room;
                // Alle Belegungen, die heute stattfinden und deren Ende nach dem aktuellen Zeitpunkt liegen
                model.CurrentDates = room.Dates.Where(d =>
                    d.End >= DateTime.Now && d.End <= DateTime.Today.AddDays(1) &&
                    d.Occurrence != null && !d.Occurrence.IsCanceled)
                    .OrderBy(d => d.Begin).ToList();

                var nextDate = model.CurrentDates.FirstOrDefault();

                if (nextDate != null)
                {
                    if (DateTime.Now <= nextDate.End)
                    {
                        model.CurrentDate = nextDate;
                    }
                    else
                    {
                        model.NextDate = nextDate;
                    }
                }
            }
            else
            {
                model.Message = "Unbekannter Raum";
                logger.ErrorFormat("Reservation requested for unknown id {0}", id);
            }


            return View(model);
        }
	}
}