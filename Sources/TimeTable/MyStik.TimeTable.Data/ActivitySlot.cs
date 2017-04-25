using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class ActivitySlot
    {
        public ActivitySlot()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Langname des Termins (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kurname (optional)
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Beschreibung (optional), HTML encoded
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Beginn Uhrzeit
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// Ende Uhrzeit
        /// </summary>
        public DateTime End { get; set; }

        public virtual ActivityDate ActivityDate { get; set; }

        public virtual Occurrence Occurrence { get; set; }
    }
}
