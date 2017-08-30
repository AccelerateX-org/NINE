using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    /// <summary>
    /// Contract zur Abfrage der nächsten Events
    /// </summary>
    public class NextEventContract
    {
        /// <summary>
        /// Guid des Events als string
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// Titel des Events
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Beschreibung des Events
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Startzeitpunkt
        /// </summary>
        public DateTime? From { get; set; }
        /// <summary>
        /// Endzeitpunkt
        /// </summary>
        public DateTime? Until { get; set; }
        /// <summary>
        /// Ort
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Veranstalter
        /// </summary>
        public string Host { get; set; }

    }
    /// <summary>
    /// Contract zur Abfrage von Events
    /// </summary>
    public class EventContract
    {
        /// <summary>
        /// Guid des Events als string
        /// </summary>
        public string EventId { get; set; }
        /// <summary>
        /// Name des Events
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// Beschreibung des Events
        /// </summary>
        public string EventDesciption { get; set; }
        /// <summary>
        /// Anzahl der Slots des Events
        /// </summary>
        public int EventSlots { get; set; }
        /// <summary>
        /// Anzahl der bereits belegten Slots des Events
        /// </summary>
        public int AvailableSlots { get; set; }
        /// <summary>
        /// Liste der Termine des Events, siehe EventDate
        /// </summary>
        public IEnumerable<EventDate> Dates { get; set; }
    }
    /// <summary>
    /// Contract über Termininfos eines Events
    /// </summary>
    public class EventDate
    {
        /// <summary>
        /// Startzeit im Format hh:mm
        /// </summary>
        public string Start { get; set; }
        /// <summary>
        /// Endzeit im Format hh:mm
        /// </summary>
        public string End { get; set; }
        /// <summary>
        /// Datum eines Eventtermins im Format dd.MM.yyyy
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Name des Veranstaltungsorts, evtl. BMW oder Raums
        /// </summary>
        public string PlaceName { get; set; }
        /// <summary>
        /// Falls Raum der Hochschule, Guid des Raums als string
        /// </summary>
        public string PlaceId { get; set; }
        /// <summary>
        /// Veranstalter des Events
        /// </summary>
        public string EventOrganiser { get; set; }

    }
}
