using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ModuleMapping
    {
        public ModuleMapping()
        {
            Trials = new HashSet<ModuleTrial>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual CoursePlan Plan { get; set; }

        
        /// <summary>
        /// Das zu belegende Modul
        /// kann auch ein virtuelles Modul sein "WPM1"
        /// </summary>
        public virtual CurriculumModule Module { get; set; }

        /// <summary>
        /// Das Semester, in dem das Modul belegt werden soll 
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Das Semester, in dem das Modul nach SPO angeordnet ist 
        /// </summary>
        public virtual Semester CurriculumSemester { get; set; }

        public virtual ICollection<ModuleTrial> Trials { get; set; }

        //public int? Mark { get; set; }

        //public int Trial { get; set; }

        /// <summary>
        /// Angerechnet
        /// Über das Module, da der MV das ja machen muss!
        /// </summary>
        public bool IsCharged { get; set; }

    }
}
