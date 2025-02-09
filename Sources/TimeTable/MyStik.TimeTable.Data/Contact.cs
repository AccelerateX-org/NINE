﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Contact
    {
        public Contact()
        {
            Advisors = new HashSet<Advisor>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ICollection<Advisor> Advisors { get; set; }
    }
}
