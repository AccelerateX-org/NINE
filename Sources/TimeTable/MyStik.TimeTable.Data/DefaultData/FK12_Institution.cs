namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK12()
        {
            var fk12 = GetOrganiser("FK 12");

            if (fk12 != null) return;

            fk12 = new ActivityOrganiser
            {
                Name = "Fakultät 12",
                ShortName = "FK 12",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk12);
            _db.SaveChanges();
        }

        public void InitMemberFK12()
        {
            var fk12 = GetOrganiser("FK 12");
            AddMember(fk12, "NAU", "Naumann");
            AddMember(fk12, "AMR", "Ammer");
            AddMember(fk12, "BUC", "Buchner");
            AddMember(fk12, "CIG", "Cigirac");
            AddMember(fk12, "DAM", "Dam");
            AddMember(fk12, "DEU", "Deumling");
            AddMember(fk12, "FRE", "Frenzl");
            AddMember(fk12, "GRS", "Gress");
            AddMember(fk12, "GUE", "Günther");
            AddMember(fk12, "KIE", "Kießling");
            AddMember(fk12, "NIE", "Niebler");
            AddMember(fk12, "OST", "Ostermann");
            AddMember(fk12, "PET", "Petri");
            AddMember(fk12, "SAN", "Santo");
            AddMember(fk12, "STE", "Stetzer");
            AddMember(fk12, "KEL", "Keller");
            AddMember(fk12, "BIR", "Birkner");
            AddMember(fk12, "WIC", "Wickenheiser", true);
        }

        public void InitCurriculaFK12()
        {
            var fk12 = GetOrganiser("FK 12");

            FK12_InitCurriculumDE(fk12);
            FK12_InitCurriculumMDE(fk12);
        }

        public void InitModulesFK12()
        {
            var fk12 = GetOrganiser("FK 12");

            InitModulkatalogDE(fk12);
            InitModulkatalogMDE(fk12);

        }


    }

}