using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class BinaryStorage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung (optional)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Kategorie, z.B. Modulbeschreibung, Logo, Skript
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Der Datentyp
        /// </summary>
        public string FileType { get; set; }

        // Das ist erforderlich, sonst geht es z.B. in SQL-Server CE nicht
        // bzw. dort wird 4000 als maximale Länge automatisch angenommen!
        [MaxLength]
        public byte[] BinaryData { get; set; }


        /// <summary>
        /// Wie oft abgerufen
        /// </summary>
        public int AccessCount { get; set; }
    }
}
