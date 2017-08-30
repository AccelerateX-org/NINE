using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;

namespace MyStik.TimeTable.Web.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class EventInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public NextEventContract GetNextEvent()
        {
            var db = new TimeTableDbContext();

            var now = GlobalSettings.Now;

            var eventList =
                db.Activities.OfType<Event>()
                    .Where(ev => ev.Published == true && ev.Dates.Any(d => d.End >= now)).ToList();

            ActivityDate nextDate = null;
            foreach (var @event in eventList)
            {
                var testDate = @event.Dates.Where(d => d.End >= now).OrderBy(d => d.Begin).FirstOrDefault();

                if (nextDate == null)
                {
                    nextDate = testDate;
                }
                else
                {
                    if (testDate.Begin < nextDate.Begin)
                    {
                        nextDate = testDate;
                    }
                }

            }

            if (nextDate != null)
            {
                var nextEvent = new NextEventContract
                {
                    EventId = nextDate.Activity.Id,
                    Title = nextDate.Activity.Name,
                    Description = nextDate.Activity.Description,
                    From = nextDate.Begin,
                    Location = nextDate.Rooms.Any() ? nextDate.Rooms.First().Number : string.Empty,
                };
                return nextEvent;
            }
            else
            {
                return new NextEventContract
                {
                    Title = "Derzeit keine Veranstaltung",
                };
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NextEventContract GetNextEvent(Guid id)
        {
            var db = new TimeTableDbContext();

            var currentEvent = db.Activities.OfType<Event>().SingleOrDefault(ev => ev.Id == id);

            var now = GlobalSettings.Now.AddMinutes(-1);

            // Gib mir von dem Event das Ende des nächsten Termins in der Zukunft
            var currentDate = currentEvent.Dates.Where(d => d.End >= now).OrderBy(d => d.End).FirstOrDefault();

            // Gib mir jetzt das Event, mit dem zeitlich am nächst gelegenen Beginn zum aktuellen Datum
            var eventList =
                db.Activities.OfType<Event>()
                    .Where(ev => ev.Published == true && ev.Id != id &&
                        ev.Dates.Any(d => d.End >= currentDate.Begin)).ToList();

            ActivityDate nextDate = null;
            foreach (var @event in eventList)
            {
                var testDate = @event.Dates.Where(d => d.End >= now).OrderBy(d => d.Begin).FirstOrDefault();

                if (nextDate == null)
                {
                    nextDate = testDate;
                }
                else
                {
                    if (testDate.Begin < nextDate.Begin)
                    {
                        nextDate = testDate;
                    }
                }

            }

            if (nextDate == null)
                return GetNextEvent();


            var nextEvent = new NextEventContract
            {
                EventId = nextDate.Activity.Id,
                Title = nextDate.Activity.Name,
                Description = nextDate.Activity.Description,
                From = nextDate.Begin,
                Location = nextDate.Rooms.Any() ? nextDate.Rooms.First().Number : string.Empty,
            };
            return nextEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventContract> GetAllEvents()
        {
            var db = new TimeTableDbContext();

            var now = GlobalSettings.Now;
            //alle zukünftigen Events
            var events = db.Activities.OfType<Event>()
                    .Where(ev =>  ev.Dates.Any(d => d.End >= now)).ToList();

            //andere ansatz
            //var events = db.Activities.OfType<Event>()
                    //.Where(ev => ev.Published == true && ev.Dates.Any(d => d.End >= now)).ToList();

            var eventList = new List<EventContract>();

             //jedes zukünftige event soll hinzugefügt werden mit Datum um Veranstalter
            foreach (var @event in events)
            {
                var nextEvent = new EventContract
                {
                    
                   EventId= @event.Id.ToString(),
                   EventName= @event.Name,
                   EventDesciption= @event.Description,

                   //geht des so?
                   EventSlots= @event.Occurrence.Capacity,
                   AvailableSlots = @event.Occurrence.Capacity - @event.Occurrence.Subscriptions.Count(),

                };
                //Die einzelnen Termine der Veranstaltung
                var eventDateList = new List<EventDate>();

                //Alle Termine der Veranstaltung in der Zukunft
                foreach (var date in @event.Dates)
                {
                    //nur zukünftige Termine oder in zukuft reichende
                    if(date.Begin >= now || (date.Begin <= now && date.End > now))
                    {
                    eventDateList.Add(new EventDate
                        {
                            Start = date.Begin.ToString("hh\\:mm"),
                            End = date.End.ToString("hh\\:mm"),
                            Date = date.Begin.Date.ToString("dd.MM.yyyy"),
                            PlaceName= date.Rooms.FirstOrDefault() != null ? date.Rooms.First().Number : "N.N.",
                            PlaceId=date.Rooms.FirstOrDefault() != null ? date.Rooms.First().Id.ToString() : "N.N.",
                            EventOrganiser=date.Hosts.FirstOrDefault() != null ? date.Hosts.First().Name : "N.N.",
                        });
                    nextEvent.Dates = eventDateList;
                    }
                }
                eventList.Add(nextEvent);
            }
            //return eventList.OrderBy(ev => ev.Dates.First(e => e.Start>=now));
            return eventList;

        }

        /// <summary>
        /// Abfrage eines speziellen Events mit Hilfe der EventId
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public EventContract GetEvent (string eventId)
        {
            var db = new TimeTableDbContext();

            var dbevent = db.Activities.FirstOrDefault(a => a.Id.ToString().Equals(eventId));
            
            var eventinfo = new EventContract();

            if(dbevent!=null)
            {
                eventinfo.EventId = dbevent.Id.ToString();
                eventinfo.EventName = dbevent.Name;
                eventinfo.EventSlots = dbevent.Occurrence.Capacity;
                eventinfo.EventDesciption = dbevent.Description;
                eventinfo.AvailableSlots = (dbevent.Occurrence.Capacity-dbevent.Occurrence.Subscriptions.Count());

                var eventDateList = new List<EventDate>();

                foreach(var date in dbevent.Dates)
                {
                    eventDateList.Add(new EventDate
                    {
                        Start = date.Begin.ToString("hh\\:mm"),
                        End = date.End.ToString("hh\\:mm"),
                        Date = date.Begin.Date.ToString("dd.MM.yyyy"),
                        PlaceName = date.Rooms.FirstOrDefault() != null ? date.Rooms.First().Number : "N.N.",
                        PlaceId = date.Rooms.FirstOrDefault() != null ? date.Rooms.First().Id.ToString() : "N.N.",
                        EventOrganiser = date.Hosts.FirstOrDefault() != null ? date.Hosts.First().Name : "N.N.",
                    });
                }

                eventinfo.Dates = eventDateList;
            }
            return eventinfo;
        }
    }
}
