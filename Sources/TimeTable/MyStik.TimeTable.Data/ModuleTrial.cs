using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Der Student muss nicht im Kurs eingetragen sein, um ihn im Plan zu haben
    /// </summary>
    public class ModuleTrial
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Das zu belegende Modul
        /// </summary>
        public virtual ModuleMapping Mapping { get; set; }

        /// <summary>
        /// Die tatsächlich belegte Lehrveranstaltung
        /// </summary>
        public virtual Course Course { get; set; }

        /// <summary>
        /// Die erziele Note
        /// </summary>
        public int? Mark { get; set; }


    }
}
