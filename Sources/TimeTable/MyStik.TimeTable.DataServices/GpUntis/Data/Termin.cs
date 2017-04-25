using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.GpUntis.Data
{
    public class Termin
    {
        public Termin()
        {
            Raeume = new HashSet<Raum>();
            Dozenten = new HashSet<Dozent>();
        }

        public int Tag { get; set; }
        public int VonStunde { get; set; }
        public int BisStunde { get; set; }

        public ICollection<Raum> Raeume { get; private set; }

        public ICollection<Dozent> Dozenten { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0} [{1}-{2}]:", Tag, VonStunde, BisStunde);

            foreach (var dozent in Dozenten)
            {
                sb.Append(dozent.DozentID);
                sb.Append(";");
            }

            foreach (var raum in Raeume)
            {
                sb.Append(raum.RaumID);
                sb.Append(";");
            }


            return sb.ToString();
        }
    }
}
