using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hangfire;
using Microsoft.AspNet.Identity;
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
            var model = new List<UserAdminViewModel>();

            foreach (var user in userList)
            {
                model.Add(new UserAdminViewModel
                {
                    User = user,
                    SubscriptionCount = string.IsNullOrEmpty(user.Id) ? -99 : Db.Subscriptions.Count(s => s.UserId.Equals(user.Id)),
                    Members = MemberService.GetMemberships(user.Id),
                    SemesterGroup = null,
                    Student = StudentService.GetCurrentStudent(user)
                });
            }

            return model;
        }


        /// <summary>
        /// Zeigt die Logins der letzten 15 min
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var time = DateTime.Now.AddMinutes(-30);

            var users = _db.Users.Where(x => x.LastLogin >= time).OrderByDescending(x => x.LastLogin).ToList();

            var model = CreateUserList(users);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public PartialViewResult GuestAccounts()
        {
            var userList = _db.Users.Where(u => u.MemberState == MemberState.Guest).OrderBy(u => u.Registered.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
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
        public PartialViewResult InactiveGuests()
        {
            var border = DateTime.Today.AddDays(-180);

            var userList = _db.Users.Where(u => u.MemberState == MemberState.Guest && u.LastLogin.HasValue && u.LastLogin < border).OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult InactiveStudents()
        {
            var border = DateTime.Today.AddDays(-360);

            var userList = _db.Users
                .Where(u => u.MemberState == MemberState.Student && u.LastLogin.HasValue && u.LastLogin < border)
                .Take(200)
                .OrderBy(u => u.LastLogin.Value).ToList();

            var model = CreateUserList(userList);

            return PartialView("_UserList", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult InactiveStaff()
        {
            var border = DateTime.Today.AddDays(-180);

            var userList = _db.Users.Where(u => u.MemberState == MemberState.Staff && u.LastLogin.HasValue && u.LastLogin < border).OrderBy(u => u.LastLogin.Value).ToList();

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
        /// Alle Doppelten
        /// Es werden zu den Gästen Konten mit identischem Nachem gesucht
        /// </summary>
        /// <returns>Liste der doppelten</returns>
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
            // es wird nur noch der User gelöscht, alle Infos bleiben erhalten
            try
            {
                var user = UserManager.FindById(id);
                // Devices löschen!
                var devices = _db.Devices.Where(d => d.User.Id.Equals(user.Id)).ToList();
                foreach (var userDevice in devices)
                {
                    _db.Devices.Remove(userDevice);
                }
                _db.SaveChanges();
                UserManager.Delete(user);

                var mailModel = new DeleteUserMailModel { User = user };
                new MailController().DeleteUserMail(mailModel).Deliver();


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

                return PartialView("_EmptyRow", msg);
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
            var studentService = new StudentService(Db);

            var model = new UserAdminViewModel
            {
                User = user,
                SubscriptionCount = Db.Subscriptions.Count(s => s.UserId.Equals(user.Id)),
                IsStudent = (StudentService.GetCurrentStudent(user.Id) != null),
                Members = MemberService.GetMemberships(user.Id),
                Student = studentService.GetCurrentStudent(user)
            };

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        public ActionResult Student(string id)
        {
            var user = UserManager.FindById(id);
            var model = GetUserModel(user);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult DeleteStudent(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);

            if (student != null)
            {
                Db.Students.Remove(student);

                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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