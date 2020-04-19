using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.GpUntis.Data
{
    public class Restriktion : ImportBase
    {
        public string Typ { get; set; }

        public string ElementID { get; set; }

        public int Tag { get; set; }
        public int Stunde { get; set; }

        /// <summary>
        /// -3 nicht verfügbar
        /// </summary>
        public int Level { get; set; }

    }
}
