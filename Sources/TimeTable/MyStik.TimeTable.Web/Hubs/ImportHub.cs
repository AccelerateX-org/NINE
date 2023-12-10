using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.Web.Hubs
{
    public class ImportHub : Hub
    {
        /// <summary>
        /// Löscht alle Kurse aus dem angegebenen Semester, die aus gpUntis importiert wurden
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        public void DeleteSemester(Guid semId, Guid orgId)
        {
            var db = new TimeTableDbContext();

            var semService = new SemesterService();
            var timeTableService = new TimeTableInfoService(db);

            var msg = "Sammle Daten";
            var perc1 = 0;

            Clients.Caller.updateProgress(msg, perc1);

            var org = db.Organisers.SingleOrDefault(x => x.Id == orgId);
            if (org == null)
            {
                msg = "Einrichtung existiert nicht";
                perc1 = 100;

                Clients.Caller.updateProgress(msg, perc1);
                return;
            }

            var semester = semService.GetSemester(semId);
            if (semester == null)
            {
                msg = "Semester existiert nicht";
                perc1 = 100;

                Clients.Caller.updateProgress(msg, perc1);
                return;
            }

            var courses = db.Activities.OfType<Course>().Where(c =>
                c.Organiser.Id == org.Id &&
                c.Semester.Id == semester.Id &&
                !string.IsNullOrEmpty(c.ExternalSource)).ToList();

            msg = string.Format("Lösche {0} von {1} Kursen", courses.Count, courses.Count);
            perc1 = 0;
            Clients.Caller.updateProgress(msg, perc1);

            var n = courses.Count;
            var i = 0;
            foreach (var course in courses)
            {
                i++;
                msg = string.Format("Lösche {0}", course.Name);
                perc1 = (i * 100) / n;

                Clients.Caller.updateProgress(msg, perc1);

                timeTableService.DeleteCourse(course.Id);
            }


            msg = "Alle Kurse gelöscht";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);
        }
    }
}