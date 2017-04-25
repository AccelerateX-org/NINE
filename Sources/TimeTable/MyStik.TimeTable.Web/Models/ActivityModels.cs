using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Models
{
    public class ActivityCurrentModel
    {
        public List<ActivityDate> CurrentDates { get; set; }

        public List<ActivityDate> CanceledDates { get; set; }

    }


    public class ActivityPlanModel
    {
        public ActivityPlanModel()
        {
            MyActivities = new List<ActivitySummary>();
            MySubscriptions = new List<ActivitySubscriptionModel>();
        }

        public List<ActivitySummary> MyActivities { get; private set; }

        public List<ActivitySubscriptionModel> MySubscriptions { get; private set; }

        public bool HasLottery { get; set; }

        public bool IsDuringLottery { get; set; }
    }

    public class ActivitySubscriptionModel
    {
        public IActivitySummary Activity { get; set; }
        public OccurrenceStateModel State { get; set; }
    }

    public interface IActivitySummary
    {
        Activity Activity { get; }

        ActivitySummary Summary { get; }

        string Name { get; }
        string TimeFrame { get; }

        string Action { get; }
        string Controller { get; }
        string Id { get; }

        string IconName { get; }

        string BannerColor { get; }

        ICollection<OccurrenceSubscription> Subscriptions { get; }

        ICollection<ActivityDate> GetDates(DateTime start, DateTime end);

        string NextDateTime { get; }

   }

    public class ActivitySummary : IActivitySummary
    {
        public ActivitySummary()
        {
        }

        public ActivitySummary(Activity act)
        {
            Activity = act;
        }

        public ActivitySummary Summary
        {
            get
            {
                return new ActivitySummary(Activity);
            }
        }

        public Activity Activity { get; set; }
        public string Name
        {
            get { return Activity.Name; }
        }

        public string TimeFrame
        {
            get { return ""; }
        }


        public string Action
        {
            get
            {
                if (Activity is OfficeHour)
                    return "Lecturer";
                return "Index";
            }
        }

        public string Controller
        {
            get
            {
                if (Activity is Course)
                    return "Course";
                if (Activity is Newsletter)
                    return "Newsletter";
                if (Activity is OfficeHour)
                    return "OfficeHour";
                if (Activity is Event)
                    return "Event";
                if (Activity is Reservation)
                    return "Reservation";
                return string.Empty;
            }
        }

        public string Id
        {
            get
            {
                if (Activity != null)
                {
                    if (Activity is OfficeHour)
                    {
                        var oh = Activity as OfficeHour;
                        var date = oh.Dates.FirstOrDefault();
                        if (date != null)
                        {
                            var host = date.Hosts.FirstOrDefault();
                            if (host != null)
                            {
                                return host.Id.ToString();
                            }
                        }
                    }
                    return Activity.Id.ToString();
                }
                return string.Empty;
            }
        }

        public string IconName
        {
            get
            {
                if (Activity is Course)
                    return "fa-lightbulb-o";
                if (Activity is Newsletter)
                    return "fa-envelope";
                if (Activity is OfficeHour)
                    return "fa-stethoscope";
                if (Activity is Event)
                    return "fa-tag";
                if (Activity is Reservation)
                    return "fa-check-square-o";
                return string.Empty;
            }
        }

        public string BannerColor
        {
            get
            {
                if (Activity is Course)
                    return "bg-fillter-lecturer";
                if (Activity is Newsletter)
                    return "bg-fillter-events";
                if (Activity is OfficeHour)
                    return "bg-fillter-lecturer";
                if (Activity is Event)
                    return "bg-fillter-events";
                if (Activity is Reservation)
                    return "bg-fillter-rooms";
                return string.Empty;
            }
        }


        public ICollection<OccurrenceSubscription> Subscriptions
        {
            get { return Activity.Occurrence.Subscriptions; }
        }

        public ICollection<ActivityDate> GetDates(DateTime start, DateTime end)
        {
            return Activity.Dates.Where(d => d.Begin >= start && d.End <= end).ToList();
        }


        public CourseDateStateModel CurrentDate { get; set; }

        public CourseDateStateModel NextDate { get; set; }

        public string Details
        {
            get
            {
                if (Activity is Course)
                {
                    var course = Activity as Course;

                    var sb = new StringBuilder();

                    foreach (var semesterGroup in course.SemesterGroups)
                    {
                        sb.AppendFormat("{0}", semesterGroup.FullName);
                        if (semesterGroup != course.SemesterGroups.Last())
                            sb.Append(", ");
                    }
                    return sb.ToString();
                }

                return "";
            }
        }



        public string NextDateTime
        {
            get
            {
                var sb = new StringBuilder();
                var nextDate =
                    Activity.Dates.Where(d => d.Begin >= GlobalSettings.Now)
                        .OrderBy(d => d.Begin)
                        .FirstOrDefault();
                if (nextDate != null)
                {
                    sb.AppendFormat("{0} [{1} - {2}] ",
                        nextDate.Begin.ToShortDateString(),
                        nextDate.Begin.ToShortTimeString(),
                        nextDate.End.ToShortTimeString());
                    return sb.ToString();
                }
                else
                {
                    return "Keine Termine";
                }
            }
        }

    }


    public class ActivityDateSummary : IActivitySummary
    {
        public ActivityDateSummary()
        {
            
        }

        public ActivityDateSummary(ActivityDate date, ActivityDateType dateType)
        {
            Date = date;
            DateType = dateType;
        }

        public Activity Activity
        {
            get { return Date.Activity; }
        }

        public ActivitySummary Summary
        {
            get
            {
                return new ActivitySummary(Activity);
            }
        }


        public ActivityDateType DateType { get; set; }


        public ActivityDate Date { get; set; }

        public string Name
        {
            get
            {
                var sb = new StringBuilder();
                if (Date.Activity != null)
                {
                    if (Date.Activity is Reservation)
                    {
                        sb.AppendFormat("{0} ({1})", Date.Description, Date.Activity.Organiser.ShortName);
                    }
                    else
                    {
                        sb.Append(Date.Activity.Name);
                        if (!string.IsNullOrEmpty(Date.Title))
                        {
                            sb.AppendFormat(" ({0})", Date.Title);
                        }
                    }
                }
                else
                {
                    sb.Append("N.N.");
                }

                return sb.ToString();
            }
        }

        public string ShortName
        {
            get
            {
                var sb = new StringBuilder();
                if (Date.Activity != null)
                {
                    if (Date.Activity is Reservation)
                    {
                        sb.AppendFormat("{0} ({1})", Date.Activity.Name, Date.Activity.Organiser.ShortName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Date.Activity.ShortName))
                        {
                            sb.Append("N.N.");
                        }
                        else
                        {
                            sb.Append(Date.Activity.ShortName);
                        }
                    }
                }
                else
                {
                    sb.Append("N.N.");
                }

                if (!string.IsNullOrEmpty(Date.ShortName))
                {
                    sb.AppendFormat(" ({0})", Date.ShortName);
                }
                
                return sb.ToString();
            }
        }

        public string TimeFrame
        {
            get { return string.Format("{0} - {1}",  Date.Begin, Date.End); }
        }

        public string Action
        {
            get { return new ActivitySummary(Date.Activity).Action; }
        }

        public string Controller
        {
            get
            {
                return new ActivitySummary(Date.Activity).Controller;
            }
        }

        public string Id
        {
            get { return new ActivitySummary(Date.Activity).Id; }
        }

        public string IconName
        {
            get { return new ActivitySummary(Date.Activity).IconName; }
        }


        public string BannerColor
        {
            get { return new ActivitySummary(Date.Activity).BannerColor; }
        }



        public ICollection<OccurrenceSubscription> Subscriptions
        {
            get
            {
                if (Date.Slots.Count > 0)
                {
                    var subscriptions = new List<OccurrenceSubscription>();
                    foreach (var slot in Date.Slots)
                    {
                        subscriptions.AddRange(slot.Occurrence.Subscriptions);
                    }
                    return subscriptions;
                }

                if (!Date.Occurrence.Subscriptions.Any())
                {
                    return Date.Activity.Occurrence.Subscriptions;
                }
                
                return Date.Occurrence.Subscriptions;
            }
        }

        public ICollection<ActivityDate> GetDates(DateTime start, DateTime end)
        {
            var dates = new List<ActivityDate>();

            if (Date.Begin >= start && Date.End <= end)
                dates.Add(Date);

            return dates;
        }

        public string TextColor
        {
            get
            {
                /*
                switch (DateType)
                {
                   case ActivityDateType.Offer:
                   case ActivityDateType.SearchResult:
                        return "#FFF";
                    default:
                        return "#000";
                }
                 */
                return "#000";
            }
        }

        public string BackgroundColor
        {
            get
            {
                // Keine Occurrence => war mal ein Fehler, jetzt gibt es Raumreservierungen
                if (Date.Occurrence == null)
                {
                    if (Date.Activity is Reservation)
                        return "#CCC";
                    return "#F00";
                }

                

                // wenn der Termin ausfällt, immer roter Hintergrund
                if (Date.Occurrence.IsCanceled)
                    return "#FFB5C5"; 

                switch (DateType)
                {
                       
                    case ActivityDateType.Offer:
                        // Eigene Angebote: weiss
                        return "#FFF";
                    case ActivityDateType.SearchResult:
                        // Suchergebnisse: grau
                        return "#CCC";
                    case ActivityDateType.Subscription:
                        // Eintragungen: grün = findet statt
                        return "#ffffb3";
                    
                    default:
                        return "#FFF";
                
                }
            }
        }



        public string NextDateTime
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0} [{1} - {2}] ",
                    Date.Begin.ToShortDateString(),
                    Date.Begin.ToShortTimeString(),
                    Date.End.ToShortTimeString());
                return sb.ToString();
            }
        }
    }

    public enum ActivityDateType
    {
        Offer,
        Subscription,
        SearchResult
    }

    public class ActivitySlotSummary : IActivitySummary
    {
        public ActivitySlot Slot { get; set; }
        public string Name
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(Slot.ActivityDate.Activity.Name);
                if (!string.IsNullOrEmpty(Slot.ActivityDate.Title))
                {
                    sb.AppendFormat(" ({0})", Slot.ActivityDate.Title);
                }
                return sb.ToString();
            }
        }

        public Activity Activity
        {
            get { return Slot.ActivityDate.Activity; }
        }

        public ActivitySummary Summary
        {
            get
            {
                return new ActivitySummary(Activity);
            }
        }


        public string TimeFrame
        {
            get
            {
                return string.Format("{0}: {1} - {2}", Slot.ActivityDate.Begin.Date, Slot.Begin, Slot.End);
            }
        }

        public string Action
        {
            get { return new ActivitySummary(Slot.ActivityDate.Activity).Action; }
        }

        public string Controller
        {
            get
            {
                return new ActivitySummary(Slot.ActivityDate.Activity).Controller;
            }
        }

        public string Id
        {
            get { return new ActivitySummary(Slot.ActivityDate.Activity).Id; }
        }

        public string IconName
        {
            get { return new ActivitySummary(Slot.ActivityDate.Activity).IconName; }
        }

        public string BannerColor
        {
            get { return new ActivitySummary(Slot.ActivityDate.Activity).BannerColor; }
        }

        public ICollection<OccurrenceSubscription> Subscriptions
        {
            get
            {
                return Slot.Occurrence.Subscriptions;
            }
        }

        public ICollection<ActivityDate> GetDates(DateTime start, DateTime end)
        {
            var dates = new List<ActivityDate>();

            if (Slot.ActivityDate.Begin >= start && Slot.ActivityDate.End <= end)
                dates.Add(Slot.ActivityDate);

            return dates;
        }




        public string NextDateTime
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0} [{1} - {2}] ",
                    Slot.Begin.ToShortDateString(),
                    Slot.Begin.ToShortTimeString(),
                    Slot.End.ToShortTimeString());
                return sb.ToString();
            }
        }
    }
}