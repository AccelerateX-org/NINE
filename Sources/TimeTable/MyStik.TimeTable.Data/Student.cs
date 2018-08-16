using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Student
    {
        public Student()
        {
            Development = new HashSet<SemesterGroup>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Matrikelnummer
        /// </summary>
        public string Number { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }


        public virtual Semester FirstSemester { get; set; }

        public virtual Semester LastSemester { get; set; }

        public bool IsPartTime { get; set; }

        public bool IsDual { get; set; }

        public bool HasCompleted { get; set; }

        public DateTime Created { get; set; } 

        public virtual ICollection<SemesterGroup> Development { get; set; }
    }
}
