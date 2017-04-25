using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.DefaultData;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    [Authorize(Roles = "SysAdmin")]
    public class DBAdmimController : BaseController
    {
        //
        // GET: /DBAdmim/
        public ActionResult Index()
        {
            var model = new ActivityAnalysisModel();

            model.CourseCount = Db.Activities.OfType<Course>().Count();
            model.CourseDateCount = Db.ActivityDates.Count(d => d.Activity is Course);

            model.OfficeHourCount = Db.Activities.OfType<OfficeHour>().Count();
            model.OfficeHourDateCount = Db.ActivityDates.Count(d => d.Activity is OfficeHour);
            model.OfficeHourSlotCount = Db.ActivitySlots.Count(d => d.ActivityDate.Activity is OfficeHour);

            model.EventCount = Db.Activities.OfType<Event>().Count();
            model.EventDateCount = Db.ActivityDates.Count(d => d.Activity is Event);

            model.EventCount = Db.Activities.OfType<Newsletter>().Count();

            model.OccurrenceCount = Db.Occurrences.Count();

            model.SubscriptionCount = Db.Subscriptions.Count();

            model.DanglingSubscriptionCount = Db.Subscriptions.OfType<OccurrenceSubscription>().Count(s => s.Occurrence == null);
            
            return View(model);
        }

        public ActionResult DeleteDanglingSubscriptions()
        {
            var subs = Db.Subscriptions.OfType<OccurrenceSubscription>().Where(s => s.Occurrence == null).ToList();

            foreach (var sub in subs)
            {
                Db.Subscriptions.Remove(sub);
            }
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Initialisiert eine Datenbank mit Testdaten
        /// </summary>
        /// <returns></returns>
        public ActionResult InitDatabase()
        {
            var ds = new InfrastructureDataService();
            ds.InitData();
             
            return RedirectToAction("Index");
        }

        public ActionResult ClearDatabase()
        {
            var cs = Db.Curricula.ToList();

            foreach (var c in cs)
            {
                var gs = c.CurriculumGroups.ToList();
                foreach (var g in gs)
                {
                    var sgs = g.SemesterGroups.ToList();
                    foreach (var sg in sgs)
                    {
                        var subs = sg.Subscriptions.ToList();
                        foreach (var sub in subs)
                        {
                            sg.Subscriptions.Remove(sub);
                            Db.Subscriptions.Remove(sub);
                        }

                        var occs = sg.OccurrenceGroups.ToList();
                        foreach (var oc in occs)
                        {
                            sg.OccurrenceGroups.Remove(oc);
                            Db.OccurrenceGroups.Remove(oc);
                        }
                        
                        g.SemesterGroups.Remove(sg);
                        Db.SemesterGroups.Remove(sg);
                    }
                    
                    c.CurriculumGroups.Remove(g);
                    Db.CurriculumGroups.Remove(g);
                }

                var alias = c.GroupAliases.ToList();
                foreach (var a in alias)
                {
                    c.GroupAliases.Remove(a);
                    Db.GroupAliases.Remove(a);
                }

                Db.Curricula.Remove(c);
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }
    
    }
}