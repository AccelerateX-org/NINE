using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Advisor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string CorporateName { get; set; }

        public string PersonFirstName { get; set; }

        public string PersonLastName { get; set; }

        /// <summary>
        /// Tätigkeitsgebiet / Abteilung
        /// </summary>
        public string PersonAction { get; set; }


        public string PersonEMail { get; set; }

        public string PersonPhone { get; set; }

        /// <summary>
        /// Der bestätigte Kontakt oder eben null
        /// </summary>
        public virtual Contact Contact { get; set; }

        public virtual Thesis Thesis { get; set; }

        public virtual Internship Internship { get; set; }
    }
}
