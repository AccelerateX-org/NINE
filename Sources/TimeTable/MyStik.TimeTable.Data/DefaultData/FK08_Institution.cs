namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK08()
        {
            var fk08 = GetOrganiser("FK 08");

            if (fk08 != null) return;

            fk08 = new ActivityOrganiser
            {
                Name = "Fakultät 08",
                ShortName = "FK 08",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk08);
            _db.SaveChanges();
        }

        public void InitMemberFK08()
        {
            var fk08 = GetOrganiser("FK 08");
            AddMember(fk08, "ABM", "Abmayr");
            AddMember(fk08, "BUZ", "Buzin");
            AddMember(fk08, "CZA", "Czaja");
            AddMember(fk08, "EIC", "Eich-Soellner");
            AddMember(fk08, "FIS", "Fischer");
            AddMember(fk08, "FOR", "Forster");
            AddMember(fk08, "HAN", "Hahn");
            AddMember(fk08, "HOE", "Hörwick");
            AddMember(fk08, "HUB", "Huebner");
            AddMember(fk08, "JOS", "Joos");
            AddMember(fk08, "KAM", "Kammerer");
            AddMember(fk08, "KIR", "Kirschenabuer");
            AddMember(fk08, "KLU", "Klauer");
            AddMember(fk08, "KRZ", "Krzystek");
            AddMember(fk08, "LEI", "Leischnig");
            AddMember(fk08, "LOT", "Lother");
            AddMember(fk08, "MIC", "Michael");
            AddMember(fk08, "N.N.", "N.N.");
            AddMember(fk08, "OST", "Oster");
            AddMember(fk08, "SCI", "Schiedermeier");
            AddMember(fk08, "SMI", "Schmitt");
            AddMember(fk08, "SUE", "Schütz");
            AddMember(fk08, "SOC", "Socher");
            AddMember(fk08, "STR", "Strauß");
            AddMember(fk08, "TIE", "Tiede");
            AddMember(fk08, "TOR", "Tornow");
            AddMember(fk08, "Wimmer", "Tiede");
            AddMember(fk08, "ZOE", "Zöllner", true);
            AddMember(fk08, "ZUG", "Zugenmaier");
            AddMember(fk08, "LEI", "Leischnig");
            AddMember(fk08, "GD", "Praktikantenbeauftragte(r)");
            AddMember(fk08, "AND", "Alle Profs für Studium Generale und Interdisziplinäre");
            AddMember(fk08, "WIM", "Wimmer");
            AddMember(fk08, "TIE", "Tiede");
            AddMember(fk08, "WPM", "Dozenten sieheModulkatalog Wahlpflichtangebot der Bachelorstudiengänge");
            AddMember(fk08, "ALL", "Alle Profs des Studiengangs");


        }

        public void InitCurriculaFK08()
        {
            var fk08 = GetOrganiser("FK 08");

            FK08_InitCurriculumGUG(fk08);
            FK08_InitCurriculumGUN(fk08);
            FK08_InitCurriculumGeoinformatik(fk08);
            FK08_InitCurriculumKartographie(fk08);
            FK08_InitCurriculumMGEO(fk08);
            FK08_InitCurriculumMKAT(fk08);
        }

        public void InitModulesFK08()
        {
            var fk08 = GetOrganiser("FK 08");

        }


    }

}