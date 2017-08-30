using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Änderung am Zeitraum eines Termins
    /// Ein Termin kann somit folgende Zustände haben
    /// - keine Änderung => Originalzeitpunkt, der beim Anlegen erzeugt wurde
    /// - geändert: erste Änderung enthält den Originalzeitpunkt
    /// </summary>
    public class ActivityDateChange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Referenz auf Benutzer, der die Änderung durchgeführt hat
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Zeitstempel der Änderung
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Referenz auf zugehörigen Termin
        /// die Datumsangaben dort spiegeln nur den neuesten Status wieder
        /// und können sich so von den Angaben hier unterscheiden
        /// </summary>
        public virtual ActivityDate Date { get; set; }

        /// <summary>
        /// Zeitpunkt des Beginns vor der Änderung
        /// </summary>
        public DateTime OldBegin { get; set; }

        /// <summary>
        /// Zeitpunkt des Endes vor der Änderung
        /// </summary>
        public DateTime OldEnd { get; set; }
        
        /// <summary>
        /// Zeitpunkt des Beginns nach der Änderung
        /// </summary>
        public DateTime NewBegin { get; set; }
        
        /// <summary>
        /// Zeitpunkt des Endes nach der Änderung
        /// </summary>
        public DateTime NewEnd { get; set; }

        /// <summary>
        /// Wenn es eine Raumänderung gab, dann lässt sich die
        /// aktuelle Belegung aus dem Date rauslesen
        /// </summary>
        public bool HasRoomChange { get; set; }

        /// <summary>
        /// Hat sich der Status verändert?
        /// Der aktuelle Status steht in der Occurrence
        /// </summary>
        public bool HasStateChange { get; set; }

        /// <summary>
        /// Haben sich die Zeiten verändert?
        /// Die Zeiten stehen hier
        /// </summary>
        public bool HasTimeChange { get; set; }

        
        /// <summary>
        /// Text der Notification - wird generiert
        /// </summary>
        public string NotificationContent { get; set; }

        /// <summary>
        /// Wurde die Notification erstellt?
        /// </summary>
        public bool IsNotificationGenerated { get; set; }


        public virtual ICollection<NotificationState> NotificationStates { get; set; }
    }
}
