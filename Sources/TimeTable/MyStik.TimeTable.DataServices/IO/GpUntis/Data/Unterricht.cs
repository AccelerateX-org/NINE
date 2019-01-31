namespace MyStik.TimeTable.DataServices.IO.GpUntis.Data
{
    public class Unterricht
    {
        public int UnterrichtID { get; set; }
        public string GruppeID { get; set; }
        public string DozentID { get; set; }
        public string FachID { get; set; }
        public string RaumID { get; set; }

        public int Tag { get; set; }
        public int Stunde { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", UnterrichtID, GruppeID, DozentID, FachID);
        }

    }
}
