﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
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

        /// <summary>
        /// Liefert das aktuelle Semester
        /// Kriterium: aktuelles Datum liegt in der Vorlesungszeit
        /// </summary>
        /// <returns></returns>
        public Semester GetCurrentSemester()
        {
            var today = DateTime.Today;
            return _db.Semesters.FirstOrDefault(s => s.StartCourses <= today && today <= s.EndCourses);
        }

        public Semester GetNextSemester()
        {
            var today = DateTime.Today;

            var sem = _db.Semesters.Where(s => s.StartCourses > today).OrderBy(s => s.StartCourses).FirstOrDefault();

            return sem;
        }

        public Semester GetNextSemester(Semester semester)
        {
            var today = semester.StartCourses;

            var sem = _db.Semesters.Where(s => s.StartCourses > today).OrderBy(s => s.StartCourses).FirstOrDefault();

            return sem;
        }


        public Semester GetPreviousSemester()
        {
            var today = DateTime.Today;
            
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



        public ICollection<DateTime> GetDays(Guid semId, DayOfWeek dayOfWeek)
        {
            var semester = _db.Semesters.SingleOrDefault(s => s.Id == semId);

            if (semester != null)
                return GetDays(semId, dayOfWeek, semester.StartCourses);
            return new List<DateTime>();
        }

        public ICollection<DateTime> GetDays(Guid semId, DateTime firstDate)
        {
            return GetDays(semId, firstDate.DayOfWeek, firstDate);
        }


        public ICollection<DateTime> GetDays(Guid semId, DayOfWeek dayOfWeek, DateTime start)
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


        public SemesterSubscription GetSubscription(Semester semester, string userId)
        {
            var sub = _db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semester.Id);

            return sub;
        }

        public ICollection<Data.Curriculum> GetActiveCurricula(Semester semester)
        {
            return 
            _db.Curricula.Where(
                x => x.Organiser.Activities.Any(a => 
                    a.SemesterGroups.Any(s => s.Semester.Id == semester.Id && s.IsAvailable))).ToList();
        }

        public ICollection<Data.Curriculum> GetActiveCurricula(ActivityOrganiser org, Semester semester)
        {
            if (org == null)
                return new List<Data.Curriculum>();

            return
            _db.Curricula.Where(
                x => x.Organiser.Id == org.Id &&
                    x.Organiser.Activities.Any(a =>
                    a.SemesterGroups.Any(s => s.Semester.Id == semester.Id && s.IsAvailable))).ToList();
        }


        public ICollection<Data.ActivityOrganiser> GetActiveOrganiser(Semester semester)
        {
            return
            _db.Organisers.Where(
                x => x.IsFaculty && x.Activities.Any(a =>
                    a.SemesterGroups.Any(s => s.Semester.Id == semester.Id && s.IsAvailable))).ToList();
        }

    }
}
