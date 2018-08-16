using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class SemesterTopic
    {
        public SemesterTopic()
        {
            Activities = new HashSet<Activity>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Das Semester
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Das Topic
        /// </summary>
        public virtual CurriculumTopic Topic { get; set; }

        /// <summary>
        /// Die verbundenen Aktivitäten
        /// </summary>
        public virtual ICollection<Activity> Activities { get; set; }

        public string TopicName => $"{Topic.Chapter.Name}: {Topic.Name}";
    }
}
