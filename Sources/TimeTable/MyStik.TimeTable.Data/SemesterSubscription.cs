using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class SemesterSubscription : Subscription
    {
        public virtual SemesterGroup SemesterGroup { get; set; }
    }
}
