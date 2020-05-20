using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Meeting : Activity
    {
        public virtual Committee Committee { get; set; }
    }
}
