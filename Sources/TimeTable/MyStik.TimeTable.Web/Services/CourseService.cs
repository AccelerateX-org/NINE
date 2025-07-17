using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Booking;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseService : BaseService
    {

        public CourseService() : base()
        {
        }

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
                    var summary = GetCourseSummary(course);
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

        public List<CourseSummaryModel> SearchCourses(string searchString, ActivityOrganiser organiser, Semester semester)
        {

            if (organiser != null)
            {
                var courses =
                    Db.Activities.OfType<Course>().Where(
                        c => 
                             c.Organiser.Id == organiser.Id &&
                             c.Semester != null && c.Semester.Id == semester.Id &&
                             (c.Name.ToUpper().Contains(searchString) ||
                              c.ShortName.ToUpper().Contains(searchString) ||
                              c.Dates.Any(
                                  d =>
                                      d.Hosts.Any(
                                          h =>
                                              h.Name.Contains(searchString) ||
                                              h.ShortName.Contains(searchString))))
                    ).OrderBy(c => c.Name).ToList();

                return CreateCourseSummaries(courses);
            }
            else
            {
                var courses =
                    Db.Activities.OfType<Course>().Where(
                        c => 
                             c.Semester != null && c.Semester.Id == semester.Id &&
                             (c.Name.ToUpper().Contains(searchString) ||
                              c.ShortName.ToUpper().Contains(searchString) ||
                              c.Dates.Any(
                                  d =>
                                      d.Hosts.Any(
                                          h =>
                                              h.Name.Contains(searchString) ||
                                              h.ShortName.Contains(searchString))))
                    ).OrderBy(c => c.Organiser.ShortName).ToList();

                return CreateCourseSummaries(courses);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semesterName"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public List<CourseSummaryModel> SearchCourses(Semester semester, string searchString, ActivityOrganiser organiser)
        {
            if (semester == null)
                return new List<CourseSummaryModel>();


            if (organiser != null)
            {
                var courses =
                    Db.Activities.OfType<Course>().Where(
                        c => c.Semester.Id == semester.Id &&
                             c.Organiser.Id == organiser.Id &&
                                                       (c.Name.ToUpper().Contains(searchString) ||
                                                        c.ShortName.ToUpper().Contains(searchString)) 
                    ).OrderBy(c => c.Name).ToList();

                return CreateCourseSummaries(courses);
            }
            else
            {
                var courses =
                    Db.Activities.OfType<Course>().Where(
                            c => c.Semester.Id == semester.Id &&
                                                           (c.Name.ToUpper().Contains(searchString) ||
                                                            c.ShortName.ToUpper().Contains(searchString) ||
                                                            c.Dates.Any(
                                                                d =>
                                                                    d.Hosts.Any(
                                                                        h =>
                                                                            h.Name.Contains(searchString) ||
                                                                            h.ShortName.Contains(searchString))))
                    ).OrderBy(c => c.Organiser.ShortName).ToList();

                return CreateCourseSummaries(courses);
            }
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
                var summary = GetCourseSummary(course);
                list.Add(summary);
            }

            return list;
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
        public CourseSummaryModel GetCourseSummary(Course course)
        {
            var summary = new CourseSummaryModel { Course = course };

            AppendBlocks(course, summary);


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
                var defaultDays = course.Dates.Where(d => 
                    d.Begin.DayOfWeek == day.Day &&
                    d.Begin.TimeOfDay == day.Begin &&
                    d.End.TimeOfDay == day.End).ToList();

                var courseDate = new CourseDateModel
                {
                    DayOfWeek = day.Day,
                    StartTime = day.Begin,
                    EndTime = day.End,
                    Dates = defaultDays
                };
                summary.Dates.Add(courseDate);
            }

            var lectures =
                Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Lecturers.AddRange(lectures);

            var rooms =
                Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Rooms.AddRange(rooms);

            var vRooms =
                Db.VirtualRooms.Where(l => l.Accesses.Any(a => a.Date.Activity.Id == course.Id)).ToList();
            summary.VirtualRooms.AddRange(vRooms);


            summary.Lottery =
                Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


            return summary;
        }

        internal void AppendBlocks(Course course, CourseSummaryModel summary)
        {
            if (!course.Dates.Any())
            {
                return;
            }

            var orderedDates = course.Dates.OrderBy(x => x.Begin).ToList();
            var currentDate = orderedDates.FirstOrDefault();

            var currentBlock = new CourseBlockModel();
            currentBlock.Dates.Add(currentDate);
            summary.Blocks.Add(currentBlock);

            foreach(var date in orderedDates)
            {
                if (date.Id == currentDate.Id) continue;

                // Anzahl der Tage zwischen den Terminen
                var delta = date.Begin - currentDate.Begin;
                // =1 => zusammenhängender Block
                if (delta.Days <= 1)
                {
                    // Datum gehört zum aktuellen Block
                    currentBlock.Dates.Add(date);
                }
                else
                {
                    // ein neuer Block
                    currentBlock = new CourseBlockModel();
                    currentBlock.Dates.Add(date);
                    summary.Blocks.Add(currentBlock);
                }
                currentDate = date;
            }
        }


        internal bool IsActive(OrganiserMember member, Semester semester)
        {
            return Db.ActivityDates.Any(d => d.Hosts.Any(h => h.Id == member.Id) &&
                                             d.Activity.SemesterGroups.Any(s => s.Semester.Id == semester.Id));
        }

        public Dictionary<ActivityDate, List<ActivityDate>> GetConflictingDates(Course course, List<Course> activities)
        {
            var conflictingDates = new Dictionary<ActivityDate, List<ActivityDate>>();

            foreach (var date in course.Dates)
            {
                var conflictingActivities = activities.Where(x =>
                    x.Id != course.Id &&
                    x.Dates.Any(d =>
                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= date.Begin && d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= date.Begin &&
                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();

                conflictingDates[date] = new List<ActivityDate>();

                foreach (var conflictingActivity in conflictingActivities)
                {
                    var conflicts = conflictingActivity.Dates.Where(d =>
                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= date.Begin &&
                             d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= date.Begin &&
                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                    ).ToList();
                    conflictingDates[date].AddRange(conflicts);
                }
            }


            return conflictingDates;
        }

        public CourseSelectModel GetCourseSelectModel(Guid id, string userId)
        {
            var model = new CourseSelectModel();

            var summary = GetCourseSummary(id);

            model.Summary = summary;

            if (!string.IsNullOrEmpty(userId))
            {
                var studentService = new StudentService(Db);
                var student = studentService.GetCurrentStudent(userId).FirstOrDefault();

                var subscriptionService = new SubscriptionService(Db);
                var subscription = subscriptionService.GetSubscription(summary.Course.Occurrence.Id, userId);

                var bookingService = new BookingServiceQuotas(Db, summary.Course.Occurrence);
                var bookingList = bookingService.GetBookingList(student);
                var bookingLists = bookingService.GetBookingLists();

                var bookingState = new BookingState
                {
                    Occurrence = summary.Course.Occurrence,
                    Student = student,
                    MyBookingList = bookingList,
                    BookingLists = bookingLists,
                };
                bookingState.Init();

                if (summary.Course.Dates.Any())
                {
                    var firstDate = summary.Course.Dates.Min(x => x.Begin);
                    var lastDate = summary.Course.Dates.Max(x => x.End);
                    var activities = Db.Activities.OfType<Course>()
                        .Where(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId)) &&
                                    x.Dates.Any(d => d.End >= firstDate && d.Begin <= lastDate)
                        ).ToList();

                    model.Summary.ConflictingDates = GetConflictingDates(summary.Course, activities);
                }

                model.Student = student;
                model.BookingState = bookingState;
                model.Subscription = subscription;

                summary.Subscription = subscription;
            }

            return model;
        }


        public CourseConflictModel GetConflicts(Course course)
        {
            var model = new CourseConflictModel();

            var opponents = new List<ActivityConflictModel>();

            foreach (var date in course.Dates)
            {
                var collisions = GetCollisions(date);

                // hole alle Kollisionen

                // suche die relevanten
                // Raumkonflikte
                foreach (var room in date.Rooms)
                {
                    var roomCollisons = collisions.Where(x => x.Rooms.Any(r => r.Id == room.Id)).ToList();
                    // aufheben
                    // der Termin
                    // den Raum
                    // die anderen Termine
                    if (roomCollisons.Any())
                    {
                        var conflict = new DateConflictModel
                        {
                            Date = date,
                            Collisions = roomCollisons,
                            Room = room
                        };

                        foreach (var collision in roomCollisons)
                        {
                            var opponent = opponents.FirstOrDefault(x => x.Activity.Id == collision.Activity.Id);
                            if (opponent == null)
                            {
                                opponent = new ActivityConflictModel()
                                {
                                    Activity = collision.Activity
                                };
                                opponents.Add(opponent);
                            }

                            opponent.Conflicts.Add(conflict);
                        }
                    }
                }
           

                // Stundenplankonflikte
                foreach (var host in date.Hosts)
                {
                    if (string.IsNullOrEmpty(host.UserId))
                    {
                        // nur den Member suchen
                        var memberCollisons = collisions.Where(x => x.Hosts.Any(r => r.Id == host.Id)).ToList();
                        if (memberCollisons.Any())
                        {
                            var conflict = new DateConflictModel
                            {
                                Date = date,
                                Collisions = memberCollisons,
                                Member = host
                            };

                            foreach (var collision in memberCollisons)
                            {
                                var opponent =
                                    opponents.FirstOrDefault(x => x.Activity.Id == collision.Activity.Id);
                                if (opponent == null)
                                {
                                    opponent = new ActivityConflictModel()
                                    {
                                        Activity = collision.Activity
                                    };
                                    opponents.Add(opponent);
                                }

                                opponent.Conflicts.Add(conflict);
                            }
                        }
                        else
                        {
                            // nach dem User suchen
                            var userCollisions = collisions.Where(x => x.Hosts.Any(r => r.UserId == host.UserId))
                                .ToList();
                            if (userCollisions.Any())
                            {
                                var conflict = new DateConflictModel
                                {
                                    Date = date,
                                    Collisions = userCollisions,
                                    Member = host
                                };

                                foreach (var collision in userCollisions)
                                {
                                    var opponent =
                                        opponents.FirstOrDefault(x => x.Activity.Id == collision.Activity.Id);
                                    if (opponent == null)
                                    {
                                        opponent = new ActivityConflictModel()
                                        {
                                            Activity = collision.Activity
                                        };
                                        opponents.Add(opponent);
                                    }

                                    opponent.Conflicts.Add(conflict);
                                }
                            }
                        }
                    }
                }

                // Verfügbarkeitsproblem
                // => das bezieht sich auf den Ausgangstermin an sich bzw. die Referenzzeit

                // Kohorte
                foreach (var label in date.Activity.LabelSet.ItemLabels)
                {
                    var labelCollisions = collisions.Where(x =>
                            x.Activity.LabelSet != null &&
                            x.Activity.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                        .ToList();
                    if (labelCollisions.Any())
                    {
                        var conflict = new DateConflictModel
                        {
                            Date = date,
                            Collisions = labelCollisions,
                            Label = label
                        };

                        foreach (var collision in labelCollisions)
                        {
                            var opponent = opponents.FirstOrDefault(x => x.Activity.Id == collision.Activity.Id);
                            if (opponent == null)
                            {
                                opponent = new ActivityConflictModel()
                                {
                                    Activity = collision.Activity
                                };
                                opponents.Add(opponent);
                            }

                            opponent.Conflicts.Add(conflict);
                        }
                    }
                }
            }

            model.Opponents = opponents;

            return model;
        }

        /// <summary>
        /// Alle Kollissionen eines Termins mit alternativem Zeitraum
        /// </summary>
        /// <param name="date"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<ActivityDate> GetCollisions(ActivityDate date)
        {
            var collisions = Db.ActivityDates.Where(x => x.Id != date.Id &&
                            ((x.End > date.Begin && x.End <= date.End) || // Veranstaltung endet im Zeitraum
                            (x.Begin >= date.Begin && x.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                            (x.Begin <= date.Begin && x.End >= date.End)) // Veranstaltung zieht sich über gesamten Zeitraum
            ).ToList();

            return collisions;
        }

        public List<ActivityDate> GetCollisions(Course course, DateTime begin, DateTime end)
        {
            var collisions = Db.ActivityDates.Where(x => x.Activity.Id != course.Id &&
                                                         ((x.End > begin && x.End <= end) || // Veranstaltung endet im Zeitraum
                                                         (x.Begin >= begin && x.Begin < end) || // Veranstaltung beginnt im Zeitraum
                                                         (x.Begin <= begin && x.End >= end)) // Veranstaltung zieht sich über gesamten Zeitraum
            ).ToList();

            return collisions;
        }


        public void RepairDates(Course course)
        {
            if (course.Semester != null && course.Organiser != null && course.Segment != null)
            {
                if (course.Semester.Id != course.Segment.Semester.Id)
                {
                    // suche aus dem Semester das Segmant mit den selben Namen
                    var segment =
                        course.Semester.Dates.FirstOrDefault(x =>
                            x.Description.Equals(course.Segment.Description) && x.Organiser != null &&
                            x.Organiser.Id == course.Organiser.Id);

                    if (segment != null)
                    {
                        course.Segment = segment;
                    }
                }
            }

            var dates = course.Dates.Where(x => x.Occurrence == null).ToList();

            if (dates.Any())
            {
                foreach (var date in dates)
                {
                    date.Occurrence = new Occurrence
                    {
                        Capacity = int.MaxValue,
                        IsAvailable = true,
                        IsCanceled = false,
                        IsMoved = false,
                        FromIsRestricted = false,
                        UntilIsRestricted = false,
                        UseGroups = false,
                    };
                }
            }

            Db.SaveChanges();
        }

        public List<ActivityDate> GetPlanningDates(CourseSummaryModel summary, SemesterDate segment)
        {
            var dates = new List<ActivityDate>();

            List<Room> favRooms = new List<Room>();
            List<OrganiserMember> favHosts = new List<OrganiserMember>();

            var room = summary.GetFavoriteRoom();
            var host = summary.GetFavoriteHost();


            if (room != null)
            {
                favRooms.Add(room);
            }

            if (host != null)
            {
                favHosts.Add(host);
            }

            var frq = 1;
            var occDate = segment.From.Date;
            var lastDate = segment.To.Date;
            var refDate = summary.Course.Dates.OrderBy(x => x.Begin).First();


            if (!summary.IsPureBlock())
            {
                frq = 7;

                var majorvDate = summary.Dates.OrderByDescending(x => x.Dates.Count).FirstOrDefault();
                refDate = majorvDate.Dates.First();
                var semesterStartTag = (int)segment.From.DayOfWeek;
                var day = (int)refDate.Begin.DayOfWeek;
                int nDays = day - semesterStartTag;
                if (nDays < 0)
                {
                    nDays += 7;
                }

                occDate = segment.From.AddDays(nDays);
            }



            //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
            int numOcc = 0;
            while (occDate <= lastDate)
            {
                bool isVorlesung = true;
                foreach (var sd in segment.Semester.Dates)
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
                    var ocStart = new DateTime(occDate.Year, occDate.Month, occDate.Day, refDate.Begin.Hour,
                        refDate.Begin.Minute, refDate.Begin.Second);
                    var ocEnd = new DateTime(occDate.Year, occDate.Month, occDate.Day, refDate.End.Hour,
                        refDate.End.Minute, refDate.End.Second);

                    var occ = new ActivityDate
                    {
                        Id = Guid.NewGuid(),
                        Begin = ocStart,
                        End = ocEnd,
                    };


                    foreach (var favRoom in favRooms)
                    {
                        occ.Rooms.Add(favRoom);
                    }

                    foreach (var favHost in favHosts)
                    {
                        occ.Hosts.Add(favHost);
                    }

                    dates.Add(occ);
                    numOcc++;
                }

                occDate = occDate.AddDays(frq);
            }

            return dates;
        }
    }
}