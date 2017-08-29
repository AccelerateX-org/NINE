using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Building
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name des Gebäudes
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Die Postanschrift des Gebäudes
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// Die Instiution, zu der das Gebäude gehört
        /// </summary>
        public virtual Institution Institution { get; set; }

        /// <summary>
        /// Die Räume des Gebäudes
        /// </summary>
        public virtual ICollection<Room> Rooms { get; set; }

        /// <summary>
        /// Bilder, Pläne etc.
        /// </summary>
        public virtual ICollection<BinaryStorage> Ressources { get; set; }
    }
}
