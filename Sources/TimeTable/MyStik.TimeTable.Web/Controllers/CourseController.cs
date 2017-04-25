using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ActionMailer.Net;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CourseController : BaseController
    {
        /// <summary>
        /// Administration einer Lehrveranstaltung
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <returns></returns>
        public ActionResult Admin(Guid? id)
        {
            if (!id.HasValue)
            {
                id = new Guid("59ae8bcc-6e19-e411-bf1b-08606eeb7e78");
            }


            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            // nur Hosts und Admin dürfen die Seite aufrufen
            var userRights = GetUserRight(User.Identity.Name, course);
            if (!(userRights.IsHost || userRights.IsOrgAdmin))
            {
                return RedirectToAction("Details", new {id = id});
            }



            if (string.IsNullOrEmpty(course.Name))
                course.Name = "N.N.";

            if (string.IsNullOrEmpty(course.ShortName))
                course.ShortName = "N.N.";


            // Alle Services, die benötigt werden
            var courseService = new CourseService(UserManager, id.Value);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                DateSummary = courseService.GetDateSummary(),
                Lecturers = courseService.GetLecturerList(),
                Rooms = courseService.GetRoomList(),
                NextDate = courseService.GetNextDate(),
                ParticipantList = courseService.GetTotalParticipantList(),
                //WaitingList = courseService.GetWaitingList(),
                Groups = courseService.GetSubscriptionGroups(),
                Description2 = course.Description,
                CapacitySettings = courseService.GetCapacitySettings(),
            };

            model.SelectedOption = model.CapacitySettings.SingleOrDefault(o => o.Selected);

            var semester = GetSemester();
            var org = GetMyOrganisation();

            model.SemesterId = semester.Id;
            model.OrganiserId = org.Id;

            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            ViewBag.Semester = Db.Semesters.OrderByDescending(s => s.StartCourses).Take(3).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            
            ViewBag.Curricula = Db.Curricula.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            model.CurriculumId = curr != null ? curr.Id : Guid.Empty;
            ViewBag.Curricula = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var semGroups = Db.SemesterGroups.Where(g => g.Semester.Id == semester.Id &&
                                                         g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)
                .ToList();

            var group = semGroups.FirstOrDefault();
            model.GroupId = group != null ? group.Id : Guid.Empty;
            ViewBag.Groups = semGroups.Select(c => new SelectListItem
            {
                Text = c.GroupName,
                Value = c.Id.ToString(),
            });


            foreach (var partcipiant in model.ParticipantList)
            {
                var subscription = 
                Db.Subscriptions.OfType<SemesterSubscription>()
                    .FirstOrDefault(s => s.UserId.Equals(partcipiant.User.Id) && s.SemesterGroup.Semester.Id == semester.Id);

                if (subscription != null)
                {
                    partcipiant.SemesterGroup = subscription.SemesterGroup;
                }
            }


            ViewBag.UserRight = userRights;


            return View(model);
        }

        public ActionResult Details(Guid? id)
        {
            if (!id.HasValue)
            {
                id = new Guid("59ae8bcc-6e19-e411-bf1b-08606eeb7e78");
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            if (string.IsNullOrEmpty(course.Name))
                course.Name = "N.N.";

            if (string.IsNullOrEmpty(course.ShortName))
                course.ShortName = "N.N.";

            // Alle Services, die benötigt werden
            var courseService = new CourseService(UserManager, id.Value);

            var user = AppUser;
            var semester = GetSemester();

            var model = new CourseDetailViewModel()
            {
                Course = course,
                DateSummary = courseService.GetDateSummary(),
                Lecturers = courseService.GetLecturerList(),
                Rooms = courseService.GetRoomList(),
                NextDate = courseService.GetNextDate(),
                ParticipantList = courseService.GetParticipantList(),
                WaitingList = courseService.GetWaitingList(),
                Groups = courseService.GetSubscriptionGroups(),
                CapacitySettings = courseService.GetCapacitySettings(),
                State = ActivityService.GetActivityState(course.Occurrence, user, semester)
            };

            model.SelectedOption = model.CapacitySettings.SingleOrDefault(o => o.Selected);


            foreach (var partcipiant in model.ParticipantList)
            {
                var subscription =
                Db.Subscriptions.OfType<SemesterSubscription>()
                    .FirstOrDefault(s => s.UserId.Equals(partcipiant.User.Id) && s.SemesterGroup.Semester.Id == semester.Id);

                if (subscription != null)
                {
                    partcipiant.SemesterGroup = subscription.SemesterGroup;
                }
            }


            foreach (var partcipiant in model.WaitingList)
            {
                var subscription =
                Db.Subscriptions.OfType<SemesterSubscription>()
                    .FirstOrDefault(s => s.UserId.Equals(partcipiant.User.Id) && s.SemesterGroup.Semester.Id == semester.Id);

                if (subscription != null)
                {
                    partcipiant.SemesterGroup = subscription.SemesterGroup;
                }
            }


            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);


            return View(model);
        }

        public ActionResult ChangeGroups(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            var org = GetMyOrganisation();
            var sem = GetSemester();

            var model = new CourseCreateModel3
            {
                Course = course,
                CourseId = course.Id,
                Semester = sem
            };


            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            ViewBag.Semester = Db.Semesters.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            model.CurriculumId = curr != null ? curr.Id : Guid.Empty;
            ViewBag.Curricula = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var semGroups = Db.SemesterGroups.Where(g => g.Semester.Id == sem.Id &&
                                                         g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)
                .ToList();

            var group = semGroups.FirstOrDefault();
            model.GroupId = group != null ? group.Id : Guid.Empty;
            ViewBag.Groups = semGroups.Select(c => new SelectListItem
            {
                Text = c.GroupName,
                Value = c.Id.ToString(),
            });
            
            
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ChangeGroups(CourseCreateModel3 model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);
            if (course == null || model.GroupIds == null)
                return PartialView("_SaveSuccess");

            
            // die aktuell vorhandenen Gruppen
            var groupList = course.SemesterGroups.ToList();

            foreach (var groupId in model.GroupIds)
            {
                // ist die Gruppe bereits vorhanden?
                var group = groupList.FirstOrDefault(g => g.Id == groupId);
                if (group != null)
                {
                    // Gruppe bereits vorhanden, aus der Liste entfernen,
                    // sont keine weitere Aktion
                    groupList.Remove(group);
                }
                else
                {
                    // neue Gruppe
                    // hinzufügen
                    var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == groupId);
                    if (semGroup != null)
                    {
                        course.SemesterGroups.Add(semGroup);

                        // zugehörige Occurrencegroup anlegen
                        var occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = course.Occurrence
                        };
                        occGroup.SemesterGroups.Add(semGroup);
                        semGroup.OccurrenceGroups.Add(occGroup);
                        course.Occurrence.Groups.Add(occGroup);
                        Db.OccurrenceGroups.Add(occGroup);
                    }
                }
            }


            // in der Liste dürften jetzt nur noch die zu löschenden Gruppen sein
            foreach (var semGroup in groupList)
            {
                course.SemesterGroups.Remove(semGroup);
                
                // zugehörige OccurrenceGroup löschen
                var group = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Contains(semGroup));
                if (group != null)
                {
                    course.Occurrence.Groups.Remove(group);
                    Db.OccurrenceGroups.Remove(group);
                }
            }


            Db.SaveChanges();

            return PartialView("_SaveSuccess");
        }





        public ActionResult Index(Guid? id)
        {
            if (id.HasValue)
                return RedirectToAction("Details", new {id = id});

            return View();
        }

        [HttpPost]
        public PartialViewResult RemoveSubscription(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    occurrence.Subscriptions.Remove(subscription);
                    Db.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var summary = ActivityService.GetSummary(id);
                    var user = UserManager.FindById(subscription.UserId);

                    logger.InfoFormat("Subscription removed: {0}, {1} by {2}", summary.Name, user.UserName, User.Identity.Name);

                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = subscription,
                        User = user,
                        SenderUser = UserManager.FindByName(User.Identity.Name),
                    };

                    var mail = new MailController();
                    mail.RemoveSubscription(mailModel).Deliver();

                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }


            return PartialView("_RemoveSubscription");
        }

        [HttpPost]
        public PartialViewResult ClearSubscriptions(Guid occurrenceId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == occurrenceId);

            if (occurrence != null)
            {
                var summary = ActivityService.GetSummary(occurrenceId);

                foreach (var subscription in occurrence.Subscriptions.ToList())
                {
                    occurrence.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = subscription,
                        User = UserManager.FindById(subscription.UserId),
                        SenderUser = UserManager.FindByName(User.Identity.Name),
                    };

                    var mail = new MailController();
                    mail.RemoveSubscription(mailModel).Deliver();
                }

                logger.InfoFormat("Subscription cleared {0} by {1}", summary.Name, User.Identity.Name);
            }

            return PartialView("_RemoveSubscription");
        }

        public ActionResult CancelDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();
            
            string sText;
            var email = mailing.CancelOccurrence(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }



        [HttpPost]
        public ActionResult CancelDate(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Course");
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = true;

                var date = Db.ActivityDates.SingleOrDefault(d => d.Occurrence.Id == occ.Id);
                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    date.Rooms.Clear();
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Course date {0} on {1} canceled by {2}", summary.Activity.Name, summary.Date.Begin, User.Identity.Name);


                // Wenn es keine Eintragungen gibt, dann muss auch keine mail versendet werden
                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});
                }
            }


            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }

        /// <summary>
        /// CourseId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CancelAllDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var summary = ActivityService.GetSummary(course.Occurrence.Id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.CancelAllOccurrences(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = course.Occurrence.Id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CancelAllDates(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == model.OccurrenceId);

            if (course != null)
            {
                foreach (var date in course.Dates)
                {
                    date.Occurrence.IsCanceled = true;
                    date.Rooms.Clear();
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId);

                logger.InfoFormat("all course dates {0} canceled by {2}", summary.Activity.Name, User.Identity.Name);

                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }


            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }



        public ActionResult ReactivateDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.ReactivateOccurrence(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ReactivateDate(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = false;
                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Course date {0} on {1} reactivated by {2}", summary.Activity.Name, summary.Date.Begin, User.Identity.Name);


                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }

            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }

        public ActionResult MoveDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;
            var course = summary.Activity as Course;
            var date = summary.Date;

            var model = new CourseMoveDateModel
            {
                Course = course,
                ActivityDateId = date.Id,
                Date = date,
                NewDate = date.Begin.ToShortDateString(),
                NewBegin = date.Begin.TimeOfDay.ToString(),
                NewEnd = date.End.TimeOfDay.ToString(),
                OrganiserId2 = course.Organiser.Id,
                OrganiserId3 = course.Organiser.Id,
            };

            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        [HttpPost]
        public PartialViewResult MoveDate(CourseMoveDateModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == model.ActivityDateId);

            if (activityDate == null)
                return PartialView("_SaveError"); // muss noch gemacht werden

            // Berechnung der neuen Zeiten
            var day = DateTime.Parse(model.NewDate);
            var from = DateTime.Parse(model.NewBegin);
            var to = DateTime.Parse(model.NewEnd);

            var start = day.Add(from.TimeOfDay);
            var end = day.Add(to.TimeOfDay);

            // Das Änderungsobjekt anlegen
            var changeService = new ChangeService(Db);
            var changeObject = changeService.CreateActivityDateChange(activityDate, start, end, model.RoomIds);

            // Es hat keine Änderungen gegeben, die eine Notification ergeben!
            // es können sicht aber immer noch die Dozenten verändert haben
            // Das Umsetzen was geändert wurde

            activityDate.Hosts.Clear();
            if (model.DozIds != null)
            {
                foreach (var dozId in model.DozIds)
                {
                    activityDate.Hosts.Add(Db.Members.SingleOrDefault(m => m.Id == dozId));
                }
            }
            
            if (changeObject != null)
            {
                // Es liegt eine Änderung vor => Änderung in DB speichern
                // Änderung umsetzen
                changeObject.UserId = AppUser.Id;
                Db.DateChanges.Add(changeObject);

                // Änderungen am DateObjekt vornehmen
                activityDate.Begin = start;
                activityDate.End = end;

                // es kommen die neuen Räume und Dozenten
                // => zuerst alle löschen!
                activityDate.Rooms.Clear();

                if (model.RoomIds != null)
                {
                    foreach (var roomId in model.RoomIds)
                    {
                        activityDate.Rooms.Add(Db.Rooms.SingleOrDefault(r => r.Id == roomId));
                    }
                }
            }

            // Um die Anzahl der Transaktionen klein zu halten werden erst
            // an dieser Stelle alle Änderungen am Datum und 
            // dem ChangeObjekt gespeichert
            Db.SaveChanges();

            // Jetzt erst die Notification auslösen
            if (changeObject != null)
            {
                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());
            }
            return PartialView("_SaveSuccess");
        }

        [HttpPost]
        public PartialViewResult MoveDates(ICollection<Guid> dateIds, string newBegin, string newEnd)
        {
            // Das sind die Zeiten!
            var from = DateTime.Parse(newBegin);
            var to = DateTime.Parse(newEnd);

            Course course = null;
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    // Berechnung der neuen Zeiten
                    var start = date.Begin.Date.Add(from.TimeOfDay);
                    var end = date.End.Date.Add(to.TimeOfDay);

                    // Das Änderungsobjekt anlegen
                    var changeObject = changeService.CreateActivityDateChange(date, start, end, null);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }

                    // Änderungen am DateObjekt vornehmen
                    date.Begin = start;
                    date.End = end;

                    if (course == null)
                        course = date.Activity as Course;
                }
                Db.SaveChanges();


            }

            return PartialView("_DateTable", course);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <returns></returns>
        public ActionResult MoveCourse(Guid id)
        {
            var memberService = new MemberService(Db, UserManager);
            var member = memberService.GetMember(User.Identity.Name, "FK 09");
            var isProf = memberService.HasRole(User.Identity.Name, "FK 09", "Prof");

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var sbGroup = new StringBuilder();
            foreach (var @group in course.SemesterGroups)
            {
                if (group.CapacityGroup.CurriculumGroup != null)
                {
                    sbGroup.AppendFormat("{0}", @group.FullName);
                    if (@group != course.SemesterGroups.Last())
                        sbGroup.Append(", ");
                }
            }


            var model = new CourseCreateModel
            {
                CourseId = id,
                Name = course.Name,
                ShortName = course.ShortName,
                Semester = GetSemester().Name,
                Lecturer = isProf ? member.ShortName : string.Empty,
                Group = sbGroup.ToString(),
            };

            SetTimeSelections();

            return View(model);
        }

        [HttpPost]
        public ActionResult MoveCourse(CourseCreateModel model)
        {
            // Das Semester ist fest eingestellt
            var semester = GetSemester();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(a => a.Id == model.CourseId);

            if (course != null)
            {
                bool createDates = model.DateOption == 0;

                // alle zukünftigen Dates löschen
                var courseService = new CourseService(UserManager);
                courseService.DeleteAllDatesAfter(course.Id, GlobalSettings.Now);

                if (createDates)
                {
                    // kein doz eineggeben
                    // wenn es der doz selber ist dann nehmen 
                    // wenn es ein admin is, dann ist es halt keiner
                    var org = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));
                    OrganiserMember doz;
                    if (string.IsNullOrEmpty(model.Lecturer))
                    {
                        var memberService = new MemberService(Db, UserManager);
                        var member = memberService.GetMember(User.Identity.Name, "FK 09");
                        var isProf = memberService.HasRole(User.Identity.Name, "FK 09", "Prof");
                        if (isProf)
                        {
                            doz = org.Members.SingleOrDefault(m => m.ShortName.ToUpper().Equals(member.ShortName));
                        }
                        else
                        {
                            doz = null;
                        }
                    }
                    else
                    {
                        doz = org.Members.SingleOrDefault(m => m.ShortName.ToUpper().Equals(model.Lecturer));
                    }


                    var room = Db.Rooms.SingleOrDefault(r => r.Number.ToUpper().Equals(model.Room.ToUpper()));

                    var dayList = new SemesterService().GetDays(semester.Name, (DayOfWeek)model.DayOfWeek, DateTime.Today);

                    foreach (var day in dayList)
                    {
                        DateTime start = day.AddHours(model.StartTimeHour).AddMinutes(model.StartTimeMinute);
                        DateTime end = day.AddHours(model.EndTimeHour).AddMinutes(model.EndTimeMinute);

                        var date = new ActivityDate
                        {
                            Activity = course,
                            Begin = start,
                            End = end,
                            Hosts = new Collection<OrganiserMember> { doz },
                            Rooms = new Collection<Room> { room },
                            Occurrence = new Occurrence
                            {
                                Capacity = -1,
                                IsAvailable = true,
                                FromIsRestricted = false,
                                UntilIsRestricted = false,
                                IsCanceled = false,
                                IsMoved = false,
                            },

                        };

                        Db.ActivityDates.Add(date);
                    }
                    Db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Course", new { id = model.CourseId });
        }

        public ActionResult MoveCourseTime(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var lectures =
                Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

            var rooms =
                Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

            var sbLecturers = new StringBuilder();
            foreach (var lecture in lectures)
            {
                sbLecturers.AppendFormat("{0} ({1})", lecture.Name, lecture.ShortName);
                if (lecture != lectures.Last())
                    sbLecturers.Append(", ");
            }

            var sbRoom = new StringBuilder();
            foreach (var room in rooms)
            {
                sbRoom.Append( room.Number);
                if (room != rooms.Last())
                    sbLecturers.Append(", ");
            }

            var sbGroup = new StringBuilder();
            foreach (var @group in course.SemesterGroups)
            {
                sbGroup.AppendFormat("{0}", @group.FullName);
                    if (@group != course.SemesterGroups.Last())
                        sbGroup.Append(", ");
                }


            // Vogelwilde Annahme => die zeiten des ersten Termins

            var date = course.Dates.Where(d => d.Begin >= GlobalSettings.Today).OrderBy(d => d.Begin).FirstOrDefault();

            var model = new CourseCreateModel
            {
                CourseId = id,
                Name = course.Name,
                ShortName = course.ShortName,
                Semester = GetSemester().Name,
                Lecturer = sbLecturers.ToString(),
                Room = sbRoom.ToString(),
                Group = sbGroup.ToString(),
                StartTimeHour = date != null ? date.Begin.Hour : 6,
                StartTimeMinute = date != null ? date.Begin.Minute : 0,
                EndTimeHour = date != null ? date.End.Hour : 6,
                EndTimeMinute = date != null ? date.Begin.Minute : 0,
            };

            SetTimeSelections();

            return View(model);
        }

        [HttpPost]
        public ActionResult MoveCourseTime(CourseCreateModel model)
        {
            var logger = LogManager.GetLogger("Course");

            // Das Semester ist fest eingestellt
            var semester = GetSemester();


            var course = Db.Activities.OfType<Course>().SingleOrDefault(a => a.Id == model.CourseId);

            if (course != null)
            {
                var dates = course.Dates.Where(d => d.Begin >= DateTime.Today).ToList();


                foreach (var date in dates)
                {
                    var day = date.Begin.Date;
                    DateTime start = day.AddHours(model.StartTimeHour).AddMinutes(model.StartTimeMinute);
                    DateTime end = day.AddHours(model.EndTimeHour).AddMinutes(model.EndTimeMinute);

                    date.Begin = start;
                    date.End = end;
                }

                Db.SaveChanges();

                logger.InfoFormat("Course time {0} moved by {1}", course.Name, User.Identity.Name);
            }
            return RedirectToAction("Index", "Course", new { id = model.CourseId });
        }



        [HttpPost]
        public PartialViewResult CheckRoomList(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);
            var model = GetRoomList(id, start, end);

            return PartialView("_RoomState", model);
        }

        [HttpPost]
        public JsonResult CheckCurriculum(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);

            // jetzt die Vorlesung
            var course = activityDate.Activity as Course;

            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);

            // Alle Kurse, die in dem Zeitraum stattfinden
            var courses = Db.ActivityDates.Where(d => (d.End > start && d.End < end ||
                                        d.Begin > start && d.Begin < end) &&
                                        d.Activity.Id != course.Id).ToList();

            var hasConflicts = false;

            foreach (var activityDate1 in courses)
            {
                // Mich interessieren nur LVs
                if (activityDate1.Activity is Course)
                {
                    // EIn Kurs, der aber bereits ein anderer ist!
                    var course1 = activityDate1.Activity as Course;

                    foreach (var group in course.SemesterGroups)
                    {
                        if (course1.SemesterGroups.Contains(group))
                        {
                            hasConflicts = true;
                        }
                    }
                }
            }


            var model = new { HasConflicts = hasConflicts};

            return Json(model);


        }

        [HttpPost]
        public PartialViewResult AddRoom(Guid id, DateTime date, TimeSpan from, TimeSpan to, string room)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var model = Db.Rooms.SingleOrDefault(r => r.Number.Equals(room));

            if (model != null && activityDate != null)
            {
                activityDate.Rooms.Add(model);
                Db.SaveChanges();

                logger.InfoFormat("Room {0} for {1} on [{2}-{3}] was added by {4}", model.Number, activityDate.Activity.ShortName,
                    activityDate.Begin, activityDate.End, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Room {0} or ActivityDate {1} not found", room, id);                
            }

            return CheckRoomList(id, date, from, to);
        }

        [HttpPost]
        public PartialViewResult RemoveRoom(Guid id, Guid roomId)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var room = Db.Rooms.SingleOrDefault(r => r.Id.Equals(roomId));

            if (room != null && activityDate != null)
            {
                activityDate.Rooms.Remove(room);
                Db.SaveChanges();

                logger.InfoFormat("Room {0} for {1} on [{2}-{3}] was removed by {4}", room.Number, activityDate.Activity.ShortName,
                    activityDate.Begin, activityDate.End, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Room {0} or ActivityDate {1} not found", roomId, id);
            }

            return PartialView("_EmptyRow");
        }



        private RoomListStateModel GetRoomList(Guid activityDateId, DateTime from, DateTime to)
        {
            var model = new RoomListStateModel() {ActivityDateId = activityDateId};

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == activityDateId);

            if (date != null)
            {
                foreach (var room in date.Rooms)
                {
                    var conflicts = Db.ActivityDates.Where(d =>
                        d.Id != activityDateId &&
                        d.Rooms.Any(r => r.Id == room.Id) &&
                        ((d.Begin > from && d.Begin < to) || // der Anfang liegt drin
                         (d.End < to && d.End > from) // das Ende liegt drin
                            )).ToList();
                    model.RoomStates.Add(new RoomStateModel {Room = room, Conflicts = conflicts});
                }
            }

            return model;
        }


        [HttpPost]
        public PartialViewResult CheckLecturerList(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);
            var model = GetLecturerList(id, start, end);

            return PartialView("_LecturerState", model);
        }

        [HttpPost]
        public PartialViewResult AddLecturer(Guid id, DateTime date, TimeSpan from, TimeSpan to, string lecturer)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var fk09 = Db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FK 09"));
            var model = fk09.Members.SingleOrDefault(r => r.ShortName.Equals(lecturer));

            if (model != null && activityDate != null)
            {
                activityDate.Hosts.Add(model);
                Db.SaveChanges();

                logger.InfoFormat("Lecturer {0} for {1} on {2} was added by {3}", model.ShortName, activityDate.Activity.ShortName,
                    activityDate.Begin, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Lecturer {0} or ActivityDate {1} not found", lecturer, id);
            }


            return CheckLecturerList(id, date, from, to);
        }

        [HttpPost]
        public PartialViewResult RemoveLecturer(Guid id, Guid lecturerId)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var lecturer = Db.Members.SingleOrDefault(r => r.Id.Equals(lecturerId));

            if (lecturer != null && activityDate != null)
            {
                activityDate.Hosts.Remove(lecturer);
                Db.SaveChanges();

                logger.InfoFormat("Lecturer {0} for {1} on {2} was removed by {3}", lecturer.ShortName, activityDate.Activity.ShortName,
                    activityDate.Begin, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Lecturer {0} or ActivityDate {1} not found", lecturerId, id);
            }

            return PartialView("_EmptyRow");
        }



        private LecturerListStateModel GetLecturerList(Guid activityDateId, DateTime from, DateTime to)
        {
            var model = new LecturerListStateModel() { ActivityDateId = activityDateId };

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == activityDateId);

            if (date != null)
            {
                foreach (var lecturer in date.Hosts)
                {
                    var conflicts = Db.ActivityDates.Where(d =>
                        d.Id != activityDateId &&
                        d.Hosts.Any(r => r.Id == lecturer.Id) &&
                        ((d.Begin > from && d.Begin < to) || // der Anfang liegt drin
                         (d.End < to && d.End > from) // das Ende liegt drin
                            )).ToList();
                    model.LecturerStates.Add(new LecturerStateModel {Lecturer = lecturer, Conflicts = conflicts});
                }
            }

            return model;
        }

        public ActionResult DeleteDate(Guid id)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == id);

            var dateSummary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateSummary.Date.Id);

            var actSummary = new ActivitySummary {Activity = date.Activity};

            occ.Subscriptions.ForEach(s => Db.Subscriptions.Remove(s));
            Db.Occurrences.Remove(occ);
            date.Occurrence = null;

            
            date.Activity.Dates.Remove(date);
            date.Hosts.Clear();
            date.Rooms.Clear();
            Db.ActivityDates.Remove(date);

            Db.SaveChanges();


            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new { id = actSummary.Id });
        }

        public ActionResult DeleteAllDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel {Course = course};
            
            return View(model);
        }


        [HttpPost]
        public ActionResult DeleteAllDates(CourseDeleteModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);

            var actSummary = new ActivitySummary { Activity = course };

            if (course != null)
                new CourseService(UserManager).DeleteAllDates(course.Id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new { id = actSummary.Id });
        }

        public ActionResult DeleteCourse(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel { Course = course };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCourse(CourseDeleteModel model)
        {
            var timeTableService = new TimeTableInfoService();

            timeTableService.DeleteCourse(model.Course.Id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction("Index", "Home");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ActivitiyDateId</param>
        /// <returns></returns>
        public ActionResult SetInfo(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(o => o.Id == id);


            var model = new CourseDateInfoModel()
            {
                Course = date.Activity as Course,
                Date = date,
                ActivityDateId = date.Id,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SetInfo(CourseDateInfoModel model)
        {
            var date = Db.ActivityDates.SingleOrDefault(o => o.Id == model.ActivityDateId);

            if (date != null)
            {
                date.Title = model.Date.Title;
                date.Description = model.Date.Description;
                date.Occurrence.Information = model.Date.Occurrence.Information;
                Db.SaveChanges();
            }

            var summary = ActivityService.GetSummary(date.Occurrence.Id);

            return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId">Id der zugehörigen Lehrveranstaltung</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CreateDate(Guid courseId)
        {
            var activity = Db.Activities.SingleOrDefault(a => a.Id == courseId);
            if (activity != null)
            {
                var today = GlobalSettings.Today;
                var from = GlobalSettings.Now;
                var to = from.AddMinutes(90);

                var date = new ActivityDate
                {
                    Begin = today.AddHours(from.Hour).AddMinutes(from.Minute),
                    End = today.AddHours(to.Hour).AddMinutes(to.Minute),
                    Occurrence = new Occurrence { Capacity = -1, IsAvailable = true, FromIsRestricted = false, UntilIsRestricted = false },
                };

                activity.Dates.Add(date);
                Db.SaveChanges();
            }


            return PartialView("_DateTable", activity as Course);

        }

        public ActionResult CreateDate2(Guid courseId)
        {
            var semester = GetSemester();
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            var memberService = new MemberService(Db, UserManager);
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var orgName = User.IsInRole("SysAdmin") ?
                "FK 09" :
                memberService.GetOrganisationName(semester, User.Identity.Name);

            var userRight = GetUserRight(User.Identity.Name, "FK 09");

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetRooms(orgName, userRight.IsOrgAdmin);


            var model = new CourseDateCreatenModel
            {
                Course = course,
                CourseId = course.Id,
                NewDate = GlobalSettings.Today.ToShortDateString(),
                NewBegin = GlobalSettings.Now.TimeOfDay.ToString(),
                NewEnd = GlobalSettings.Now.TimeOfDay.ToString(),
                Rooms = rooms,
                Lecturers = Db.Members.Where(m => m.Organiser.ShortName.Equals("FK 09")).ToList(),
            };
            
            
            return View("CreateDate",model);
        }

        [HttpPost]
        public PartialViewResult CreateDate2(CourseDateCreatenModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);

            var day = DateTime.Parse(model.NewDate);
            var from = DateTime.Parse(model.NewBegin);
            var to = DateTime.Parse(model.NewEnd);


            var start = day.Add(from.TimeOfDay);
            var end = day.Add(to.TimeOfDay);

            var date = new ActivityDate
            {
                Begin = start,
                End = end,
                Occurrence = new Occurrence { Capacity = -1, IsAvailable = true, FromIsRestricted = false, UntilIsRestricted = false },
            };

            if (model.RoomIds != null)
            {
                foreach (var roomId in model.RoomIds)
                {
                    date.Rooms.Add(Db.Rooms.SingleOrDefault(r => r.Id == roomId));
                }
            }

            if (model.LecturerIds != null)
            {
                foreach (var dozId in model.LecturerIds)
                {
                    date.Hosts.Add(Db.Members.SingleOrDefault(m => m.Id == dozId));
                }
            }

            course.Dates.Add(date);



            if (model.IsWeekly)
            {
                Semester semester = null;
                if (course.SemesterGroups.Any())
                {
                    semester = course.SemesterGroups.First().Semester;
                }

                if (semester != null)
                {
                    day = day.AddDays(7);
                    while (day < semester.EndCourses)
                    {
                        bool isVorlesung = true;
                        foreach (var sd in semester.Dates)
                        {
                            // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                            if (sd.From.Date <= day.Date &&
                                day.Date <= sd.To.Date &&
                                sd.HasCourses == false)
                            {
                                isVorlesung = false;
                            }
                        }

                        if (isVorlesung)
                        {

                            start = day.Add(from.TimeOfDay);
                            end = day.Add(to.TimeOfDay);

                            date = new ActivityDate
                            {
                                Begin = start,
                                End = end,
                                Occurrence = new Occurrence { Capacity = -1, IsAvailable = true, FromIsRestricted = false, UntilIsRestricted = false },
                            };

                            if (model.RoomIds != null)
                            {
                                foreach (var roomId in model.RoomIds)
                                {
                                    date.Rooms.Add(Db.Rooms.SingleOrDefault(r => r.Id == roomId));
                                }
                            }

                            if (model.LecturerIds != null)
                            {
                                foreach (var dozId in model.LecturerIds)
                                {
                                    date.Hosts.Add(Db.Members.SingleOrDefault(m => m.Id == dozId));
                                }
                            }

                            course.Dates.Add(date);
                        }
                        day = day.AddDays(7);
                    }
                }
            }

            Db.SaveChanges();

            return PartialView("_SaveSuccess");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id des Kurses</param>
        /// <returns></returns>
        public ActionResult EditOccurrenceRule(Guid id)
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult CreateCourse(Guid? id)
        {
            var sem = id.HasValue ?
                Db.Semesters.SingleOrDefault(s => s.Id == id.Value) :
                GetSemester();

            var org = GetMyOrganisation();

            CourseCreateModel2 model = new CourseCreateModel2();

            model.SemesterId = sem.Id;
            model.OrganiserId = org.Id;
            model.OrganiserId2 = org.Id;
            model.OrganiserId3 = org.Id;

            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            ViewBag.Semester = Db.Semesters.OrderByDescending(s => s.StartCourses).Take(3).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            model.CurriculumId = curr != null ? curr.Id : Guid.Empty;
            ViewBag.Curricula = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var semGroups = Db.SemesterGroups.Where(g => g.Semester.Id == sem.Id &&
                                                         g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)
                .ToList();

            var group = semGroups.FirstOrDefault();
            model.GroupId = group != null ? group.Id : Guid.Empty;
            ViewBag.Groups = semGroups.Select(c => new SelectListItem
            {
                Text = c.GroupName,
                Value = c.Id.ToString(),
            });



            return View(model);
        }


        [HttpPost]
        public ActionResult CreateCourse(CourseCreateModelExtended model)
        {
            var org = GetMyOrganisation();

            var course = new Course
            {
                Name = model.Name,
                ShortName = model.ShortName,
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
                        course.SemesterGroups.Add(semGroup);

                        var occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = course.Occurrence
                        };
                        occGroup.SemesterGroups.Add(semGroup);
                        semGroup.OccurrenceGroups.Add(occGroup);
                        course.Occurrence.Groups.Add(occGroup);
                        Db.OccurrenceGroups.Add(occGroup);
                    }
                }
            }

            var member = GetMyMembership();

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = course,
                    Member = me,
                    IsLocked = false
                };

                course.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }


            // Jetzt die Termine - falls vorhanden
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
                        dayList = semesterService.GetDays(semester.Name, day);
                    }
                    else
                    {
                        dayList = new List<DateTime> {day};
                    }


                    foreach (var dateDay in dayList)
                    {
                        var activityDate = new ActivityDate
                        {
                            Activity = course,
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


            Db.Activities.Add(course);
            Db.SaveChanges();

            /*
            return Json(new
            {
                ok = true, 
                newurl = Url.Action("Admin", new { id = course.Id })
            });
            */
            return PartialView("_CreateCourseSuccess",course);
        }

        [HttpPost]
        public PartialViewResult GetMyCourseList()
        {
            var member = GetMyMembership();

            if (member != null)
            {
                var model =
                    Db.Activities.OfType<Course>().Where(c => c.Owners.Any(o => o.Member.Id == member.Id)).ToList();
                return PartialView("_CourseTable", model);
            }

            return PartialView("_CourseTable", new List<Course>());
        }



        [HttpPost]
        public PartialViewResult CheckConflicts(string group, int day, TimeSpan from, TimeSpan to, string room, string doz)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <returns></returns>
        public ActionResult EditCourse(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);


            return View(course);
        }

        [HttpPost]
        public ActionResult UpdateGeneralSettings(Course model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Id);

            if (!string.IsNullOrEmpty(model.Name))
                course.Name = model.Name;

            if (!string.IsNullOrEmpty(model.ShortName))
                course.ShortName = model.ShortName;

            if (!string.IsNullOrEmpty(model.UrlMoodleCourse))
                course.UrlMoodleCourse = model.UrlMoodleCourse;

            if (!string.IsNullOrEmpty(model.Description))
                course.Description = model.Description;

            Db.SaveChanges();

            return RedirectToAction("Index", new { id = course.Id });
        }

        [HttpPost]
        public PartialViewResult AddGroup(Guid courseId, Guid semGroupId, int capacity)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);

            // TODO: ERROR pages
            if (semGroup == null || course == null)
                return null;

            // Semestergruppe in Studenplanzuordnung

            // Semestergruppen dürfen nicht doppelt vorkommen
            if (!course.SemesterGroups.Contains(semGroup))
            {
                course.SemesterGroups.Add(semGroup);
            }

            // TODO: aktuell dürfen noch keine Gruppen vorkommen, die mehr als eine Semestergruppe haben
            // zudem gilt auch hier: jede Semestergruppe darf einmal vorkommen
            var group = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Contains(semGroup));
            if (group == null)
            {
                // eine neue Gruppe muss her
                var occGroup = new OccurrenceGroup
                {
                    FitToCurriculumOnly = false,
                    Capacity = capacity,
                };

                occGroup.SemesterGroups.Add(semGroup);

                course.Occurrence.Groups.Add(occGroup);
            }
            else
            {
                group.Capacity = capacity;
            }
            Db.SaveChanges();


            var courseService = new CourseService(UserManager, courseId);
            var groups = courseService.GetSubscriptionGroups();

            return PartialView("_GroupList", groups);
        }

        [HttpPost]
        public PartialViewResult RemoveGroup(Guid courseId, Guid groupId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            var semGroup = Db.SemesterGroups.SingleOrDefault(c => c.Id == groupId);


            if (course != null && semGroup != null && course.SemesterGroups.Contains(semGroup))
            {
                course.SemesterGroups.Remove(semGroup);
                Db.SaveChanges();
            }

            // TODO: aktuell dürfen noch keine Gruppen vorkommen, die mehr als eine Semestergruppe haben
            // zudem gilt auch hier: jede Semestergruppe darf einmal vorkommen
            var group = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Contains(semGroup));
            if (group != null)
            {
                course.Occurrence.Groups.Remove(group);
                Db.SaveChanges();
            }


            // Dummy, da nix zurückgegeben wird, die Zeile wird gelöscht
            return PartialView("_RemoveSubscription");
        }

        [HttpPost]
        public ActionResult UpdateOccurrenceSettings(Course model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id ==model.Id);

            if (course != null)
            {
                course.Occurrence.Capacity = model.Occurrence.Capacity;
                Db.SaveChanges();
            }

            return RedirectToAction("Index", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadRessource(Guid id, HttpPostedFileBase uploadFile)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var storage = course.Ressources.SingleOrDefault(r => r.Category.Equals("Modulbeschreibung"));

            if (uploadFile != null)
            {
                if (storage == null)
                {
                    storage = new BinaryStorage
                    {
                        Category = "Modulbeschreibung",
                    };

                    Db.Storages.Add(storage);
                    course.Ressources.Add(storage);
                    Db.SaveChanges();
                }


                storage.FileType = uploadFile.ContentType;
                storage.BinaryData = new byte[uploadFile.ContentLength];

                uploadFile.InputStream.Read(storage.BinaryData, 0, uploadFile.ContentLength);

                Db.SaveChanges();
            }

            return RedirectToAction("EditCourse", new { id = course.Id });
        }

        public ActionResult UploadDocument(Guid id)
        {
            var model = new CourseDocumentUploadModel {CourseId = id};

            return View(model);
        }

        [HttpPost]
        public ActionResult UploadDocument(CourseDocumentUploadModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);

            if (course != null)
            {
                var storage = new BinaryStorage
                {
                    Category = "Material",
                    Name = Path.GetFileName(model.Document.FileName),
                    Description = model.Description,
                };

                Db.Storages.Add(storage);
                course.Ressources.Add(storage);
                Db.SaveChanges();

                storage.FileType = model.Document.ContentType;
                storage.BinaryData = new byte[model.Document.ContentLength];

                model.Document.InputStream.Read(storage.BinaryData, 0, model.Document.ContentLength);

                Db.SaveChanges();

                return RedirectToAction("Index", new { id = course.Id });
            }

            return View(model);
        }

        [HttpPost]
        public PartialViewResult DeleteDocument(Guid CourseId, Guid DocId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == CourseId);
            if (course != null)
            {
                var doc = course.Ressources.SingleOrDefault(r => r.Id == DocId);
                if (doc != null)
                {
                    course.Ressources.Remove(doc);
                    Db.Storages.Remove(doc);
                    Db.SaveChanges();
                }
            }

            return PartialView("_RemoveSubscription");
        }

        [HttpPost]
        public PartialViewResult ChangeGeneralSettings(CourseDetailViewModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);

            if (course != null)
            {
                if (!string.IsNullOrEmpty(model.Course.Name) &&
                    !model.Course.Name.Equals(course.Name))
                {
                    course.Name = model.Course.Name;
                }

                if (!string.IsNullOrEmpty(model.Course.ShortName) &&
                    !model.Course.ShortName.Equals(course.ShortName))
                {
                    course.ShortName = model.Course.ShortName;
                }

                course.Description = model.Description2;
                course.UrlMoodleCourse = model.Course.UrlMoodleCourse;
                course.KeyMoodleCourse = model.Course.KeyMoodleCourse;

                Db.SaveChanges();
            }

            return PartialView("_SaveSuccess");
        }

        [HttpPost]
        public PartialViewResult ChangeCapacitySettings(CourseDetailViewModel model)
        {
            if (model.SelectedOption == null)
                return PartialView("_SaveSuccess");

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);
            if (course == null)
                return PartialView("_SaveSuccess");

            switch (model.SelectedOption.Id)
            {
                case 1:
                    // keine Beschränkung
                    course.Occurrence.Capacity = -1;
                    course.Occurrence.UseGroups = false;
                    course.Occurrence.UseExactFit = false;
                    break;
                case 2:
                    // Beschränkung auf globaler ebene
                    course.Occurrence.Capacity = model.SelectedOption.Capacity;
                    course.Occurrence.UseGroups = false;
                    course.Occurrence.UseExactFit = false;
                    break;
                case 3:
                    course.Occurrence.Capacity = 0;
                    course.Occurrence.UseGroups = true;
                    course.Occurrence.UseExactFit = false;
                    break;
                case 4:
                    course.Occurrence.Capacity = 0;
                    course.Occurrence.UseGroups = true;
                    course.Occurrence.UseExactFit = true;
                    break;
                default:
                    break;
            }

            Db.SaveChanges();

            return PartialView("_SaveSuccess");
        }

        /// <summary>
        /// Absage über Schaltfläche
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CancelDate2(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            // Alle Räume freigeben
            // Die Zuordnung zum Dozent bleibt aber bestehen!
            if (date != null)
            {
                var changeService = new ChangeService(Db);
                // Absage => auch Raumänderung (Raum wird freigegeben)
                var changeObject = changeService.CreateActivityDateStateChange(date, true);
                if (changeObject != null)
                {
                    changeObject.UserId = AppUser.Id;
                    Db.DateChanges.Add(changeObject);
                }


                date.Occurrence.IsCanceled = true;
                date.Rooms.Clear();

                Db.SaveChanges();

                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());

            }

            return PartialView("_DateTable", date.Activity as Course);
        }

        /// <summary>
        /// Absage über Menüpunkt
        /// </summary>
        /// <param name="dateIds">Liste der ausgewählten Termine</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CancelDateList(ICollection<Guid> dateIds)
        {
            Course course = null;
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    // Absage => auch Raumänderung (Raum wird freigegeben)
                    var changeObject = changeService.CreateActivityDateStateChange(date, true);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }

                    date.Occurrence.IsCanceled = true;
                    date.Rooms.Clear();

                    course = date.Activity as Course;

                }
                Db.SaveChanges();



            }

            return PartialView("_DateTable", course);
        }


        [HttpPost]
        public ActionResult ReactivateDate2(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            if (date != null)
            {
                var changeService = new ChangeService(Db);

                // Reaktivierung => KEINE Raumänderung
                var changeObject = changeService.CreateActivityDateStateChange(date, false);
                if (changeObject != null)
                {
                    changeObject.UserId = AppUser.Id;
                    Db.DateChanges.Add(changeObject);

                    Db.SaveChanges();

                    NotificationService nservice = new NotificationService();
                    nservice.CreateSingleNotification(changeObject.Id.ToString());
                }

                
                date.Occurrence.IsCanceled = false;

                Db.SaveChanges();

            }

            return PartialView("_DateTable", date.Activity as Course);
            
        }

        [HttpPost]
        public PartialViewResult ReactivateDateList(ICollection<Guid> dateIds)
        {
            Course course = null;
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    // Reaktivierung => KEINE Raumänderung
                    var changeObject = changeService.CreateActivityDateStateChange(date, false);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }


                    date.Occurrence.IsCanceled = false;

                    course = date.Activity as Course;
                }
                Db.SaveChanges();



            }

            return PartialView("_DateTable", course);
        }

        [HttpPost]
        public PartialViewResult DeleteDateConfirm(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            return PartialView("_DlgConfirmDeleteDate", date);
        }

        [HttpPost]
        public PartialViewResult DeleteDateConfirmed(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            var activity = date.Activity as Course;

            if (date != null)
            {
                Db.Occurrences.Remove(date.Occurrence);
                date.Occurrence = null;


                activity.Dates.Remove(date);
                
                // TODO: was soll da mit den Änderungen passieren???
                date.Changes.Clear();

                date.Hosts.Clear();
                date.Rooms.Clear();
                Db.ActivityDates.Remove(date);

                Db.SaveChanges();

            }

            return PartialView("_DateTable", activity);
        }

        [HttpPost]
        public PartialViewResult DeleteDateList(ICollection<Guid> dateIds)
        {
            Course course = null;
            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    Db.Occurrences.Remove(date.Occurrence);
                    date.Occurrence = null;

                    course = date.Activity as Course;

                    course.Dates.Remove(date);
                    date.Changes.Clear();
                    date.Hosts.Clear();
                    date.Rooms.Clear();
                    Db.ActivityDates.Remove(date);
                }
            }

            Db.SaveChanges();

            return PartialView("_DateTable", course);
        }

        /// <summary>
        /// Löscht bei allen angegebenen Terminen die Rauminformation
        /// </summary>
        /// <param name="dateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveRooms(ICollection<Guid> dateIds)
        {
            Course course = null;

            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                if (date != null)
                {
                    // Raumänderung
                    var changeObject = changeService.CreateActivityDateRoomChange(date);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }

                    date.Rooms.Clear();

                    if (course == null)
                        course = date.Activity as Course;
                }
                Db.SaveChanges();


            }

            return PartialView("_DateTable", course);
        }


        [HttpPost]
        public PartialViewResult AddRoomToDates(ICollection<Guid> dateIds, string roomNumber)
        {
            var room = Db.Rooms.FirstOrDefault(r => r.Number.Equals(roomNumber));

            Course course = null;
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    if (room != null)
                    {
                        // Raumänderung
                        var changeObject = changeService.CreateActivityDateRoomChange(date);
                        if (changeObject != null)
                        {
                            changeObject.UserId = AppUser.Id;
                            Db.DateChanges.Add(changeObject);

                            Db.SaveChanges();

                            NotificationService nservice = new NotificationService();
                            nservice.CreateSingleNotification(changeObject.Id.ToString());

                        }


                        date.Rooms.Add(room);
                    }

                    if (course == null)
                        course = date.Activity as Course;
                }
                Db.SaveChanges();


            }

            return PartialView("_DateTable", course);
        }

        [HttpPost]
        public PartialViewResult AddHostToDates(ICollection<Guid> dateIds, string hostName)
        {
            var host = Db.Members.FirstOrDefault(r => r.Name.Equals(hostName) || r.ShortName.Equals(hostName));

            Course course = null;
            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    if (host != null)
                    {
                        date.Hosts.Add(host);
                    }

                    if (course == null)
                        course = date.Activity as Course;
                }
                Db.SaveChanges();
            }

            return PartialView("_DateTable", course);
        }

        [HttpPost]
        public PartialViewResult RemoveHosts(ICollection<Guid> dateIds)
        {
            Course course = null;
            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                if (date != null)
                {
                    date.Hosts.Clear();

                    if (course == null)
                        course = date.Activity as Course;
                }
                Db.SaveChanges();
            }

            return PartialView("_DateTable", course);
        }






        [HttpPost]
        public PartialViewResult ChangeDateInformation(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            var model = new CourseDateInformationModel
            {
                DateId = date.Id,
                Title =  date.Title,
                DateDescription = date.Description,
                ShortInfo = date.Occurrence.Information,
                Date = date
            };

            
            return PartialView("_DlgEditInformation", model);
        }

        [HttpPost]
        public PartialViewResult ChangeDateInformationConfirmed(CourseDateInformationModel model)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == model.DateId);
            var course = date.Activity as Course;

            date.Title = model.Title;
            date.Description = model.DateDescription;
            date.Occurrence.Information = model.ShortInfo;

            Db.SaveChanges();

            
            return PartialView("_DateTable", course);
        }


        [HttpPost]
        public PartialViewResult SetOnWaitingList(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.SingleOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    subscription.OnWaitingList = true;
                    subscription.LapCount = 0;
                    subscription.IsConfirmed = false;
                    Db.SaveChanges();
                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);

            var courseService = new CourseService(UserManager, course.Id);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                ParticipantList = courseService.GetTotalParticipantList(),
            };

            var semester = GetSemester();

            foreach (var partcipiant in model.ParticipantList)
            {
                var subscription =
                Db.Subscriptions.OfType<SemesterSubscription>()
                    .FirstOrDefault(s => s.UserId.Equals(partcipiant.User.Id) && s.SemesterGroup.Semester.Id == semester.Id);

                if (subscription != null)
                {
                    partcipiant.SemesterGroup = subscription.SemesterGroup;
                }
            }

            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);


            return PartialView("_ParticipantTable", model);
        }


        [HttpPost]
        public PartialViewResult SetOnParticipiantList(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.SingleOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    subscription.OnWaitingList = false;
                    subscription.LapCount = 0;
                    subscription.IsConfirmed = true;
                    Db.SaveChanges();
                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);

            var courseService = new CourseService(UserManager, course.Id);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                ParticipantList = courseService.GetTotalParticipantList(),
            };

            var semester = GetSemester();

            foreach (var partcipiant in model.ParticipantList)
            {
                var subscription =
                Db.Subscriptions.OfType<SemesterSubscription>()
                    .FirstOrDefault(s => s.UserId.Equals(partcipiant.User.Id) && s.SemesterGroup.Semester.Id == semester.Id);

                if (subscription != null)
                {
                    partcipiant.SemesterGroup = subscription.SemesterGroup;
                }
            }


            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);


            return PartialView("_ParticipantTable", model);
        }

        public ActionResult CreateDummy()
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Name.Equals("SS15"));
            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            var course = new Course
            {
                Name = "Dummy",
                ShortName = "Dummy",
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
                //Description = model.Description,
            };


            Db.Activities.Add(course);
            Db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }


    }
}