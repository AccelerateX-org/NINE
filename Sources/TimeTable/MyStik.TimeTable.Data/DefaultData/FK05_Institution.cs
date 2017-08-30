namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK05()
        {
            var fk05 = GetOrganiser("FK 05");

            if (fk05 != null) return;

            fk05 = new ActivityOrganiser
            {
                Name = "Fakultät 05",
                ShortName = "FK 05",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk05);
            _db.SaveChanges();
        }

        public void InitMemberFK05()
        {
            var fk05 = GetOrganiser("FK 05");
            AddMember(fk05, "EBT", "Ebert");
            AddMember(fk05, "EHL", "Ehlers");
            AddMember(fk05, "HEZ", "Herz");
            AddMember(fk05, "IBR", "Ibrom");
            AddMember(fk05, "JEN", "Jensch");
            AddMember(fk05, "KRA", "Kraus");
            AddMember(fk05, "MAD", "Madjidi");
            AddMember(fk05, "MAI", "Mair");
            AddMember(fk05, "MUB", "Mühlbacher");
            AddMember(fk05, "RAS", "Rasthofer");
            AddMember(fk05, "REN", "Renner");
            AddMember(fk05, "SCH", "Schenk");
            AddMember(fk05, "SWE", "Schweigler");
            AddMember(fk05, "WIS", "Wieser");
            AddMember(fk05, "ZIE", "Ziegler");
            AddMember(fk05, "PAE", "Paerschke");
            AddMember(fk05, "PIE", "Pietsch");
            AddMember(fk05, "LIE", "Liepsch");
            //Papier- und Verpackungstechnik
            AddMember(fk05, "ANG", "Angerhöfer");
            AddMember(fk05, "BUR", "Burth");
            AddMember(fk05, "EGG", "Eggerath");
            AddMember(fk05, "GIE", "Giera");
            AddMember(fk05, "KLE", "Kleemann");
            AddMember(fk05, "NAU", "Naujock");
            AddMember(fk05, "WEB", "Weber");
            AddMember(fk05, "ZIE", "Ziegler");
            AddMember(fk05, "ZOL", "´Zollner-Croll");
            //DRUCK- UND MEDIENTECHNIK
            AddMember(fk05, "BER", "Berchtold");
            AddMember(fk05, "BON", "Bonefeld");
            AddMember(fk05, "DEL", "Delp");
            AddMember(fk05, "FIL", "Fillmann");
            AddMember(fk05, "IBR", "Ibrom");
            AddMember(fk05, "KRE", "Kreulich");
            AddMember(fk05, "KUE", "Kuen");
            AddMember(fk05, "LUI", "Luidl");
            AddMember(fk05, "MOS", "Moosheimer");
            AddMember(fk05, "WOE", "Wölflick");
            AddMember(fk05, "GIE", "Giera", true);
        }

        public void InitCurriculaFK05()
        {
            var fk05 = GetOrganiser("FK 05");

            FK05_InitCurriculumEG(fk05);
            FK05_InitCurriculumPV(fk05);
            FK05_InitCurriculumDM(fk05);
            FK05_InitCurriculumTRK(fk05);
        }

        public void InitModulesFK05()
        {
            var fk05 = GetOrganiser("FK 05");

            InitModulkatalogEG(fk05);

        }


    }

}