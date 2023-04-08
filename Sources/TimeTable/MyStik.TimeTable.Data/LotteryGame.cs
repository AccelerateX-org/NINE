using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class LotteryGame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public virtual Lottery Lottery { get; set; }

        /// <summary>
        /// Das Datum der ersten Wahl
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Das Datum der letzten Änderung
        /// </summary>
        public DateTime LastChange { get; set; }

        /// <summary>
        /// Akzeotiere irgendwas, wenn ich keinen Zuschlag erhalte
        /// </summary>
        public bool AcceptDefault { get; set; }

        /// <summary>
        /// Persönlicher Bedarf an Kursen
        /// </summary>
        public int CoursesWanted { get; set; }

        /// <summary>
        /// Datum der letzten Ziehung für dieses Spiel
        /// </summary>
        public DateTime? DrawingDate { get; set; }
    }
}
