using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();
        /// <summary>
        /// 
        /// </summary>
        protected readonly ActivityService ActivityService;

        /// <summary>
        /// 
        /// </summary>
        protected readonly SemesterService SemesterService;

        protected readonly MemberService MemberService;

        /// <summary>
        /// 
        /// </summary>
        protected readonly CascadingDeleteService DeleteService;

        protected readonly StudentService StudentService;

        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        protected ApplicationUser _appUser;

        /// <summary>
        /// 
        /// </summary>
        public class SelectionHelper
        {
            /// <summary>
            /// 
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Value { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected BaseController()
        {
            ActivityService = new ActivityService(Db);
            SemesterService = new SemesterService(Db);
            DeleteService = new CascadingDeleteService(Db);
            StudentService = new StudentService(Db);
            MemberService = new MemberService(Db);
        }

        /// <summary>
        /// 
        /// </summary>
        public IdentifyConfig.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? 
                    (_userManager = HttpContext.GetOwinContext().GetUserManager<IdentifyConfig.ApplicationUserManager>());
            }
            protected set { _userManager = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser AppUser
        {
            get { return _appUser ?? (_appUser = UserManager.FindByName(User.Identity.Name)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected UserRight GetUserRight()
        {
            var user = UserManager.FindByName(User.Identity.Name);


            if (user.MemberState == MemberState.Student)
            {
                var member = Db.Members.FirstOrDefault(m => m.UserId.Equals(user.Id) && m.Organiser.IsStudent);
                return new UserRight
                {
                    IsSysAdmin = User.IsInRole("SysAdmin"),
                    IsHost = false,
                    IsSubscriber = false,
                    User = user,
                    Member = member
                };
            }
            else if (user.MemberState == MemberState.Staff)
            {
                var member =
                    Db.Members.FirstOrDefault(m => m.UserId.Equals(user.Id) && m.Organiser.IsFaculty && !m.Organiser.IsStudent);
                return new UserRight
                {
                    IsSysAdmin = User.IsInRole("SysAdmin"),
                    IsHost = false,
                    IsSubscriber = false,
                    User = user,
                    Member = member
                };
            }
            else // Gast oder sonst was
            {
                return new UserRight
                {
                    IsSysAdmin = User.IsInRole("SysAdmin"),
                    IsHost = false,
                    IsSubscriber = false,
                    User = user
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        protected UserRight GetUserRight(ActivityOrganiser org)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            if (user != null && org != null)
            {
                var member =
                    org.Members.FirstOrDefault(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id));

                return new UserRight
                {
                    IsSysAdmin = User.IsInRole("SysAdmin"),
                    IsHost = false,
                    IsSubscriber = false,
                    User = user,
                    Member = member
                };
            }

            return new UserRight
            {
                IsSysAdmin = false,
                IsHost = false,
                IsSubscriber = false,
                User = user,
                Member = null
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        protected UserRight GetUserRight(string userName, Activity activity)
        {
            var user = UserManager.FindByName(userName);


            // Hypothese: keine Rechte
            var userRight = new UserRight
            {
                IsSysAdmin = User.IsInRole("SysAdmin"),
                IsHost = false,
                IsSubscriber = false,
                IsOwner = false,
                User = user,
                Member = null
            };

            if (activity == null)
                return userRight;


            if (user != null)
            {
                // Ist der Benutzuer Mitglied der Organisation zu der die Aktivität gehört?
                var member =
                    activity.Organiser.Members.SingleOrDefault(
                        m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id));



                if (member != null)
                {
                    userRight.Member = member;
                    if (activity is OfficeHour oh && oh.ByAgreement && oh.Owners.Any(x => x.Member.Id == member.Id))
                    {
                        userRight.IsHost = true;
                    }
                    else
                    {
                        userRight.IsHost =
                            Db.Members.Any(
                                l => l.Dates.Any(occ => occ.Activity.Id == activity.Id) && l.Id == member.Id); // Hält mindestens einen Termin
                    }
                    userRight.IsOwner =
                        activity.Owners.Any(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id)); // Ist Owner der Aktivität


                    // member ist Modulverantwortlicher
                    if (activity is Course)
                    {
                        var course = activity as Course;
                        foreach (var teaching in course.SubjectTeachings)
                        {
                            // Modulverantwortlicher
                            var isMV = teaching.Subject.Module.ModuleResponsibilities.Any(x => x.Member.Id == member.Id);
                            if (isMV)
                            {
                                userRight.IsOwner = true;
                            }

                            var isCV = teaching.Subject.Module.Catalog.CatalogResponsibilities.Any(x => x.Member.Id == member.Id);
                            if (isCV)
                            {
                                userRight.IsOwner = true;
                            }
                        }
                    }
                }
                else
                {
                    // Benutzer gehört nicht zur organisation ist aber owner => das sollte selten sein, aber denkbar
                    userRight.IsOwner =
                        activity.Owners.Any(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id)); // Ist Owner der Aktivität

                    // jeder der mindestens bei einem Temin dabei ist
                    userRight.IsHost = activity.Dates.Any(d => d.Hosts.Any(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id)));
                }

                userRight.IsSubscriber = activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id));
                userRight.User = user;
            }

            return userRight;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="org"></param>
        /// <param name="isHost"></param>
        /// <returns></returns>
        protected UserRight GetUserRight(string userName, string org, bool isHost = false)
        {
            var user = UserManager.FindByName(userName);

            // Hypothese: keine Rechte
            var userRight = new UserRight
            {
                IsSysAdmin = User.IsInRole("SysAdmin"),
                IsHost = false,
                User = user
            };


            var organiser = Db.Organisers.SingleOrDefault(o => o.ShortName.ToUpper().Equals(org.ToUpper()));

            if (user != null && organiser != null)
            {

                var member =
                    organiser.Members.SingleOrDefault(
                        m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id));

                if (member != null)
                {
                    userRight.Member = member;
                    userRight.IsHost = isHost;                   // keine Entscheidung möglich
                }
            }

            return userRight;
        }

 /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ActivityOrganiser GetMyOrganisation()
        {
            var user = GetCurrentUser();
            return MemberService.GetOrganisation(user.Id);
        }

         protected ActivityOrganiser GetOrganisation(Guid id)
         {
             return Db.Organisers.SingleOrDefault(x => x.Id == id);
         }


        protected OrganiserMember GetMyMembership(Guid? orgId = null)
         {
             var user = GetCurrentUser();
             if (orgId != null)
             {
                 var member = MemberService.GetMember(user.Id, orgId.Value);
                 if (member != null)
                     return member;
             }

             var members = MemberService.GetMemberships(user.Id);
             return members.FirstOrDefault();
         }


        protected ICollection<Alumnus> GetMyAlumni()
        {
            return new List<Alumnus>();
        }


        /// <summary>
        /// 
        /// </summary>
        protected void SetTimeSelections()
        {
            var wd = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Montag", Value = 1},
                new SelectionHelper {Text = "Dienstag", Value = 2},
                new SelectionHelper {Text = "Mittwoch", Value = 3},
                new SelectionHelper {Text = "Donnerstag", Value = 4},
                new SelectionHelper {Text = "Freitag", Value = 5},
                new SelectionHelper {Text = "Samstag", Value = 6}
            };

            var hours = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "06", Value = 6},
                new SelectionHelper {Text = "07", Value = 7},
                new SelectionHelper {Text = "08", Value = 8},
                new SelectionHelper {Text = "09", Value = 9},
                new SelectionHelper {Text = "10", Value = 10},
                new SelectionHelper {Text = "11", Value = 11},
                new SelectionHelper {Text = "12", Value = 12},
                new SelectionHelper {Text = "13", Value = 13},
                new SelectionHelper {Text = "14", Value = 14},
                new SelectionHelper {Text = "15", Value = 15},
                new SelectionHelper {Text = "16", Value = 16},
                new SelectionHelper {Text = "17", Value = 17},
                new SelectionHelper {Text = "18", Value = 18},
                new SelectionHelper {Text = "19", Value = 19},
                new SelectionHelper {Text = "20", Value = 20},
                new SelectionHelper {Text = "21", Value = 21},
            };

            var minutes = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "00", Value = 0},
                new SelectionHelper {Text = "15", Value = 15},
                new SelectionHelper {Text = "30", Value = 30},
                new SelectionHelper {Text = "45", Value = 45}
            };

            ViewBag.WeekDays = new SelectList(wd, "Value", "Text", "Montag");
            ViewBag.Hours = new SelectList(hours, "Value", "Text", "09");
            ViewBag.Minutes = new SelectList(minutes, "Value", "Text", "00");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ApplicationUser GetCurrentUser()
        {
            return UserManager.FindByName(User.Identity.Name);
        }


        protected ApplicationUser GetUser(string userId)
        {
            return UserManager.FindById(userId);
        }

        protected Student GetCurrentStudent(string userId)
        {
            return StudentService.GetCurrentStudent(userId);
        }

        protected Semester GetLatestSemester(ActivityOrganiser org)
        {
            return SemesterService.GetLatestSemester(org);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected ActivityOrganiser GetOrganiser(Guid id)
        {
            return Db.Organisers.SingleOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        protected ActivityOrganiser GetOrganiser(string shortName)
        {
            return Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(shortName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected Curriculum GetCurriculum(Guid id)
        {
            return Db.Curricula.SingleOrDefault(x => x.Id == id);
        }


        protected Semester GetSemesterFromFilter()
        {
            Semester semester = null;

            if (Session["SemesterName"] != null)
            {
                var semName = Session["SemesterName"].ToString();
                semester = SemesterService.GetSemester(semName);
            }
            else
            {
                semester = SemesterService.GetSemester(DateTime.Today);
                Session["SemesterName"] = semester.Name;
                Session["SemesterId"] = semester.Id;
            }

            return semester;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            var cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }
    }
}