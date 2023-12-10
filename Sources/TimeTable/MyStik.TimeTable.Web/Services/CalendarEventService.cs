using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.DataServices.IO.GpUntis.Data;

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
                CalendarEventModel ev = null;

                    if (date.Slot != null)
                    {
                        ev = new CalendarEventModel
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
                            extendendProps = new Dictionary<string, string>()
                        };
                    }
                    else
                    {
                        ev = new CalendarEventModel
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
                            extendendProps = new Dictionary<string, string>()
                        };
                    }

                    var doz = date.Date.Hosts.FirstOrDefault();

                    if (doz != null)
                    {
                        ev.extendendProps["professor"] = doz.FullName;
                    }

                    var room = date.Date.Rooms.FirstOrDefault();
                    if (room != null)
                    {
                        ev.extendendProps["raum"] =room.FullName;
                    }

                    events.Add(ev);
            }


            return events;
        }


    }
}