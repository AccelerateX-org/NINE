using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Caching;
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


        private Semester GetSemester(Guid semId)
        {
            return _db.Semesters.SingleOrDefault(s => (s.Id == semId));
        }

        /// <summary>
        /// Liefert das aktuelle Semester
        /// Kriterium: aktuelles Datum liegt in der Vorlesungszeit
        /// </summary>
        /// <returns></returns>
        public Semester GetSemester(DateTime day)
        {
            var sem = _db.Semesters
                .FirstOrDefault(s => s.StartCourses <= day && day <= s.EndCourses);
            if (sem != null) return sem;

            sem = CreateSemester(day);
            _db.Semesters.Add(sem);
            _db.SaveChanges();

            return sem;
        }

        public Semester CreateSemester(DateTime day)
        {
            // Semester anlegen
            // SoSe yyyy: 15.03.yyyy - 30.09.yyyy
            // WiSe yyyy: 01.10.yyyy - 14.03.yyyy+1
            var year = day.Year;
            var soseStart = new DateTime(year, 3, 15);
            var soseEnd   = new DateTime(year, 9, 30);

            var semName = "";
            DateTime startCourses;
            DateTime endCourses;

            if (day < soseStart)
            {
                semName = $"WiSe {year - 1}";
                startCourses = new DateTime(year - 1, 10, 1);
                endCourses = new DateTime(year, 3, 14);
            }
            else if (day > soseEnd)
            {
                semName = $"WiSe {year}";
                startCourses = new DateTime(year, 10, 1);
                endCourses = new DateTime(year + 1, 3, 14);
            }
            else
            {
                semName = $"SoSe {year}";
                startCourses = soseStart;
                endCourses = soseEnd;
            }

            var sem = new Semester()
            {
                Name = semName,
                StartCourses = startCourses,
                EndCourses = endCourses,
            };
            sem.Dates = CreateHolidays(sem);

            return sem;
        }

        public ICollection<SemesterDate> CreateHolidays(Semester sem)
        {
            var list = new List<SemesterDate>();
            var year = sem.StartCourses.Year;

            if (sem.StartCourses.Month == 3)
            {
                // Sommer
                // Ostern
                var easter = GetEaster(year);
                list.Add(new SemesterDate()
                {
                    Description = "Ostern",
                    Semester = sem,
                    HasCourses = false,
                    From = easter.AddDays(-3),
                    To = easter.AddDays(2)
                });
                // Pfingsten => 6 Wochen nach Ostern
                list.Add(new SemesterDate()
                {
                    Description = "Pfingsten",
                    Semester = sem,
                    HasCourses = false,
                    From = easter.AddDays(42 - 2),
                    To = easter.AddDays(42 + 2)
                });
                // 1. Mai
                list.Add(new SemesterDate()
                {
                    Description = "1. Mai",
                    Semester = sem,
                    HasCourses = false,
                    From = new DateTime(year, 5,1),
                    To = new DateTime(year, 5, 1),
                });
                // Christi Himmelfahrt => Do vor Pfingsten
                list.Add(new SemesterDate()
                {
                    Description = "Christi Himmelfahrt",
                    Semester = sem,
                    HasCourses = false,
                    From = easter.AddDays(42 - 10),
                    To = easter.AddDays(42 - 10)
                });
                // Fronleichnam => Do nach Pfingsten
                list.Add(new SemesterDate()
                {
                    Description = "Fronleichnam",
                    Semester = sem,
                    HasCourses = false,
                    From = easter.AddDays(42 + 11),
                    To = easter.AddDays(42 + 11)
                });
            }
            else
            {
                // Winter
                // 03.10.
                list.Add(new SemesterDate()
                {
                    Description = "Tag der deutschen Einheit",
                    Semester = sem,
                    HasCourses = false,
                    From = new DateTime(year, 10, 3),
                    To = new DateTime(year, 10, 3),
                });
                // 01.11.
                list.Add(new SemesterDate()
                {
                    Description = "Allerheiligen",
                    Semester = sem,
                    HasCourses = false,
                    From = new DateTime(year, 11, 1),
                    To = new DateTime(year, 11, 1),
                });
                // Weihnachten 24.12. 
                var xMas = new DateTime(year, 12, 24);
                var kings = new DateTime(year + 1, 1, 6);
                var xMasStart = xMas;
                var xMasEnd = kings;

                switch (xMas.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        xMasStart = xMas.AddDays(-1);
                        break;
                    case DayOfWeek.Monday:
                        xMasStart = xMas.AddDays(-2);
                        break;
                    case DayOfWeek.Tuesday:
                        xMasStart = xMas.AddDays(-3);
                        break;
                }

                switch (kings.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        xMasEnd = xMas.AddDays(1);
                        break;
                    case DayOfWeek.Friday:
                        xMasStart = xMas.AddDays(2);
                        break;
                    case DayOfWeek.Thursday:
                        xMasStart = xMas.AddDays(3);
                        break;
                }


                list.Add(new SemesterDate()
                {
                    Description = "Weihnachten",
                    Semester = sem,
                    HasCourses = false,
                    From = xMasStart,
                    To = xMasEnd,
                });

            }



            return list;
        }

        private DateTime GetEaster(int year)
        {
            var day = 0;
            var month = 0;

            var g = year % 19;
            var c = year / 100;
            var h = (c - c / 4 - (8 * c + 13) / 25 + 19 * g + 15) % 30;
            var i = h - h / 28 * (1 - h / 28 * (29 / (h + 1)) * ((21 - g) / 11));

            day = i - ((year + year / 4 + i + 2 - c + c / 4) % 7) + 28;
            month = 3;
            if (day > 31)
            {
                month++;
                day -= 31;
            }

            var result = new DateTime(year, month, day);
            return result;
        }

        public Semester GetSemester(string name)
        {
            return _db.Semesters.SingleOrDefault(s => (s.Name.ToUpper().Equals(name.ToUpper())));
        }



        public Semester GetNextSemester(DateTime day)
        {
            var sem = _db.Semesters.Where(s => s.StartCourses > day).OrderBy(s => s.StartCourses).FirstOrDefault();
            if (sem != null) return sem;

            // das letzte Semester, das vor dem Tag endet
            var lastSem = _db.Semesters.Where(s => s.EndCourses >= day).OrderBy(s => s.StartCourses).FirstOrDefault();

            // wenn dieses Semester nicht existiert, dann existiert kein Semester
            // das wäre dann eine Exception wert
            if (lastSem == null)
                throw new Exception("Es existiert kein Semester, das vor dem Tag endet. Es kann kein neues Semester angelegt werden.");

            sem = CreateSemester(lastSem.EndCourses.AddDays(1));
            _db.Semesters.Add(sem);
            _db.SaveChanges();

            return sem;
        }

        public Semester GetPreviousSemester(DateTime day)
        {
            var sem = _db.Semesters.Where(s => s.EndCourses < day).OrderByDescending(s => s.EndCourses).FirstOrDefault();
            if (sem != null) return sem;

            var futureSem = _db.Semesters.Where(s => s.StartCourses <= day).OrderByDescending(s => s.StartCourses).FirstOrDefault();

            if (futureSem == null)
                throw new Exception("Es existiert kein Semester, das nach dem Tag beginnt. Es kann kein neues Semester angelegt werden.");

            sem = CreateSemester(futureSem.StartCourses.AddDays(-1));
            _db.Semesters.Add(sem);
            _db.SaveChanges();

            return sem;
        }

        public Semester GetNextSemester(Semester semester)
        {
            if (semester == null)
                return null;

            var day = semester.StartCourses;

            return GetNextSemester(day);
        }


        public Semester GetPreviousSemester(Semester semester)
        {
            if (semester == null)
                return null;

            var day = semester.StartCourses;

            return GetPreviousSemester(day);

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
            var semester = _db.Semesters.Include(semester1 => semester1.Dates).SingleOrDefault(s => s.Id == semId);
            if (semester == null)
            {
                return new List<DateTime>();
            }

            var dates = new List<DateTime>();

            var firstDate = start > semester.StartCourses ? start : semester.StartCourses;
            var lastDate = end < semester.EndCourses ? end : semester.EndCourses;

            var semesterStartTag = (int)((DateTime)firstDate).DayOfWeek;

            var nDays = (int)dayOfWeek - semesterStartTag;
            if (nDays < 0)
                nDays += 7;

            var occDate = firstDate.AddDays(nDays);

            //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
            while (occDate <= lastDate)
            {
                var isVorlesung = true;
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
