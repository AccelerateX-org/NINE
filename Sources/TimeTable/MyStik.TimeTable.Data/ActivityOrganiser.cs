using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class ActivityOrganiser
    {
        public ActivityOrganiser()
        {
            this.Activities = new HashSet<Activity>();
            this.Members = new HashSet<OrganiserMember>();
            this.RoomAssignments = new HashSet<RoomAssignment>();
            this.Curricula = new HashSet<Curriculum>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        /// <summary>
        /// Handelt es sich um eine studentische Organisation?
        /// </summary>
        public bool IsStudent { get; set; }

        /// <summary>
        /// Handelt es sich um eine Fakultät oder eine zentrale Organisation
        /// </summary>
        public bool IsFaculty { get; set; }

        public virtual ICollection<Curriculum> Curricula { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<OrganiserMember> Members { get; set; }
    
        public virtual ICollection<RoomAssignment> RoomAssignments { get; set; }

        public virtual ICollection<Infoscreen> Infoscreens { get; set; }
    }
}
