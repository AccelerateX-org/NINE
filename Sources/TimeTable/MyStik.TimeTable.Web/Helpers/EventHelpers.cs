using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Helpers
{
    public class EventHelpers
    {
        DateTime start; // Starttermin
        DateTime end; // Endtermin
        EventCreateSeriesModel model; // Model
        public ICollection<ActivityDate> dates = new List<ActivityDate>(); // Liste für Einzeltermine

        // Constructor
        public EventHelpers(Event @event, EventCreateSeriesModel model)
        {
            // Übertragung des Models
            this.model = model;

            // Übertragung der Termine
            start = model.Day.AddHours(model.StartTimeHour).AddMinutes(model.StartTimeMinute);
            if (model.endType == "useEndDate")
            {
                end = model.EndDay.AddHours(model.EndTimeHour).AddMinutes(model.EndTimeMinute);
            }
            else
            {
                if (model.endType == "noEnd")
                {
                    model.countDates = 100; // Maximale Anzahl an Events, wenn "Kein Ende" ausgewählt wurde
                }
                end = new DateTime().AddHours(model.EndTimeHour).AddMinutes(model.EndTimeMinute);
            }

            var firstDate = new ActivityDate {
                Begin = start,
                End = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute)
            };

            dates.Add(firstDate);

            // Termine erzeugen
            CreateDates();
            
        }

        // Logik für Hauptfrequenz
        public void CreateDates()
        {
            switch (model.MainFrequency.ToUpper())
            {
                case "DAILY":
                    createDailyDates();
                   break;
                case "WORKDAYS":
                   createWorkdayDates();
                   break;
                case "WEEKLY":
                   createWeeklyDates();
                   break;
                case "MONTHLY":
                   createMonthlyDates();
                   break;
                default:
                   createAnnualDates();
                   break;
            }
        }

        public void createDailyDates()
        {
            if (model.endType == "useEndDate")
            {
                start = start.AddDays(model.dailyCount);
                while (start < end)
                {
                    
                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                    start = start.AddDays(model.dailyCount);
                } 
            }
            else
            {
                for (int i = 2; i <= model.countDates; i++)
                {
                    start = start.AddDays(model.dailyCount);
                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                }
            }
        }

        public void createWorkdayDates()
        {
            if (model.endType == "useEndDate")
            {
                start = start.AddDays(1);
                while (start < end)
                {
                    

                    if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                    start = start.AddDays(1);
                } 
            }
            else
            {
                for (int i = 2; i <= model.countDates; i++)
                {
                    start = start.AddDays(1);

                    if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                }
            }
        }

        public void createWeeklyDates()
        {
            if (model.endType == "useEndDate")
            {
                start = start.AddDays(7 * model.weeklyCount);
                while (start < end)
                {
                    

                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                    start = start.AddDays(7 * model.weeklyCount);
                } 
            }
            else
            {
                for (int i = 2; i <= model.countDates; i++)
                {
                    start = start.AddDays(7 * model.weeklyCount);

                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                }
            }
        }

        public void createMonthlyDates()
        {
            if (model.endType == "useEndDate")
            {
                start = start.AddMonths(1);
                while (start < end)
                {
                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                    start = start.AddMonths(1);
                } 
            } 
            else
            {
                for (int i = 2; i <= model.countDates; i++)
                {
                    start = start.AddMonths(1);

                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                }
            }
        }

        public void createAnnualDates()
        {
            if (model.endType == "useEndDate")
            {
                start = start.AddYears(model.yearCount);
                while (start < end)
                {
                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                    start = start.AddYears(model.yearCount);
                } 
            }
            else
            {
                for (int i = 2; i <= model.countDates; i++)
                {
                    start = start.AddDays(model.yearCount);
                    DateTime endDate = new DateTime().AddYears(start.Year).AddMonths(start.Month).AddDays(start.Day).AddHours(end.Hour).AddMinutes(end.Minute);
                    var date = new ActivityDate
                    {
                        Begin = start,
                        End = endDate
                    };

                    dates.Add(date);
                }
            }
        }
    }
}