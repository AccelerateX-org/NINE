using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OccurrenceController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult SubscriptionList(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var logger = LogManager.GetLogger("SubscriptionList");
            logger.InfoFormat("List for [{0}]", summary.Activity.Name);


            var semester = GetSemester();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write(
                "Name;Vorname;E-Mail;Studiengruppe;Status");

            writer.Write(Environment.NewLine);

            foreach (var subscription in summary.Subscriptions.OrderBy(s => s.TimeStamp))
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    // Aktuelle Semestergruppe aus SemesterSubscription ermitteln
                    var semSub =
                    Db.Subscriptions.OfType<SemesterSubscription>()
                        .FirstOrDefault(s => s.UserId.Equals(user.Id) && s.SemesterGroup.Semester.Id == semester.Id);

                    var group = "keine Angabe";

                    if (semSub != null)
                    {
                        group = semSub.SemesterGroup.FullName;
                    }

                    string subState;
                    if (subscription.OnWaitingList)
                    {
                        subState = "Warteliste";
                    }
                    else
                    {
                        subState = subscription.IsConfirmed ? "Teilnehmer" : "Teilnehmer (unbestätigt)";
                    }


                    writer.Write("{0};{1};{2};{3};{4}",
                        user.LastName, user.FirstName, user.Email,
                        group,
                        subState);
                    writer.Write(Environment.NewLine);
                }
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Teilnehmer_");
            sb.Append(summary.Activity.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubscriptionRules(Guid id)
        {

            var model = new SubscriptionRuleModel();

            var summary = ActivityService.GetSummary(id);
            model.Summary = summary;

            var occurrence = Db.Occurrences.SingleOrDefault(occ => occ.Id == id);

            model.OccurrenceId = occurrence.Id;

            // für Slots gilt: Es gelten die Kapazitätsbeschränkungen des Slots
            var capacity = occurrence.Capacity;
            if (summary is ActivityDateSummary)
            {
                var dateSummary = summary as ActivityDateSummary;
                if (dateSummary.Date.Slots.Any())
                {
                    capacity = dateSummary.Date.Slots.First().Occurrence.Capacity;
                }
            }


            model.Capacity = capacity;
            model.CapacityOption = capacity > 0 ? 1 : capacity;


            model.SubscriptionBegin = DateTime.Today.ToShortDateString();
            model.SubscriptionBeginTime = "18:00";

            if (occurrence.FromIsRestricted)
            {
                if (occurrence.FromDateTime.HasValue)
                {
                    model.SubscriptionBeginLimitOption = -1;
                    model.SubscriptionBegin = occurrence.FromDateTime.Value.ToShortDateString();
                    model.SubscriptionBeginTime = occurrence.FromDateTime.Value.ToString("t");
                }
                if (occurrence.FromTimeSpan.HasValue)
                {
                    if (occurrence.FromTimeSpan.Value.Hours == 12)
                    {
                        model.SubscriptionBeginLimitOption = 12;
                    }
                    else
                    {
                        model.SubscriptionBeginLimitOption = 24;
                    }
                }
            }
            else
            {
                model.SubscriptionBeginLimitOption = 0;
            }



            model.SubscriptionEnd = DateTime.Today.ToShortDateString();
            model.SubscriptionEndTime = "18:00";

            if (occurrence.UntilIsRestricted)
            {
                if (occurrence.UntilDateTime.HasValue)
                {
                    model.SubscriptionEndLimitOption = -1;
                    model.SubscriptionEnd = occurrence.UntilDateTime.Value.ToShortDateString();
                    model.SubscriptionEndTime = occurrence.UntilDateTime.Value.ToString("t");
                }
                if (occurrence.UntilTimeSpan.HasValue)
                {
                    if (occurrence.UntilTimeSpan.Value.Hours == 12)
                    {
                        model.SubscriptionEndLimitOption = 12;
                    }
                    else
                    {
                        model.SubscriptionEndLimitOption = 24;
                    }
                }
            }
            else
            {
                model.SubscriptionEndLimitOption = 0;
            }





            SetTimeSelections();
            SetRestrictionSelections();


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubscriptionRules(SubscriptionRuleModel model)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == model.OccurrenceId);

            var capacity = model.CapacityOption > 0 ? model.Capacity : model.CapacityOption;
            
            // in der occurrence speichern ist nie verkehrt
            occurrence.Capacity = capacity;

            // und jetzt zurück in die slots, falls vorhanden
            var summary = ActivityService.GetSummary(model.OccurrenceId);
            if (summary is ActivityDateSummary)
            {
                var dateSummary = summary as ActivityDateSummary;
                foreach (var slot in dateSummary.Date.Slots)
                {
                    slot.Occurrence.Capacity = capacity;
                }
            }

            switch (model.SubscriptionBeginLimitOption)
            {
                case -1:        // Absolute Beschränkung
                    occurrence.FromIsRestricted = true;

                    var day = DateTime.Parse(model.SubscriptionBegin);
                    var time = DateTime.Parse(model.SubscriptionBeginTime);
                    var begin = day.Add(time.TimeOfDay);
                    occurrence.FromDateTime = begin;
                    occurrence.FromTimeSpan = null;
                    break;
                case 0:         // keine Beschränkung
                    occurrence.FromIsRestricted = false;
                    occurrence.FromDateTime = null;
                    occurrence.FromTimeSpan = null;
                    break;
                case 12:        // relative Beschränkung 12h
                case 24:        // relative Beschränkung 24h
                    occurrence.FromIsRestricted = true;
                    occurrence.FromDateTime = null;
                    occurrence.FromTimeSpan = new TimeSpan(model.SubscriptionBeginLimitOption - 1, 59, 0);
                    break;
            }


            switch (model.SubscriptionEndLimitOption)
            {
                case -1:        // Absolute Beschränkung
                    occurrence.UntilIsRestricted = true;

                    var day = DateTime.Parse(model.SubscriptionEnd);
                    var time = DateTime.Parse(model.SubscriptionEndTime);
                    var end = day.Add(time.TimeOfDay);

                    occurrence.UntilDateTime = end;
                    occurrence.UntilTimeSpan = null;
                    break;
                case 0:         // keine Beschränkung
                    occurrence.UntilIsRestricted = false;
                    occurrence.UntilDateTime = null;
                    occurrence.UntilTimeSpan = null;
                    break;
                case 12:        // relative Beschränkung 12h
                case 24:        // relative Beschränkung 24h
                    occurrence.UntilIsRestricted = true;
                    occurrence.UntilDateTime = null;
                    occurrence.UntilTimeSpan = new TimeSpan(model.SubscriptionEndLimitOption - 1, 59, 0);
                    break;
            }


            Db.SaveChanges();

            return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
        }

        private void SetRestrictionSelections()
        {
            var cap = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Keine Beschränkung", Value = -1},
                new SelectionHelper {Text = "Keine Eintragung erlauben", Value = 0},
                new SelectionHelper {Text = "Anzahl Plätze festlegen", Value = 1},
            };

            var limits = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Keine Beschränkung", Value = 0},
                new SelectionHelper {Text = "Beschränkung zu Datum / Uhrzeit", Value = -1},
                new SelectionHelper {Text = "jeweils 12 h vor Beginn des Termins", Value = 12},
                new SelectionHelper {Text = "jeweils 24 h vor Beginn des Termins", Value = 24},
            };

            ViewBag.Capacities = new SelectList(cap, "Value", "Text", "keine Beschränkung");
            ViewBag.Limits = new SelectList(limits, "Value", "Text", "keine Beschränkung");
        }

	}
}