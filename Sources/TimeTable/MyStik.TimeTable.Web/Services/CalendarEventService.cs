using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Services
{
    public class CalendarEventService
    {
        private string _calDateFormatCalendar = "yyyy-MM-ddTHH:mm:ss";


        public IEnumerable<CalendarEventModel> GetCalendarEvents(IEnumerable<ActivityDateSummary> dates)
        {
            var events = new List<CalendarEventModel>();

            foreach (var date in dates)
            {
                    // Workaround für fullcalendar
                    // wenn der Kalendereintrag ein zu geringe höhe hat,
                    // dann wird statt des Endes der Titel angezeigt
                    // Den Titel rendern wir selber, d.h. i.d.R. geben wir ihn nicht an!
                    // file: fullcalendar.js line 3945
                    /*
                    var duration = date.Date.End - date.Date.Begin;
                        if (duration.TotalMinutes <= 60)
                            string title = null;
                            title = date.Date.End.TimeOfDay.ToString(@"hh\:mm");
                     */

                    if (date.Slot != null)
                    {
                        events.Add(new CalendarEventModel
                        {
                            id = date.Date.Id.ToString(),
                            courseId = date.Activity.Id.ToString(),
                            title = date.Activity.Name,
                            allDay = false,
                            start = date.Date.Begin.ToString(_calDateFormatCalendar),
                            end = date.Date.End.ToString(_calDateFormatCalendar),
                            textColor = date.TextColor,
                            backgroundColor = date.BackgroundColor,
                            borderColor = "#ff0000",
                            htmlContent = String.Empty,
                        });
                    }
                    else
                    {
                        events.Add(new CalendarEventModel
                        {
                            id = date.Date.Id.ToString(),
                            courseId = date.Activity.Id.ToString(),
                            title = date.Activity.Name,
                            allDay = false,
                            start = date.Date.Begin.ToString(_calDateFormatCalendar),
                            end = date.Date.End.ToString(_calDateFormatCalendar),
                            textColor = date.TextColor,
                            backgroundColor = date.BackgroundColor,
                            borderColor = "#000",
                            htmlContent = String.Empty,
                        });
                    }
            }


            return events;
        }


    }
}