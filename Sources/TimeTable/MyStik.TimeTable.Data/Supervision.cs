using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Supervision : Activity
    {
        public virtual ICollection<Thesis> Theses { get; set; }
    }
}
