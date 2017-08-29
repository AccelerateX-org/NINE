using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseService
    {
        private TimeTableDbContext db = new TimeTableDbContext();
        private Course _course;

        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager UserManager;

        /// <summary>
        /// 
        /// </summary>
        public CourseService()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>

        public CourseService(IdentifyConfig.ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="courseId"></param>
        public CourseService(IdentifyConfig.ApplicationUserManager userManager, Guid courseId)
        {
            UserManager = userManager;
            _course = db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semesterName"></param>
        /// <param name="lecturer"></param>
        /// <returns></returns>
        public List<CourseSummaryModel> GetCourses(string semesterName, OrganiserMember lecturer)
        {
            var list = new List<CourseSummaryModel>();
            var semester = db.Semesters.SingleOrDefault(l => l.Name.ToUpper().Equals(semesterName.ToUpper()));

            if (lecturer != null && semester != null)
            {
                var courses =
                    db.Activities.OfType<Course>().Where(c =>
                        c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                        c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)))
                        .OrderBy(c => c.Name)
                        .ToList();

                foreach (var course in courses)
                {
                    var summary = new CourseSummaryModel {Course = course};

                    var days = (from occ in course.Dates
                        select
                            new
                            {
                                Day = occ.Begin.DayOfWeek,
                                Begin = occ.Begin.TimeOfDay,
                                End = occ.End.TimeOfDay,
                            }).Distinct();

                    foreach (var day in days)
                    {
                        var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                        var courseDate = new CourseDateModel
                        {
                            DayOfWeek = day.Day,
                            StartTime = day.Begin,
                            EndTime = day.End,
                            DefaultDate = defaultDay.Begin
                        };
                        summary.Dates.Add(courseDate);
                    }

                    list.Add(summary);
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lecturer"></param>
        /// <returns></returns>
        public List<CourseHistoryModel> GetCourseHistory(OrganiserMember lecturer)
        {
            var list = new List<CourseHistoryModel>();

            if (lecturer != null)
            {
                var courses =
                    db.Activities.OfType<Course>().Where(c =>
                        c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)))
                        .OrderBy(c => c.Name)
                        .ToList();

                var dummySemester = new Semester() {Name = "XXX"};

                foreach (var course in courses)
                {
                    // Annahme alle Gruppen gehören zum selben Semester!
                    var group = course.SemesterGroups.FirstOrDefault();

                    if (group != null)
                    {
                        var summary = new CourseHistoryModel
                        {
                            Course = course,
                            Semester = group.Semester,
                        };

                        list.Add(summary);
                    }
                    else
                    {
                        var summary = new CourseHistoryModel
                        {
                            Course = course,
                            Semester = dummySemester,
                        };

                        list.Add(summary);
                    }
                }
            }

            return list.OrderBy(l => l.Semester.Name).ThenBy(l => l.Course.Name).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semesterName"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public List<CourseSummaryModel> SearchCourses(string semesterName, string searchString)
        {
            var semester = db.Semesters.SingleOrDefault(l => l.Name.ToUpper().Equals(semesterName.ToUpper()));

            if (semester != null)
            {
                var courses =
                    db.Activities.OfType<Course>().Where(
                        c => c.SemesterGroups.Any(g => g.Semester.Id == semester.Id &&
                                                       (c.Name.ToUpper().Contains(searchString) ||
                                                        c.ShortName.ToUpper().Contains(searchString) ||
                                                        c.Dates.Any(
                                                            d =>
                                                                d.Hosts.Any(
                                                                    h =>
                                                                        h.Name.Contains(searchString) ||
                                                                        h.ShortName.Contains(searchString)))))
                        ).OrderBy(c => c.Name).ToList();


                return CreateCourseSummaries(courses);
            }
            return new List<CourseSummaryModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public List<CourseSummaryModel> SearchUnassignedCourses(string searchString)
        {
            var courses =
                db.Activities.OfType<Course>().Where(
                    c => !c.SemesterGroups.Any()).ToList();

            return CreateCourseSummaries(courses);
        }

        private List<CourseSummaryModel> CreateCourseSummaries(ICollection<Course> courses)
        {
            var list = new List<CourseSummaryModel>();

            foreach (var course in courses)
            {
                var summary = new CourseSummaryModel { Course = course };

                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct();

                foreach (var day in days)
                {
                    var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                    var courseDate = new CourseDateModel
                    {
                        DayOfWeek = day.Day,
                        StartTime = day.Begin,
                        EndTime = day.End,
                        DefaultDate = defaultDay.Begin
                    };
                    summary.Dates.Add(courseDate);
                }

                list.Add(summary);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public CourseSummaryModel CreateCourseSummary(Course course)
        {
            var summary = new CourseSummaryModel { Course = course };

            var days = (from occ in course.Dates
                select
                new
                {
                    Day = occ.Begin.DayOfWeek,
                    Begin = occ.Begin.TimeOfDay,
                    End = occ.End.TimeOfDay,
                }).Distinct();

            foreach (var day in days)
            {
                var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                var courseDate = new CourseDateModel
                {
                    DayOfWeek = day.Day,
                    StartTime = day.Begin,
                    EndTime = day.End,
                    DefaultDate = defaultDay.Begin
                };
                summary.Dates.Add(courseDate);
            }

            return summary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lecturer"></param>
        /// <param name="sem"></param>
        /// <returns></returns>
        public OfficeHour GetOfficeHour(OrganiserMember lecturer, Semester sem)
        {
            // zuerst richtg
            var officeHour =
                db.Activities.OfType<OfficeHour>().FirstOrDefault(x =>
                    x.Semester.Id == sem.Id &&
                    x.Owners.Any(k => k.Member.Id == lecturer.Id)
                );

            if (officeHour != null)
                return officeHour;

            // jetzt irgendwie suchen
            // alle mit Terminen
            var allOfficeHours =
                db.Activities.OfType<OfficeHour>().Where(
                        c =>
                            c.Semester.Id == sem.Id &&
                            c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)))
                    .ToList();
            
            // keine mit Terminen
            if (!allOfficeHours.Any())
            {
                // alle ohne Termine
                allOfficeHours =
                    db.Activities.OfType<OfficeHour>().Where(
                            c =>
                                c.Semester.Id == sem.Id &&
                                c.Name.Equals("Sprechstunde") &&
                                c.ShortName.Equals(lecturer.ShortName))
                        .ToList();

                // ok! wirklich keine
                if (!allOfficeHours.Any())
                    return null;

                // wenn mhr als 1, dann reparieren
                return allOfficeHours.First();
            }

            // wenn mehr als 1 => dann reparieren
            return allOfficeHours.First();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lecturer"></param>
        /// <returns></returns>
        public List<OfficeHourSummaryModel> GetOfficeHours(OrganiserMember lecturer)
        {
            var list = new List<OfficeHourSummaryModel>();
            
            if (lecturer != null)
            {
                var courses =
                    db.Activities.OfType<OfficeHour>().Where(
                        c => c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)))
                        .ToList();

                foreach (var course in courses)
                {
                    var summary = new OfficeHourSummaryModel { OfficeHour = course };
                    summary.Lecturers.Add(lecturer);

                    var days = (from occ in course.Dates
                                select
                                    new
                                    {
                                        Day = occ.Begin.DayOfWeek,
                                        Begin = occ.Begin.TimeOfDay,
                                        End = occ.End.TimeOfDay,
                                    }).Distinct();

                    foreach (var day in days)
                    {
                        var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                        var courseDate = new CourseDateModel
                        {
                            DayOfWeek = day.Day,
                            StartTime = day.Begin,
                            EndTime = day.End,
                            DefaultDate = defaultDay.Begin
                        };
                        summary.Dates.Add(courseDate);
                    }

                    list.Add(summary);
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        public void DeleteAllDates(Guid courseId)
        {
            var course = db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            if (course != null)
            {
                var date = course.Dates.OrderBy(d => d.Begin).FirstOrDefault();
                if (date != null)
                    DeleteAllDatesAfter(courseId, date.Begin);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="dateTime"></param>
        public void DeleteAllDatesAfter(Guid courseId, DateTime dateTime)
        {
            var course = db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            if (course != null)
            {
                // aus dem richtigen Kontext löshen!
                var dates = course.Dates.Where(d => d.Begin >= dateTime && d.Activity.Id == course.Id).ToList();

                foreach (var date in dates)
                {
                    foreach (var slot in date.Slots.ToList())
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                        {
                            slot.Occurrence.Subscriptions.Remove(sub);
                            db.Subscriptions.Remove(sub);
                        }
                        db.Occurrences.Remove(slot.Occurrence);
                        date.Slots.Remove(slot);
                        db.ActivitySlots.Remove(slot);
                    }

                    foreach (var sub in date.Occurrence.Subscriptions.ToList())
                    {
                        date.Occurrence.Subscriptions.Remove(sub);
                        db.Subscriptions.Remove(sub);
                    }

                    date.Hosts.Clear();
                    date.Rooms.Clear();

                    db.Occurrences.Remove(date.Occurrence);
                    course.Dates.Remove(date);
                    db.ActivityDates.Remove(date);
                }

                db.SaveChanges();
            }
        }

        internal bool IsAvailableFor(Course course, string curriculum)
        {
            return course.SemesterGroups.Any(g => 
                g.CapacityGroup.CurriculumGroup != null &&
                g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(curriculum));
        }

        internal List<CourseDateModel> GetDateSummary()
        {
            var days = (from occ in _course.Dates
                        select
                            new
                            {
                                Day = occ.Begin.DayOfWeek,
                                Begin = occ.Begin.TimeOfDay,
                                End = occ.End.TimeOfDay,
                            }).Distinct().ToList();

            var summary = new List<CourseDateModel>();

            foreach (var day in days)
            {
                var defaultDay = _course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                var courseDate = new CourseDateModel
                {
                    DayOfWeek = day.Day,
                    StartTime = day.Begin, 
                    EndTime = day.End,
                    DefaultDate = defaultDay.Begin
                };

                summary.Add(courseDate);
            }

            return summary;
        }

        internal List<OrganiserMember> GetLecturerList()
        {
            return db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == _course.Id)).ToList();
        }

        internal List<Room> GetRoomList()
        {
            return db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == _course.Id)).ToList();
        }

        internal ActivityDate GetNextDate(bool activeOnly = false)
        {
            if (activeOnly)
                return _course.Dates
                    .Where(occ => occ.End >= GlobalSettings.Now && !occ.Occurrence.IsCanceled)
                    .OrderBy(occ => occ.Begin)
                    .FirstOrDefault();

            return _course.Dates
                .Where(occ => occ.End >= GlobalSettings.Now)
                .OrderBy(occ => occ.Begin)
                .FirstOrDefault();
        }

        internal List<SubscriptionDetailViewModel> GetWaitingList()
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in _course.Occurrence.Subscriptions.Where(s => s.OnWaitingList==true))
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    pList.Add(new SubscriptionDetailViewModel
                    {
                        Subscription = subscription,
                        User = user
                    });
                }
            }

            return pList;
        }

        internal List<SubscriptionDetailViewModel> GetTotalParticipantList()
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in _course.Occurrence.Subscriptions)
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    pList.Add(new SubscriptionDetailViewModel
                    {
                        Subscription = subscription,
                        User = user
                    });
                }
            }

            return pList;
        }

        internal List<SubscriptionDetailViewModel> GetParticipantList()
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in _course.Occurrence.Subscriptions.Where(s => s.OnWaitingList == false))
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    pList.Add(new SubscriptionDetailViewModel
                    {
                        Subscription = subscription,
                        User = user
                    });
                }
            }

            return pList;
        }

        internal List<OccurrenceGroupCapacityModel> GetSubscriptionGroups()
        {
            var groups = new List<OccurrenceGroupCapacityModel>();

            foreach (var semGroup in _course.SemesterGroups)
            {
                var occGroup = _course.Occurrence.Groups.SingleOrDefault(g => g.SemesterGroups.Any(s => s.Id == semGroup.Id));

                // u.U. fehlende Gruppen automatisch ergänzen
                if (occGroup == null)
                {
                    occGroup = new OccurrenceGroup
                    {
                        FitToCurriculumOnly = false,
                        Capacity = 0,
                    };
                    occGroup.SemesterGroups.Add(semGroup);
                    _course.Occurrence.Groups.Add(occGroup);
                    db.SaveChanges();
                }

                groups.Add(new OccurrenceGroupCapacityModel
                {
                    CourseId = _course.Id,
                    SemesterGroupId = semGroup.Id,
                    Semester = semGroup.Semester.Name,
                    Curriculum = semGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName,
                    Group = semGroup.GroupName,
                    Organiser = semGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.ShortName,
                    Capacity = occGroup.Capacity
                });
            }

            return groups;
        }

        internal List<OccurrenceCapacityOption> GetCapacitySettings()
        {
            var model = new List<OccurrenceCapacityOption>();

            model.Add(new OccurrenceCapacityOption
            {
                Id = 1,
                HasValue = false,
                Selected = _course.Occurrence.UseGroups == false && _course.Occurrence.Capacity < 0,
                Text = "Keine Platzbeschränkung",
            });

            model.Add(new OccurrenceCapacityOption
            {
                Id = 2,
                HasValue = true,
                Selected = !_course.Occurrence.UseGroups && _course.Occurrence.Capacity >= 0,
                Text = "Gesamtzahl an Plätzen festlegen (ohne Berücksichtigung der Gruppeninformationen)",
                Capacity = _course.Occurrence.Capacity
            });

            model.Add(new OccurrenceCapacityOption
            {
                Id = 3,
                HasValue = false,
                Selected = _course.Occurrence.UseGroups && !_course.Occurrence.UseExactFit,
                Text = "Platzbeschränkung auf Ebene Studienprogramme festlegen",
            });

            model.Add(new OccurrenceCapacityOption
            {
                Id = 4,
                HasValue = false,
                Selected = _course.Occurrence.UseGroups && _course.Occurrence.UseExactFit,
                Text = "Platzbeschränkung auf Ebene der Semestergruppen festlegen",
            });

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public CourseSummaryModel GetCourseSummary(Activity course)
        {
            var summary = new CourseSummaryModel();

            summary.Course = course;

            var lectures =
                db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Lecturers.AddRange(lectures);

            var rooms =
                db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Rooms.AddRange(rooms);


            var days = (from occ in course.Dates
                        select
                            new
                            {
                                Day = occ.Begin.DayOfWeek,
                                Begin = occ.Begin.TimeOfDay,
                                End = occ.End.TimeOfDay,
                            }).Distinct();

            foreach (var day in days)
            {
                var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                var courseDate = new CourseDateModel
                {
                    DayOfWeek = day.Day,
                    StartTime = day.Begin,
                    EndTime = day.End,
                    DefaultDate = defaultDay.Begin
                };
                summary.Dates.Add(courseDate);
            }

            return summary;
        }

        internal bool IsActive(OrganiserMember member, Semester semester)
        {
            return db.ActivityDates.Any(d => d.Hosts.Any(h => h.Id == member.Id) &&
                                             d.Activity.SemesterGroups.Any(s => s.Semester.Id == semester.Id));
        }
    }
}