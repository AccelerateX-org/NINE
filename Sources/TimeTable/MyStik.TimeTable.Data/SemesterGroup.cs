using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyStik.TimeTable.Data
{
    public class SemesterGroup
    {
        public SemesterGroup()
        {
            this.Activities = new HashSet<Activity>();
            this.Subscriptions = new HashSet<SemesterSubscription>();
            this.OccurrenceGroups = new HashSet<OccurrenceGroup>();
            this.Students = new HashSet<Student>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name der Semestergruppe
        /// kann auch leer sein
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Freigabe der Semestergruppe für Planer etc.
        /// </summary>
        public bool IsAvailable { get; set; }

        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Das ist veraltet
        /// </summary>
        public virtual CurriculumGroup CurriculumGroup { get; set; }

        /// <summary>
        /// Verweis auf die Kapazitätsgruppe
        /// </summary>
        public virtual CapacityGroup CapacityGroup { get; set; }


        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<SemesterSubscription> Subscriptions { get; set; }

        public virtual ICollection<OccurrenceGroup> OccurrenceGroups { get; set; }


        public virtual ICollection<Student> Students { get; set; }

        public string CompleteName
        {
            get
            {
                if (CapacityGroup == null)
                    return "Falsche Zuordnung";

                var sb = new StringBuilder();

                sb.AppendFormat("{0} - {1}", Semester.Name, CapacityGroup.FullName);

                return sb.ToString();
            }
        }

        public string FullName
        {
            get
            {
                if (CapacityGroup != null)
                    return CapacityGroup.FullName;

                return "Falsche Zuordnung";
            }
        }

        public string FullNameCompact
        {
            get
            {
                if (CapacityGroup != null)
                    return CapacityGroup.FullName;

                return "Falsche Zuordnung";
            }
        }
        
        public string GroupName
        {
            get
            {
                if (CapacityGroup != null)
                    return CapacityGroup.GroupName;

                return "Falsche Zuordnung";
            }
        }
    }
}
