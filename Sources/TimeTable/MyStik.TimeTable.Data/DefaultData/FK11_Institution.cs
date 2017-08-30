namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK11()
        {
            var fk11 = GetOrganiser("FK 11");

            if (fk11 != null) return;

            fk11 = new ActivityOrganiser
            {
                Name = "Fakultät 11",
                ShortName = "FK 11",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk11);
            _db.SaveChanges();
        }

        public void InitMemberFK11()
        {
            var fk11 = GetOrganiser("FK 11");
            AddMember(fk11, "AMU", "Anane-Mundthal");
            AddMember(fk11, "ARN", "Arnold");
            AddMember(fk11, "BEK", "Beck");
            AddMember(fk11, "BER", "Beranek");
            AddMember(fk11, "BRU", "Bruin");
            AddMember(fk11, "BLD", "Boldt");
            AddMember(fk11, "BKR", "Boos-Krüger");
            AddMember(fk11, "BON", "Broenner");
            AddMember(fk11, "BUT", "Buttner");
            AddMember(fk11, "DAI", "Daiminger");
            AddMember(fk11, "DOT", "Dotzler");
            AddMember(fk11, "DUE", "Dürr");
            AddMember(fk11, "ENG", "Engelfried");
            AddMember(fk11, "GEH", "Gehra");
            AddMember(fk11, "GOS", "Gosch");
            AddMember(fk11, "HER", "Herold-Majumdar");
            AddMember(fk11, "HIL", "Hill");
            AddMember(fk11, "ISE", "Iser");
            AddMember(fk11, "JAN", "Janßen");
            AddMember(fk11, "KAU", "Kaufmann");
            AddMember(fk11, "KLO", "Klöck");
            AddMember(fk11, "KOT", "Kötter");
            AddMember(fk11, "LEC", "Lechner");
            AddMember(fk11, "LIM", "Limm");
            AddMember(fk11, "MAR", "Martius");
            AddMember(fk11, "MUT", "Mutz");
            AddMember(fk11, "NIT", "Nitsch");
            AddMember(fk11, "POE", "Pötter");
            AddMember(fk11, "POH", "Pohlmann");
            AddMember(fk11, "REI", "Reinhardt");
            AddMember(fk11, "RER", "Rerrich");
            AddMember(fk11, "SAG", "Sagebiel");
            AddMember(fk11, "SAN", "Sandmeir");
            AddMember(fk11, "SIN", "Schindler");
            AddMember(fk11, "SOE", "Schönberger");
            AddMember(fk11, "STE", "Stecklina");
            AddMember(fk11, "STI", "Steindorff-Classen");
            AddMember(fk11, "STR", "Stracke-Baumann");
            AddMember(fk11, "STR", "Strupeit");
            AddMember(fk11, "UEF", "Ueffing");
            AddMember(fk11, "VIE", "Vierzigmann");
            AddMember(fk11, "WEB", "Weber-Teuber");
            AddMember(fk11, "WEG", "Wegner");
            AddMember(fk11, "WIT", "Witzmann");
            AddMember(fk11, "YOL", "Yollu-Tok");
            AddMember(fk11, "ZIN", "Zink", true);
        }

        public void InitCurriculaFK11()
        {
            var fk11 = GetOrganiser("FK 11");

            FK11_InitCurriculumSA(fk11);
            FK11_InitCurriculumSAT(fk11);
            FK11_InitCurriculumSABASA(fk11);
            FK11_InitCurriculumBIERKI(fk11);
            FK11_InitCurriculumMSI(fk11);
            FK11_InitCurriculumPFL(fk11);
            FK11_InitCurriculumAFSA(fk11);
            FK11_InitCurriculumGWT(fk11);
            FK11_InitCurriculumANP(fk11);
            FK11_InitCurriculumDBI(fk11);
            FK11_InitCurriculumGLOE(fk11);
            FK11_InitCurriculumMH(fk11);
            FK11_InitCurriculumPSY(fk11);
            FK11_InitCurriculumSOZI(fk11);
        }

        public void InitModulesFK11()
        {
            var fk11 = GetOrganiser("FK 11");

            InitModulkatalogSA(fk11);
        }

    }

}