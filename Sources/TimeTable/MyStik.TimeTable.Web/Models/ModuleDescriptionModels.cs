using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ExaminationEditModel
    {
        public Guid moduleId { get; set; }

        public Guid examinationId { get; set; }

        public Guid accredidationId { get; set; }

        public Guid semesteId { get; set; }

        public Guid examOptId { get; set; }

        public Guid firstMemberId { get; set; }

        public Guid secondMemberId { get; set;}

        public string Conditions { get; set; }

        public string Utilities { get; set; }

        public Semester Semester { get; set; }

        public CurriculumModule Module { get; set; }

    }
}