namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Sprechstunden werden pro Semester angelegt
    /// Die Zuordnung zu Dozenten erfolgt in den Terminen
    /// </summary>
    public class OfficeHour : Activity
    {
        /// <summary>
        /// Keine Termine
        /// </summary>
        public bool ByAgreement { get; set; }

        /// <summary>
        /// offene Tür, d.h. keine Terminbuchung erforderlich
        /// </summary>
        public bool? IsOpenDoor { get; set; }


        /// <summary>
        /// Anzahl der Slots pro Termin
        /// </summary>
        public int? SlotsPerDate { get; set; }

        /// <summary>
        /// Anzahl der Slots, die in der Zukunft gebucht werden können
        /// Zukunft = Nach der letzen Buchung, falls vorhanden
        /// </summary>
        public int? FutureSubscriptions { get; set; }
    }

    public class PersonalDate : Activity
    {
        public int Preference { get; set; }

    }
}
