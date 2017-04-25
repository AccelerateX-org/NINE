using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();
        protected readonly ActivityService ActivityService;

        protected IdentifyConfig.ApplicationUserManager _userManager;
        protected ApplicationUser _appUser;

        public class SelectionHelper
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }


        protected BaseController()
        {
            ActivityService = new ActivityService(Db);
        }

        public IdentifyConfig.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? 
                    (_userManager = HttpContext.GetOwinContext().GetUserManager<IdentifyConfig.ApplicationUserManager>());
            }
            protected set { _userManager = value; }
        }

        public ApplicationUser AppUser
        {
            get { return _appUser ?? (_appUser = UserManager.FindByName(User.Identity.Name)); }
        }


        protected UserRight GetUserRight()
        {
            var user = UserManager.FindByName(User.Identity.Name);

            if (User.IsInRole("SysAdmin"))
            {
                return new UserRight
                {
                    IsOrgAdmin = true,
                    IsHost = true,
                    IsOrgMember = true,
                    IsSubscriber = true,
                    User = user
                };
            }

            if (user.MemberState == MemberState.Student)
            {
                return new UserRight
                {
                    IsOrgAdmin =
                        Db.Members.Any(
                            m =>
                                m.UserId.Equals(user.Id) && m.IsAdmin && m.Organiser.IsStudent),
                    IsHost = false,
                    IsOrgMember =
                        Db.Members.Any(m => m.UserId.Equals(user.Id) && m.Organiser.IsStudent),
                    IsSubscriber = false,
                    User = user
                };
            }
            else if (user.MemberState == MemberState.Staff)
            {
                return new UserRight
                {
                    IsOrgAdmin =
                        Db.Members.Any(
                            m =>
                                m.UserId.Equals(user.Id) && m.IsAdmin && m.Organiser.IsFaculty && !m.Organiser.IsStudent),
                    IsHost = false,
                    IsOrgMember =
                        Db.Members.Any(m => m.UserId.Equals(user.Id) && m.Organiser.IsFaculty && !m.Organiser.IsStudent),
                    IsSubscriber = false,
                    User = user
                };
            }
            else // Gast oder sonst was
            {
                return new UserRight
                {
                    IsOrgAdmin = false,
                    IsHost = false,
                    IsOrgMember = false,
                    IsSubscriber = false,
                    User = user
                };
            }
        }


        protected UserRight GetUserRight(string userName, Activity activity)
        {
            var user = UserManager.FindByName(userName);

            if (User.IsInRole("SysAdmin"))
            {
                return new UserRight
                {
                    IsOrgAdmin = true,
                    IsHost = true,
                    IsOrgMember = true,
                    IsSubscriber = true,
                    User = user
                };
            }

            // Hypothese: keine Rechte
            var userRight = new UserRight
            {
                IsOrgAdmin = false,
                IsHost = false,
                IsOrgMember = false,
                IsSubscriber = false,
                User = user
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
                    userRight.IsOrgAdmin = member.IsAdmin;
                    userRight.IsHost =
                        Db.Members.Any(
                            l => l.Dates.Any(occ => occ.Activity.Id == activity.Id) && l.Id == member.Id) ||
                        // Hält mindestens einen Termin
                        activity.Owners.Any(o => o.Member.UserId.Equals(user.Id)); // Ist Owner der Aktivität
                    userRight.IsOrgMember = true;
                }
                else
                {
                    // Benutzer gehört nicht zur organisation ist aber owner => das sollte selten sein, aber denkbar
                    userRight.IsHost =
                        activity.Owners.Any(o => o.Member.UserId.Equals(user.Id)); // Ist Owner der Aktivität
                }

                userRight.IsSubscriber = activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id));
                userRight.User = user;
            }

            return userRight;
        }

        protected UserRight GetUserRight(string userName, string org, bool isHost = false)
        {
            var user = UserManager.FindByName(userName);
            if (User.IsInRole("SysAdmin"))
            {
                return new UserRight
                {
                    IsOrgAdmin = true,
                    IsHost = true,
                    IsOrgMember = true,
                    User = user
                };
            }

            // Hypothese: keine Rechte
            var userRight = new UserRight
            {
                IsOrgAdmin = false,
                IsHost = false,
                IsOrgMember = false,
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
                    userRight.IsOrgAdmin = member.IsAdmin;
                    userRight.IsHost = isHost;                   // keine Entscheidung möglich
                    userRight.IsOrgMember = true;
                }
            }

            return userRight;
        }

        protected bool HasUserRole(string userName, string orgName, string roleName)
        {
            return new MemberService(Db, UserManager).HasRole(userName, orgName, roleName);
        }

        protected OrganiserMember GetMember(string userName, string orgName)
        {
            return new MemberService(Db, UserManager).GetMember(userName, orgName);
        }

        protected string GetMyOrganisationName()
        {
            return new MemberService(Db, UserManager).GetOrganisationName(GetSemester(), User.Identity.Name);
        }

        protected ActivityOrganiser GetMyOrganisation()
        {
            return new MemberService(Db, UserManager).GetOrganisation(User.Identity.Name);
        }

        protected OrganiserMember GetMyMembership()
        {
            return new MemberService(Db, UserManager).GetMember(User.Identity.Name);
        }

        protected bool IsUserAdminOf(string orgName)
        {
            return new MemberService(Db, UserManager).IsUserAdminOf(User.Identity.Name, orgName);
        }

        protected bool IsUserMemberOf(string orgName)
        {
            return new MemberService(Db, UserManager).IsUserMemberOf(User.Identity.Name, orgName);
        }

        protected bool IsOrgAdmin()
        {
            return new MemberService(Db, UserManager).IsUserOrgAdmin(User.Identity.Name) || User.IsInRole("SysAdmin");
        }

        protected bool IsOrgAdmin(Guid orgId)
        {
            return new MemberService(Db, UserManager).IsUserOrgAdmin(User.Identity.Name, orgId) || User.IsInRole("SysAdmin");
        }


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


        protected Semester GetSemester(ApplicationUser user)
        {
            var semService = new SemesterService();
            // aktuelles Semester
            var currentSemester = semService.GetCurrentSemester();
            if (currentSemester != null)
                return currentSemester;

            // Kein Semester mit dieser Vorlesungszeit
            // hier kommt immer was zurück
            var nextSemester = semService.GetNextSemester();

            // wenn es freigegeben ist, dann zurückgeben
            if (nextSemester.BookingEnabled)
                return nextSemester;

            // (Noch) nicht freigegeben
            // Für Staff => das nächste
            if (user.MemberState == MemberState.Staff || User.IsInRole("SysAdmin"))
            {
                return nextSemester;
            }

            // für alle anderen das vorgehende
            return semService.GetPreviousSemester();
        }

        protected Semester GetSemester()
        {
            return GetSemester(AppUser);
        }

        protected Semester GetSemester(string semester)
        {
            return new SemesterService().GetSemester(semester);
        }
    }
}