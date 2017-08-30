namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK01()
        {
            var fk01 = GetOrganiser("FK 01");

            if (fk01 != null) return;

            fk01 = new ActivityOrganiser
            {
                Name = "Fakultät 01",
                ShortName = "FK 01",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk01);
            _db.SaveChanges();
        }

        public void InitMemberFK01()
        {
            var fk01 = GetOrganiser("FK 01");

            AddMember(fk01, "BAI", "Baier");
            AddMember(fk01, "BEN", "Benze");
            AddMember(fk01, "BER", "Berktold");
            AddMember(fk01, "BEE", "Beek");
            AddMember(fk01, "BOT", "Botti");
            AddMember(fk01, "BRU", "Bruno");
            AddMember(fk01, "ESS", "Eßig");
            AddMember(fk01, "HAM", "Hammer");
            AddMember(fk01, "HEN", "Henne");
            AddMember(fk01, "KAP", "Kappler");
            AddMember(fk01, "KEG", "Kegler");
            AddMember(fk01, "KRE", "Kretschmann");
            AddMember(fk01, "KUE", "Künzel");
            AddMember(fk01, "LAN", "Langenberg");
            AddMember(fk01, "MEC", "Meck");
            AddMember(fk01, "PAU", "Paulat");
            AddMember(fk01, "RIC", "Richarz");
            AddMember(fk01, "SCH", "Schiemann");
            AddMember(fk01, "SMI", "Schmid");
            AddMember(fk01, "WOL", "Wolfrum");
            AddMember(fk01, "WEB", "Weber");
            AddMember(fk01, "ZOL", "Zoll", true);
        }

        public void InitCurriculaFK01()
        {
            var fk01 = GetOrganiser("FK 01");

            FK01_InitCurriculumAR(fk01);
        }

        public void InitModulesFK01()
        {
            var fk01 = GetOrganiser("FK 01");

            InitModulkatalogAR(fk01);
        }

    }

}