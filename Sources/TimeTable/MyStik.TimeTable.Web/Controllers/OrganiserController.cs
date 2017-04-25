using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using PagedList;

namespace MyStik.TimeTable.Web.Controllers
{
    public class OrganiserController : BaseController
    {
        // GET: /Organiser/
        public ActionResult Index(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                var list = Db.Organisers.OrderBy(o => o.ShortName);
                var model = list.Select(organiser => new OrganiserViewModel {Organiser = organiser}).ToList();

                return View(model);
            }

            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.ToUpper().Equals(id.ToUpper()));
            if (org == null)
            {
                var model = new List<OrganiserViewModel>();
                foreach (var organiser in Db.Organisers)
                {
                    model.Add(new OrganiserViewModel { Organiser = organiser });
                }
                return View(model);
            }


            return View("Details", org);
        }

        // GET: /Organiser/Details/5
        public ActionResult Details(Guid? id)
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

        public ActionResult Members(string id, int? page, string startWith)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(id.ToUpper()));
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var pageNumber = page ?? 1;

            var members = string.IsNullOrEmpty(startWith) ?
                organiser.Members.OrderBy(m => m.Name) :
                organiser.Members.Where(m => !string.IsNullOrEmpty(m.Name) && m.Name.ToUpper().StartsWith(startWith.ToUpper())).OrderBy(m => m.Name);

            var listLength = 20;
            if (!string.IsNullOrEmpty(startWith) && members.Any())
                listLength = members.Count();


            var memberPage = members.ToPagedList(pageNumber, listLength);
            ViewBag.MemberPage = memberPage;

            var myUser = UserManager.FindByName(User.Identity.Name);


            var semester = GetSemester();
            var vorSemester = new SemesterService().GetSemester(semester, 1);
            var vorVorSemester = new SemesterService().GetSemester(semester, 2);

            var courseService = new CourseService(UserManager);

            foreach (var member in memberPage)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                    IsActive = courseService.IsActive(member, semester),
                    WasActiveLastSemester = courseService.IsActive(member, vorSemester),
                    WasActiveLastYear = courseService.IsActive(member, vorVorSemester)
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;
            ViewBag.LastYear = vorVorSemester.Name;

            return View(model);
        }

        public ActionResult Member(string orgId, string shortName)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgId.ToUpper()));
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var member = organiser.Members.SingleOrDefault(m => m.ShortName.ToUpper().Equals(shortName.ToUpper()));
            if (member == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Alle Vorlesungen
            // Alle Sprechstunden
            // Alle Newsletter
            // Alle Veranstaltungen

            // => 

            var model = new LecturerCharacteristicModel { Lecturer = member };

            var semester = GetSemester();
            var user = AppUser;

            var courseService = new CourseService(UserManager);

            model.Courses = courseService.GetCourses(semester.Name, member);

            foreach (var course in model.Courses)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();

                course.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();
                course.Rooms.AddRange(rooms);

                course.State = ActivityService.GetActivityState(course.Course.Occurrence, user, semester);
            }

            // Sprechstunden - wird derzeit nicht verwendet
            //model.OfficeHours = courseService.GetOfficeHours(member);

            ViewBag.Semester = semester;
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            if (ViewBag.UserRight.IsOrgMember)
            {
                model.OldCourses = courseService.GetCourseHistory(member);
            }


            return View(model);
        }


        // GET: /Organiser/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Organiser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,ShortName")] ActivityOrganiser activityorganiser)
        {
            if (ModelState.IsValid)
            {
                activityorganiser.Id = Guid.NewGuid();
                Db.Organisers.Add(activityorganiser);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activityorganiser);
        }

        // GET: /Organiser/Edit/5
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

        // POST: /Organiser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,ShortName,IsFaculty,IsStudent")] ActivityOrganiser activityorganiser)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(activityorganiser).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activityorganiser);
        }

        // GET: /Organiser/Delete/5
        public ActionResult Delete(Guid? id)
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

        // POST: /Organiser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ActivityOrganiser activityorganiser = Db.Organisers.Find(id);
            Db.Organisers.Remove(activityorganiser);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">MemberId</param>
        /// <returns></returns>
        public ActionResult EditMember(Guid id)
        {

            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var model = new MemberUserViewModel
                {
                    MemberId = id,
                    Role = member.Role,
                    Description = member.Description,
                    UrlProfile = member.UrlProfile,
                    Name = member.Name,
                    IsAdmin = member.IsAdmin,
                    ShortName = member.ShortName,
                };

                if (!string.IsNullOrEmpty(member.UserId))
                {
                    var user = UserManager.FindById(member.UserId);
                    if (user != null)
                    {
                        model.UserName = user.UserName;
                    }
                }

                ViewBag.Member = member;



                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditMember(MemberUserViewModel model)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == model.MemberId);

            if (member != null)
            {
                member.IsAdmin = model.IsAdmin;

                if (string.IsNullOrEmpty(model.UserName))
                {
                    member.UserId = string.Empty;
                }
                else
                {
                    var user = UserManager.FindByName(model.UserName);
                    if (user != null)
                    {
                        member.UserId = user.Id;

                        // wenn es keine stud-orga ist, dann muss der Nutzer "Staff" werden
                        if (!member.Organiser.IsStudent)
                        {
                            user.MemberState = MemberState.Staff;
                            UserManager.Update(user);
                        }
                    }
                }

                if (User.IsInRole("SysAdmin"))
                {
                    member.ShortName = model.ShortName;
                }


                member.Role = model.Role;
                member.Description = model.Description;
                member.UrlProfile = model.UrlProfile;
                member.Name = model.Name;

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Members", new { id = member.Organiser.ShortName });
            }

            return RedirectToAction("Index");
        }
        public ActionResult EditMemberDescription(Guid id)
        {

            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var model = new MemberUserViewModel
                {
                    MemberId = id,
                    Role = member.Role,
                    Description = member.Description,
                    UrlProfile = member.UrlProfile,
                    Name = member.Name,
                };

                if (!string.IsNullOrEmpty(member.UserId))
                {
                    var user = UserManager.FindById(member.UserId);
                    model.UserName = user.UserName;
                }

                ViewBag.Member = member;



                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditMemberDescription(MemberUserViewModel model)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == model.MemberId);

            if (member != null)
            {
                member.Description = model.Description;
                member.UrlProfile = model.UrlProfile;

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Members", new { id = member.Organiser.ShortName });
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var org = member.Organiser;

                org.Members.Remove(member);
                Db.Members.Remove(member);

                // TODO: wie löscht man den Rest, z.B. Termine?
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateMember(Guid id)
        {
            var model = new MemberUserViewModel
            {
                OrganiserId = id,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateMember(MemberUserViewModel model)
        {
            var org = Db.Organisers.SingleOrDefault(m => m.Id == model.OrganiserId);

            if (org != null)
            {
                var member = new OrganiserMember
                {
                    IsAdmin = model.IsAdmin,
                    Role = model.Role,
                    Description = model.Description,
                    ShortName = model.ShortName,
                    Name = model.Name,
                };

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    var user = UserManager.FindByName(model.UserName);
                    if (user != null)
                    {
                        member.UserId = user.Id;
                    }
                }

                org.Members.Add(member);

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Members", new { id = org.ShortName });
            }

            return RedirectToAction("Index");
        }



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

        public ActionResult Courses(string id, int? page, string startWith)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(id.ToUpper()));
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var semester = GetSemester();
            var courses = string.IsNullOrEmpty(startWith) ? 
                Db.Activities.OfType<Course>().Where(c => 
                c.SemesterGroups.Any(g =>g.Semester.Name.Equals(semester.Name)) ||
                !c.SemesterGroups.Any()
                ).OrderBy(c => c.ShortName).ToList() :
                Db.Activities.OfType<Course>().Where(c => (
                c.SemesterGroups.Any(g =>g.Semester.Name.Equals(semester.Name)) ||
                !c.SemesterGroups.Any()) && c.ShortName.StartsWith(startWith)
                ).OrderBy(c => c.ShortName).ToList()
                ;


            var pageNumber = page ?? 1;

            var listLength = 20;
            if (!string.IsNullOrEmpty(startWith) && courses.Any())
                listLength = courses.Count();


            var coursePage = courses.ToPagedList(pageNumber, listLength);
            ViewBag.CoursePage = coursePage;

            foreach (var course in coursePage)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

                var summary = new CourseSummaryModel();
                summary.Course = course;
                summary.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
                summary.Rooms.AddRange(rooms);


                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct();

                foreach (var day in days)
                {
                    var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                    var courseDate = new CourseDateModel
                    {
                        DayOfWeek = day.Day,
                        StartTime = day.Begin,
                        EndTime = day.End,
                        DefaultDate = defaultDay.Begin
                    };
                    summary.Dates.Add(courseDate);
                }

                model.Courses.Add(summary);
            }

            // Benutzerrechte
            ViewBag.UserRight = userRight;
            ViewBag.Semester = semester;

            return View(model);
        }

        public ActionResult Events(string id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(id.ToUpper()));
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");


            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var events =
                string.IsNullOrEmpty(id)
                    ? Db.Activities.OfType<Event>().ToList()
                    : Db.Activities.OfType<Event>().Where(n => n.Organiser.ShortName.ToUpper().Equals(id.ToUpper())).ToList();


            foreach (var @event in events)
            {
                model.Events.Add( new EventViewModel{ Event = @event});
            }

            ViewBag.UserRight = userRight;

            return View(model);
        }
        public ActionResult Newsletter(string id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(id.ToUpper()));
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var newsletters =
                string.IsNullOrEmpty(id)
                    ? Db.Activities.OfType<Newsletter>().ToList()
                    : Db.Activities.OfType<Newsletter>().Where(n => n.Organiser.ShortName.ToUpper().Equals(id.ToUpper())).ToList();

            var model = new List<NewsletterViewModel>();

            var user = AppUser;
            var semester = GetSemester();


            foreach (var newsletter in newsletters)
            {
                if (newsletter.Occurrence == null)
                {
                    newsletter.Occurrence = new Occurrence
                    {
                        IsAvailable = true,
                        Capacity = -1,
                    };
                    Db.SaveChanges();
                }
                else
                {
                    newsletter.Occurrence.IsAvailable = true;
                    newsletter.Occurrence.Capacity = -1;
                    Db.SaveChanges();
                }


                bool isMember = IsUserMemberOf(newsletter.Organiser.ShortName) || User.IsInRole("SysAdmin");
                bool isAdmin = IsUserAdminOf(newsletter.Organiser.ShortName) || User.IsInRole("SysAdmin");

                model.Add(new NewsletterViewModel
                {
                    Newsletter = newsletter,
                    State = ActivityService.GetActivityState(newsletter.Occurrence, user, semester),
                    IsMember = isMember,
                    IsAdmin = isAdmin,
                });

                if (!userRight.IsOrgMember)
                    userRight.IsOrgMember = isMember;

                if (!userRight.IsOrgAdmin)
                    userRight.IsOrgAdmin = isAdmin;
            }

            if (!string.IsNullOrEmpty(id))
            {
                bool isMember = IsUserMemberOf(id) || User.IsInRole("SysAdmin");
                bool isAdmin = IsUserAdminOf(id) || User.IsInRole("SysAdmin");

                if (!userRight.IsOrgMember)
                    userRight.IsOrgMember = isMember;

                if (!userRight.IsOrgAdmin)
                    userRight.IsOrgAdmin = isAdmin;
            }


            ViewBag.UserRight = userRight;

            return View(model);
        }

        [HttpPost]
        public PartialViewResult Persons(string searchString)
        {
            var semester = GetSemester();
            var vorSemester = new SemesterService().GetSemester(semester, 1);

            var model = new List<StudentViewModel>();

            if (!string.IsNullOrEmpty(searchString))
            {
                var userDb = new ApplicationDbContext();
                var users = from s in userDb.Users select s;
                if (!string.IsNullOrEmpty(searchString))
                {
                    users = users.Where(u =>
                        u.MemberState == MemberState.Student &&
                        (u.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
                        u.LastName.ToUpper().Contains(searchString.ToUpper()))
                        );
                }


                var semSubService = new SemesterSubscriptionService();

                foreach (var user in users)
                {
                    var studModel = new StudentViewModel
                    {
                        User = user,
                        CurrentSubscription = semSubService.GetSubscription(user.Id, semester.Id),
                        LastSubscription = semSubService.GetSubscription(user.Id, vorSemester.Id)
                    };

                    var semesterSubscription = Db.Subscriptions.OfType<SemesterSubscription>()
                        .FirstOrDefault(s => s.SemesterGroup.Semester.Id == semester.Id && s.UserId.Equals(user.Id));

                    // alle Kurse des Benutzers
                    var courses =
                        Db.Activities.OfType<Course>()
                        .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) && c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                        .OrderBy(c => c.Name)
                        .ToList();


                    studModel.AllCourses = courses;

                    var coursesFit = new List<Course>();

                    if (semesterSubscription != null)
                    {
                        coursesFit = Db.Activities.OfType<Course>()
                            .Where(
                                c =>
                                    c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                    c.SemesterGroups.Any((g => g.Id == semesterSubscription.SemesterGroup.Id)))
                            .OrderBy(c => c.Name)
                            .ToList();
                    }


                    studModel.CoursesFit = coursesFit;

                    model.Add(studModel);
                }
            }

            model = model.OrderBy(u => u.User.LastName).ToList();

            ViewBag.CurrentSemester = semester;
            ViewBag.LastSemester = vorSemester;

            return PartialView("Persons", model);
        }


        [HttpPost]
        public PartialViewResult ListByGroup(Guid semGroupId)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);

            var semester = GetSemester();
            var vorSemester = new SemesterService().GetSemester(semester, 1);

            var model = new List<StudentViewModel>();

            if (semGroup != null)
            {
                var semSubService = new SemesterSubscriptionService();

                foreach (var sub in semGroup.Subscriptions)
                {
                    var user = UserManager.FindById(sub.UserId);

                    if (user != null)
                    {
                        var studModel = new StudentViewModel
                        {
                            User = user,
                            CurrentSubscription = semSubService.GetSubscription(user.Id, semester.Id),
                            LastSubscription = semSubService.GetSubscription(user.Id, vorSemester.Id)
                        };


                        
                        // alle Kurse des Benutzers
                        var courses =
                            Db.Activities.OfType<Course>()
                            .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) && c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                            .OrderBy(c => c.Name)
                            .ToList();


                        studModel.AllCourses = courses;

                        var coursesFit =
                            Db.Activities.OfType<Course>()
                            .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) && c.SemesterGroups.Any((g => g.Id == semGroupId)))
                            .OrderBy(c => c.Name)
                            .ToList();


                        studModel.CoursesFit = coursesFit;

                        model.Add(studModel);

                    }
                }
            }

            model = model.OrderBy(u => u.User.LastName).ToList();

            ViewBag.CurrentSemester = semester;
            ViewBag.LastSemester = vorSemester;

            return PartialView("Persons", model);
        }



        public ActionResult NoMember()
        {
            return View();
        }

        public FileResult MemberList(string id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(id.ToUpper()));

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);



            var members = organiser.Members.OrderBy(m => m.Name);


            var semester = GetSemester();
            var vorSemester = new SemesterService().GetSemester(semester, 1);
            var vorVorSemester = new SemesterService().GetSemester(semester, 1);



            var courseService = new CourseService(UserManager);


            writer.Write("Kurzname;Name;Rolle;Beschreibung;E-Mail");
            writer.Write(";");
            writer.Write(semester.Name);
            writer.Write(";");
            writer.Write(vorSemester.Name);
            writer.Write(";");
            writer.Write(vorVorSemester.Name);

            writer.Write(Environment.NewLine);


            foreach (var member in members)
            {

                var user = member.UserId != null ? UserManager.FindById(member.UserId) : null;
                var eMail = user != null ? user.Email : "Kein Benutzerkonto";


                var active1 = courseService.IsActive(member, semester) ? "Ja" : "Nein";
                var active2 = courseService.IsActive(member, vorSemester) ? "Ja" : "Nein";
                var active3 = courseService.IsActive(member, vorVorSemester) ? "Ja" : "Nein";

                writer.Write("{0};{1};{2};{3};{4};{5};{6}",
                    member.ShortName, member.Name, 
                    member.Role,
                    eMail,
                    active1, active2, active3);
                writer.Write(Environment.NewLine);

            }


            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Mitglieder_");
            sb.Append(organiser.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

    }
}
