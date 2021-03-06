﻿using System;
using System.Collections.Generic;
using MyStik.TimeTable.DataServices.IO.GpUntis.Data;

namespace MyStik.TimeTable.DataServices.IO.GpUntis
{
    public class ImportContext : BaseImportContext
    {
        public ImportContext()
        {
            Dozenten = new HashSet<Dozent>();
            Faecher = new HashSet<Fach>();
            Raeume = new HashSet<Raum>();
            Gruppen = new HashSet<Gruppe>();
            Kurse = new HashSet<Kurs>();

            Tage = new Dictionary<int, bool>();
            Stunden = new Dictionary<int, ImportTimeSpan>();
            GruppenZuordnungen = new List<Zuordnung>();

            Restriktionen = new List<Restriktion>();

            Blockaden = new List<RaumBlockade>();

        }


        public ICollection<Dozent> Dozenten { get; private set; }
        public ICollection<Fach> Faecher { get; private set; }
        public ICollection<Raum> Raeume { get; private set; }
        public ICollection<Gruppe> Gruppen { get; private set; }
        public ICollection<Kurs> Kurse { get; private set; }

        public Dictionary<int, bool> Tage { get; set; }
        public Dictionary<int, ImportTimeSpan> Stunden { get; set; }

        public ICollection<Zuordnung> GruppenZuordnungen { get; set; }

        public ICollection<Restriktion> Restriktionen { get; set; }
        public ICollection<RaumBlockade> Blockaden { get; set; }
    }

    public class ImportTimeSpan
    {
        public DateTime Start { get; set; }
        public DateTime Ende { get; set; }
    }
}
