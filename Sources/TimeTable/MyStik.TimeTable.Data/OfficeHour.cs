using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Sprechstunden werden pro Semester angelegt
    /// Die Zuordnung zu Dozenten erfolgt in den Terminen
    /// </summary>
    public class OfficeHour : Activity
    {
        /// <summary>
        /// Keine Termine
        /// </summary>
        public bool ByAgreement { get; set; }

        public Semester Semester { get; set; }
    }
}
