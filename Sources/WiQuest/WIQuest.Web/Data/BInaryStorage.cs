using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class BinaryStorage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der Datentyp
        /// </summary>
        public string ImageFileType { get; set; }

        // Das ist erforderlich, sonst geht es z.B. in SQL-Server CE nicht
        // bzw. dort wird 4000 als maximale Länge automatisch angenommen!
        [MaxLength]
        public byte[] ImageData { get; set; }

    }
}