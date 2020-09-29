using System;
using System.Collections.Generic;
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
    public class UnionController : BaseController
    {
        private ActivityOrganiser _faculty;
        private ActivityOrganiser _union;

        private void Init(Guid? id)
        {
            _faculty = GetMyOrganisation();
            _union = null;

            if (id != null)
            {
                _union = GetOrganiser(id.Value);
            }
            else
            {
                var unionName = _faculty.ShortName.Replace("FK", "FS");
                _union = GetOrganiser(unionName);
            }


            ViewBag.FacultyUserRight = GetUserRight(_faculty);
            ViewBag.UnionUserRight = GetUserRight(_union);

        }

        /// <summary>
        /// Übersicht der Fachschaft
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            Init(id);

            var user = GetCurrentUser();

            if (_union == null)
                return View("NoUnion", _faculty);

            var model = new OrganiserViewModel
            {
                Semester = SemesterService.GetSemester(DateTime.Today),
                Organiser = _union
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Member(Guid id)
        {
            Init(id);

            return View(_union);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Newsletter(Guid id)
        {
            var union = GetOrganiser(id);
            var userRight = GetUserRight(union);

            ViewBag.UserRight = userRight;
            ViewBag.Semester = SemesterService.GetSemester(DateTime.Today);
            ViewBag.Union = union;

            var newsletters = Db.Activities.OfType<Newsletter>().Where(x => x.Organiser.Id == union.Id).ToList();



            return View(newsletters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.Id == id);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var members = organiser.Members.OrderBy(m => m.Name);
            var myUser = UserManager.FindByName(User.Identity.Name);

            foreach (var member in members)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View(model);
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

                DeleteMember(member);

                return RedirectToAction("Member", new {id = org.Id});
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult WithdrawMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var org = member.Organiser;

                DeleteMember(member);

                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
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

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                //member.IsAdmin = model.IsAdmin;
                member.Role = model.Role;

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Details", new {id = member.Organiser.Id});
            }

            return RedirectToAction("Index", "Students");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetMember(Guid id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.Id == id);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Benutzer als neues Mitglied eintragen
            var user = AppUser;
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            // nach dem Benutzer suchen
            var member = organiser.Members.FirstOrDefault(m => m.UserId.Equals(user.Id));
            if (member == null)
            {

                member = new OrganiserMember
                {
                    IsAdmin = false,
                    UserId = user.Id,
                    IsAssociated = true // "Gast"
                };

                organiser.Members.Add(member);
            }

            // Den Token generieren
            var token = System.Web.Security.Membership.GeneratePassword(8, 2);
            member.ShortName = token;

            Db.SaveChanges();

            /*
            // Eine E-Mail senden
            var mailModel = new OrgMemberMailModel
            {
                User = user,
                Organiser = organiser,
                Token = token,
            };

            new MailController().RegisterUnionEMail(mailModel).Deliver();
            */


            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ConfirmMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            if (member != null)
            {
                member.ShortName = string.Empty;
                member.IsAssociated = false;

                Db.SaveChanges();
            }


            return RedirectToAction("Member", new { id = member.Organiser.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmMember(Guid id, string token)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);
            var org = member.Organiser;

            if (member.ShortName.Equals(token))
            {
                member.ShortName = string.Empty;
                member.IsAssociated = false;

                Db.SaveChanges();
            }


            return RedirectToAction("Member", new {id = org.Id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddAdmin(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);
            var org = member.Organiser;

            member.IsAdmin = true;
            member.IsMemberAdmin = true;
            member.IsRoomAdmin = true;
            member.IsEventAdmin = true;
            member.IsNewsAdmin = true;
            member.IsAssociated = false;
            Db.SaveChanges();

            return RedirectToAction("Member", new { id = org.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveAdmin(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);
            var org = member.Organiser;

            member.IsAdmin = false;
            member.IsMemberAdmin = false;
            member.IsRoomAdmin = false;
            member.IsEventAdmin = false;
            member.IsNewsAdmin = false;
            Db.SaveChanges();

            return RedirectToAction("Member", new { id = org.Id });

        }


        public ActionResult Clear(Guid id)
        {
            var org = GetOrganiser(id);
            var userService = new UserInfoService();

            foreach (var member in org.Members.ToList())
            {
                var user = userService.GetUser(member.UserId);

                if (user == null)
                {
                    DeleteMember(member);
                }
            }

            return RedirectToAction("Member", new {id = id});
        }


        private void DeleteMember(OrganiserMember member)
        {
            var allOwners = Db.ActivityOwners.Where(x => x.Member.Id == member.Id).ToList();
            foreach (var owner in allOwners)
            {
                Db.ActivityOwners.Remove(owner);
            }

            var org = member.Organiser;

            org.Members.Remove(member);
            Db.Members.Remove(member);

            Db.SaveChanges();

        }

        public ActionResult CreateEvent()
        {
            Init(null);
            var sem = SemesterService.GetSemester(DateTime.Today);
            var org = _union;

            CourseCreateModel2 model = new CourseCreateModel2();

            model.SemesterId = sem.Id;
            model.OrganiserId = org.Id;
            model.OrganiserId2 = org.Id;
            model.OrganiserId3 = org.Id;

            // Liste aller Fakultäten
            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Liste aller Fakultäten, auf die Zugriff auf Räume bestehen
            // aktuell nur meine
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            ViewBag.Semester = Db.Semesters.Where(x => x.EndCourses >= DateTime.Today).OrderBy(s => s.StartCourses).Take(5).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            model.SemesterId = sem.Id;

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateEvent(CourseCreateModelExtended model)
        {
            Init(null);
            var user = GetCurrentUser();

            var org = _union;

            var @event = new Event
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Description = model.Description,
                Organiser = org,
                Occurrence = new Occurrence
                {
                    Capacity = -1,
                    IsAvailable = true,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    IsCanceled = false,
                    IsMoved = false,
                    UseGroups = false,
                },
            };

            if (model.GroupIds != null)
            {
                foreach (var groupId in model.GroupIds)
                {
                    var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == groupId);

                    if (semGroup != null)
                    {
                        @event.SemesterGroups.Add(semGroup);

                        var occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = @event.Occurrence
                        };
                        occGroup.SemesterGroups.Add(semGroup);
                        semGroup.OccurrenceGroups.Add(occGroup);
                        @event.Occurrence.Groups.Add(occGroup);
                        Db.OccurrenceGroups.Add(occGroup);
                    }
                }
            }

            /*
            var member = GetMember(user.UserName, _union.ShortName);

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = @event,
                    Member = me,
                    IsLocked = false
                };

                @event.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }*/

            var dozList = new List<OrganiserMember>();
            if (model.DozIds != null)
            {
                dozList.AddRange(model.DozIds.Select(dozId => Db.Members.SingleOrDefault(g => g.Id == dozId)).Where(doz => doz != null));
            }

            var roomList = new List<Room>();
            if (model.RoomIds != null)
            {
                roomList.AddRange(model.RoomIds.Select(roomId => Db.Rooms.SingleOrDefault(g => g.Id == roomId)).Where(doz => doz != null));
            }

            // Termine anelegen
            var semesterService = new SemesterService();

            if (model.Dates != null)
            {
                foreach (var date in model.Dates)
                {
                    string[] elems = date.Split('#');
                    var day = DateTime.Parse(elems[0]);
                    var begin = TimeSpan.Parse(elems[1]);
                    var end = TimeSpan.Parse(elems[2]);
                    var isWdh = bool.Parse(elems[3]);

                    ICollection<DateTime> dayList;
                    var semester = semesterService.GetSemester(day);

                    if (isWdh && semester != null)
                    {
                        dayList = semesterService.GetDays(semester.Id, day);
                    }
                    else
                    {
                        dayList = new List<DateTime> { day };
                    }


                    foreach (var dateDay in dayList)
                    {
                        var activityDate = new ActivityDate
                        {
                            Activity = @event,
                            Begin = dateDay.Add(begin),
                            End = dateDay.Add(end),
                            Occurrence = new Occurrence
                            {
                                Capacity = -1,
                                IsAvailable = true,
                                FromIsRestricted = false,
                                UntilIsRestricted = false,
                                IsCanceled = false,
                                IsMoved = false,
                                UseGroups = false,
                            },

                        };

                        foreach (var room in roomList)
                        {
                            activityDate.Rooms.Add(room);
                        }

                        foreach (var doz in dozList)
                        {
                            activityDate.Hosts.Add(doz);
                        }

                        Db.ActivityDates.Add(activityDate);

                    }
                }
            }


            Db.Activities.Add(@event);
            Db.SaveChanges();


            return PartialView("_CreateEventSuccess", @event);
        }

    }
}
