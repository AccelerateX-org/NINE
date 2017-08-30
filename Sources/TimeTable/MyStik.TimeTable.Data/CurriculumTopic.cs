using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CurriculumTopic
    {
        public CurriculumTopic()
        {
            SemesterTopics = new HashSet<SemesterTopic>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual CurriculumChapter Chapter { get; set; }


        public virtual ICollection<SemesterTopic>  SemesterTopics { get; set; }

    }
}
