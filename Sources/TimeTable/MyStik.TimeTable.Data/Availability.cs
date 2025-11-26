using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Availability
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual ICollection<PlanningGrid> PlanningGrids { get; set; }    
    }

    public class PlanningGrid 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual SemesterDate Segment { get; set; }

        /// <summary>
        /// Gültigkeitszeitraum des Rasters, d.h. wann das Raster zur Anwendung kommt bzw. zur Verfügung steht
        /// Wenn es kein Ratser gibt, dann kann auch kein Termin geplant werden
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gültigkeitszeitraum des Rasters
        /// </summary>
        public DateTime? ValidTo { get; set; }

        public virtual Availability Availability { get; set; }

        public virtual ICollection<PlanningSlot> PlanningSlots { get; set; }

        public string State { 
            get
            {
                var sb = new StringBuilder();

                if (Semester == null && Segment == null)
                {
                    sb.Append("Dauerhaft ");
                }
                else
                {
                    if (Semester != null && Segment != null)
                    {
                        sb.Append($"Semesterbezogen ({Semester.Name} | {Segment.Description}) ");
                    }
                    else
                    {
                        sb.Append("ungültige Konfiguration ");
                    }
                }

                sb.Append($"für {Organiser.ShortName}");

                return sb.ToString();
            } 
        }
    }

    public class PlanningSlot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime? Date { get; set; }

        public string Renark { get; set; }

        public virtual PlanningGrid PlanningGrid { get; set; }

    }

}
