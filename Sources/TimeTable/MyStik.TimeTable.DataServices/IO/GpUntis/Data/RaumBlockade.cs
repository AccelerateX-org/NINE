namespace MyStik.TimeTable.DataServices.IO.GpUntis.Data
{
    public class RaumBlockade : ImportBase
    {
        public Raum Raum { get; set; }

        public int Tag { get; set; }
        public int VonStunde { get; set; }
        public int BisStunde { get; set; }


    }
}
