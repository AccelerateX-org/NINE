namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK02()
        {
            var fk02 = GetOrganiser("FK 02");

            if (fk02 != null) return;

            fk02 = new ActivityOrganiser
            {
                Name = "Fakultät 02",
                ShortName = "FK 02",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk02);
            _db.SaveChanges();
        }
        public void InitMemberFK02()
        {
            var fk02 = GetOrganiser("FK 02");
            AddMember(fk02, "Ack", "Ackermann");
            AddMember(fk02, "ANS", "Ansorge");
            AddMember(fk02, "BIS", "Bisani");
            AddMember(fk02, "BOS", "Bosl");
            AddMember(fk02, "CLA", "Clausen");
            AddMember(fk02, "DAU", "Dauberschmidt");
            AddMember(fk02, "DUE", "Dürr");
            AddMember(fk02, "EGE", "Eger");
            AddMember(fk02, "ENG", "Engelhardt");
            AddMember(fk02, "FRE", "Freimann");
            AddMember(fk02, "GAE", "Gäßler");
            AddMember(fk02, "GEB", "Gebhard");
            AddMember(fk02, "HAU", "Hausser");
            AddMember(fk02, "HOL", "Holm");
            AddMember(fk02, "JUN", "Jungwirth");
            AddMember(fk02, "KAI", "Kainz");
            AddMember(fk02, "KEL", "Kellner");
            AddMember(fk02, "KNE", "Kneidl");
            AddMember(fk02, "KON", "Konrad");
            AddMember(fk02, "KUS", "Kustermann");
            AddMember(fk02, "SMI", "Schmidt");
            AddMember(fk02, "SOL", "Scholz");
            AddMember(fk02, "SCU", "Schuler");
            AddMember(fk02, "SUL", "Schulte");
            AddMember(fk02, "SEE", "Seeßelberg");
            AddMember(fk02, "SEI", "Seiler");
            AddMember(fk02, "SLO", "Slominski");
            AddMember(fk02, "SPA", "Spannring");
            AddMember(fk02, "STE", "Steinmann", true);



        }

        public void InitCurriculaFK02()
        {
            var fk02 = GetOrganiser("FK 02");

            FK02_InitCurriculumBAU(fk02);
            FK02_InitCurriculumBAUDUAL(fk02);
            FK02_InitCurriculumMAG(fk02);
            FK02_InitCurriculumMAI(fk02);
        }

        public void InitModulesFK02()
        {
            var fk02 = GetOrganiser("FK 02");

            InitModulkatalogBAU(fk02);
            InitModulkatalogBAUDUAL(fk02);
        }

    }
}
