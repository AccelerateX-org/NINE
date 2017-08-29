namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK09()
        {
            var fk09 = GetOrganiser("FK 09");

            if (fk09 != null) return;

            fk09 = new ActivityOrganiser
            {
                Name = "Fakultät 09",
                ShortName = "FK 09",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk09);
            _db.SaveChanges();
        }

        public void InitMemberFK09()
        {
            var fk09 = GetOrganiser("FK 09");

            AddMember(fk09, "ANZ", "Anzinger");
            AddMember(fk09, "BRB", "Brombach");
            AddMember(fk09, "COR", "Cornelius");
            AddMember(fk09, "DAE", "Däubel");
            AddMember(fk09, "DOE", "Döhl");
            AddMember(fk09, "ELI", "Elias");
            AddMember(fk09, "ENB", "Englbrecht");
            AddMember(fk09, "ENG", "Englberger");
            AddMember(fk09, "GAB", "Gabrysch");
            AddMember(fk09, "GLS", "Glas");
            AddMember(fk09, "GÜN", "Günther");
            AddMember(fk09, "HIN", "Hinz");
            AddMember(fk09, "HOFF", "Hoffmann");
            AddMember(fk09, "HUB", "Huber");
            AddMember(fk09, "KOE", "Koether");
            AddMember(fk09, "KRA", "Krahe");
            AddMember(fk09, "KUR", "Kurz");
            AddMember(fk09, "MAU", "Mauerer");
            AddMember(fk09, "MCI", "McIntosh");
            AddMember(fk09, "MER", "Meier");
            AddMember(fk09, "MST", "Meier-Staude");
            AddMember(fk09, "OST", "Osterchrist");
            AddMember(fk09, "PIR", "Pischeltsrieder");
            AddMember(fk09, "PUC", "Puchan");
            AddMember(fk09, "RAB", "Raber");
            AddMember(fk09, "REB", "Rebhan");
            AddMember(fk09, "RUE", "Rühlemann");
            AddMember(fk09, "SAC", "Sachenbacher");
            AddMember(fk09, "SCH", "Schick");
            AddMember(fk09, "SCN", "Schönecker");
            AddMember(fk09, "SCU", "Schulz");
            AddMember(fk09, "SPI", "Spitznagel");
            AddMember(fk09, "STU", "Stumpp");
            AddMember(fk09, "TEI", "Teich");
            AddMember(fk09, "TMF", "Meier-Fohrbeck");
            AddMember(fk09, "TRE", "Trebesius");
            AddMember(fk09, "VOEL", "Voelkmann");
            AddMember(fk09, "WLH", "Wilrich");
            AddMember(fk09, "WOL", "Wolf");
            AddMember(fk09, "WUN", "Wünsche", true);
        }

        public void InitCurriculaFK09()
        {
            var fk09 = GetOrganiser("FK 09");

            InitCurriculumWI(fk09);
            InitCurriculumLM(fk09);
            InitCurriculumAU(fk09);
            InitCurriculumWIM(fk09);
            InitCurriculumMBA(fk09);
        }

        public void InitModulesFK09()
        {
            var fk09 = GetOrganiser("FK 09");

            InitModulkatalogWI(fk09);
        }

    }
}
