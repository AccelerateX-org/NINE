using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK11_InitCurriculumSA(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "SA");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "SA",
                Name = "Bachelor Soziale Arbeit Vollzeit",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(sa, "7", "A", true, true, new string[] { "7A" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumSAT(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "SAT");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "SAT",
                Name = "Bachelor Soziale Arbeit Teilzeit",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "8", IsSubscribable = true},
                    new CurriculumGroup { Name = "9", IsSubscribable = true},
                    new CurriculumGroup { Name = "10", IsSubscribable = true},
                    new CurriculumGroup { Name = "11", IsSubscribable = true},
                    new CurriculumGroup { Name = "12", IsSubscribable = true},
                    new CurriculumGroup { Name = "13", IsSubscribable = true},
                    new CurriculumGroup { Name = "14", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(sa, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(sa, "8", "A", true, true, new string[] { "8A" });
            AddCapacityGroup(sa, "9", "A", true, true, new string[] { "9A" });
            AddCapacityGroup(sa, "10", "A", true, true, new string[] { "10A" });
            AddCapacityGroup(sa, "11", "A", true, true, new string[] { "11A" });
            AddCapacityGroup(sa, "12", "A", true, true, new string[] { "12A" });
            AddCapacityGroup(sa, "13", "A", true, true, new string[] { "13A" });
            AddCapacityGroup(sa, "14", "A", true, true, new string[] { "14A" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumSABASA(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "SABASA");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "SABASA",
                Name = "Bachelor Soziale Arbeit basa-online",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "8", IsSubscribable = true},

                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(sa, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(sa, "8", "A", true, true, new string[] { "8A" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumBIERKI(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "BIERKI");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "BIERKI",
                Name = "Bachelor Bildung und Erziehung im Kindesalter",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "4", "B", true, true, new string[] { "4B" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "5", "B", true, true, new string[] { "5B" });
            AddCapacityGroup(sa, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(sa, "6", "B", true, true, new string[] { "6B" });
            AddCapacityGroup(sa, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(sa, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumMSI(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "MSI");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "MSI",
                Name = "Bachelor Management Sozialer Innovationen",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "4", "B", true, true, new string[] { "4B" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "5", "B", true, true, new string[] { "5B" });
            AddCapacityGroup(sa, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(sa, "6", "B", true, true, new string[] { "6B" });
            AddCapacityGroup(sa, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(sa, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }
        public void FK11_InitCurriculumPFL(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "PFL");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "PFL",
                Name = "Bachelor Pflege",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "8", IsSubscribable = true},
                    new CurriculumGroup { Name = "9", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "4", "B", true, true, new string[] { "4B" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "5", "B", true, true, new string[] { "5B" });
            AddCapacityGroup(sa, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(sa, "6", "B", true, true, new string[] { "6B" });
            AddCapacityGroup(sa, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(sa, "7", "B", true, true, new string[] { "7B" });
            AddCapacityGroup(sa, "8", "A", true, true, new string[] { "8A" });
            AddCapacityGroup(sa, "8", "B", true, true, new string[] { "8B" });
            AddCapacityGroup(sa, "9", "A", true, true, new string[] { "9A" });
            AddCapacityGroup(sa, "9", "B", true, true, new string[] { "9B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumAFSA(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "AFSA");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "AFSA",
                Name = "Master Angewandte Forschung in der Sozialen Arbeit",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "Vollzeit", true, true, new string[] { "1Vollzeit" });
            AddCapacityGroup(sa, "1", "Teilzeit", true, true, new string[] { "1Teilzeit" });
            AddCapacityGroup(sa, "2", "Vollzeit", true, true, new string[] { "2Vollzeit" });
            AddCapacityGroup(sa, "2", "Teilzeit", true, true, new string[] { "2Teilzeit" });
            AddCapacityGroup(sa, "3", "Vollzeit", true, true, new string[] { "3Vollzeit" });
            AddCapacityGroup(sa, "3", "Teilzeit", true, true, new string[] { "3Teilzeit" });
            AddCapacityGroup(sa, "4", "Teilzeit", true, true, new string[] { "4Teilzeit" });
            AddCapacityGroup(sa, "5", "Teilzeit", true, true, new string[] { "5Teilzeit" });
            AddCapacityGroup(sa, "6", "Teilzeit", true, true, new string[] { "6Teilzeit" });
            

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumGWT(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "GWT");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "GWT",
                Name = "Master Gesellschaftlicher Wandel und Teilhabe",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},

                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumANP(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "ANP");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "ANP",
                Name = "Master Advanced Nursing Practice (ANP)",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(sa, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(sa, "4", "B", true, true, new string[] { "4B" });
            AddCapacityGroup(sa, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(sa, "5", "B", true, true, new string[] { "5B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumDBI(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "DBI");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "DBI",
                Name = "Master Diagnostik, Beratung und Intervention",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumGLOE(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "GLOE");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "GLOE",
                Name = "Master Gemeinwesenentwicklung und Lokale Oekonomie",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumMH(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "MH");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "MH",
                Name = "Master Mental Health",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumPSY(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "PSY");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "PSY",
                Name = "Master Psychotherapie (Erwachsene)",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });

            _db.SaveChanges();
        }

        public void FK11_InitCurriculumSOZI(ActivityOrganiser fk11)
        {
            var sa = GetCurriculum(fk11, "SOZI");

            if (sa != null)
                return;

            // Studienprogramme
            sa = new Curriculum
            {
                ShortName = "SOZI",
                Name = "Master Sozialmanagement",
                Organiser = fk11,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(sa);

            AddCapacityGroup(sa, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(sa, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(sa, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(sa, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(sa, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(sa, "3", "B", true, true, new string[] { "3B" });

            _db.SaveChanges();
        }

    }
}