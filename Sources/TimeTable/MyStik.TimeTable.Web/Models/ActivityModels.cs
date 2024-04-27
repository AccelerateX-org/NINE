using System.Text;
using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityCurrentModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> CurrentDates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> CanceledDates { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ActivityPlanModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityPlanModel()
        {
            MyActivities = new List<ActivitySummary>();
            MySubscriptions = new List<ActivitySubscriptionModel>();
            Courses = new List<CourseSummaryModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySummary> MyActivities { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySubscriptionModel> MySubscriptions { get; }

        public ActivityOrganiser Organiser { get; set; }

        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasLottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDuringLottery { get; set; }

        public List<CourseSummaryModel> Courses { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ActivitySubscriptionModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary Activity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IActivitySummary
    {
        /// <summary>
        /// 
        /// </summary>
        Activity Activity { get; }

        /// <summary>
        /// 
        /// </summary>
        ActivitySummary Summary { get; }

        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        string TimeFrame { get; }

        /// <summary>
        /// 
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 
        /// </summary>
        string Controller { get; }

        /// <summary>
        /// 
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 
        /// </summary>
        string IconName { get; }

        /// <summary>
        /// 
        /// </summary>
        string BannerColor { get; }

        /// <summary>
        /// 
        /// </summary>
        ICollection<OccurrenceSubscription> Subscriptions { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        ICollection<ActivityDate> GetDates(DateTime start, DateTime end);

        /// <summary>
        /// 
        /// </summary>
        string NextDateTime { get; }

   }

    /// <summary>
    /// 
    /// </summary>
    public class ActivitySummary : IActivitySummary
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivitySummary()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="act"></param>
        public ActivitySummary(Activity act)
        {
            Activity = act;
        }

        /// <summary>
        /// 
        /// </summary>
        public ActivitySummary Summary => new ActivitySummary(Activity);

        /// <summary>
        /// 
        /// </summary>
        public Activity Activity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Activity.Name))
                {
                    if (string.IsNullOrEmpty(Activity.ShortName))
                    {
                        return "N.N.";
                    }
                    return Activity.ShortName;
                }
                return Activity.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TimeFrame => "";

        /// <summary>
        /// 
        /// </summary>
        public string Action
        {
            get
            {
                if (Activity is OfficeHour)
                    return "OfficeHour";
                return "Index";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Controller
        {
            get
            {
                if (Activity is Course)
                    return "Course";
                if (Activity is Newsletter)
                    return "Newsletter";
                if (Activity is OfficeHour)
                    return "Lecturer";
                if (Activity is Event)
                    return "Event";
                if (Activity is Reservation)
                    return "Reservation";
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get
            {
                if (Activity != null)
                {
                    /*
                    if (Activity is OfficeHour hour)
                    {
                        var oh = hour;
                        var date = oh.Dates.FirstOrDefault();
                        var host = date?.Hosts.FirstOrDefault();
                        if (host != null)
                        {
                            return hour.Semester.Id.ToString();
                        }
                    }
                    */

                    return Activity.Id.ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OccurrenceSubscription> Subscriptions
        {
            get
            {
                if (Activity.Occurrence != null)
                {
                    return Activity.Occurrence.Subscriptions;
                }
                else
                {
                    return new List<OccurrenceSubscription>();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ICollection<ActivityDate> GetDates(DateTime start, DateTime end)
        {
            return Activity.Dates.Where(d => d.Begin >= start && d.End <= end).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        public CourseDateStateModel CurrentDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseDateStateModel NextDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Details
        {
            get
            {
                var activity = Activity as Course;
                if (activity != null)
                {
                    var course = activity;

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


        /// <summary>
        /// 
        /// </summary>
        public string NextDateTime
        {
            get
            {
                var sb = new StringBuilder();
                var nextDate =
                    Activity.Dates.Where(d => d.Begin >= DateTime.Now)
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

    /// <summary>
    /// 
    /// </summary>
    public class ActivityDateSummary : IActivitySummary
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityDateSummary()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateType"></param>
        public ActivityDateSummary(ActivityDate date, ActivityDateType dateType)
        {
            Date = date;
            DateType = dateType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateType"></param>
        public ActivityDateSummary(ActivityDate date, OccurrenceSubscription sub)
        {
            Date = date;
            DateType = ActivityDateType.Subscription;
            Subscription = sub;
        }


        /// <summary>
        /// 
        /// </summary>
        public Activity Activity => Date.Activity;

        /// <summary>
        /// 
        /// </summary>
        public ActivitySummary Summary => new ActivitySummary(Activity);

        /// <summary>
        /// 
        /// </summary>
        public ActivityDateType DateType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivitySlot Slot { get; set; }


        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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
                        sb.Append(string.IsNullOrEmpty(Date.Activity.ShortName) ? "N.N." : Date.Activity.ShortName);
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

        /// <summary>
        /// 
        /// </summary>
        public string TimeFrame => $"{Date.Begin} - {Date.End}";

        /// <summary>
        /// 
        /// </summary>
        public string Action => new ActivitySummary(Date.Activity).Action;

        /// <summary>
        /// 
        /// </summary>
        public string Controller => new ActivitySummary(Date.Activity).Controller;

        /// <summary>
        /// 
        /// </summary>
        public string Id => new ActivitySummary(Date.Activity).Id;

        /// <summary>
        /// 
        /// </summary>
        public string IconName => new ActivitySummary(Date.Activity).IconName;

        /// <summary>
        /// 
        /// </summary>
        public string BannerColor => new ActivitySummary(Date.Activity).BannerColor;


        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ICollection<ActivityDate> GetDates(DateTime start, DateTime end)
        {
            var dates = new List<ActivityDate>();

            if (Date.Begin >= start && Date.End <= end)
                dates.Add(Date);

            return dates;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TextColor => "#000";

        /// <summary>
        /// 
        /// </summary>
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

                if (Subscription == null)
                {
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
                else
                {
                    if (Subscription.OnWaitingList)
                    {
                        return "#c89f23";
                    }
                    else
                    {
                        return "#37918b";
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public enum ActivityDateType
    {
        /// <summary>
        /// 
        /// </summary>
        Offer,
        /// <summary>
        /// 
        /// </summary>
        Subscription,
        /// <summary>
        /// 
        /// </summary>
        SearchResult
    }

    /// <summary>
    /// 
    /// </summary>
    public class ActivitySlotSummary : IActivitySummary
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivitySlot Slot { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public Activity Activity => Slot.ActivityDate.Activity;

        /// <summary>
        /// 
        /// </summary>
        public ActivitySummary Summary => new ActivitySummary(Activity);


        /// <summary>
        /// 
        /// </summary>
        public string TimeFrame => $"{Slot.ActivityDate.Begin.Date}: {Slot.Begin} - {Slot.End}";

        /// <summary>
        /// 
        /// </summary>
        public string Action => new ActivitySummary(Slot.ActivityDate.Activity).Action;

        /// <summary>
        /// 
        /// </summary>
        public string Controller => new ActivitySummary(Slot.ActivityDate.Activity).Controller;

        /// <summary>
        /// 
        /// </summary>
        public string Id => new ActivitySummary(Slot.ActivityDate.Activity).Id;

        /// <summary>
        /// 
        /// </summary>
        public string IconName => new ActivitySummary(Slot.ActivityDate.Activity).IconName;

        /// <summary>
        /// 
        /// </summary>
        public string BannerColor => new ActivitySummary(Slot.ActivityDate.Activity).BannerColor;

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OccurrenceSubscription> Subscriptions => Slot.Occurrence.Subscriptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ICollection<ActivityDate> GetDates(DateTime start, DateTime end)
        {
            var dates = new List<ActivityDate>();

            if (Slot.ActivityDate.Begin >= start && Slot.ActivityDate.End <= end)
                dates.Add(Slot.ActivityDate);

            return dates;
        }



        /// <summary>
        /// 
        /// </summary>
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

    public class ActivitySemesterViewModel
    {
        public Semester Semester { get; set; }

        public ICollection<Course> Courses { get; set; }

    }
}