using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Level
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name des Stockwerks
        /// </summary>
        public string Stockwerke { get; set; }

        /// <summary>
        /// Plan des Stockwerks
        /// </summary>
        public string Plan { get; set; }

        /// <summary>
        /// Die Räume des Gebäudes
        /// </summary>
        public string Rooms { get; set; }
    }
}
