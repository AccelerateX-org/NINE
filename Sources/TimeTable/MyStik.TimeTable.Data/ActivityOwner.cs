﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class ActivityOwner
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual OrganiserMember Member { get; set; }

        /// <summary>
        /// Ein Owner, der keine Änderungen vornehmen darf
        /// </summary>
        public bool IsLocked { get; set; }

    }
}
