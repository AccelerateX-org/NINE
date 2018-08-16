using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class AdvertisementRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Advertisement Advertisement { get; set; }

        public virtual PersonalContact Contact { get; set; }

        /// <summary>
        /// Fachabteilung, Personlabteilung
        /// </summary>
        public string RoleName { get; set; }

    }
}
