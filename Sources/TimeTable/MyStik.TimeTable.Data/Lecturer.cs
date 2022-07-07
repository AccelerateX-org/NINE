using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Lecturer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        //public virtual TeachingBuildingBlock Module { get; set; }

        public virtual OrganiserMember Member { get; set; }

        /// <summary>
        /// Ist Modulverantwortlicher
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
