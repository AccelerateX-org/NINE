using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.IO.GpUntis;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class FollowUpCoursesHub : Hub
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        /// <param name="firstDate"></param>
        /// <param name="lastDate"></param>
        public void CopyCoursesOrg(Guid orgId, Guid sourceSemId, Guid targetSemId)
        {
            var db = new TimeTableDbContext();

            var org = db.Organisers.SingleOrDefault(o => o.Id == orgId);
            var sourceSemester = db.Semesters.SingleOrDefault(s => s.Id == sourceSemId);
            var targetSemester = db.Semesters.SingleOrDefault(s => s.Id == targetSemId);

            var msg = "";
            var perc1 = 0;

            if (sourceSemester == null || targetSemester == null || org == null)
            {
                msg = "Fehler";
                perc1 = 100;
                Clients.Caller.updateProgress(msg, perc1);
                return;
            }

            var courses = db.Activities.OfType<Course>().Where(x =>
                x.Organiser != null && x.Organiser.Id == orgId &&
                x.Semester != null && x.Semester.Id == sourceSemId
            ).ToList();

            var n = courses.Count;
            var i = 0;
            foreach (var course in courses)
            {
                msg = $"Kopiere \"{course.Name} ({course.ShortName})\"";
                perc1 = (i * 100) / n;
                Clients.Caller.updateProgress(msg, perc1);

                

                i++;
            }

            msg = "Alle Lehrveranstaltungen kopiert";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);
        }
    }
}