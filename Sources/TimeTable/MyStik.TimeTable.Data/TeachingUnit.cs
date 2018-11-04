using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class TeachingUnit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public double SWS { get; set; }

        public virtual TeachingForm Form { get; set; }


        public virtual TeachingBuildingBlock Module { get; set; }
    }
}
