using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class PublishingChannel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Das ist auch der Owner
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Leer: alle Studiengänge
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        public virtual ItemLabelSet LabelSet { get; set; }

        /// <summary>
        /// Fachsemester => passt zum Attribut Semester beim CurriculumSlot
        /// </summary>
        public int Semester { get; set; }

        public virtual ICollection<ActivityPublication> Publications { get; set; }
    }


    public class ActivityPublication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual PublishingChannel PublishingChannel { get; set; }
    }


    public class ContentChannel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string TokenName { get; set; }

        public string Token { get; set; }

        public string AccessUrl { get; set; }

        public bool ParticipientsOnly { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
