namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void InitOrganisationFK06()
        {
            var fk06 = GetOrganiser("FK 06");

            if (fk06 != null) return;

            fk06 = new ActivityOrganiser
            {
                Name = "Fakultät 06",
                ShortName = "FK 06",
                IsFaculty = true,
                IsStudent = false,
            };

            _db.Organisers.Add(fk06);
            _db.SaveChanges();
        }

        public void InitMemberFK06()
        {
            var fk06 = GetOrganiser("FK 06");
            AddMember(fk06, "ALT", "Alt");
            AddMember(fk06, "BAE", "Bäßler");
            AddMember(fk06, "Brau", "Braun");
            AddMember(fk06, "CLS", "Clausen-Schaumann");
            AddMember(fk06, "DIM", "Diemer");
            AddMember(fk06, "EGR", "Eggers");
            AddMember(fk06, "EIS", "Eisenbarth");
            AddMember(fk06, "FIS", "Fickenscher");
            AddMember(fk06, "FSR", "Fischer");
            AddMember(fk06, "FIS", "Fischer");
            AddMember(fk06, "FRA", "Franz", true);
            AddMember(fk06, "FRO", "Froriep");
            AddMember(fk06, "HIR", "Hirth");
            AddMember(fk06, "HOL", "Holler");
            AddMember(fk06, "LIB", "Libon");
            AddMember(fk06, "N.N.", "N.N.");
            AddMember(fk06, "NIE", "Niessner");
            AddMember(fk06, "POL", "Pollok");
            AddMember(fk06, "SAL", "Salehi");
            AddMember(fk06, "FUC", "Fuchsberger");
            AddMember(fk06, "GIE", "Giebel");
            AddMember(fk06, "HIR", "Hirth");
            AddMember(fk06, "KOC", "Koch");
            AddMember(fk06, "LIE", "Liepsch");
            AddMember(fk06, "LIN", "Linner");
            AddMember(fk06, "MAI", "Maier");
            AddMember(fk06, "MAH", "Mahnke");
            AddMember(fk06, "NIE", "Niessner");
            AddMember(fk06, "SAS", "Sachs");
            AddMember(fk06, "SCH", "Schwankner");
            AddMember(fk06, "WAG", "Wagner");
            AddMember(fk06, "WON", "Wondrazek");
            AddMember(fk06, "VAS", "Vass");
            AddMember(fk06, "SHE", "Schenk");
            AddMember(fk06, "STE", "Steinhauser");
            AddMember(fk06, "LEI", "Leibl");
            AddMember(fk06, "WEB", "Webers");
            AddMember(fk06, "HER", "Hermann");
            AddMember(fk06, "WIE", "Wiedemann");
            AddMember(fk06, "PAR", "Parzhuber");
            AddMember(fk06, "SHL", "Schlüter");
            AddMember(fk06, "HAL", "Haller");
            AddMember(fk06, "DRE", "Dressler");
            AddMember(fk06, "N.N.", "Unbekannt");
            AddMember(fk06, "HILL", "Hilleringmann");
            AddMember(fk06, "STEI", "Steinhauser");
            AddMember(fk06, "SAC", "Sachs");
            AddMember(fk06, "MAH", "Mahnke");
            AddMember(fk06, "MAI", "Maier");
            AddMember(fk06, "STE", "Steinkogler");
            AddMember(fk06, "QU", "Qu");
            AddMember(fk06, "HER", "Herz");
            AddMember(fk06, "Men", "Menczigar");
            AddMember(fk06, "ZAN", "Zangl");
            AddMember(fk06, "TRE", "Trebesius");
            AddMember(fk06, "GRÜ", "Grüner-Lempart");
            AddMember(fk06, "HUB", "Huber");
            AddMember(fk06, "SCHW", "Schwankner");
            AddMember(fk06, "GER", "Gerstner");
            AddMember(fk06, "WEG", "Wegener");
            AddMember(fk06, "BRI", "Brill");
            AddMember(fk06, "GEI", "Geisweid");
            AddMember(fk06, "LES", "Lesser");
            AddMember(fk06, "SAR", "Sarlo");
        }

        public void InitCurriculaFK06()
        {
            var fk06 = GetOrganiser("FK 06");

            FK06_InitCurriculumAOB(fk06);
            FK06_InitCurriculumAOB(fk06);
            FK06_InitCurriculumPHT(fk06);
            FK06_InitCurriculumBBM(fk06);
            FK06_InitCurriculumBOB(fk06);
            FK06_InitCurriculumBOR(fk06);
            FK06_InitCurriculumMFB(fk06);
            FK06_InitCurriculumMFM(fk06);
            FK06_InitCurriculumMNM(fk06);
            FK06_InitCurriculumPAB(fk06);
            FK06_InitCurriculumPHB(fk06);
            FK06_InitCurriculumPNB(fk06);
        }

        public void InitModulesFK06()
        {
            var fk06 = GetOrganiser("FK 06");

        }


    }

}