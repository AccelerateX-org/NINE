﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class InfoText
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ShowFrom { get; set; }

        public DateTime? ShowUntil { get; set; }

        public virtual OrganiserMember Member { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual Infoscreen Infoscreen { get; set; }
    }
}
