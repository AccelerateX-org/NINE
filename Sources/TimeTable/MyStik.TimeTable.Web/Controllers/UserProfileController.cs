using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// Anzeige der Benutzereinstellungen
    /// </summary>
    public class UserProfileController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalData()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EditPersonalData()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new UserProfileViewModel();

            var user = AppUser;

            model.Profile = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            var semester = SemesterService.GetSemester(DateTime.Today);

            var subscription = Db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(x => 
                x.UserId.Equals(user.Id) &&
                x.SemesterGroup.Semester.Id == semester.Id);


            if (subscription != null)
            {
                model.SemesterProfile = new UserSemesterViewModel
                {
                    Faculty = subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.ShortName,
                    Curriculum = subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName,
                    Group = subscription.SemesterGroup.Id.ToString()
                };

                model.SemesterGroup = subscription.SemesterGroup;
            }
            else
            {
                model.SemesterProfile = new UserSemesterViewModel
                {
                    Faculty = string.Empty,
                    Curriculum = string.Empty,
                    Group = string.Empty
                };
            }

            model.MsgProfile = new UserMsgProfileViewModel
            {
                LikeEmailOnGlobalLevel = user.LikeEMails,
            };

            model.MemberState = user.MemberState;
            //model.Semester = semester;

            //notification Section Daten abrufen
            model.UserDevices = new List<UserDeviceViewModel>();

            foreach (UserDevice d in user.Devices){
                UserDeviceViewModel userDevice = new UserDeviceViewModel{
                    DeviceName = d.DeviceName,
                    Activated = d.IsActive,
                    Id = d.Id.ToString(),
                    UserId = d.User.Id.ToString()
                };

                switch (d.Platform)
                {
                    case DevicePlatform.Android:
                        userDevice.DeviceName = "Android";
                        break;
                    case DevicePlatform.IOS:
                        userDevice.DeviceName = "iOS";
                        break;
                    case DevicePlatform.WinPhone:
                        userDevice.DeviceName = "Windows Phone";
                        break;
                }

                model.UserDevices.Add(userDevice);
            }

            //InitSelectionLists(user);

            ViewBag.CalendarToken = user.Id;
            ViewBag.CalendarPeriod = SemesterService.GetSemester(DateTime.Today).Name;


            return View(model);
        }
        /*

        [HttpPost]
        public PartialViewResult SaveSemesterData(UserProfileViewModel model)
        {
            var _db = new ApplicationDbContext();

            var user = _db.Users.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            if (user != null)
            {
                var group = Db.SemesterGroups.SingleOrDefault(g => g.Id.ToString().Equals(model.SemesterProfile.Group));

                if (group != null)
                {
                    // Abwärtskompatibilität
                    user.Faculty = group.CurriculumGroup.Curriculum.Organiser.ShortName;
                    user.Curriculum = group.CurriculumGroup.Curriculum.ShortName;
                    user.Group = group.GroupName;

                    _db.SaveChanges();

                    // es darf nur eine Semesterzugehörigkeit geben
                    var semester = GetSemester();

                    var currentSubscription = Db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(x =>
                        x.UserId.Equals(user.Id) &&
                        x.SemesterGroup.Semester.Id == semester.Id);

                    if (currentSubscription != null)
                    {
                        currentSubscription.SemesterGroup.Subscriptions.Remove(currentSubscription);
                        Db.Subscriptions.Remove(currentSubscription);
                    }

                    var subscription = new SemesterSubscription
                    {
                        TimeStamp = DateTime.Now,
                        UserId = user.Id,
                        LikesEMails = false,
                        LikesNotifications = false,
                        SemesterGroup = group
                    };

                    group.Subscriptions.Add(subscription);
                    Db.Subscriptions.Add(subscription);

                    Db.SaveChanges();

                    model.SemesterGroup = group;
                    model.Semester = semester;

                    return PartialView("_SemesterState", model);
                }
            }

            return null;
        }
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveMsgData(UserProfileViewModel model)
        {
            var _db = new ApplicationDbContext();

            var user = _db.Users.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            if (user != null)
            {
                user.LikeEMails = model.MsgProfile.LikeEmailOnGlobalLevel;

                _db.SaveChanges();
            }

            return new EmptyResult();// View("Index", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveContactData(UserProfileViewModel model)
        {
            var _db = new ApplicationDbContext();

            var user = _db.Users.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            if (user != null)
            {
                if (!string.IsNullOrEmpty(model.Profile.FirstName))
                    user.FirstName = model.Profile.FirstName;

                if (!string.IsNullOrEmpty(model.Profile.LastName))
                    user.LastName = model.Profile.LastName;

                _db.SaveChanges();
            }


            return new EmptyResult();// View("Index", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceList"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult SaveUserDeviceData(List<string[]> deviceList)
        {
            var _db = new ApplicationDbContext();

            var user = _db.Users.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            if (user != null)
            {
                foreach (string[] device in deviceList)
                {
                    if (device[1].Equals("true")){
                        user.Devices.SingleOrDefault(d => d.Id.ToString().Equals(device[0])).IsActive = true;
                    }
                    else
                    {
                        user.Devices.SingleOrDefault(d => d.Id.ToString().Equals(device[0])).IsActive = false;
                    } 
                }
                _db.SaveChanges();
            }

            var model = new UserProfileViewModel();

            model.UserDevices = new List<UserDeviceViewModel>();

            foreach (UserDevice d in user.Devices)
            {
                UserDeviceViewModel userDevice = new UserDeviceViewModel
                {
                    DeviceName = d.DeviceName,
                    Activated = d.IsActive,
                    Id = d.Id.ToString(),
                    UserId = d.User.Id.ToString()
                };
                model.UserDevices.Add(userDevice);
            }

            return PartialView("_UserDevices", model);
        }




        /*
        [HttpPost]
        public PartialViewResult GroupList(string currId)
        {
            var curr = Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(currId));

            var semester = GetSemester();

            var selectList = new List<SelectListItem>();

            if (curr != null)
            {

                var groupList = Db.SemesterGroups.Where(g => g.Semester.Id == semester.Id &&
                                             g.CurriculumGroup.Curriculum.Id == curr.Id).ToList();


                foreach (var group in groupList.OrderBy(g => g.GroupName))
                {
                        selectList.Add(new SelectListItem
                        {
                            Text = group.FullName,
                            Value = group.Id.ToString()
                        });
                    
                }
            }

            return PartialView("_GroupList", selectList);
        }

        private void InitSelectionLists(ApplicationUser user)
        {
            ViewBag.Faculties = Db.Organisers.Where(o => o.ShortName.Equals("FK 09")).Select(f => new SelectListItem
            {
                Text = f.ShortName,
                Value = f.ShortName,
            });


            ViewBag.Curricula = Db.Curricula.Where(x => !x.ShortName.Equals("Export")).Select(c => new SelectListItem
            {
                Text = c.Name + " (" + c.ShortName + ")",
                Value = c.ShortName,
            });


            var curr = string.IsNullOrEmpty(user.Curriculum) ?
                Db.Curricula.First() :
                Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(user.Curriculum));

            var semester = GetSemester();

            if (curr != null)
            {
                var selectList = new List<SelectListItem>();

                    var groupList = Db.SemesterGroups.Where(g => g.Semester.Id == semester.Id &&
                                                 g.CurriculumGroup.Curriculum.Id == curr.Id).ToList();


                    foreach (var group in groupList.OrderBy(g => g.GroupName))
                    {
                        if (!group.CurriculumGroup.Name.Equals("WPM"))
                        {
                            selectList.Add(new SelectListItem
                            {
                                Text = group.FullName,
                                Value = group.Id.ToString()
                            });
                        }
                    }

                ViewBag.Groups = selectList;
            }
        }

        public ActionResult Warning()
        {
            return View();
        }

        public ActionResult Info()
        {
            return View();
        }
         */
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }
    }
}