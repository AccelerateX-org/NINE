using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class BulletinBoardController : BaseController
    {
        // GET: EventSummary
        public ActionResult Index()
        {
            var org = GetMyOrganisation();


            var dateService = new ActivityDateService(Db);

            ;
#if DEBUG
            var now = new DateTime(2020, 6, 16, 9, 12, 00);
#else
            var now = DateTime.Now;
#endif

            var ads = Db.Advertisements
                .Where(x => x.Owner.Organiser.Id == org.Id)
                .OrderByDescending(x => x.Created)
                .Take(5).ToList();



            var model = new EventSummaryModel
            {
                Now = now,
                Dates = dateService.GetDates(now, org),
                Advertisements = ads
            };

            return View(model);
        }
    }

    public class EventSummaryModel
    {
        public DateTime Now { get; set; }

        public ICollection<ActivityDate> Dates { get; set; }

        public ICollection<Advertisement> Advertisements { get; set; }
    }
}
