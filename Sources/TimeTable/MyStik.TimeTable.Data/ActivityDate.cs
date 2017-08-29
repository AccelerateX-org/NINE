using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ActivityDate
    {
        public ActivityDate()
        {
            this.Rooms = new HashSet<Room>();
            this.RoomBookings = new HashSet<RoomBooking>();
            this.Hosts = new HashSet<OrganiserMember>();
            this.Slots = new HashSet<ActivitySlot>();
            this.Changes = new HashSet<ActivityDateChange>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Titel / Überschrift für Termin (optional)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Kurzname (optional)
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Beschreibung (optional), HTML encoded
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Beginn Datum
        /// </summary>
        public DateTime Begin { get; set; }
        
        /// <summary>
        /// Ende Datum
        /// </summary>
        public DateTime End { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual Occurrence Occurrence { get; set; }


        /// <summary>
        /// Direkte Belegung des Raumes (altes Konzept)
        /// </summary>
        public virtual ICollection<Room> Rooms { get; set; }

        /// <summary>
        /// Belegung des Raumes
        /// </summary>
        public virtual ICollection<RoomBooking> RoomBookings { get; set; }

        /// <summary>
        /// Veranstalter des Termins
        /// </summary>
        public virtual ICollection<OrganiserMember> Hosts { get; set; }

        public virtual ICollection<ActivitySlot> Slots { get; set; }

        public virtual ICollection<ActivityDateChange> Changes { get; set; }
    }
}
