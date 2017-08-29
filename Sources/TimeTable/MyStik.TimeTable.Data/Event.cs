using System;

namespace MyStik.TimeTable.Data
{
    public class Event : Activity
    {
        /// <summary>
        /// Wird auf Infoscreen veröffentlicht. Ab sofort bis zum Ende des letzen Termins
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Optionale Einschränkung der Veröffentlichung ab einem Datum
        /// </summary>
        public DateTime? FromDateTime { get; set; }

        /// <summary>
        /// Optionale Einschränkung der Veröffentlichung als Zeitraum vor dem ersten Termin
        /// </summary>
        public TimeSpan? FromTimeSpan { get; set; }

        public bool? IsBookable { get; set; }

        public string RommBooked { get; set; }

        public string Info { get; set; }





       
    }
}
