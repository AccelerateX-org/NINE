using System.Collections.Generic;

namespace MyStik.TimeTable.Data
{
    public class Supervision : Activity
    {
        public virtual ICollection<Thesis> Theses { get; set; }
    }
}
