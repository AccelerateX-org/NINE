using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hangfire;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = "SysAdmin")]
    public class UserController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var users = _db.Users.OrderByDescending(x => x.LastLogin).Take(50).ToList();

            var model = CreateUserList(users);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult LatestRegistrations()
        {
            var userList = _db.Users.Where(u => u.Registered.HasValue).OrderByDescending(u => u.Registered.Value).Take(50).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
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
        public ActionResult ConfirmAll()
        {
            var allUsers = _db.Users.Where(x => !x.IsApproved).ToList();

            foreach (var user in allUsers)
            {
                user.IsApproved = true;
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
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
                var subscriptions = Db.Subscriptions.Where(s => !string.IsNullOrEmpty(s.UserId) && s.UserId.Equals(id)).ToList();

                foreach (var subscription in subscriptions)
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
        public ActionResult Jobs()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckInactive()
        {
            // Sofort ausführen
            BackgroundJob.Enqueue<MailService>(x => x.CheckInactive());

            return RedirectToAction("Jobs");
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
        /// <param name="user"></param>
        /// <returns></returns>
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

    }
}