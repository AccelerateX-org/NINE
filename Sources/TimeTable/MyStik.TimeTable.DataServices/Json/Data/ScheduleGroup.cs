using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fk11.Model
{
    public class ScheduleGroup
    {
        /// <summary>
        /// SS17
        /// </summary>
        public string SemesterName { get; set; }

        /// <summary>
        /// FK 09
        /// </summary>
        public string FacultyName { get; set; }

        /// <summary>
        /// Bachelor Wirtschaftsingenieurwesen
        /// </summary>
        public string CurriculumName { get; set; }

        /// <summary>
        /// WI
        /// </summary>
        public string CurriculumShortName { get; set; }


        /// <summary>
        /// 1
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// A
        /// </summary>
        public string SubGroupName { get; set; }

        /// <summary>
        /// Hierarchie aus QB: .. MB:
        /// </summary>
        public string ChapterName { get; set; }

        /// <summary>
        /// Thema
        /// </summary>
        public string TopicName { get; set; }
    }
}
