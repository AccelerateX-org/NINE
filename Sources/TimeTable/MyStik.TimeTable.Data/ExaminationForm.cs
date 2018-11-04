using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Prüfungsform nach ASPO
    /// </summary>
    public class ExaminationForm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        /// <summary>
        /// Hier könnte der jeweilige Paragraoph aus der ASPO rein
        /// </summary>
        public string Description { get; set; }

    }
}
