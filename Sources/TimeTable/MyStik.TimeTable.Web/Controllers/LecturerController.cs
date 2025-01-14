using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using Org.BouncyCastle.Asn1.X509.SigI;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LecturerController : BaseController
    {
        // GET: Rooms
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = Db.Organisers.Where(o => o.Members.Any()).OrderBy(s => s.Name).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Organiser(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Calendar(Guid id)
        {
            var model = Db.Members.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

        public ActionResult PersonalDates(Guid id)
        {
            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(id);

            var members = MemberService.GetFacultyMemberships(user.Id);

            if (members.Any())
            {
                var orgs = members.Select(x => x.Organiser).Distinct().ToList();

                if (orgs.Any())
                {
                    var org = orgs.FirstOrDefault();
                    var member = MemberService.GetMember(user.Id, org.Id);

                    var segment =
                        semester.Dates.FirstOrDefault(x =>
                            x.HasCourses && x.Organiser != null && x.Organiser.Id == org.Id);
                    var personalDate = Db.Activities.OfType<PersonalDate>().FirstOrDefault(x =>
                        x.Organiser.Id == org.Id && x.Segment.Id == segment.Id &&
                        x.Owners.Any(o => o.Member.Id == member.Id));


                    var summaryModel = new LecturerSummaryModel()
                    {
                        Memberships = members,
                        Semester = semester,
                        Dates = new List<PersonalDate>(),
                    };
                    if (personalDate != null)
                    {
                        summaryModel.Dates.Add(personalDate);
                    }

                    ViewBag.Segments = semester.Dates
                        .Where(x => x.HasCourses && x.Organiser != null && x.Organiser.Id == orgs.First().Id).Select(
                            c =>
                                new SelectListItem
                                {
                                    Text = c.Description,
                                    Value = c.Id.ToString(),
                                });

                    ViewBag.Organiser = orgs.OrderBy(x => x.ShortName).Select(c => new SelectListItem
                    {
                        Text = c.ShortName,
                        Value = c.Id.ToString(),
                    });


                    return View(summaryModel);
                }
            }
            
            return View("NoMember");
        }

        public ActionResult CreatePersonalDates(Guid id)
        {
            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(id);

            var members = MemberService.GetFacultyMemberships(user.Id);
            var orgs = members.Select(x => x.Organiser).Distinct().ToList();


            var org = orgs.FirstOrDefault();
            var member = MemberService.GetMember(user.Id, org.Id);

            var segment =
                semester.Dates.FirstOrDefault(x => x.HasCourses && x.Organiser != null && x.Organiser.Id == orgs.First().Id);
            var personalDate = Db.Activities.OfType<PersonalDate>().FirstOrDefault(x =>
                x.Organiser.Id == org.Id && x.Segment.Id == segment.Id &&
                x.Owners.Any(o => o.Member.Id == member.Id));


            var summaryModel = new LecturerSummaryModel()
            {
                Memberships = members,
                Semester = semester,
                Dates = new List<PersonalDate>(),
            };
            if (personalDate != null)
            {
                summaryModel.Dates.Add(personalDate);
            }

            ViewBag.Segments = semester.Dates.Where(x => x.HasCourses && x.Organiser != null && x.Organiser.Id == orgs.First().Id).Select(c => new SelectListItem
            {
                Text = c.Description,
                Value = c.Id.ToString(),
            });

            ViewBag.Organiser = orgs.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(summaryModel);
        }


        [HttpPost]
        public PartialViewResult CreateDates(string[] timeArray, Guid orgId, Guid segmentId)
        {
            var user = GetCurrentUser();
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segmentId);

            var member = MemberService.GetMember(user.Id, org.Id);

            var personalDate = Db.Activities.OfType<PersonalDate>().FirstOrDefault(x =>
                x.Organiser.Id == org.Id && x.Segment.Id == segment.Id &&
                x.Owners.Any(o => o.Member.Id == member.Id));

            if (personalDate == null)
            {
                personalDate = new PersonalDate()
                {
                    Name = "Verfügbarkeit",
                    ShortName = "VF",
                    Organiser = org,
                    Segment = segment,
                    Semester = segment.Semester,
                    IsProjected = true,
                    IsInternal = true,
                    Occurrence = new Occurrence
                    {
                        Capacity = -1,
                        IsAvailable = false,
                        FromIsRestricted = false,
                        UntilIsRestricted = false,
                        IsCanceled = false,
                        IsMoved = false,
                        UseGroups = false,
                    },
                };

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = personalDate,
                    Member = member,
                    IsLocked = false
                };

                personalDate.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
                Db.Activities.Add(personalDate);
            }



            var semester = segment.Semester;
            var startDate = segment.From.Date;
            var occDate = startDate;

            while(occDate <= segment.To.Date)
            {
                bool isVorlesung = true;
                foreach (var sd in semester.Dates)
                {
                    // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                    if (sd.From.Date <= occDate.Date &&
                        occDate.Date <= sd.To.Date &&
                        sd.HasCourses == false)
                    {
                        isVorlesung = false;
                    }
                }

                if (isVorlesung)
                {
                    var begin = new DateTime();
                    var end = new DateTime();

                    for (var b = 0; b < 3; b++)
                    {
                        var isValid = false;
                        var d = (int)occDate.DayOfWeek;
                        if (d >= 1 && d <= 5)
                        {
                            var i1 = (d - 1) * 2 + (b * 10);

                            if (!string.IsNullOrEmpty(timeArray[i1]) && !string.IsNullOrEmpty(timeArray[i1 + 1]))
                            {
                                begin = DateTime.Parse(timeArray[i1]);
                                end = DateTime.Parse(timeArray[i1 + 1]);

                                if (begin <= end && begin.Hour > 0 && end.Hour > 0)
                                {
                                    isValid = true;
                                }
                            }
                        }

                        if (isValid)
                        {
                            var ocStart = new DateTime(occDate.Year, occDate.Month, occDate.Day, begin.Hour,
                                begin.Minute, begin.Second);
                            var ocEnd = new DateTime(occDate.Year, occDate.Month, occDate.Day, end.Hour,
                                end.Minute, end.Second);

                            var occ = new ActivityDate
                            {
                                Begin = ocStart,
                                End = ocEnd,
                                Occurrence = new Occurrence()
                            };

                            personalDate.Dates.Add(occ);
                            Db.ActivityDates.Add(occ);
                        }
                    }
                }

                occDate = occDate.AddDays(1);
            }

            Db.SaveChanges();


            var model = new PersonalDateCreateModel
            {
                Organiser = org,
                Segment = segment,
                PersonalDate = personalDate,
            };


            return PartialView("_DateList", model);
        }

        [HttpPost]
        public PartialViewResult DeleteSingleDate(Guid dateId)
        {
            DeleteService.DeleteActivityDate(dateId);

            return PartialView("_EmptyRow");
        }

        [HttpPost]
        public PartialViewResult DeleteAllDates(Guid orgId, Guid segmentId)
        {
            var user = GetCurrentUser();
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segmentId);

            var member = MemberService.GetMember(user.Id, org.Id);

            var personalDate = Db.Activities.OfType<PersonalDate>().FirstOrDefault(x =>
                x.Organiser.Id == org.Id && x.Segment.Id == segment.Id &&
                x.Owners.Any(o => o.Member.Id == member.Id));

            if (personalDate != null)
            {
                var dates = personalDate.Dates.ToList();
                foreach (var date in dates)
                {
                    DeleteService.DeleteActivityDate(date.Id);
                }
            }

            var model = new PersonalDateCreateModel
            {
                Organiser = org,
                Segment = segment,
                PersonalDate = personalDate,
            };

            return PartialView("_DateList", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficeHour(Guid? id)
        {
            var user = GetCurrentUser();
            var infoService = new OfficeHourInfoService(UserManager);

            if (id == null)
            {
                var summaryModel = new LecturerSummaryModel()
                {
                    Memberships = MemberService.GetFacultyMemberships(user.Id)
                };

                var officeHours = 
                    Db.Activities.OfType<OfficeHour>().Where(x =>
                        x.Owners.Any(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id)))
                    .ToList();

                foreach (var oh in officeHours)
                {
                    var ohModel = new LecturerOfficehourSummaryModel()
                    {
                        OfficeHour = oh
                    };


                    summaryModel.OfficeHours.Add(ohModel);
                }

                var semester = SemesterService.GetSemester(DateTime.Today);

                ViewBag.ThisSemester = semester;
                ViewBag.NextSemester = SemesterService.GetNextSemester(semester);


                return View(summaryModel);
            }

            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(x => x.Id == id.Value);

            var model = new OfficeHourSubscriptionViewModel
            {
                OfficeHour = officeHour,
                Semester = officeHour.Semester,
                Host = infoService.GetHost(officeHour),
            };

            if (officeHour.ByAgreement)
            {
                return View("DateListAgreement", model);
            }


            model.Dates.AddRange(infoService.GetDates(officeHour));


            return View("DateList", model);
        }

        public ActionResult Thesis(Guid? id)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var member = GetMyMembership();

            var thesisActivities = Db.Activities.OfType<Supervision>().Where(x =>
               x.Owners.Any(y => y.Member.Id == member.Id)).ToList();

            var theses = Db.Theses.Where(x => x.Supervision.Owners.Any(y => y.Member.Id == member.Id)).ToList();

            var model = new ThesisSemesterSummaryModel();
            model.Semester = semester;
            model.Supervisions = thesisActivities;
            model.Theses = theses;

            return View(model);
        }

        public ActionResult EditDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);

            return View(date);
        }

        [HttpPost]
        public ActionResult EditDate(ActivityDate model)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == model.Id);

            date.Title = model.Title;
            Db.SaveChanges();

            var officeHour = date.Activity as OfficeHour;

            return RedirectToAction("DateDetails", new{id=date.Id});
        }

        public ActionResult DateDetails(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var officeHour = date.Activity as OfficeHour;
            var infoService = new OfficeHourInfoService(UserManager);

            var model = new OfficeHourDateViewModel();

            model.OfficeHour = officeHour;
            model.Date = date;
            model.Subscriptions.AddRange(infoService.GetSubscriptions(date));

            var member = officeHour.Owners.FirstOrDefault();

            ViewBag.VirtualRooms = Db.VirtualRooms.Where(x => x.Owner.Id == member.Member.Id).ToList();

            return View(model);
        }

        public ActionResult AddVirtualRoom(Guid dateId, Guid roomId)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);
            var vRoom = Db.VirtualRooms.SingleOrDefault(x => x.Id == roomId);

            if (vRoom != null)
            {
                var roomAssign = date.VirtualRooms.FirstOrDefault(x => x.Room.Id == vRoom.Id);
                if (roomAssign == null)
                {
                    roomAssign = new VirtualRoomAccess
                    {
                        Date = date,
                        Room = vRoom,
                        isDefault = true
                    };


                    date.VirtualRooms.Add(roomAssign);

                    Db.VirtualRoomAccesses.Add(roomAssign);

                    Db.SaveChanges();
                }
            }

            return RedirectToAction("DateDetails", new {id = date.Id});
        }


        public ActionResult RemoveVirtualRoom(Guid dateId, Guid roomId)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);
            var vRoom = Db.VirtualRooms.SingleOrDefault(x => x.Id == roomId);

            if (vRoom != null)
            {
                var roomAssign = date.VirtualRooms.FirstOrDefault(x => x.Room.Id == vRoom.Id);
                if (roomAssign != null)
                {
                    date.VirtualRooms.Remove(roomAssign);
                    Db.VirtualRoomAccesses.Remove(roomAssign);
                    Db.SaveChanges();
                }
            }

            return RedirectToAction("DateDetails", new { id = date.Id });
        }

        [HttpPost]
        public ActionResult AddRoom(Guid dateId, string roomNumber)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);
            var room = Db.Rooms.SingleOrDefault(x => x.Number.Equals(roomNumber));

            if (room != null)
            {
                date.Rooms.Add(room);
                Db.SaveChanges();
            }

            return RedirectToAction("DateDetails", new { id = date.Id });
        }

        public ActionResult RemoveRooms(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);

            if (date != null)
            {
                date.Rooms.Clear();
                Db.SaveChanges();
            }

            return RedirectToAction("DateDetails", new { id = date.Id });

        }


        public ActionResult Responsibilities(Guid id)
        {
            var model = Db.Members.SingleOrDefault(x => x.Id == id);
            return View(model);
        }


        public ActionResult Courses()
        {
            var user = GetCurrentUser();

            var courses = Db.Activities.OfType<Course>().Where(x => 
                x.Owners.Any(o => o.Member.UserId.Equals(user.Id)) ||
                x.Dates.Any(d => d.Hosts.Any(h => !string.IsNullOrEmpty(h.UserId) && h.UserId.Equals(user.Id)))).ToList();

            var model = new LecturerSummaryModel();

            foreach (var c in courses)
            {
                var orderedDates = c.Dates.OrderBy(x => x.Begin).ToList();
                var firstDate = orderedDates.FirstOrDefault();
                var lastDate = orderedDates.LastOrDefault();
                var owner = c.Owners.FirstOrDefault(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id));
                var dates = c.Dates.Where(x => x.Hosts.Any(h => !string.IsNullOrEmpty(h.UserId) && h.UserId.Equals(user.Id))).ToList();

                var courseModel = new LecturerCourseSummaryModel
                {
                    Course = c,
                    FirstDate = firstDate,
                    LastDate = lastDate,
                    Owner = owner,
                    HostingDates = dates
                };


                model.Courses.Add(courseModel);
            }


            return View(model);
        }


        public ActionResult OfficeHours(Guid id)
        {
            var user = GetCurrentUser();
            var infoService = new OfficeHourInfoService(UserManager);
            var semester = SemesterService.GetSemester(id);

            var summaryModel = new LecturerSummaryModel()
            {
                Memberships = MemberService.GetFacultyMemberships(user.Id)
            };

            var officeHours =
                Db.Activities.OfType<OfficeHour>().Where(x =>
                        x.Owners.Any(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id)))
                    .ToList();

            foreach (var oh in officeHours)
            {
                var ohModel = new LecturerOfficehourSummaryModel()
                {
                    OfficeHour = oh
                };


                summaryModel.OfficeHours.Add(ohModel);
            }

            var currentSemester = semester;
            var nextSemester = SemesterService.GetNextSemester(semester);
            var prevSemester = SemesterService.GetPreviousSemester(semester);

            ViewBag.CurrentSemester = currentSemester;
            ViewBag.NextSemester = nextSemester;
            ViewBag.PrevSemester = prevSemester;


            return View(summaryModel);
        }


        [HttpPost]
        public PartialViewResult GetSegments(Guid orgId, Guid semid)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semid);

            ViewBag.Segments = semester.Dates.Where(x => x.HasCourses && x.Organiser != null && x.Organiser.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.Description,
                Value = c.Id.ToString(),
            });


            return PartialView("_SegmentSelect");
        }


        [HttpPost]
        public PartialViewResult GetPersonalDates(Guid orgId, Guid segmentId)
        {
            var user = GetCurrentUser();
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segmentId);

            var member = MemberService.GetMember(user.Id, org.Id);

            var personalDate = Db.Activities.OfType<PersonalDate>().FirstOrDefault(x =>
                x.Organiser.Id == org.Id && x.Segment.Id == segment.Id &&
                x.Owners.Any(o => o.Member.Id == member.Id));

            var model = new PersonalDateCreateModel
            {
                Organiser = org,
                Segment = segment,
                PersonalDate = personalDate,
            };

            return PartialView("_DateList", model);
        }
    }
}