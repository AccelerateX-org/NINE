using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace MyStik.TimeTable.DataServices
{
    public class DatePeriod
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }

    public class SemesterService
    {
        private readonly TimeTableDbContext _db;

        public SemesterService()
        {
            _db = new TimeTableDbContext();
        }

        public SemesterService(TimeTableDbContext db)
        {
            _db = db;
        }

        public Semester GetSemester(Guid? id)
        {
            return id.HasValue ? GetSemester(id.Value) : GetSemester(DateTime.Today);
        }


        /// <summary>
        /// Liefert das aktuelle Semester
        /// Kriterium: aktuelles Datum liegt in der Vorlesungszeit
        /// </summary>
        /// <returns></returns>
        public Semester GetSemester(DateTime day)
        {
            return _db.Semesters.FirstOrDefault(s => s.StartCourses <= day && day <= s.EndCourses);
        }


        public Semester GetNextSemester(DateTime day)
        {
            var sem = _db.Semesters.Where(s => s.StartCourses > day).OrderBy(s => s.StartCourses).FirstOrDefault();

            return sem;
        }

        public Semester GetNextSemester(Semester semester)
        {
            if (semester == null)
                return null;

            var today = semester.StartCourses;

            var sem = _db.Semesters.Where(s => s.StartCourses > today).OrderBy(s => s.StartCourses).FirstOrDefault();

            return sem;
        }


        public Semester GetPreviousSemester(Semester semester)
        {
            var today = semester.EndCourses;
            
            var sem = _db.Semesters.Where(s => s.EndCourses < today).OrderByDescending(s => s.EndCourses).FirstOrDefault();

            return sem;
        }


        public Semester GetSemester(Semester semester, int delta)
        {
            var semesters = _db.Semesters.Where(s => s.StartCourses < semester.StartCourses).OrderByDescending(s => s.StartCourses);
            if (semesters.Count() >= delta)
            {
                return semesters.ToArray()[delta-1];
            }

            return new Semester { Name = "N.A." };
        }





        public Semester GetSemester(DateTime from, DateTime to)
        {
            var semester = _db.Semesters.SingleOrDefault(s => (s.StartCourses<=from && to <= s.EndCourses) ||
                (from <= s.StartCourses && to >= s.StartCourses) ||
                (to >= s.EndCourses && from <= s.EndCourses));

            return semester;
        }

        public Semester GetSemester(string name)
        {
            return _db.Semesters.SingleOrDefault(s => (s.Name.ToUpper().Equals(name.ToUpper())));
        }

        public Semester GetSemester(Guid semId)
        {
            return _db.Semesters.SingleOrDefault(s => (s.Id == semId));
        }



        public ICollection<DateTime> GetDays(Guid semId, DayOfWeek dayOfWeek)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Id == semId);

            if (semester != null)
                return GetDays(semId, dayOfWeek, semester.StartCourses, semester.EndCourses);
            return new List<DateTime>();
        }

        public ICollection<DateTime> GetDays(Guid semId, DateTime firstDate)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Id == semId);

            if (semester != null)
                return GetDays(semId, firstDate.DayOfWeek, firstDate, semester.EndCourses);
            return new List<DateTime>();
        }


        public ICollection<DateTime> GetDays(Guid semId, DayOfWeek dayOfWeek, DateTime start, DateTime end)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Id == semId);
            if (semester == null)
            {
                return new List<DateTime>();
            }

            var dates = new List<DateTime>();


            if (semester == null)
                return dates;

            DateTime firstDate = start > semester.StartCourses ? start : semester.StartCourses;
            DateTime lastDate = end < semester.EndCourses ? end : semester.EndCourses;

            var semesterStartTag = (int)((DateTime)firstDate).DayOfWeek;

            var nDays = (int)dayOfWeek - semesterStartTag;
            if (nDays < 0)
                nDays += 7;

            var occDate = firstDate.AddDays(nDays);

            //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
            while (occDate <= lastDate)
            {
                var isVorlesung = true;
                foreach (SemesterDate sd in semester.Dates)
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
                    dates.Add(occDate);
                }

                occDate = occDate.AddDays(7);
            }

            return dates;

        }

        public ICollection<DateTime> GetDays(SemesterDate segment, DayOfWeek dayOfWeek, DateTime start, DateTime end)
        {
            var dates = new List<DateTime>();

            DateTime firstDate = start;
            DateTime lastDate = end;

            var semesterStartTag = (int)((DateTime)firstDate).DayOfWeek;

            var nDays = (int)dayOfWeek - semesterStartTag;
            if (nDays < 0)
                nDays += 7;

            var occDate = firstDate.AddDays(nDays);

            var noLectureDates = segment.Semester.Dates.Where(x => x.HasCourses == false &&
                                                          (x.Organiser == null || x.Organiser.Id == segment.Organiser.Id)).ToList();

            //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
            while (occDate <= lastDate)
            {
                var isVorlesung = true;
                foreach (SemesterDate sd in noLectureDates)
                {
                    // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                    if (sd.From.Date <= occDate.Date &&
                        occDate.Date <= sd.To.Date)
                    {
                        isVorlesung = false;
                    }
                }

                if (isVorlesung)
                {
                    dates.Add(occDate);
                }

                occDate = occDate.AddDays(7);
            }

            return dates;

        }


        /// <summary>
        /// tagesliste auf Basis Start- und Enddatum
        /// Die vorlesungsfreien Tage werden jeweils nach Semester gefiltert
        /// </summary>
        /// <param name="firstDate"></param>
        /// <param name="lastDate"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public ICollection<DateTime> GetDays(DateTime firstDate, DateTime lastDate, int frequency)
        {
            var dates = new List<DateTime>();

            if (firstDate > lastDate)
                return dates;

            if (frequency == 0)
            {
                dates.Add(firstDate);
                return dates;
            }

            var day = firstDate;
            while (day <= lastDate)
            {
                // zunächst ohne Semester-Check
                var semester = GetSemester(day);
                if (semester == null) // liegt außerhalb Vorlesungszeit, derzeit kein Feiertagskalender => also anfügen
                {
                    dates.Add(day);
                }
                else
                {
                    // prüfen, ob vorlesungsfrei
                    var isFree = IsFree(semester, day);
                    if (!isFree)
                    {
                        dates.Add(day);
                    }
                }

                day = day.AddDays(frequency);
            }

            return dates;
        }

        public bool IsFree(Semester semester, DateTime day)
        {
            // innerhalb Vorlesungszeit   
            if (semester.StartCourses <= day && day <= semester.EndCourses)
            {
                var isVorlesung = true;
                foreach (SemesterDate sd in semester.Dates)
                {
                    // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                    if (sd.From.Date <= day.Date &&
                        day.Date <= sd.To.Date &&
                        sd.HasCourses == false)
                    {
                        isVorlesung = false;
                    }
                }

                return !isVorlesung;
            }

            // ist nicht explizit vorlesungsfrei
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentId"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ICollection<DatePeriod> GetDatePeriods(Guid segmentId, DayOfWeek dayOfWeek, DateTime start, DateTime end)
        {
            var segment = _db.SemesterDates.SingleOrDefault(x => x.Id == segmentId);

            var days = GetDays(segment, dayOfWeek, segment.From, segment.To);

            var dates = new HashSet<DatePeriod>();

            foreach (var day in days)
            {
                var period = new DatePeriod
                {
                    Begin = day.Date.Add(start.TimeOfDay),
                    End = day.Date.Add(end.TimeOfDay)
                };

                dates.Add(period);
            }


            return dates;
        }



        public SemesterSubscription GetSubscription(Semester semester, string userId)
        {
            var sub = _db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semester.Id);

            return sub;
        }

        /// <summary>
        /// Alle Lehrangebote
        /// </summary>
        /// <param name="semester"></param>
        /// <param name="availableOnly"></param>
        /// <returns></returns>
        public ICollection<Data.Curriculum> GetActiveCurricula(Semester semester, bool availableOnly)
        {
            if (availableOnly)
            {
                return
                    _db.Curricula.Where(
                        x => !x.IsDeprecated &&
                            x.CurriculumGroups.Any(g => g.CapacityGroups.Any(a =>
                            a.SemesterGroups.Any(s => s.Semester.Id == semester.Id && s.IsAvailable)))).ToList();
            }
            return
                _db.Curricula.Where(
                    x => x.CurriculumGroups.Any(g => g.CapacityGroups.Any(a =>
                        a.SemesterGroups.Any(s => s.Semester.Id == semester.Id)))).ToList();
        }

        public ICollection<Data.Curriculum> GetActiveCurricula(ActivityOrganiser org, Semester semester, bool availableOnly)
        {
            if (org == null)
                return new List<Data.Curriculum>();

            if (availableOnly)
            {
                return
                    _db.Curricula.Where(
                        x => x.Organiser.Id == org.Id &&
                             x.CurriculumGroups.Any(
                                 y => y.CapacityGroups.Any(
                                     z => z.SemesterGroups.Any(
                                         k => k.IsAvailable && k.Semester.Id == semester.Id)
                                 )))
                                 .ToList();
            }

            return
                _db.Curricula.Where(
                        x => x.Organiser.Id == org.Id &&
                             x.CurriculumGroups.Any(
                                 y => y.CapacityGroups.Any(
                                     z => z.SemesterGroups.Any(
                                         k => k.Semester.Id == semester.Id)
                                 )))
                    .ToList();
        }


        public ICollection<Data.ActivityOrganiser> GetActiveOrganiser(Semester semester)
        {
            return _db.Activities.OfType<Course>()
                .Where(x => x.Organiser != null && x.Semester != null && x.Semester.Id == semester.Id)
                .Select(x => x.Organiser)
                .Distinct().ToList();
        }

        public ICollection<Data.ActivityOrganiser> GetActiveOrganiserOfficeHour(Semester semester)
        {
            return _db.Activities.OfType<OfficeHour>()
                .Where(x => x.Organiser != null && x.Semester != null && x.Semester.Id == semester.Id)
                .Select(x => x.Organiser)
                .Distinct().ToList();
        }

        public ICollection<Data.ActivityOrganiser> GetActiveEventOrganiser(Semester semester)
        {
            /*
            return _db.Activities.OfType<Event>()
                .Where(x => 
                    x.Organiser != null && x.Semester != null && x.Semester.Id == semester.Id)
                .Select(x => x.Organiser)
                .Distinct().ToList();
            */
            return
                _db.Organisers.Where(
                    x => x.Activities.OfType<Event>().Any(
                        e => e.Dates.Any(
                            d => semester.StartCourses <= d.Begin && d.Begin <= semester.EndCourses))).ToList();
        }


        public ICollection<Course> GetCourses(Semester semester, Data.Curriculum curr)
        {
            return
            _db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(g =>
                g.Semester.Id == semester.Id && g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)).ToList();
        }

        public ICollection<Data.Semester> GetActiveSemester(ActivityOrganiser org)
        {
            if (org == null)
                return new List<Data.Semester>();


            var semList = _db.Activities.OfType<Course>().Where(x => 
                x.Semester != null && x.Organiser != null && 
                x.Organiser.Id == org.Id).Select(x => x.Semester).Distinct()
                    .OrderByDescending(x => x.StartCourses).ToList();
        
            return semList ?? new List<Data.Semester>();
        }

        /// <summary>
        /// das aktuellste aktive (freigegebene) Semester des Veranstalters
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public Semester GetLatestSemester(ActivityOrganiser org)
        {
            var sem =
            _db.Semesters
                .Where(x => x.Groups.Any(g =>
                    g.IsAvailable && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .OrderByDescending(x => x.StartCourses).FirstOrDefault();

            return sem ?? GetSemester(DateTime.Today);
        }

        /// <summary>
        /// Das neueste Semester, das Semestergruppen hat
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public Semester GetNewestSemester(ActivityOrganiser org)
        {
            var sem =
            _db.Activities.OfType<Course>().Where(x => x.Organiser.Id == org.Id).Select(x => x.Semester).Distinct()
                .OrderByDescending(x => x.StartCourses).FirstOrDefault();

            /*
            var sem =
                _db.Semesters
                    .Where(x => x.Groups.Any(g =>
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                    .OrderByDescending(x => x.StartCourses).FirstOrDefault();
            */

            return sem ?? GetSemester(DateTime.Today);
        }


        public int GetSemesterIndex(Semester semester)
        {
            var current = GetSemester(DateTime.Today);

            return _db.Semesters
                .Count(x => x.StartCourses >= semester.StartCourses && x.StartCourses <= current.StartCourses);
        }

        public bool IsActive(Semester semester)
        {
            if (semester == null)
                return false;
            return _db.SemesterGroups.Any(x => x.Semester.Id == semester.Id);
        }

        public bool IsLectureDay(Semester sem, ActivityOrganiser org, DateTime date)
        {
            var dateList = sem.Dates.Where(x => x.Organiser == null || x.Organiser.Id == org.Id)
                .ToList();


            bool isVorlesung = true;
            foreach (var sd in dateList)
            {
                // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                if (sd.From.Date <= date &&
                    date <= sd.To.Date &&
                    sd.HasCourses == false)
                {
                    isVorlesung = false;
                }
            }

            return isVorlesung;
        }
    }
}
