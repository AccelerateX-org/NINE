using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CorporateContact
    {
        public CorporateContact()
        {
            ContactPersons = new HashSet<PersonalContact>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name des Unternehmens inkl. Abteilung etc.
        /// </summary>
        public string Name { get; set; }

        public string Url { get; set; }

        public virtual ICollection<PersonalContact> ContactPersons { get; set; }

    }
}
