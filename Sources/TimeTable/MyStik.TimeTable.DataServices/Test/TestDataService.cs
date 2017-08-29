using System;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Test
{
    public class TestDataService
    {
        private TimeTableDbContext _db;

        public TestDataService(TimeTableDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 3 Semester (aktuell, letztes, nächstes)
        /// </summary>
        public void InitSemester()
        {
            var semService = new SemesterService();

            var currentSemester = semService.GetCurrentSemester() ?? CreateCurrentSemester();
            var prevSemester = semService.GetPreviousSemester() ?? CreatePreviousSemester();
            var nextSemester = semService.GetNextSemester() ?? CreateNextSemester();
        }

        private Semester CreateCurrentSemester()
        {
            var sem = new Semester();
            if (DateTime.Today.Month >= 3 && DateTime.Today.Month < 10)
            {
                var date = new DateTime(DateTime.Today.Year, 3, 15);
                // heute liegt im SS
                sem.Name = string.Format("SS{0}", DateTime.Today.Year - 2000);
                sem.StartCourses = date;
                sem.EndCourses = date.AddDays(120);
            }
            else
            {
                // offenbar im WS => das nächste ist das SS
                if (DateTime.Today.Month < 3) // "dieses Jahr"
                {
                    sem.Name = string.Format("WS{0}", DateTime.Today.Year - 2001);
                    sem.StartCourses = new DateTime(DateTime.Today.AddYears(-1).Year, 10, 1);
                }
                else
                {
                    // nächstes Jahr
                    sem.Name = string.Format("WS{0}", DateTime.Today.Year - 2000);
                    sem.StartCourses = new DateTime(DateTime.Today.Year, 10, 1);
                }
                sem.EndCourses = sem.StartCourses.AddDays(120);
            }

            _db.Semesters.Add(sem);
            _db.SaveChanges();
            return sem;
        }
        private Semester CreateNextSemester()
        {
            var sem = new Semester();
            if (DateTime.Today.Month >= 3 && DateTime.Today.Month < 10)
            {
                var date = new DateTime(DateTime.Today.Year, 10, 1);
                // heute liegt im SS => das nächste ist das WS
                sem.Name = string.Format("WS{0}", DateTime.Today.Year - 2000);
                sem.StartCourses = date;
                sem.EndCourses = date.AddDays(120);
            }
            else
            {
                var date = new DateTime(DateTime.Today.Year, 3, 15);
                // offenbar im WS => das nächste ist das SS
                if (DateTime.Today.Month < 3) // "dieses Jahr"
                {
                    sem.Name = string.Format("SS{0}", DateTime.Today.Year - 2000);
                }
                else
                {
                    // nächstes Jahr
                    sem.Name = string.Format("SS{0}", DateTime.Today.Year - 1999);
                    date = date.AddYears(1);
                }
                sem.StartCourses = date;
                sem.EndCourses = date.AddDays(120);
            }
            _db.Semesters.Add(sem);
            _db.SaveChanges();
            return sem;
        }

        private Semester CreatePreviousSemester()
        {
            var sem = new Semester();
            if (DateTime.Today.Month >= 3 && DateTime.Today.Month < 10)
            {
                var date = new DateTime(DateTime.Today.Year - 1, 10, 1);
                // heute liegt im SS => das vorhergehende ist das WS im Vorjahr
                sem.Name = string.Format("WS{0}", DateTime.Today.Year - 2001);
                sem.StartCourses = date;
                sem.EndCourses = date.AddDays(120);
            }
            else
            {
                // offenbar im WS => das vorhergehende ist das SS
                var date = new DateTime(DateTime.Today.Year, 3, 15);
                if (DateTime.Today.Month < 3) // "letztes Jahr"
                {
                    sem.Name = string.Format("SS{0}", DateTime.Today.Year - 1999);
                    date = date.AddYears(1);
                }
                else
                {
                    // dieses Jahr
                    sem.Name = string.Format("SS{0}", DateTime.Today.Year - 2000);
                }
                sem.StartCourses = date;
                sem.EndCourses = date.AddDays(120);
            }
            _db.Semesters.Add(sem);
            _db.SaveChanges();
            return sem;
        }

        public ActivityOrganiser InitOrganiser(string shortName, string Name)
        {
            var org = _db.Organisers.SingleOrDefault(x => x.ShortName.Equals(shortName));

            if (org == null)
            {
                org = new ActivityOrganiser
                {
                    ShortName = shortName,
                    Name = Name,
                    IsFaculty = true,
                    IsStudent = false
                };

                _db.Organisers.Add(org);
                _db.SaveChanges();
            }

            return org;
        }
    }
}
