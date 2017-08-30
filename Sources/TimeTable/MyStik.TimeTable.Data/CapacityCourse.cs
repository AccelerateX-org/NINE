using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CapacityCourse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fach
        /// Identität auf ShortName von Course!
        /// </summary>
        public string ShortName { get; set; }

        public virtual ModuleCourse Course { get; set; }
    }
}
