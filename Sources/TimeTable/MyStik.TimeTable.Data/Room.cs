﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Room
    {
        public Room()
        {
            Dates = new HashSet<ActivityDate>();
            Assignments = new HashSet<RoomAssignment>();
            Bookings = new HashSet<RoomBooking>();
            Ressources = new HashSet<BinaryStorage>();
            Equipments = new HashSet<RoomEquipment>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Codierte Raumnummer, Codierung hängt vom Gebäude ab
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Name des Raums, z.B. "roter Würfel"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kapazität, d.h. Anzahl Plätze
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Beschreibung, z.B. Hörsaal, Labor etc.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ausstattung
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Steht als Lernraum zur Verfügung
        /// Bei Suche nach freien Räumen
        /// </summary>
        public bool IsForLearning { get; set; }

        /// <summary>
        /// Hat Zugangskontrolle, z.B. Labor
        /// </summary>
        public bool HasAccessControl { get; set; }

        /// <summary>
        /// Die echten Belegungstermine mit Datum und Uhrzeit
        /// many-to-many
        /// </summary>
        public virtual ICollection<ActivityDate> Dates { get; set; }

        /// <summary>
        /// Belegung des Raumes
        /// </summary>
        public virtual ICollection<RoomBooking> Bookings { get; set; }


        public virtual ICollection<RoomAssignment> Assignments { get; set; }

        /// <summary>
        /// Bilder und Dokumente
        /// </summary>
        public virtual ICollection<BinaryStorage> Ressources { get; set; }

        public virtual ICollection<RoomEquipment> Equipments { get; set; }

        public string FullName => string.IsNullOrEmpty(Name) ? Number : $"{Number} ({Name})";
    }
}
