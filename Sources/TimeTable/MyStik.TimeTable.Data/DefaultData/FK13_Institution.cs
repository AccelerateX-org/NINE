namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK13()
        {
            var fk13 = GetOrganiser("FK 13");

            if (fk13 != null) return;

            fk13 = new ActivityOrganiser
            {
                Name = "Fakultät 13",
                ShortName = "FK 13",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk13);
            _db.SaveChanges();
        }

        public void InitMemberFK13()
        {
            var fk13 = GetOrganiser("FK 13");

            AddMember(fk13, "BRA", "Brandstetter");
            AddMember(fk13, "BRU", "Brunner");
            AddMember(fk13, "DOB", "Dobler");
            AddMember(fk13, "HEL", "von Helmolt");
            AddMember(fk13, "ITT", "Ittstein");
            AddMember(fk13, "JAE", "Järvenpää");
            AddMember(fk13, "JAN", "Jandok");
            AddMember(fk13, "KAM", "Kaminski");
            AddMember(fk13, "KUR", "Kurz");
            AddMember(fk13, "MATT", "Mattedi-Puhr-Westerheide");
            AddMember(fk13, "PER", "Peral");
            AddMember(fk13, "RAP", "Rappenglück");
            AddMember(fk13, "STO", "Stoffels");
            AddMember(fk13, "ZIM", "Zimmermann");
            AddMember(fk13, "BEL", "Belwe");
            AddMember(fk13, "REY", "Reyna");
            AddMember(fk13, "SCH", "Schutz");
            AddMember(fk13, "SIN", "Sinn", true);
        }

        public void InitCurriculaFK13()
        {
            var fk13 = GetOrganiser("FK 13");

            FK13_InitCurriculumAW(fk13);
        }

        public void InitModulesFK13()
        {
            var fk13 = GetOrganiser("FK 13");

            InitModulkatalogAW(fk13);
        }


    }
}
