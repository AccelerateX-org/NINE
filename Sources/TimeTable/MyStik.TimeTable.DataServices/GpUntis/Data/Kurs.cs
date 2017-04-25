using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.GpUntis.Data
{
    public class Kurs
    {
        public Kurs()
        {
            Unterricht = new HashSet<Unterricht>();
            Termine = new HashSet<Termin>();
            Gruppen = new HashSet<Gruppe>();

        }

        public String Id { get; set; }

        public Fach Fach { get; set; }

        // zum Test und Fehler finden
        public ICollection<Unterricht> Unterricht { get; private set; }

        public ICollection<Gruppe> Gruppen { get; private set; }

        public ICollection<Termin> Termine { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Fach.FachID);
            sb.Append(";");

            foreach (var gruppe in Gruppen)
            {
                sb.Append(gruppe.GruppenID);
                sb.Append(";");
            }

            return sb.ToString();
        }
    }
}
