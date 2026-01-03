using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganiserMembersController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);
            var prevSemester = SemesterService.GetPreviousSemester(semester);
            
            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester,
                NextSemester = nextSemester,
                PreviousSemester = prevSemester
            };

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var memberPage = organiser.Members.OrderBy(m => m.Name);

            var myUser = UserManager.FindByName(User.Identity.Name);

            var vorSemester = new SemesterService().GetSemester(semester, 1);

            var userService = new UserInfoService();

            foreach (var member in memberPage)
            {
                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    User = member.UserId != null ? userService.GetUser(member.UserId) : null,
                });
            }

            var userMember = model.Members.Where(x => x.User != null).ToList();
            foreach (var member in userMember)
            {
                var n = userMember.Count(x => x.User.Email.Equals(member.User.Email));

                if (n > 1)
                {
                    member.IsDouble = true;
                }
            }




            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Active(Guid? id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(id);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var memberPage = organiser.Members.OrderBy(m => m.Name);

            var courseService = new CourseInfoService(Db);

            foreach (var member in memberPage)
            {
                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    ActiveCourses = courseService.GetCourses(semester, member)
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Roles()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            model.Roles = organiser.Members.GroupBy(x => x.Role).ToList();


            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View(model);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Today()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var day = DateTime.Today;
            var nextDay = day.AddDays(1);

            var memberPage = organiser.Members.Where(x => x.Dates.Any(d => d.Begin >= day && d.End < nextDay)).OrderBy(m => m.Name);

            var vorSemester = new SemesterService().GetSemester(semester, 1);


            foreach (var member in memberPage)
            {
                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    ItsMe = false,
                    IsActive = true,
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;


            return View(model);

        }

        public ActionResult MoveDates()
        {
            var model = new OrganiserViewModel();

            model.Organiser = GetMyOrganisation();

            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        [HttpPost]
        public JsonResult MoveDates(Guid sourceDozId, Guid targetDozId)
        {
            var sourceMember = Db.Members.SingleOrDefault(x => x.Id == sourceDozId);
            var targetMember = Db.Members.SingleOrDefault(x => x.Id == targetDozId);

            // alle termine, des abgebenden Dozenten
            var dates = Db.ActivityDates.Where(x => x.Hosts.Any(h => h.Id == sourceMember.Id) && x.End > DateTime.Now).OrderBy(x => x.Begin).ToList();

            // Umhängen
            foreach (var date in dates)
            {
                date.Hosts.Remove(sourceMember);
                // nur hinzufügen, falls nicht eh schon dabe
                if (!date.Hosts.Contains(targetMember))
                {
                    date.Hosts.Add(targetMember);
                }
            }
            Db.SaveChanges();

            // jetzt noch eine Mail senden
            var userService = new UserInfoService();
            
            var mailModel = new MemberMoveDateMailModel
            {
                SourceUser = userService.GetUser(sourceMember.UserId),
                TargetUser = userService.GetUser(targetMember.UserId),
                SourceMember = sourceMember,
                TargetMember = targetMember,
                AdminMember = GetMyMembership(),
                User = GetCurrentUser(),
                Dates = dates
            };

            var mail = new MailController();
            mail.MemberMoveDateEMail(mailModel).Deliver();


            // Redirect
            return Json(new { result = "Redirect", url = Url.Action("Index")});
        }

        public ActionResult Manage()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var memberPage = organiser.Members.OrderBy(m => m.Name);

            var myUser = UserManager.FindByName(User.Identity.Name);


            var semester = SemesterService.GetSemester(DateTime.Today);
            var vorSemester = SemesterService.GetPreviousSemester(semester);
            var vorVorSemester = SemesterService.GetPreviousSemester(vorSemester);

            var courseService = new CourseInfoService(Db);
            var userService = new UserInfoService();

            foreach (var member in memberPage)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    //User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                    IsActive = courseService.IsActive(member, semester),
                    WasActiveLastSemester = courseService.IsActive(member, vorSemester),
                    WasActiveLastYear = courseService.IsActive(member, vorVorSemester),
                    User = userService.GetUser(member.UserId)
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;
            ViewBag.LastYear = vorVorSemester.Name;


            return View(model);
        }


        public ActionResult PersonalProfile()
        {
            var member = GetMyMembership();

            var model = new MemberProfileViewModel();
            model.MemberId = member.Id;
            model.Name = member.Name;
            model.FirstName = member.FirstName;
            model.Title = member.Title;
            model.UrlProfile = member.UrlProfile;
            model.Description = member.Description;
            model.ShowDescription = member.ShowDescription;

            ViewBag.Member = member;

            return View("Profile", model);
        }

        [HttpPost]
        public ActionResult PersonalProfile(MemberProfileViewModel model)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == model.MemberId);

            if (member != null)
            {
                member.Name = model.Name;
                member.FirstName = model.FirstName;
                member.Title = model.Title;
                member.Description = model.Description;
                member.ShowDescription = model.ShowDescription;
                member.UrlProfile = model.UrlProfile;

                Db.SaveChanges();
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public PartialViewResult DateList(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            var dates = Db.ActivityDates.Where(x => x.Hosts.Any(h => h.Id == id) && x.End > DateTime.Now).OrderBy(x => x.Begin).ToList();

            return PartialView("_DateList", dates);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = Db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
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
        /// <param name="organiserMember"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ShortName,Name,Role,Description,IsAdmin,UrlProfile")] OrganiserMember organiserMember)
        {
            if (ModelState.IsValid)
            {
                organiserMember.Id = Guid.NewGuid();
                Db.Members.Add(organiserMember);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organiserMember);
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
            OrganiserMember organiserMember = Db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organiserMember"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ShortName,Name,Role,Description,IsAdmin,UrlProfile")] OrganiserMember organiserMember)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(organiserMember).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = Db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            OrganiserMember organiserMember = Db.Members.Find(id);
            Db.Members.Remove(organiserMember);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Responsibilities()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            model.Responsibilities = Db.MemberResponsibilities.Where(x => x.Member.Organiser.Id == organiser.Id).GroupBy(g => g.Tag).ToList();


            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View(model);
        }

        public ActionResult GenerateApiKey(Guid memberId)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == memberId);
            var user = GetCurrentUser();

            if (user.Id.Equals(member.UserId))
            {
                member.ApiKey = Guid.NewGuid().ToString().Replace("-", "");
                member.ApiKeyValidUntil = DateTime.Today.AddYears(1);
                Db.SaveChanges();

                return RedirectToAction("Card", "Person");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
