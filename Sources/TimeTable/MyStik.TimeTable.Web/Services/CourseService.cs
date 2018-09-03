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
    public class CourseService : BaseService
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public CourseService(TimeTableDbContext db) : base(db)
        {
        }

        public Course GetCourse(Guid id)
        {
            return Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id ==id );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semester"></param>
        /// <param name="lecturer"></param>
        /// <returns></returns>
        public List<Course> GetCourses(Semester semester, OrganiserMember lecturer)
        {
            var list = new List<Course>();

            if (lecturer != null && semester != null)
            {
                list =
                    Db.Activities.OfType<Course>().Where(c =>
                            c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                            c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)))
                        .OrderBy(c => c.Name)
                        .ToList();
            }

            return list;
        }

        public List<Course> GetCourses(ApplicationUser user, SemesterGroup group)
        {
            return Db.Activities.OfType<Course>().Where(x =>
                x.SemesterGroups.Any(g => g.Id == group.Id) &&
                x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();
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
            var semester = Db.Semesters.SingleOrDefault(l => l.Name.ToUpper().Equals(semesterName.ToUpper()));

            if (lecturer != null && semester != null)
            {
                var courses =
                    Db.Activities.OfType<Course>().Where(c =>
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
                    Db.Activities.OfType<Course>().Where(c =>
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
            var semester = Db.Semesters.SingleOrDefault(l => l.Name.ToUpper().Equals(semesterName.ToUpper()));

            if (semester != null)
            {
                var courses =
                    Db.Activities.OfType<Course>().Where(
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
                Db.Activities.OfType<Course>().Where(
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
        /// <param name="courseId"></param>
        public void DeleteAllDates(Guid courseId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
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
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

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
                            Db.Subscriptions.Remove(sub);
                        }
                        Db.Occurrences.Remove(slot.Occurrence);
                        date.Slots.Remove(slot);
                        Db.ActivitySlots.Remove(slot);
                    }

                    foreach (var sub in date.Occurrence.Subscriptions.ToList())
                    {
                        date.Occurrence.Subscriptions.Remove(sub);
                        Db.Subscriptions.Remove(sub);
                    }

                    date.Hosts.Clear();
                    date.Rooms.Clear();

                    Db.Occurrences.Remove(date.Occurrence);
                    course.Dates.Remove(date);
                    Db.ActivityDates.Remove(date);
                }

                Db.SaveChanges();
            }
        }

        internal List<CourseDateModel> GetDateSummary(Course course)
        {
            var days = (from occ in course.Dates
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
                var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

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

        internal List<OrganiserMember> GetLecturerList(Course course)
        {
            return Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
        }

        internal List<Room> GetRoomList(Course course)
        {
            return Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
        }

        internal ActivityDate GetNextDate(Course course, bool activeOnly = false)
        {
            if (activeOnly)
                return course.Dates
                    .Where(occ => occ.End >= DateTime.Now && !occ.Occurrence.IsCanceled)
                    .OrderBy(occ => occ.Begin)
                    .FirstOrDefault();

            return course.Dates
                .Where(occ => occ.End >= DateTime.Now)
                .OrderBy(occ => occ.Begin)
                .FirstOrDefault();
        }

        internal List<ActivityDate> GetDatesThisWeek(Course course)
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            return course.Dates
                .Where(occ => occ.Begin >= startOfWeek && occ.End <= endOfWeek)
                .OrderBy(occ => occ.Begin).ToList();
        }


        internal List<ActivityDate> GetDatesNextWeek(Course course)
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday + 7);
            var endOfWeek = startOfWeek.AddDays(7);

            return course.Dates
                .Where(occ => occ.Begin >= startOfWeek && occ.End <= endOfWeek)
                .OrderBy(occ => occ.Begin).ToList();
        }


        internal List<SubscriptionDetailViewModel> GetWaitingList(Course course)
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in course.Occurrence.Subscriptions.Where(s => s.OnWaitingList==true))
            {
                pList.Add(new SubscriptionDetailViewModel
                {
                    Subscription = subscription,
                });
            }

            return pList;
        }

        internal List<SubscriptionDetailViewModel> GetTotalParticipantList(Course course)
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in course.Occurrence.Subscriptions)
            {
                pList.Add(new SubscriptionDetailViewModel
                {
                    Subscription = subscription,
                });
            }

            return pList;
        }

        internal List<SubscriptionDetailViewModel> GetParticipantList(Course course)
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in course.Occurrence.Subscriptions.Where(s => s.OnWaitingList == false && s.IsConfirmed))
            {
                pList.Add(new SubscriptionDetailViewModel
                {
                    Subscription = subscription,
                });
            }

            return pList;
        }

        internal List<SubscriptionDetailViewModel> GetReservationList(Course course)
        {
            var pList = new List<SubscriptionDetailViewModel>();

            foreach (var subscription in course.Occurrence.Subscriptions.Where(s => s.OnWaitingList == false && s.IsConfirmed==false))
            {
                pList.Add(new SubscriptionDetailViewModel
                {
                    Subscription = subscription,
                });
            }

            return pList;
        }


        internal List<OccurrenceGroupCapacityModel> GetSubscriptionGroups(Course course)
        {
            var groups = new List<OccurrenceGroupCapacityModel>();

            foreach (var semGroup in course.SemesterGroups)
            {
                var occGroup = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Any(s => s.Id == semGroup.Id));

                // u.U. fehlende Gruppen automatisch ergänzen
                if (occGroup == null)
                {
                    occGroup = new OccurrenceGroup
                    {
                        FitToCurriculumOnly = false,
                        Capacity = 0,
                    };
                    occGroup.SemesterGroups.Add(semGroup);
                    course.Occurrence.Groups.Add(occGroup);
                    Db.SaveChanges();
                }

                groups.Add(new OccurrenceGroupCapacityModel
                {
                    CourseId = course.Id,
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

        internal List<OccurrenceCapacityOption> GetCapacitySettings(Course course)
        {
            var model = new List<OccurrenceCapacityOption>();

            model.Add(new OccurrenceCapacityOption
            {
                Id = 1,
                HasValue = false,
                Selected = course.Occurrence.UseGroups == false && course.Occurrence.Capacity < 0,
                Text = "Keine Platzbeschränkung",
            });

            model.Add(new OccurrenceCapacityOption
            {
                Id = 2,
                HasValue = true,
                Selected = !course.Occurrence.UseGroups && course.Occurrence.Capacity >= 0,
                Text = "Gesamtzahl an Plätzen festlegen (ohne Berücksichtigung der Gruppeninformationen)",
                Capacity = course.Occurrence.Capacity
            });

            model.Add(new OccurrenceCapacityOption
            {
                Id = 3,
                HasValue = false,
                Selected = course.Occurrence.UseGroups && !course.Occurrence.UseExactFit,
                Text = "Platzbeschränkung auf Ebene Studienprogramme festlegen",
            });

            model.Add(new OccurrenceCapacityOption
            {
                Id = 4,
                HasValue = false,
                Selected = course.Occurrence.UseGroups && course.Occurrence.UseExactFit,
                Text = "Platzbeschränkung auf Ebene der Semestergruppen festlegen",
            });

            return model;
        }

        public CourseSummaryModel GetCourseSummary(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);
            return GetCourseSummary(course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public CourseSummaryModel GetCourseSummary(Activity course)
        {
            var summary = new CourseSummaryModel();

            summary.Course = course as Course;

            var lectures =
                Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Lecturers.AddRange(lectures);

            var rooms =
                Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Rooms.AddRange(rooms);


            foreach (var semesterGroup in course.SemesterGroups)
            {
                if (!summary.Curricula.Contains(semesterGroup.CapacityGroup.CurriculumGroup.Curriculum))
                {
                    summary.Curricula.Add(semesterGroup.CapacityGroup.CurriculumGroup.Curriculum);
                }
            }


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
            return Db.ActivityDates.Any(d => d.Hosts.Any(h => h.Id == member.Id) &&
                                             d.Activity.SemesterGroups.Any(s => s.Semester.Id == semester.Id));
        }

        
    }
}