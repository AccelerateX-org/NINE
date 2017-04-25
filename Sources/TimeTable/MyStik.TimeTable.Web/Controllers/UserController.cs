using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using PagedList;

namespace MyStik.TimeTable.Web.Controllers
{
    [Authorize(Roles = "SysAdmin")]
    public class UserController : BaseController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        //
        // GET: /User/
        public ActionResult Index()
        {
            var model = new List<UserAdminViewModel>();

            ViewBag.Students = _db.Users.Count(u => u.MemberState == MemberState.Student);
            ViewBag.Guests = _db.Users.Count(u => u.MemberState == MemberState.Guest);
            ViewBag.Staff = _db.Users.Count(u => u.MemberState == MemberState.Staff);
            ViewBag.TotalUserCount = _db.Users.Count();


            return View(model);
        }

        [HttpPost]
        public PartialViewResult DeleteUser(string id)
        {

            // Alle Eintragungen löschen
            // Das darf nur der Admin, der weiss, was er tut. Daher hier auch keine E-Mail oder ähnliches
            var subscriptions = Db.Subscriptions.Where(s => !string.IsNullOrEmpty(s.UserId) && s.UserId.Equals(id)).ToList();

            foreach (var subscription in subscriptions)
            {
                Db.Subscriptions.Remove(subscription);
            }
            Db.SaveChanges();

            var user = UserManager.FindById(id);
            UserManager.Delete(user);

            return PartialView("_EmptyRow");
        }


        public ActionResult Statistics()
        {
            var userDB = new ApplicationDbContext();

            var model = new UserStatisticsModel
            {
                UserCount = userDB.Users.Count(),
                RegisteredToday = 0,
                ApprovedToday = 0,
                LogedInToday = 0,
            };


            foreach (var role in userDB.Roles.ToList())
            {
                model.RoleStatistics.Add(new RoleStatisticsModel
                {
                    RoleName = role.Name,
                    UserCount = userDB.Users.Count(u => u.Roles.Any(r => r.RoleId.Equals(role.Id))),
                });
            }

            var timeDB = new TimeTableDbContext();

            foreach (var org in timeDB.Organisers.ToList())
            {
                model.OrgStatistics.Add(new OrgStatisticsModel
                {
                    OrgName = org.Name,
                    UserCount = org.Members.Count(m => !string.IsNullOrEmpty(m.UserId)),
                    AdminCount = org.Members.Count(m => m.IsAdmin),
                });
            }




            return View(model);
        }

        public ActionResult Subscriptions(string id)
        {
            var subscriptions = Db.Subscriptions.OfType<OccurrenceSubscription>().Where(s => s.UserId.Equals(id)).OrderBy(s => s.TimeStamp).ToList();

            var model = new UserSubscriptionViewModel();
            
            // Alle Subscriptions
            foreach (var subscription in subscriptions)
            {
                model.Subscriptions.Add(new UserSubscriptionModel
                {
                    Subscription = subscription,
                    Summary = subscription.Occurrence != null ? ActivityService.GetSummary(subscription.Occurrence.Id) : null,
                });
            }

            // Test: Abfrage der WPMs
            var semester = GetSemester();

            var wpmList = Db.Activities.OfType<Course>().Where(a => 
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(id)) &&
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.SemesterGroups.Any(g => g.CapacityGroup.CurriculumGroup.Name.Equals("WPM"))).ToList();

            foreach (var wpm in wpmList)
            {
                // es könnten ja auch mehrere Eintragungen vorliegen
                var wpmsubs = wpm.Occurrence.Subscriptions.Where(s => s.UserId.Equals(id));

                foreach (var subscription in wpmsubs)
                {
                    model.WPMSubscriptions.Add(new UserSubscriptionModel
                    {
                        Subscription = subscription,
                        Summary = subscription.Occurrence != null ? ActivityService.GetSummary(subscription.Occurrence.Id) : null,
                    });
                }
            }

            model.Semester = Db.Subscriptions.OfType<SemesterSubscription>().Where(s => s.UserId.Equals(id)).OrderBy(s => s.TimeStamp).ToList();

            return View(model);
        }


        [HttpPost]
        public PartialViewResult TodayLoggedIn()
        {
            var userList = _db.Users.Where(u => u.LastLogin.HasValue && u.LastLogin >= DateTime.Today).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult Inactive()
        {
            var border = DateTime.Today.AddDays(-180);

            var userList = _db.Users.Where(u => u.LastLogin.HasValue && u.LastLogin < border).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult NeverLoggedIn()
        {
            var userList = _db.Users.Where(u => !u.LastLogin.HasValue).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult Pending()
        {
            var userList = _db.Users.Where(u => !u.EmailConfirmed).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult Remarked()
        {
            var userList = _db.Users.Where(u => !string.IsNullOrEmpty(u.Remark)).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult Expired()
        {
            var userList = _db.Users.Where(u => u.ExpiryDate.HasValue).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public async Task<PartialViewResult> ConfirmAll()
        {
            await _db.Users.ForEachAsync(u => u.EmailConfirmed = true);
            await _db.Users.ForEachAsync(u => u.Remark = String.Empty);
            await _db.Users.ForEachAsync(u => u.Submitted = null);
            await _db.SaveChangesAsync();

            var userList = _db.Users.Take(50).OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.UserName).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        private ICollection<UserAdminViewModel> CreateUserList(ICollection<ApplicationUser> userList)
        {
            var memberService = new MemberService(Db, UserManager);

            var model = new List<UserAdminViewModel>();

            foreach (var user in userList)
            {
                var semSub =
                    Db.Subscriptions.OfType<SemesterSubscription>()
                        .Where(s => s.UserId.Equals(user.Id))
                        .OrderByDescending(s => s.SemesterGroup.Semester.StartCourses).FirstOrDefault();




                model.Add(new UserAdminViewModel
                {
                    User = user,
                    SubscriptionCount = string.IsNullOrEmpty(user.Id) ? -99 : Db.Subscriptions.Count(s => s.UserId.Equals(user.Id)),
                    Members = memberService.GetMemberships(user.UserName),
                    SemesterGroup = semSub != null ? semSub.SemesterGroup : null
                });
            }

            return model;
        }

        private UserAdminViewModel GetUserModel(ApplicationUser user)
        {
            var memberService = new MemberService(Db, UserManager);

            var model = new UserAdminViewModel
            {
                User = user,
                SubscriptionCount = Db.Subscriptions.Count(s => s.UserId.Equals(user.Id)),
                IsStudent = memberService.IsStudent(user.UserName),
                Members = memberService.GetMemberships(user.UserName),
            };

            return model;
        }

        [HttpPost]
        public PartialViewResult GuestAccounts()
        {
            var userList = _db.Users.Where(u => u.MemberState == MemberState.Guest).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult Doubles()
        {
            // alle Gäste
            var guests = _db.Users.Where(u => u.MemberState == MemberState.Guest).OrderBy(u => u.LastLogin.Value).ToList();

            var userList = new List<ApplicationUser>();

            foreach (var guest in guests)
            {
                var candidates = _db.Users.Where(u => u.LastName.ToUpper().Equals(guest.LastName.ToUpper()) &&
                    u.FirstName.ToUpper().Equals(guest.FirstName.ToUpper()) &&
                    u.Id != guest.Id);
                
                if (candidates.Any())
                {
                    userList.Add(guest);
                    userList.AddRange(candidates.ToList());
                }
            }


            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult MakeStudent(string id)
        {
            var user = UserManager.FindById(id);
            if (user != null)
            {
                user.MemberState = MemberState.Student;
                UserManager.Update(user);
            }

            var model = GetUserModel(user);

            return PartialView("_UserListEntry", model);
        }

        [HttpPost]
        public PartialViewResult MakeGuest(string id)
        {
            var user = UserManager.FindById(id);
            if (user != null)
            {
                user.MemberState = MemberState.Guest;
                UserManager.Update(user);
            }

            var model = GetUserModel(user);

            return PartialView("_UserListEntry", model);
        }

        [HttpPost]
        public PartialViewResult MakeStaff(string id)
        {
            var user = UserManager.FindById(id);
            if (user != null)
            {
                user.MemberState = MemberState.Staff;
                UserManager.Update(user);
            }

            var model = GetUserModel(user);

            return PartialView("_UserListEntry", model);
        }


        [HttpPost]
        public PartialViewResult RepairUser(string id)
        {
            var user = UserManager.FindById(id);
            if (user != null)
            {
                user.EmailConfirmed = true;
                user.Remark = string.Empty;
                user.Submitted = null;
                user.ExpiryDate = null;
                UserManager.Update(user);
            }

            var model = GetUserModel(user);

            return PartialView("_UserListEntry", model);
        }

        [HttpPost]
        public PartialViewResult Search(string searchString)
        {
            var userList = _db.Users.Where(u => 
                u.UserName.Contains(searchString) ||
                u.FirstName.Contains(searchString) ||
                u.LastName.Contains(searchString) ||
                u.Email.Contains(searchString)).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        [HttpPost]
        public PartialViewResult TestAccounts()
        {
            var userList = new List<ApplicationUser>();

            //try
            //{

            var fk09 = Db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FK 09"));
            if (fk09 != null)
            {
                var profsWithoutAccount =
                    fk09.Members.Where(m => !string.IsNullOrEmpty(m.Role) &&
                        m.Role.Equals("Prof") &&
                        string.IsNullOrEmpty(m.UserId) &&
                        !string.IsNullOrEmpty(m.Name) &&
                        !string.IsNullOrEmpty(m.ShortName)
                        ).ToList();
                var now = DateTime.Now;

                foreach (var prof in profsWithoutAccount)
                {
                    var user = new ApplicationUser
                    {
                        UserName = prof.Name,
                        Email = prof.ShortName + "@wi.hm.edu",
                        FirstName = string.Empty,
                        LastName = prof.Name,
                        Registered = now,
                        MemberState = MemberState.Staff,
                        Remark = "Testkonto",
                        ExpiryDate = null, // Einladung bleibt dauerhaft bestehen - Deprovisionierung automatisch
                        Submitted = now,
                        EmailConfirmed = true,
                    };

                    // Benutzer anlegen, mit Dummy Passwort
                    var result = UserManager.Create(user, "Test09#15");
                    if (result.Succeeded)
                    {
                        var profUser = UserManager.FindByName(prof.Name);
                        userList.Add(profUser);
                        prof.UserId = profUser.Id;
                    }
                    else
                    {
                        user.Id = Guid.NewGuid().ToString();
                        foreach (var error in result.Errors)
                        {
                            user.Remark += error;
                        }
                        userList.Add(user);
                    }
                }
                Db.SaveChanges();
            }
            /*
            }
            catch (Exception ex)
            {
                var errorUser = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Remark = ex.Message
                };
                userList.Add(errorUser);
            }
            */

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
            //return View(model);
        }


        public ActionResult AccountManagement()
        {
            var today = DateTime.Today;
            var dateWarning = DateTime.Today.AddDays(-120);
            var dateExpire = DateTime.Today.AddDays(-150);

            ViewBag.Inactive = _db.Users.Count(x => x.MemberState == MemberState.Student && x.LastLogin.HasValue && x.LastLogin < dateWarning && !x.Approved.HasValue);
            ViewBag.InactiveWithWarning = _db.Users.Count(x => x.MemberState == MemberState.Student && x.LastLogin.HasValue && x.LastLogin < dateWarning && x.Approved.HasValue);
            ViewBag.InactiveStage2 = _db.Users.Count(x => x.MemberState == MemberState.Student && x.LastLogin.HasValue && x.LastLogin < dateExpire && x.Approved.HasValue && !x.ExpiryDate.HasValue);
            ViewBag.InactiveDeactivated = _db.Users.Count(x => x.MemberState == MemberState.Student && x.LastLogin.HasValue && x.LastLogin < dateExpire && x.Approved.HasValue && x.ExpiryDate.HasValue);
            ViewBag.Expired = _db.Users.Count(x => x.MemberState == MemberState.Student && x.LastLogin.HasValue && x.LastLogin < dateExpire && x.Approved.HasValue && x.ExpiryDate.HasValue && x.ExpiryDate < today);

            return View();
        }



        [HttpPost]
        public PartialViewResult InactiveStage1()
        {
            var today = DateTime.Today;
            var dateWarning = DateTime.Today.AddDays(-120);
            var dateExpire = DateTime.Today.AddDays(-150);

            var userList = _db.Users.Where(x => x.MemberState == MemberState.Student &&
                x.LastLogin.HasValue && x.LastLogin < dateWarning &&
                !x.Approved.HasValue).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        [HttpPost]
        public PartialViewResult InactiveWarned()
        {
            var today = DateTime.Today;
            var dateWarning = DateTime.Today.AddDays(-120);
            var dateExpire = DateTime.Today.AddDays(-150);

            var userList = _db.Users.Where(x => x.MemberState == MemberState.Student && 
                x.LastLogin.HasValue && x.LastLogin < dateWarning &&
                x.Approved.HasValue).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        [HttpPost]
        public PartialViewResult InactiveStage2()
        {
            var today = DateTime.Today;
            var dateWarning = DateTime.Today.AddDays(-120);
            var dateExpire = DateTime.Today.AddDays(-150);

            var userList = _db.Users.Where(x => x.MemberState == MemberState.Student && 
                x.LastLogin.HasValue && x.LastLogin < dateExpire && x.Approved.HasValue &&
                !x.ExpiryDate.HasValue).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        [HttpPost]
        public PartialViewResult InactiveDeactivated()
        {
            var today = DateTime.Today;
            var dateWarning = DateTime.Today.AddDays(-120);
            var dateExpire = DateTime.Today.AddDays(-150);

            var userList = _db.Users.Where(x => x.MemberState == MemberState.Student && 
                x.LastLogin.HasValue && x.LastLogin < dateExpire && x.Approved.HasValue &&
                x.ExpiryDate.HasValue).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        [HttpPost]
        public PartialViewResult InactiveExpired()
        {
            var today = DateTime.Today;
            var dateWarning = DateTime.Today.AddDays(-120);
            var dateExpire = DateTime.Today.AddDays(-150);

            var userList = _db.Users.Where(x => x.MemberState == MemberState.Student && 
                x.LastLogin.HasValue && x.LastLogin < dateExpire && x.Approved.HasValue && x.ExpiryDate.HasValue &&
                x.ExpiryDate < today).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        public ActionResult ResetWarnings()
        {
            var userList = _db.Users.Where(x => x.Approved.HasValue).ToList();
            foreach (var user in userList)
            {
                user.Approved = null;
            }
            _db.SaveChanges();
            
            return RedirectToAction("AccountManagement");
        }

        public ActionResult ChangeUserName(string id)
        {
            var model = new UserManageViewModel();

            var user = UserManager.FindById(id);
            if (user != null)
            {
                model.User = user;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeUserName(UserManageViewModel model)
        {
            var user = UserManager.FindById(model.User.Id);
            if (user != null)
            {
                user.Email = model.EMail;
                user.UserName = model.EMail;

                UserManager.Update(user);
            }


            return RedirectToAction("Index");
        }

    }
}