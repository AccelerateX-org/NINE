using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganiserController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = Db.Organisers.OrderBy(x => x.ShortName).ToList();
            
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            // wenn es den Veranstaler nicht gibt, zurück zur Übersicht
            if (org == null)
                return RedirectToAction("Index");

            return View(org);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ActivityOrganiser model)
        {
            if (Db.Organisers.Any(x => x.ShortName.Equals(model.ShortName)))
            {
                ModelState.AddModelError("ShortName", "Ein Veranstalter mit diesem Kurznamen existiert schon.");
            }

            if (!ModelState.IsValid)
                return View(model);

            model.Id = Guid.NewGuid();
            Db.Organisers.Add(model);
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityOrganiser activityorganiser = Db.Organisers.Find(id);
            if (activityorganiser == null)
            {
                return HttpNotFound();
            }
            return View(activityorganiser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityorganiser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ShortName,HtmlColor,IsFaculty,IsStudent,IsVisible")] ActivityOrganiser activityorganiser)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(activityorganiser).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activityorganiser);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateAdmin(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            var model = new MemberUserViewModel
            {
                OrganiserId = id,
            };

            ViewBag.Organiser = org;

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateAdmin(MemberUserViewModel model)
        {
            var org = Db.Organisers.SingleOrDefault(m => m.Id == model.OrganiserId);

            if (org != null)
            {
                // vorhandene suchen
                var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(model.ShortName));
                if (member != null)
                {
                    member.IsAdmin = true;
                    Db.SaveChanges();
                }
                else
                {
                    member = new OrganiserMember
                    {
                        ShortName = model.ShortName,
                        Name = model.Name,
                        IsAdmin = true,
                    };

                    if (!string.IsNullOrEmpty(model.UserName))
                    {
                        var user = UserManager.FindByName(model.UserName);
                        if (user != null)
                        {
                            member.UserId = user.Id;

                            // wenn es keine stud-orga ist, dann muss der Nutzer "Staff" werden
                            if (!org.IsStudent)
                            {
                                user.MemberState = MemberState.Staff;
                                UserManager.Update(user);
                            }
                        }
                    }

                    org.Members.Add(member);
                }


                Db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = model.OrganiserId });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAdmin(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            var orgId = member.Organiser.Id;

            member.IsAdmin = false;

            Db.SaveChanges();

            return RedirectToAction("Details", new {id = orgId});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserList(string token)
        {
            var userDb = new ApplicationDbContext();

            var list = from l in userDb.Users
                       where l.UserName.ToUpper().Contains(token.ToUpper()) ||
                             l.LastName.ToUpper().Contains(token.ToUpper())
                       select
                           new
                           {
                               userId = l.Id,
                               userName = l.UserName,
                               firstName = l.FirstName,
                               lastName = l.LastName,
                           };


            return Json(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            return View(org);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteOrg(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            var service = new TimeTableInfoService(Db);

            var currs = org.Curricula.ToList();

            var semGroups = Db.SemesterGroups.Where(x => x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == id).ToList();
            foreach (var semesterGroup in semGroups)
            {
                var occGroups = semesterGroup.OccurrenceGroups.ToList();
                foreach (var occurrenceGroup in occGroups)
                {
                    semesterGroup.OccurrenceGroups.Remove(occurrenceGroup);
                    Db.OccurrenceGroups.Remove(occurrenceGroup);
                }

                var sub = semesterGroup.Subscriptions.ToList();
                foreach (var subscription in sub)
                {
                    Db.Subscriptions.Remove(subscription);
                }

                Db.SemesterGroups.Remove(semesterGroup);
            }
            Db.SaveChanges();


            foreach (var curriculum in currs)
            {
                service.DeleteCurriculum(curriculum);
            }

            var acts = org.Activities.ToList();
            foreach (var activity in acts)
            {
                service.DeleteActivity(activity);
            }

            var members = org.Members.ToList();
            foreach (var member in members)
            {
                Db.Members.Remove(member);
            }

            var rooms = org.RoomAssignments.ToList();
            foreach (var room in rooms)
            {
                Db.RoomAssignments.Remove(room);
            }


            Db.Organisers.Remove(org);
            Db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ClearOrg(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            var service = new TimeTableInfoService(Db);

            var currs = org.Curricula.ToList();

            var semGroups = Db.SemesterGroups.Where(x => x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == id).ToList();
            foreach (var semesterGroup in semGroups)
            {
                var occGroups = semesterGroup.OccurrenceGroups.ToList();
                foreach (var occurrenceGroup in occGroups)
                {
                    semesterGroup.OccurrenceGroups.Remove(occurrenceGroup);
                    Db.OccurrenceGroups.Remove(occurrenceGroup);
                }

                var sub = semesterGroup.Subscriptions.ToList();
                foreach (var subscription in sub)
                {
                    Db.Subscriptions.Remove(subscription);
                }

                Db.SemesterGroups.Remove(semesterGroup);
            }
            Db.SaveChanges();


            foreach (var curriculum in currs)
            {
                service.DeleteCurriculum(curriculum);
            }

            var acts = org.Activities.ToList();
            foreach (var activity in acts)
            {
                service.DeleteActivity(activity);
            }

            var members = org.Members.ToList();
            foreach (var member in members)
            {
                if (!member.IsAdmin)
                {
                    Db.Members.Remove(member);
                }
            }

            var rooms = org.RoomAssignments.ToList();
            foreach (var room in rooms)
            {
                // wenn der Raum nur eine Zuordnung ha, dann löschen
                if (room.Room.Assignments.Count == 1)
                {
                    Db.Rooms.Remove(room.Room);
                }

                Db.RoomAssignments.Remove(room);
            }


            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateCurriculum(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            var curr = org.Curricula.SingleOrDefault(x => x.ShortName.Equals("WI"));
            if (curr == null)
            {
                curr = new Curriculum
                {
                    ShortName = "WI",
                    Name = "Bachelor WI",
                    Organiser = org
                };

                Db.Curricula.Add(curr);
                Db.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Capture(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            // wenn es den Veranstaler nicht gibt, zurück zur Übersicht
            if (org == null)
                return RedirectToAction("Index");

            Session["OrgAdminId"] = org.Id;

            return RedirectToAction("Details", new {id = org.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult EditCurriculum(Guid id)
        {
            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public ActionResult EditCurriculum(Curriculum model)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == model.Id);

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(model.Organiser.ShortName));
            if (org != null && curriculum.Organiser.Id != org.Id)
            {
                // der neue Veranstalter
                curriculum.Organiser = org;
                Db.SaveChanges();
            }


            return RedirectToAction("Details", new {id = model.Organiser.Id});
        }

    }

}