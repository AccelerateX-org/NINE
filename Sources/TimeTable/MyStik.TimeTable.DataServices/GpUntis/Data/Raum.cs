namespace MyStik.TimeTable.DataServices.GpUntis.Data
{
    public class Raum : ImportBase
    {
        public string RaumID { get; set; }
        public string Nummer { get; set; }

        public int Kapazitaet { get; set; }

        public string Beschreibung { get; set; }

        public string Nutzer { get; set; }
    }
}
