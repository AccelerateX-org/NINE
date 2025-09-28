using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace MyStik.TimeTable.Web.Controllers
{
    public class InfoscreensController : BaseController
    {
        // GET: Infoscreens
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue)
            {
                var model = new List<ActivityOrganiser>();
                var org = GetOrganiser(id.Value);
                model.Add(org);
                var userRight = GetUserRight(org);
                ViewBag.UserRight = userRight;
                return View(model);
            }
            else
            {
                var model = Db.Organisers.Where(x => x.Infoscreens.Any()).OrderBy(g => g.ShortName).ToList();
                var userRight = GetUserRight();
                ViewBag.UserRight = userRight;
                return View(model);
            }
        }

        public ActionResult Details(Guid id)
        {
            var model = Db.Infoscreens.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var userRight = GetUserRight();
            ViewBag.UserRight = userRight;
            return View(model);
        }

        public ActionResult Create(Guid id)
        {
            var org = GetOrganisation(id);

            if (Db.Infoscreens.Any(x => x.Organisers.Any(o => o.Id == org.Id)))
            {
                // Es gibt schon einen
                return RedirectToAction("Details", "Infoscreens", new { id = org.Infoscreens.First().Id });
            }

            // Dummy anlegen
            var screen = new Infoscreen
            {
                Tag = "RFOY",
                Name = "R-Bau Foyer",
                Description = "Infoscreen im Foyer des R-Baus",
                Organisers = new List<ActivityOrganiser> { org },
                Pages = new List<InfoscreenPage>(),
                PublicTransporrtInfo = "Hochschule München (Lothstr)",
                PublicTransporrtUrl = "eyJsYW5ndWFnZSI6eyJkZXBhcnR1cmUiOiJBYmZhaHJ0IiwidHJhaW5TdG9wcyI6IkhhbHRlc3RlbGxlbiIsImRpcmVjdGlvbiI6IlJpY2h0dW5nIiwiZm9vdGVyTm90ZSI6IqkgQ29weXJpZ2h0IiwiZm9vdGVyVGV4dCI6IldlaXRlcmUgRmFocnBsYW5hdXNr/G5mdGUgdW50ZXIgd3d3Lm12di1hdXNrdW5mdC5kZSBvZGVyIG1pdCBkZXIgTVZWLUFwcCIsImhlYWRlclRleHQiOiJBYmZhaHJ0ZW4gZvxyIGhldXRlLCAiLCJsYW5ndWFnZSI6ImRlIiwibGluZSI6IkxpbmllIiwibGl2ZSI6IkxpdmUiLCJzdG9wIjoiSGFsdGVzdGVsbGUiLCJ0cmFjayI6IkdsZWlzIn0sImlzRnVsbHNjcmVlbiI6ZmFsc2UsInN0YXRpb25zIjpbeyJzdGF0aW9uIjp7InVzYWdlIjoic2YiLCJ0eXBlIjoiYW55IiwibmFtZSI6Ik38bmNoZW4sIEhvY2hzY2h1bGUgTfxuY2hlbiAoTG90aHN0ci4pIiwic3RhdGVsZXNzIjoiOTEwMDAwMTIiLCJhbnlUeXBlIjoic3RvcCIsInNvcnQiOiIyIiwicXVhbGl0eSI6IjQyOCIsImJlc3QiOiIxIiwib2JqZWN0IjoiSG9jaHNjaHVsZSBN/G5jaGVuIChMb3Roc3RyLikiLCJtYWluTG9jIjoiTfxuY2hlbiIsIm1vZGVzIjoiNCw1IiwicmVmIjp7ImlkIjoiOTEwMDAwMTIiLCJnaWQiOiJkZTowOTE2MjoxMiIsIm9tYyI6IjkxNjIwMDAiLCJwbGFjZUlEIjoiMSIsInBsYWNlIjoiTfxuY2hlbiIsImNvb3JkcyI6IjEyODYxNjcuMDAwMDAsNTg2NzQ0Mi4wMDAwMCJ9LCJpZCI6ImRlOjA5MTYyOjEyIn0sImxpbmVzIjpbeyJudW1iZXIiOiIyMCIsInN5bWJvbCI6IjAyMDIwLnN2ZyIsImRpcmVjdGlvbiI6IkthcmxzcGxhdHogKFN0YWNodXMpIiwic3RhdGVsZXNzIjoic3dtOjAyMDIwOkc6SDowMTUiLCJuYW1lIjoiVHJhbSJ9LHsibnVtYmVyIjoiMjAiLCJzeW1ib2wiOiIwMjAyMC5zdmciLCJkaXJlY3Rpb24iOiJNb29zYWNoIiwic3RhdGVsZXNzIjoic3dtOjAyMDIwOkc6UjowMTUiLCJuYW1lIjoiVHJhbSJ9LHsibnVtYmVyIjoiMjEiLCJzeW1ib2wiOiIwMjAzMS5zdmciLCJkaXJlY3Rpb24iOiJTdC4tVmVpdC1TdHJh32UiLCJzdGF0ZWxlc3MiOiJzd206MDIwMjE6RzpIOjAxNSIsIm5hbWUiOiJUcmFtIn0seyJudW1iZXIiOiIyMSIsInN5bWJvbCI6IjAyMDMxLnN2ZyIsImRpcmVjdGlvbiI6Ildlc3RmcmllZGhvZiIsInN0YXRlbGVzcyI6InN3bTowMjAyMTpHOlI6MDE1IiwibmFtZSI6IlRyYW0ifSx7Im51bWJlciI6Ik4yMCIsInN5bWJvbCI6IjMyTjIwLnN2ZyIsImRpcmVjdGlvbiI6IkthcmxzcGxhdHogKFN0YWNodXMpIiwic3RhdGVsZXNzIjoic3dtOjMyOTIwOkc6SDowMTUiLCJuYW1lIjoiTmFjaHRUcmFtIn0seyJudW1iZXIiOiJOMjAiLCJzeW1ib2wiOiIzMk4yMC5zdmciLCJkaXJlY3Rpb24iOiJNb29zYWNoIiwic3RhdGVsZXNzIjoic3dtOjMyOTIwOkc6UjowMTUiLCJuYW1lIjoiTmFjaHRUcmFtIn0seyJudW1iZXIiOiIxNTMiLCJzeW1ib2wiOiIwMzE1My5zdmciLCJkaXJlY3Rpb24iOiJPZGVvbnNwbGF0eiIsInN0YXRlbGVzcyI6InN3bTowMzE1MzpHOkg6MDE1IiwibmFtZSI6IkJ1cyJ9LHsibnVtYmVyIjoiMTUzIiwic3ltYm9sIjoiMDMxNTMuc3ZnIiwiZGlyZWN0aW9uIjoiVHJhcHBlbnRyZXVzdHJh32UiLCJzdGF0ZWxlc3MiOiJzd206MDMxNTM6RzpSOjAxNSIsIm5hbWUiOiJCdXMifV0sImFsbExpbmVzIjpbeyJudW1iZXIiOiIyMCIsInN5bWJvbCI6IjAyMDIwLnN2ZyIsImRpcmVjdGlvbiI6IkthcmxzcGxhdHogKFN0YWNodXMpIiwic3RhdGVsZXNzIjoic3dtOjAyMDIwOkc6SDowMTUiLCJuYW1lIjoiVHJhbSJ9LHsibnVtYmVyIjoiMjAiLCJzeW1ib2wiOiIwMjAyMC5zdmciLCJkaXJlY3Rpb24iOiJNb29zYWNoIiwic3RhdGVsZXNzIjoic3dtOjAyMDIwOkc6UjowMTUiLCJuYW1lIjoiVHJhbSJ9LHsibnVtYmVyIjoiMjEiLCJzeW1ib2wiOiIwMjAzMS5zdmciLCJkaXJlY3Rpb24iOiJTdC4tVmVpdC1TdHJh32UiLCJzdGF0ZWxlc3MiOiJzd206MDIwMjE6RzpIOjAxNSIsIm5hbWUiOiJUcmFtIn0seyJudW1iZXIiOiIyMSIsInN5bWJvbCI6IjAyMDMxLnN2ZyIsImRpcmVjdGlvbiI6Ildlc3RmcmllZGhvZiIsInN0YXRlbGVzcyI6InN3bTowMjAyMTpHOlI6MDE1IiwibmFtZSI6IlRyYW0ifSx7Im51bWJlciI6Ik4yMCIsInN5bWJvbCI6IjMyTjIwLnN2ZyIsImRpcmVjdGlvbiI6IkthcmxzcGxhdHogKFN0YWNodXMpIiwic3RhdGVsZXNzIjoic3dtOjMyOTIwOkc6SDowMTUiLCJuYW1lIjoiTmFjaHRUcmFtIn0seyJudW1iZXIiOiJOMjAiLCJzeW1ib2wiOiIzMk4yMC5zdmciLCJkaXJlY3Rpb24iOiJNb29zYWNoIiwic3RhdGVsZXNzIjoic3dtOjMyOTIwOkc6UjowMTUiLCJuYW1lIjoiTmFjaHRUcmFtIn0seyJudW1iZXIiOiIxNTMiLCJzeW1ib2wiOiIwMzE1My5zdmciLCJkaXJlY3Rpb24iOiJPZGVvbnNwbGF0eiIsInN0YXRlbGVzcyI6InN3bTowMzE1MzpHOkg6MDE1IiwibmFtZSI6IkJ1cyJ9LHsibnVtYmVyIjoiMTUzIiwic3ltYm9sIjoiMDMxNTMuc3ZnIiwiZGlyZWN0aW9uIjoiVHJhcHBlbnRyZXVzdHJh32UiLCJzdGF0ZWxlc3MiOiJzd206MDMxNTM6RzpSOjAxNSIsIm5hbWUiOiJCdXMifV0sImxlYWRUaW1lTWludXRlcyI6NX1dLCJsaW5lcyI6W10sIm1heFJlc3VsdHMiOjEwLCJmZXRjaEludGVydmFsSW5NaW51dGVzIjozLCJzaG93Tm90aWZpY2F0aW9uIjp0cnVlfQ=="
            };

            // Seiten anlegen
            var page1 = CreateRoomAllocationPage(org, "Lehrräume 1. und 2. Stock (rechter Flügel)", new[] { "R 1.084", "R 1.085", "R 1.086", "R 1.087", "R 2.088", "R 2.089", "R 2.090", "R 2.091" });
            page1.Index = 1;
            screen.Pages.Add(page1);

            var page2 = CreateRoomAllocationPage(org, "Lehrräume 3. und 4. Stock (rechter Flügel)", new[] { "R 3.096", "R 3.097", "R 3.098", "R 3.099", "R 4.077", "R 4.078", "R 4.079", "R 4.080" });
            page2.Index = 2;
            screen.Pages.Add(page2);


            Db.Infoscreens.Add(screen);
            Db.SaveChanges();

            return RedirectToAction("Index", new { id = org.Id });
        }

        private InfoscreenPage CreateRoomAllocationPage(ActivityOrganiser org, string name, string[] rooms)
        {
            var group = new RoomAllocationGroup
            {
                Name = name,
                Description = "",
                Organiser = org,
                RoomAllocations = new List<RoomAllocation>()
            };

            Db.RoomAllocationGroups.Add(group);

            foreach (var room in rooms)
            {
                var alloc = new RoomAllocation
                {
                    Room = Db.Rooms.SingleOrDefault(r => r.Number.Equals(room)),
                    Group = group
                };
                Db.RoomAllocations.Add(alloc);
            }

            var page = new InfoscreenPage
            {
                Index = 0,
                Type = InfoscreeenPageType.Playing,
                Name = name,
                RoomAllocationGroup = group
            };

            Db.InfoscreenPagess.Add(page);

            return page;
        }
    }
}