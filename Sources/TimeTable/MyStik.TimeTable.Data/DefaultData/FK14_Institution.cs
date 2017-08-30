namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK14()
        {
            var fk14 = GetOrganiser("FK 14");

            if (fk14 != null) return;

            fk14 = new ActivityOrganiser
            {
                Name = "Fakultät 14",
                ShortName = "FK 14",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk14);
            _db.SaveChanges();
        }

        public void InitMemberFK14()
        {
            var fk14 = GetOrganiser("FK 14");
            AddMember(fk14, "APU", "Achilles-Pujol");
            AddMember(fk14, "BAU", "Bauer");
            AddMember(fk14, "BAS", "Bausch");
            AddMember(fk14, "BER", "Berchtenbreiter");
            AddMember(fk14, "BUS", "Busacker");
            AddMember(fk14, "CHG", "Chang");
            AddMember(fk14, "FRY", "Freyberg");
            AddMember(fk14, "GOE", "Goecke");
            AddMember(fk14, "GES", "Greischel");
            AddMember(fk14, "GRU", "Gruner");
            AddMember(fk14, "KLA", "Klassen");
            AddMember(fk14, "KOL", "Kolbeck");
            AddMember(fk14, "MET", "Metzler");
            AddMember(fk14, "MUN", "Munz");
            AddMember(fk14, "REI", "Reitsam");
            AddMember(fk14, "STE", "Sterzenbach", true);
        }

        public void InitCurriculaFK14()
        {
            var fk14 = GetOrganiser("FK 14");

            FK14_InitCurriculumTourismus(fk14);
            FK14_InitCurriculumTourismusM(fk14);
            FK14_InitCurriculumHospitalityM(fk14);
        }

        public void InitModulesFK14()
        {
            var fk14 = GetOrganiser("FK 14");

        }


    }

}