using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

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
            //this.SubOrganisers = new HashSet<ActivityOrganiser>();
            ModuleCatalogs = new HashSet<CurriculumModuleCatalog>();
            RoomAllocationGroups = new HashSet<RoomAllocationGroup>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Tag { get; set; }

        /// <summary>
        /// Handelt es sich um eine studentische Organisation?
        /// </summary>
        public bool IsStudent { get; set; }

        public string HtmlColor { get; set; }   

        public string SupportUrl { get; set; }

        public string SupportEMail { get; set; }

        public bool IsVisible { get; set; }

        /// <summary>
        /// Handelt es sich um eine Fakultät oder eine zentrale Organisation
        /// </summary>
        public bool IsFaculty { get; set; }

        /// <summary>
        /// Die Selbstverwaltung
        /// </summary>
        public virtual Autonomy Autonomy { get; set; }

        public virtual BulletinBoard BulletinBoard { get; set; }

        public virtual ItemLabelSet LabelSet { get; set; }


        public virtual ICollection<Curriculum> Curricula { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<OrganiserMember> Members { get; set; }
    
        public virtual ICollection<RoomAssignment> RoomAssignments { get; set; }

        public virtual ICollection<CurriculumModuleCatalog> ModuleCatalogs { get; set; }

        public virtual ICollection<Infoscreen> Infoscreens { get; set; }

        public virtual ICollection<RoomAllocationGroup> RoomAllocationGroups { get; set; }


        public virtual Institution Institution { get; set; }

        /*
        public virtual ICollection<Institution> Institution { get; set; }
        public virtual ActivityOrganiser ParentOrganiser { get; set; }
        public virtual ICollection<ActivityOrganiser> SubOrganisers { get; set; }
        */
    }
}
