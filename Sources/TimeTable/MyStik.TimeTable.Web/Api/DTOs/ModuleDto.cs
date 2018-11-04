using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumDto Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Ects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double UsCredits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Sws { get; set; }

    }
}