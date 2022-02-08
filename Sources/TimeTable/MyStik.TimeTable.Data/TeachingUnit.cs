﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class TeachingUnit
    {
        public TeachingUnit()
        {
            Accreditations = new HashSet<SubjectAccreditation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Kennzeichnung der Alternative
        /// </summary>
        public string Tag { get; set; }

        public string Name { get; set; }


        public string Description { get; set; }

        public double SWS { get; set; }

        public virtual TeachingForm Form { get; set; }


        public virtual TeachingBuildingBlock Module { get; set; }

        public virtual ICollection<SubjectAccreditation> Accreditations { get; set; }
    }
}
