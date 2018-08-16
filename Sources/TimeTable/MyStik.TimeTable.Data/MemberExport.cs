using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class MemberExport
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Verbindung zum "Heimat"Veranstalter und damit zum Benutzerkonto
        /// </summary>
        public virtual OrganiserMember Member { get; set; }

        /// <summary>
        /// Der veranstalter bei dem man tätig ist
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Der Kurzname unter der man beim "fremden" Veranstalter geführt wird
        /// </summary>
        public string ShortName { get; set; }

    }
}
