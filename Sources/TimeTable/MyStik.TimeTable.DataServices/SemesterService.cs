using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class SemesterService
    {
        private readonly TimeTableDbContext _db = new TimeTableDbContext();

        public Semester GetCurrentSemester()
        {
            var today = DateTime.Today;
            return _db.Semesters.FirstOrDefault(s => s.StartCourses <= today && today <= s.EndCourses);
        }

        public Semester GetNextSemester()
        {
            var today = DateTime.Today;

            var sem = _db.Semesters.Where(s => s.StartCourses > today).OrderBy(s => s.StartCourses).First();

            if (sem != null)
                return sem;

            return new Semester { Name = "N.A." };
        }

        public Semester GetPreviousSemester()
        {
            var today = DateTime.Today;
            
            var sem = _db.Semesters.Where(s => s.EndCourses < today).OrderByDescending(s => s.EndCourses).First();
            
            if (sem != null)
                return sem;

            return new Semester { Name = "N.A." };
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


        public void EnableNewestSemester()
        {
            var semesters = _db.Semesters.OrderByDescending(s => s.StartCourses);
            if (semesters.Any())
            {
                semesters.First().BookingEnabled = true;
                _db.SaveChanges();
            }
        }

        public void DisableNewestSemester()
        {
            var semesters = _db.Semesters.OrderByDescending(s => s.StartCourses);
            if (semesters.Any())
            {
                semesters.First().BookingEnabled = false;
                _db.SaveChanges();
            }
        }




        public Semester GetSemester(DateTime from, DateTime to)
        {
            var semester = _db.Semesters.SingleOrDefault(s => (s.StartCourses<=from && to <= s.EndCourses) ||
                (from <= s.StartCourses && to >= s.StartCourses) ||
                (to >= s.EndCourses && from <= s.EndCourses));

            if (semester != null && semester.Name.Equals("WS13") && !semester.BookingEnabled)
            {
                semester.BookingEnabled = true;
                _db.SaveChanges();
            }

            return semester;
        }

        public Semester GetSemester(DateTime at)
        {
            var semester = _db.Semesters.SingleOrDefault(s => (s.StartCourses <= at && at <= s.EndCourses));
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



        public ICollection<DateTime> GetDays(string semesterName, DayOfWeek dayOfWeek)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Name.Equals(semesterName));

            if (semester != null)
                return GetDays(semesterName, dayOfWeek, semester.StartCourses);
            return new List<DateTime>();
        }

        public ICollection<DateTime> GetDays(string semesterName, DateTime firstDate)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Name.Equals(semesterName));

            if (semester != null)
                return GetDays(semesterName, firstDate.DayOfWeek, firstDate);
            return new List<DateTime>();
        }


        public ICollection<DateTime> GetDays(string semesterName, DayOfWeek dayOfWeek, DateTime start)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Name.Equals(semesterName));

            var dates = new List<DateTime>();


            if (semester == null)
                return dates;

            DateTime firstDate = start > semester.StartCourses ? start : semester.StartCourses;
            
            var semesterStartTag = (int)((DateTime)firstDate).DayOfWeek;

            var nDays = (int)dayOfWeek - semesterStartTag;
            if (nDays < 0)
                nDays += 7;

            var occDate = firstDate.AddDays(nDays);

            //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
            while (occDate <= semester.EndCourses)
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



        public SemesterSubscription GetSubscription(Semester semester, string userId)
        {
            var sub = _db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semester.Id);

            return sub;
        }
    }
}
