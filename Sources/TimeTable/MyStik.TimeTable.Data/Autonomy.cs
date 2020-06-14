using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Eine Einheit der Selbstverwaltung
    /// - Institution
    /// - Organiser
    /// - Curriculum
    ///
    /// Zuordnung zu Elementen
    /// - Assessments (Auswahlkommission)
    /// </summary>
    public class Autonomy
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ICollection<Committee> Committees { get; set; }


    }
}
