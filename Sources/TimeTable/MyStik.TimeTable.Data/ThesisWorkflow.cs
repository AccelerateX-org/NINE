using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ThesisWorkflow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public string UserId { get; set; }

        public OrganiserMember Examiner { get; set; }

        public DateTime Created { get; set; }

        public bool? IsAccepted { get; set; }

        public bool? IsFinished { get; set; }


        public virtual ThesisAnnouncement Announcement { get; set; }

        public virtual ICollection<ThesisFeedback> Feedbacks { get; set; }
    }
}
