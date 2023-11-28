using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    /*
    [System.Web.Http.RoutePrefix("api/v2/agenda")]
    public class AgendaController : ApiBaseController
    {


        /// <summary>
        /// 
        /// </summary>
        protected AgendaController()
        {
        }


        //Alle eigenen Termine in Zeitraum
        /// <summary>
        /// Abfrage aller eigenen Termine im gewünschten Zeitraum
        /// </summary>
        /// <param name="UserId">Die UserId des Accounts in der Datenbank</param>
        /// <param name="From">Anfangsdatum  des Zeitraums im Format dd.MM.yyyy</param>
        /// <param name="Until">Enddatum des Zeitraums im Format dd.MM.yyyy</param>
        /// <returns>Persönlichen Termine für jeden Tag im gewählten Zeitraum</returns>
        [System.Web.Http.Route("dates")]
        public PersonalPlanResponse GetPersonalDatesSpan(string UserId, string From, string Until)
        {
            var From2 = DateTime.ParseExact(From, "MM-dd-yyyy", null);
            var Until2 = DateTime.ParseExact(Until, "MM-dd-yyyy", null);
            var activityService = new ActivityInfoService();

            var activityList = activityService.GetPersonalDates(UserId, From2, Until2);

            var response = new PersonalPlanResponse
            {
                Courses = activityList,
            };

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Date"></param>
        /// <returns></returns>
        [System.Web.Http.Route("agenda")]
        public List<CalendarEntry> GetCalendar(string UserId, string Date)
        {
            var day = DateTime.Parse(Date);
            var startDate = day.Date;
            var endDate = startDate.AddDays(1);

            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            var db = new TimeTableDbContext();


                // Was der User anbietet
                // Termine als Host
                var allMyDates = db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(UserId))).ToList();

                foreach (var date in allMyDates)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                }

                // 2. die gebuchten
                var myOcs = db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(UserId))).ToList();

                var ac = new ActivityService();

                foreach (var occ in myOcs)
                {
                    var summary = ac.GetSummary(occ.Id);

                    var dates = summary.GetDates(startDate, endDate);

                    foreach (var date in dates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            var dateSummary = new ActivityDateSummary(date, ActivityDateType.Subscription);
                            if (summary is ActivitySlotSummary slotSummary)
                            {
                                dateSummary.Slot = slotSummary.Slot;
                            }
                            dateMap[date.Id] = dateSummary;

                        }
                    }
                }

            var alleDates = dateMap.Values.ToList();


            var model = new List<CalendarEntry>();

            foreach (var date in alleDates)
            {
                var cal = new CalendarEntry();

                cal.begin = date.Date.Begin.ToString("yyyy-MM-ddTHH:mm:ssZ");
                cal.end = date.Date.End.ToString("yyyy-MM-ddTHH:mm:ssZ");
                cal.title = date.Activity.Name;

                model.Add(cal);
            }


            return model;
        }
    }
    */
}
