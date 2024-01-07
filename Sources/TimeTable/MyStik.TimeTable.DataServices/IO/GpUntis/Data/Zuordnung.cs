namespace MyStik.TimeTable.DataServices.IO.GpUntis.Data
{
    public class Zuordnung
    {
        public string Studiengang { get; set; }

        public string Studiengruppe { get; set; }

        public string Kapazitätsgruppe { get; set; }

        public string Alias { get; set; }

        public string LabelName
        {
            get
            {
                if (string.IsNullOrEmpty(Kapazitätsgruppe))
                    return Studiengruppe;

                if (Studiengruppe.Contains(" "))
                    return $"{Studiengruppe} - {Kapazitätsgruppe}";

                return $"{Studiengruppe}{Kapazitätsgruppe}";
            }
        }
    }
}
