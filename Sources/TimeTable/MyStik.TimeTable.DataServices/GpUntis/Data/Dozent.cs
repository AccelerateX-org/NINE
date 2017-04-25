using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.GpUntis.Data
{
    public class Dozent : ImportBase
    {
        public string DozentID { get; set; }
        public string Name { get; set; }
        public string Typ { get; set; }
    }
}
