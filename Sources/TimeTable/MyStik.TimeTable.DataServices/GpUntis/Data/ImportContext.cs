using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.GpUntis.Data
{
    public class ImportContext
    {
        public ImportContext()
        {
            Dozenten = new HashSet<Dozent>();
            Faecher = new HashSet<Fach>();
            Raeume = new HashSet<Raum>();
            Gruppen = new HashSet<Gruppe>();
            Kurse = new HashSet<Kurs>();

            ErrorMessages = new List<string>();
        }


        public ICollection<Dozent> Dozenten { get; private set; }
        public ICollection<Fach> Faecher { get; private set; }
        public ICollection<Raum> Raeume { get; private set; }
        public ICollection<Gruppe> Gruppen { get; private set; }
        public ICollection<Kurs> Kurse { get; private set; }

        public ICollection<string> ErrorMessages { get; private set; }
    }
}
