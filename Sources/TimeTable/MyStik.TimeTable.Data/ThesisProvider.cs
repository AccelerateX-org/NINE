using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ThesisProvider
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public virtual ICollection<ThesisAnnouncement> Announcements { get; set; }

        public virtual ICollection<ThesisFeedback> Feedbacks { get; set; }

    }
}
