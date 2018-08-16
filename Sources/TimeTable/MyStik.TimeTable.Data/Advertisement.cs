using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
            Infos = new HashSet<AdvertisementInfo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public AdvertisementType Type { get; set; }

        /// <summary>
        /// Titel
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Beschreibung - HTML
        /// </summary>
        public string Description { get; set; }

        public virtual ICollection<AdvertisementRole> Roles { get; set; }

        public virtual  ICollection<AdvertisementInfo> Infos { get; set; }

        /// <summary>
        /// Dem es gehört - hat es hochgeladen - kann nicht delegiert werdeb
        /// </summary>
        public virtual OrganiserMember Owner { get; set; }

        /// <summary>
        /// Automatisch
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime VisibleUntil { get; set; }

        /// <summary>
        /// ok 1 pdf geht
        /// </summary>
        public virtual BinaryStorage Attachment { get; set; }

        public bool ForInternship { get; set; }

        public bool ForThesis { get; set; }

        public bool ForStayAbroad { get; set; }

        public bool ForAdvancement { get; set; }

        public bool ForWorkingStudent { get; set; }

        public bool ForTutor { get; set; }

        public bool ForCompetition { get; set; }

    }
}
