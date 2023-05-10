using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Examiner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual OrganiserMember Member { get; set; }

        public virtual ExaminationDescription Examination { get; set; }

        public bool? IsFirstExaminer { get; set; }

        public bool? IsSecondExaminer { get; set; }

        public bool? IsStaff { get; set; }

        public string Description { get; set; }
    }
}
