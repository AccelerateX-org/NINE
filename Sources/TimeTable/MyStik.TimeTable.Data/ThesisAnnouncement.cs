using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ThesisAnnouncement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Department { get; set; }

        public string Company { get; set; }

        public virtual ThesisProvider Provider { get; set; }

        public ICollection<ThesisWorkflow> Workflows { get; set; }
    }
}
