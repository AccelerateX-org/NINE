using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public enum AdvertisementType
    {
        Placement,      // Praktikum
        Employment,     // (Fest)Anstellung
        Trainee,        // Werkstudent
        BachelorThesis,
        MasterThesis,
        DoctoralThesis,
        Award,
        Studentship,
        Incorporator
    }


    public class Advertisement
    {
        public Advertisement()
        {
            Roles = new HashSet<AdvertisementRole>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public AdvertisementType Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual ICollection<AdvertisementRole> Roles { get; set; }

        public virtual OrganiserMember Owner { get; set; }

        public DateTime Created { get; set; }

        public DateTime VisibleUntil { get; set; }

        public virtual BinaryStorage Attachment { get; set; }
    }
}
