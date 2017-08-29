using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// JSON-Model zur Kommunikation mit Kalender-Komponente
    /// 
    /// Referenz: http://arshaw.com/fullcalendar/docs/event_data/Event_Object/
    /// </summary>
    public class CalendarEventModel
    {
        /// <summary>
        /// Uniquely identifies the given event. Different instances of repeating events should all have the same id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text on an event's element.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The date/time an event begins.
        /// </summary>
        public string start { get; set; }

        /// <summary>
        /// The date/time an event ends.
        /// 
        /// If an event is all-day...
        /// the end date is inclusive. This means an event with start Nov 10 and end Nov 12 will span 3 days on the calendar.
        /// 
        /// If an event is NOT all-day...
        /// the end date is exclusive. This is only a gotcha when your end has time 00:00. It means your event ends on midnight, and it will not span through the next day.
        /// </summary>
        public string end { get; set; }

        /// <summary>
        /// Whether an event occurs at a specific time-of-day. This property affects whether an event's time is shown. 
        /// Also, in the agenda views, determines if it is displayed in the "all-day" section.
        /// </summary>
        public bool allDay { get; set; }

        /// <summary>
        /// A URL that will be visited when this event is clicked by the user.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// A CSS class (or array of classes) that will be attached to this event's element.
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// Overrides the master editable option for this single event.
        /// </summary>
        public bool editable { get; set; }

        /// <summary>
        /// Sets an event's background and border color just like the calendar-wide eventColor option.
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// Sets an event's background color just like the calendar-wide eventBackgroundColor option.
        /// </summary>
        public string backgroundColor { get; set; }

        /// <summary>
        /// Sets an event's border color just like the the calendar-wide eventBorderColor option.
        /// </summary>
        public string borderColor { get; set; }

        /// <summary>
        /// Sets an event's text color just like the calendar-wide eventTextColor option.
        /// </summary>
        public string textColor { get; set; }

        /// <summary>
        /// Toolbar
        /// </summary>
        public string htmlToolbar { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string htmlToolbarInfo { get; set; }

        /// <summary>
        /// Inhalt
        /// </summary>
        public string htmlContent { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CalenderEventViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityDateSummary Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CalenderEventPrintViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Room> Rooms { get; set; }
    }

}