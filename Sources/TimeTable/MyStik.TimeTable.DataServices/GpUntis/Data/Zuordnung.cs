using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.GpUntis.Data
{
    public class Zuordnung
    {
        public Zuordnung()
        {
            Gruppen = new HashSet<Gruppe>();
        }


        public Fach Fach { get; set; }

        public ICollection<Gruppe> Gruppen { get; set; }

        public string Programm { get; set; }
        public string Studiengruppe { get; set; }

        public string GetName()
        {
            return Programm + "-" + Studiengruppe;
        }
    }
}
