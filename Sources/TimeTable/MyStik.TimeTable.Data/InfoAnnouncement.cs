using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class InfoAnnouncement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string ImageURL { get; set; }

        public virtual BinaryStorage Image { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual ActivityDate Date { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ShowFrom { get; set; }

        public DateTime? ShowUntil { get; set; }

        public virtual OrganiserMember Member { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual Infoscreen Infoscreen { get; set; }
    }
}
