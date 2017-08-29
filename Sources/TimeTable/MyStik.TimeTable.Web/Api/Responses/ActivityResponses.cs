using System.Collections.Generic;
using MyStik.TimeTable.Web.Api.Contracts;

namespace MyStik.TimeTable.Web.Api.Responses
{
    /// <summary>
    /// Response zur Abfrage des persönlichen Stundenplans
    /// </summary>
    public class PersonalPlanResponse
    {
        /// <summary>
        /// Liste der einzelnen Termine der gebuchten Kurse, siehe OwnDatesContract
        /// </summary>
        public IEnumerable<OwnDatesContract> Courses { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage aller Events und den dazugehörigen Informationen
    /// </summary>
    public class EventInfoResponse
    {
        /// <summary>
        /// Liste aller Events, siehe EventContract
        /// </summary>
        public IEnumerable<EventContract> Events { get;set;}
    }
    /// <summary>
    /// Response zur Abfrage von Informationen eines Events
    /// </summary>
    public class EventSingleResponse
    {
        /// <summary>
        /// Infos über das Event, siehe EventContract
        /// </summary>
        public EventContract Event { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Informationen eines Termins
    /// </summary>
    public class DateInfoResponse
    {
        /// <summary>
        /// Infos über Termin, siehe DatesContract
        /// </summary>
        public DatesContract DateInfo { get; set; }
        
    }
}
