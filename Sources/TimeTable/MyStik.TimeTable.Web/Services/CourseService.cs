using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class CourseService
    {
        private TimeTableDbContext db = new TimeTableDbContext();
        private Course _course;
        protected IdentifyConfig.ApplicationUserManager UserManager;


        public CourseService(IdentifyConfig.ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public CourseService(IdentifyConfig.ApplicationUserManager userManager, Guid courseId)
        {
            UserManager = userManager;
            _course = db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
        }

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
                    Organiser = semGroup.CurriculumGroup.Curriculum.Organiser.ShortName,
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

        internal CourseSummaryModel GetCourseSummary(Activity course)
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