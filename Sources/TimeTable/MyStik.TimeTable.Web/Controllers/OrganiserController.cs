using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    public class OrganiserController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                    model.Add(new OrganiserViewModel {Organiser = organiser});
                }
                return View(model);
            }


            return View("Details", org);
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
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Admins(string id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            /* sollte nicht mehr erforderlich sein
            if (User.IsInRole("SysAdmin"))
            {
                organiser = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(id));
            }
            */

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var memberPage = organiser.Members.OrderBy(m => m.Name);

            var myUser = UserManager.FindByName(User.Identity.Name);


            foreach (var member in memberPage)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    ItsMe = itsMe,
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View("Admins", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Member(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);
            if (member == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Alle Vorlesungen
            // Alle Sprechstunden
            // Alle Newsletter
            // Alle Veranstaltungen

            // => 

            var model = new LecturerCharacteristicModel {Lecturer = member};

            var semester = SemesterService.GetLatestSemester(member.Organiser);
            var user = AppUser;

            var courseService = new CourseService(Db);

            model.Courses = courseService.GetCourses(semester.Name, member);

            foreach (var course in model.Courses)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();

                course.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();
                course.Rooms.AddRange(rooms);

                course.State = ActivityService.GetActivityState(course.Course.Occurrence, user);
            }

            // Sprechstunde im aktuellen Semester
            var ohService = new OfficeHourService(Db);
            model.OfficeHour = ohService.GetLatestOfficeHour(member);

            model.Semester = semester;

            ViewBag.UserRight = GetUserRight(User.Identity.Name, member.Organiser.ShortName);

            return View("MemberPublic", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MemberAdmin(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);
            if (member == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Alle Vorlesungen
            // Alle Sprechstunden
            // Alle Newsletter
            // Alle Veranstaltungen

            // => 

            var model = new LecturerCharacteristicModel { Lecturer = member };

            var semester = SemesterService.GetLatestSemester(member.Organiser);
            var user = AppUser;

            var courseService = new CourseService(Db);

            model.Courses = courseService.GetCourses(semester.Name, member);

            foreach (var course in model.Courses)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();

                course.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();
                course.Rooms.AddRange(rooms);

                course.State = ActivityService.GetActivityState(course.Course.Occurrence, user);
            }

            // Sprechstunde im aktuellen Semester
            var ohService = new OfficeHourService(Db);
            model.OfficeHour = ohService.GetLatestOfficeHour(member);

            model.Semester = semester;

            ViewBag.UserRight = GetUserRight(User.Identity.Name, member.Organiser.ShortName);

            return View("Member", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Calendar(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);
            if (member == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new LecturerCharacteristicModel
            {
                Lecturer = member
            };

            ViewBag.UserRight = GetUserRight(User.Identity.Name, member.Organiser.ShortName);

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Newsletter(Guid? id)
        {
            var org = id==null ? GetMyOrganisation() : GetOrganiser(id.Value);

            var userRight = GetUserRight();
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var newsletters = Db.Activities.OfType<Newsletter>().Where(n => n.Organiser.Id == org.Id).ToList();

            var model = new List<NewsletterViewModel>();

            var user = AppUser;

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


                bool isMember = userRight.IsOrgMember;
                bool isAdmin = userRight.IsNewsAdmin;

                model.Add(new NewsletterViewModel
                {
                    Newsletter = newsletter,
                    State = ActivityService.GetActivityState(newsletter.Occurrence, user),
                    IsMember = isMember,
                    IsAdmin = isAdmin,
                });
            }

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Rooms()
        {
            var org = GetMyOrganisation();

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var rooms = roomService.GetRooms(org.Id, true);

            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);

            ViewBag.Organiser = org;


            return View(rooms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RoomList()
        {
            var org = GetMyOrganisation();

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var rooms = roomService.GetRooms(org.Id, true);

            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);
            ViewBag.Organiser = org;

            return View(rooms);
        }



 
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Curricula()
        {
            var org = GetMyOrganisation();

            var model = Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList();

            ViewBag.Organiser = org;
            ViewBag.UserRight = GetUserRight();

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Groups(Guid? id)
        {
            var organiser = GetMyOrganisation();
            var semester = SemesterService.GetSemester(id);
            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = organiser;
            ViewBag.Semester = semester;

            var model = Db.SemesterGroups.Where(x =>
                x.Semester.Id == semester.Id &&
                x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id).ToList();

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Events()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");


            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var events = Db.Activities.OfType<Event>().Where(n => n.Organiser.Id == organiser.Id).ToList();


            foreach (var @event in events)
            {
                model.Events.Add(new EventViewModel {Event = @event});
            }

            ViewBag.UserRight = userRight;

            return View(model);
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
        /// <param name="activityorganiser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ShortName")] ActivityOrganiser activityorganiser)
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ActivityOrganiser activityorganiser = Db.Organisers.Find(id);
            Db.Organisers.Remove(activityorganiser);
            Db.SaveChanges();
            return RedirectToAction("Index", "OrganiserMembers");

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
                    Name = member.Name,
                    ShortName = member.ShortName,
                    IsAssociated = member.IsAssociated,
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

            return RedirectToAction("Index", "OrganiserMembers");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMember(MemberUserViewModel model)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == model.MemberId);

            if (member != null)
            {
                ViewBag.Member = member;

                if (string.IsNullOrEmpty(model.ShortName))
                {
                    ModelState.AddModelError("", "Kurzname darf nicht leer sein");
                    return View(model);
                }


                var shortName = model.ShortName.Trim().ToUpper();

                if (member.Organiser.Members.Any(x => x.Id != member.Id && x.ShortName.ToUpper().Equals(shortName)))
                {
                    ModelState.AddModelError("", "Diesen Kurznamen gibt es schon bei jemand anderem");
                    return View(model);
                }



                member.IsAssociated = model.IsAssociated;

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

                // nur übernehmen, wenn nicht leer
                if (!string.IsNullOrEmpty(model.ShortName))
                {
                    member.ShortName = model.ShortName;
                }


                member.Role = model.Role;
                member.Name = model.Name;

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Index", "OrganiserMembers");
            }

            return RedirectToAction("Index", "OrganiserMembers");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            return RedirectToAction("Index", "OrganiserMembers");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                return RedirectToAction("Members", new {id = member.Organiser.ShortName});
            }

            return RedirectToAction("Index", "OrganiserMembers");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            return RedirectToAction("Index", "OrganiserMembers");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateMember()
        {
            var org = GetMyOrganisation();

            var model = new MemberUserViewModel
            {
                OrganiserId = org.Id
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateMember(MemberUserViewModel model)
        {
            var org = Db.Organisers.SingleOrDefault(m => m.Id == model.OrganiserId);

            if (org != null)
            {
                var shortName = model.ShortName.Trim().ToUpper();
                if (org.Members.Any(x => x.ShortName.ToUpper().Equals(shortName)))
                {
                    ModelState.AddModelError("", "Diesen Kurznamen gibt es schon");
                    return View(model);
                }



                var member = new OrganiserMember
                {
                    Role = model.Role,
                    ShortName = model.ShortName,
                    Name = model.Name,
                    IsAssociated = model.IsAssociated,
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

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Index", "OrganiserMembers");

            }

            return RedirectToAction("Index", "OrganiserMembers");

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
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Persons(string searchString)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
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
                            .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                        c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ListByGroup(Guid semGroupId)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);

            var semester = SemesterService.GetSemester(DateTime.Today);
            var vorSemester = SemesterService.GetPreviousSemester(semester);

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
                                .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                            c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                                .OrderBy(c => c.Name)
                                .ToList();


                        studModel.AllCourses = courses;

                        var coursesFit =
                            Db.Activities.OfType<Course>()
                                .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                            c.SemesterGroups.Any((g => g.Id == semGroupId)))
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult NoMember()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult MemberList(string id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(id.ToUpper()));

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);



            var members = organiser.Members.OrderBy(m => m.Name);


            var semester = SemesterService.GetSemester(DateTime.Today);
            var vorSemester = SemesterService.GetPreviousSemester(semester);
            var vorVorSemester = SemesterService.GetPreviousSemester(vorSemester);



            var courseService = new CourseService(Db);


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
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EditAdmins()
        {
            var org = GetMyOrganisation();

            return View(org);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MemberList2(string token)
        {
            var org = GetMyOrganisation();


            var list = from l in org.Members
                where l.ShortName.ToUpper().Contains(token.ToUpper())
                select
                new
                {
                    memberId = l.Id,
                    shortName = l.ShortName,
                };


            return Json(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddMemberAdminRight(string memberAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(memberAdminName));

            if (member != null && !member.IsMemberAdmin)
            {
                member.IsMemberAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsMemberAdmin).ToList();
            ViewBag.AdminRight = "Member";

            return PartialView("_MemberRow", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddCourseAdminRight(string courseAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(courseAdminName));

            if (member != null && !member.IsCourseAdmin)
            {
                member.IsCourseAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsCourseAdmin).ToList();
            ViewBag.AdminRight = "Course";

            return PartialView("_MemberRow", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curriculumAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddCurriculumAdminRight(string curriculumAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(curriculumAdminName));

            if (member != null && !member.IsCurriculumAdmin)
            {
                member.IsCurriculumAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsCurriculumAdmin).ToList();
            ViewBag.AdminRight = "Curriculum";

            return PartialView("_MemberRow", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddRoomAdminRight(string roomAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(roomAdminName));

            if (member != null && !member.IsRoomAdmin)
            {
                member.IsRoomAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsRoomAdmin).ToList();
            ViewBag.AdminRight = "Room";

            return PartialView("_MemberRow", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddEventAdminRight(string eventAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(eventAdminName));

            if (member != null && !member.IsEventAdmin)
            {
                member.IsEventAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsEventAdmin).ToList();
            ViewBag.AdminRight = "Event";

            return PartialView("_MemberRow", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsletterAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddNewsletterAdminRight(string newsletterAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(newsletterAdminName));

            if (member != null && !member.IsNewsAdmin)
            {
                member.IsNewsAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsNewsAdmin).ToList();
            ViewBag.AdminRight = "Newsletter";

            return PartialView("_MemberRow", list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentAdminName"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddStudentAdminRight(string studentAdminName)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(studentAdminName));

            if (member != null && !member.IsStudentAdmin)
            {
                member.IsStudentAdmin = true;
                Db.SaveChanges();
            }

            var list = org.Members.Where(x => x.IsStudentAdmin).ToList();
            ViewBag.AdminRight = "Student";

            return PartialView("_MemberRow", list);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DeleteAdminRight(Guid id, string right)
        {
            var org = GetMyOrganisation();
            var member = org.Members.SingleOrDefault(x => x.Id == id);

            if (member != null)
            {
                if (right.Equals("Course") && member.IsCourseAdmin)
                {
                    member.IsCourseAdmin = false;
                }
                if (right.Equals("Curriculum") && member.IsCurriculumAdmin)
                {
                    member.IsCurriculumAdmin = false;
                }
                if (right.Equals("Member") && member.IsMemberAdmin)
                {
                    member.IsMemberAdmin = false;
                }
                if (right.Equals("Semester") && member.IsSemesterAdmin)
                {
                    member.IsSemesterAdmin = false;
                }
                if (right.Equals("Room") && member.IsRoomAdmin)
                {
                    member.IsRoomAdmin = false;
                }
                if (right.Equals("Event") && member.IsEventAdmin)
                {
                    member.IsEventAdmin = false;
                }
                if (right.Equals("Newsletter") && member.IsEventAdmin)
                {
                    member.IsNewsAdmin = false;
                }
                if (right.Equals("Student") && member.IsEventAdmin)
                {
                    member.IsNewsAdmin = false;
                }

                Db.SaveChanges();
            }

            return PartialView("_EmptyRow");
        }

        public ActionResult LinkMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            var model = new MemberViewModel
            {
                Member = member,
                Exports = new List<MemberExportViewModel>()
            };

            foreach (var memberExport in member.Exports)
            {
                var externalMember =
                    memberExport.Organiser.Members.FirstOrDefault(x => x.ShortName.Equals(memberExport.ShortName));

                if (externalMember != null)
                {

                    var allExportActivities = Db.Activities.OfType<Course>()
                        .Where(x =>
                            x.Dates.Any(d => d.Hosts.Any(h => h.Id == externalMember.Id)) ||
                            x.Owners.Any(o => o.Member.Id == externalMember.Id))
                        .ToList();
                    if (allExportActivities.Any())
                    {
                        foreach (var course in allExportActivities)
                        {
                            model.Exports.Add(new MemberExportViewModel
                            {
                                Activity = course,
                                Export = memberExport
                            });
                        }
                    }
                    else
                    {
                        model.Exports.Add(new MemberExportViewModel
                        {
                            Export = memberExport,
                            ExternalMember = externalMember
                        });
                    }
                }
                else
                {
                    model.Exports.Add(new MemberExportViewModel
                    {
                        Export = memberExport
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult LinkMember(Guid id, string orgName, string shortName)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));

            if (org != null && member != null && org.Id != member.Organiser.Id)
            {
                var externaltMember = org.Members.FirstOrDefault(x => x.ShortName.Equals(shortName));

                // es gibt diesen Member beim anderen Veranstalter
                if (externaltMember != null)
                {
                    // Export schon da
                    var exportMember =
                        member.Exports.SingleOrDefault(x => x.Organiser.Id == org.Id && x.ShortName.Equals(shortName));

                    if (exportMember == null)
                    {
                        exportMember = new MemberExport
                        {
                            Member = member,
                            Organiser = org,
                            ShortName = shortName
                        };

                        member.Exports.Add(exportMember);
                        Db.MemberExports.Add(exportMember);
                        Db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("LinkMember", new {id = member.Id});
        }

        public ActionResult ImportExport(Guid exportId, Guid courseId)
        {
            var export = Db.MemberExports.SingleOrDefault(x => x.Id == exportId);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var org = Db.Organisers.SingleOrDefault(x => x.Id == export.Organiser.Id);

            var member = export.Member;
            var externalMember = org.Members.FirstOrDefault(x => x.ShortName.Equals(export.ShortName));

            // die Owner austauschen
            var owners = course.Owners.Where(x => x.Member.Id == externalMember.Id).ToList();
            foreach (var owner in owners)
            {
                if (course.Owners.All(x => x.Member.Id != member.Id))
                {
                    owner.Member = member;
                }
            }


            // Termine austauschen
            var dates = course.Dates.Where(x => x.Hosts.Any(h => h.Id == externalMember.Id)).ToList();
            foreach (var date in dates)
            {
                date.Hosts.Remove(externalMember);

                if (!date.Hosts.Contains(member))
                {
                    date.Hosts.Add(member);
                }
            }

            Db.SaveChanges();

            return RedirectToAction("LinkMember", new { id = member.Id });
        }

        public ActionResult DeleteExternalMember(Guid memberId, Guid externalMemberId)
        {
            var externallMember = Db.Members.SingleOrDefault(m => m.Id == externalMemberId);
            var member = Db.Members.SingleOrDefault(m => m.Id == memberId);

            if (externallMember != null)
            {
                var org = externallMember.Organiser;

                // jetzt noch export dazu löschen
                var export = member.Exports.FirstOrDefault(x =>
                    x.Organiser.Id == org.Id && x.ShortName.Equals(externallMember.ShortName));

                if (export != null)
                {
                    Db.MemberExports.Remove(export);
                }


                org.Members.Remove(externallMember);
                Db.Members.Remove(externallMember);


                // TODO: wie löscht man den Rest, z.B. Termine?
                Db.SaveChanges();
            }

            return RedirectToAction("LinkMember", new { id = memberId });

        }

        public ActionResult DeleteLink(Guid id)
        {
            var link = Db.MemberExports.SingleOrDefault(m => m.Id == id);

            var org = link.Member;

            Db.MemberExports.Remove(link);

            // TODO: wie löscht man den Rest, z.B. Termine?
            Db.SaveChanges();

            return RedirectToAction("LinkMember", new { id = org.Id });

        }

    }
}
