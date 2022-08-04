using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Institution
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name der Institution
        /// </summary>
        public string Name { get; set; }

        public string Tag { get; set; }

        /// <summary>
        /// E-Mail Domain
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Die Selbstverwaltung
        /// </summary>
        public virtual Autonomy Autonomy { get; set; }


        /// <summary>
        /// Liste aller Veranstalter der Institution
        /// </summary>
        public virtual ICollection<ActivityOrganiser> Organisers { get; set; }

        /// <summary>
        /// Liste aller Gebäude der Institution
        /// </summary>
        public virtual ICollection<Building> Buildings { get; set; }
    
    }
}
