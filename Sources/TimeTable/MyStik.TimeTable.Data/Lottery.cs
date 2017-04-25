using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public enum DrawingFrequency
    {
        Daily = 1,
        Weekly,
        Monthly
    }

    public class Lottery
    {
        public Lottery()
        {
            Occurrences = new HashSet<Occurrence>();
            Drawings = new HashSet<LotteryDrawing>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string JobId { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Nur der Owener darf die Einstellungen ändern
        /// </summary>
        public virtual OrganiserMember Owner { get; set; }

        /// <summary>
        /// Anzahl der Plätze, die ein Teilnehmer maximal annehmen darf
        /// </summary>
        public int MaxConfirm { get; set; }

        public DateTime FirstDrawing { get; set; }

        public DateTime LastDrawing { get; set; }

        public TimeSpan DrawingTime { get; set; }

        public DrawingFrequency DrawingFrequency { get; set; }

        public virtual ICollection<Occurrence> Occurrences { get; set; }

        public virtual ICollection<LotteryDrawing> Drawings { get; set; }
    }
}
