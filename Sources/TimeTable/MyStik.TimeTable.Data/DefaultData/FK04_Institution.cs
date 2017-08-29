namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK04()
        {
            var fk04 = GetOrganiser("FK 04");

            if (fk04 != null) return;

            fk04 = new ActivityOrganiser
            {
                Name = "Fakultät 04",
                ShortName = "FK 04",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk04);
            _db.SaveChanges();
        }

        public void InitMemberFK04()
        {
            var fk04 = GetOrganiser("FK 04");
            AddMember(fk04, "BOH", "Bohlen");
            AddMember(fk04, "BRM", "Bruecklmeier");
            AddMember(fk04, "DIP", "Dippold");
            AddMember(fk04, "FEI", "Feiertag");
            AddMember(fk04, "GEN", "Geng");
            AddMember(fk04, "GER", "Gerstner");
            AddMember(fk04, "GRA", "Graf");
            AddMember(fk04, "HEK", "Hecker");
            AddMember(fk04, "HIB", "Hiebel");
            AddMember(fk04, "HIR", "Hirschmann");
            AddMember(fk04, "HOE", "Höger");
            AddMember(fk04, "IRB", "Irber");
            AddMember(fk04, "KAH", "Kahl");
            AddMember(fk04, "KIS", "Kißling");
            AddMember(fk04, "KLE", "Klein");
            AddMember(fk04, "MAY", "Mayr");
            AddMember(fk04, "MEY", "Meyberg");
            AddMember(fk04, "MIC", "Michael");
            AddMember(fk04, "MUE", "Mühlbauer");
            AddMember(fk04, "MUN", "Münker");
            AddMember(fk04, "PAL", "Palm");
            AddMember(fk04, "PAU", "Paul");
            AddMember(fk04, "PLA", "Plate");
            AddMember(fk04, "RAC", "Rackles");
            AddMember(fk04, "RAP", "Rapp");
            AddMember(fk04, "RAU", "Rauh");
            AddMember(fk04, "REH", "Rehm");
            AddMember(fk04, "RES", "Ressel");
            AddMember(fk04, "ROS", "Rosehr");
            AddMember(fk04, "SCH", "Schillhuber");
            AddMember(fk04, "SCM", "Schmitt");
            AddMember(fk04, "SCR", "Schramm");
            AddMember(fk04, "SHR", "S. Schramm");
            AddMember(fk04, "SCO", "Schöttl");
            AddMember(fk04, "SEC", "Seck");
            AddMember(fk04, "SOM", "Sommer");
            AddMember(fk04, "STE", "Stehr");
            AddMember(fk04, "STR", "Strauß");
            AddMember(fk04, "STI", "Striegler");
            AddMember(fk04, "UNT", "Unterricker");
            AddMember(fk04, "ZUC", "Zuccaro");
            AddMember(fk04, "WAG", "Wagenhäuser");
            AddMember(fk04, "MAY", "Mayer");
            AddMember(fk04, "Walter", "Tasin", true);
        }

        public void InitCurriculaFK04()
        {
            var fk04 = GetOrganiser("FK 04");

            FK04_InitCurriculumBA(fk04);
            FK04_InitCurriculumEIB(fk04);
            FK04_InitCurriculumREB(fk04);
            FK04_InitCurriculumEMB(fk04);
            FK04_InitCurriculumELM(fk04);
            FK04_InitCurriculumEEM(fk04);
            FK04_InitCurriculumSMM(fk04);

        }

        public void InitModulesFK04()
        {
            var fk04 = GetOrganiser("FK 04");

        }

    }

}