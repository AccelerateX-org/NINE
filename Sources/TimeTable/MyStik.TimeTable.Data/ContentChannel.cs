﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ContentChannel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string TokenName { get; set; }

        public string Token { get; set; }

        public string AccessUrl { get; set; }

        public bool ParticipientsOnly { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
