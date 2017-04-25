using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Reservation : Activity
    {
        /// <summary>
        /// Benutzer der die letzte Änderung durchgeführt hat
        /// </summary>
        public string UserId { get; set; }

    }
}
