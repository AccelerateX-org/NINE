using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = "SysAdmin")]
    public class UserController : BaseController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new List<UserAdminViewModel>();

            ViewBag.Students = _db.Users.Count(u => u.MemberState == MemberState.Student);
            ViewBag.Guests = _db.Users.Count(u => u.MemberState == MemberState.Guest);
            ViewBag.Staff = _db.Users.Count(u => u.MemberState == MemberState.Staff);
            ViewBag.TotalUserCount = _db.Users.Count();


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DeleteUser(string id)
        {

            // Alle Eintragungen löschen
            // Das darf nur der Admin, der weiss, was er tut. Daher hier auch keine E-Mail oder ähnliches
            try
            {
                var subscriptions = Db.Subscriptions.OfType<OccurrenceSubscription>().Where(s => !string.IsNullOrEmpty(s.UserId) && s.UserId.Equals(id)).ToList();

                foreach (var subscription in subscriptions)
                {
                    var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var drawing in allDrawings)
                    {
                        Db.SubscriptionDrawings.Remove(drawing);
                    }

                    var bets = subscription.Bets.ToList();
                    foreach (var bet in bets)
                    {
                        Db.LotteriyBets.Remove(bet);
                    }

                    Db.Subscriptions.Remove(subscription);
                }

                var games = Db.LotteryGames.Where(x => x.UserId.Equals(id)).ToList();
                foreach (var lotteryGame in games)
                {
                    Db.LotteryGames.Remove(lotteryGame);
                }


                var semSubscriptions = Db.Subscriptions.OfType<SemesterSubscription>().Where(s => !string.IsNullOrEmpty(s.UserId) && s.UserId.Equals(id)).ToList();

                foreach (var subscription in semSubscriptions)
                {
                    Db.Subscriptions.Remove(subscription);
                }


                Db.SaveChanges();

                var user = UserManager.FindById(id);
                // Devices löschen!
                var devices = _db.Devices.Where(d => d.User.Id.Equals(user.Id)).ToList();
                foreach (var userDevice in devices)
                {
                    _db.Devices.Remove(userDevice);
                }
                _db.SaveChanges();
                UserManager.Delete(user);

                return PartialView("_EmptyRow");
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

                if (ex.InnerException != null)
                {
                    msg += " - ";
                    msg += ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        msg += " - ";
                        msg += ex.InnerException.InnerException.Message;
                    }
                }

                return PartialView("_ErrorRow", msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            var semester = SemesterService.GetSemester(DateTime.Today);

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult TodayLoggedIn()
        {
            var userList = _db.Users.Where(u => u.LastLogin.HasValue && u.LastLogin >= DateTime.Today).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Inactive()
        {
            var border = DateTime.Today.AddDays(-180);

            var userList = _db.Users.Where(u => u.LastLogin.HasValue && u.LastLogin < border).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult NeverLoggedIn()
        {
            var userList = _db.Users.Where(u => !u.LastLogin.HasValue).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Pending()
        {
            var userList = _db.Users.Where(u => !u.EmailConfirmed).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Remarked()
        {
            var userList = _db.Users.Where(u => !string.IsNullOrEmpty(u.Remark)).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Expired()
        {
            var userList = _db.Users.Where(u => u.ExpiryDate.HasValue).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
            var model = new List<UserAdminViewModel>();

            foreach (var user in userList)
            {
                /*
                var semSub =
                    Db.Subscriptions.OfType<SemesterSubscription>()
                        .Where(s => s.UserId.Equals(user.Id))
                        .OrderByDescending(s => s.SemesterGroup.Semester.StartCourses).FirstOrDefault();

                */


                model.Add(new UserAdminViewModel
                {
                    User = user,
                    // SubscriptionCount = string.IsNullOrEmpty(user.Id) ? -99 : Db.Subscriptions.Count(s => s.UserId.Equals(user.Id)),
                    // Members = MemberService.GetMemberships(user.Id),
                    // SemesterGroup = semSub != null ? semSub.SemesterGroup : null
                });
            }

            return model;
        }

        private UserAdminViewModel GetUserModel(ApplicationUser user)
        {
            var model = new UserAdminViewModel
            {
                User = user,
                SubscriptionCount = Db.Subscriptions.Count(s => s.UserId.Equals(user.Id)),
                IsStudent = (StudentService.GetCurrentStudent(user.Id) != null),
                Members = MemberService.GetMemberships(user.Id),
            };

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GuestAccounts()
        {
            var userList = _db.Users.Where(u => u.MemberState == MemberState.Guest).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

    }
}