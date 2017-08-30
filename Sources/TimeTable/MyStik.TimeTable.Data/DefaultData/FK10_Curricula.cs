using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK10_InitCurriculumBW(ActivityOrganiser fk10)
        {
            var bw = GetCurriculum(fk10, "BW");

            if (bw != null)
                return;

            // Studienprogramme
            bw = new Curriculum
            {
                ShortName = "BW",
                Name = "Bachelor Betriebswirtschaft",
                Organiser = fk10,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "PF", IsSubscribable = false}, // Allgemein
                    new CurriculumGroup { Name = "WPF", IsSubscribable = false},
                    new CurriculumGroup { Name = "Schwerpunkt", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(bw);

            AddCapacityGroup(bw, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(bw, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(bw, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(bw, "1", "D", true, true, new string[] { "1D" });

            AddCapacityGroup(bw, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(bw, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(bw, "2", "C", true, true, new string[] { "2C" });
            AddCapacityGroup(bw, "2", "D", true, true, new string[] { "2D" });

            AddCapacityGroup(bw, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(bw, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(bw, "3", "C", true, true, new string[] { "3C" });
            AddCapacityGroup(bw, "3", "D", true, true, new string[] { "3D" });

            AddCapacityGroup(bw, "PF", "", true, true, new string[] { "6 allg", "7 allg", "Allg" });
            AddCapacityGroup(bw, "WPF", "", true, true, new string[] { "5WPF" });
            AddCapacityGroup(bw, "Schwerpunkt", "BC", true, true, new string[] { "BC" });
            AddCapacityGroup(bw, "Schwerpunkt", "BF", true, true, new string[] { "BF" });
            AddCapacityGroup(bw, "Schwerpunkt", "BHR", true, true, new string[] { "BHR" });
            AddCapacityGroup(bw, "Schwerpunkt", "BI", true, true, new string[] { "BI" });
            AddCapacityGroup(bw, "Schwerpunkt", "BL", true, true, new string[] { "BL" });
            AddCapacityGroup(bw, "Schwerpunkt", "BM", true, true, new string[] { "BM" });
            AddCapacityGroup(bw, "Schwerpunkt", "BP", true, true, new string[] { "BP" });
            AddCapacityGroup(bw, "Schwerpunkt", "BS", true, true, new string[] { "BS" });

            _db.SaveChanges();
        }

        public void FK10_InitCurriculumWIF(ActivityOrganiser fk10)
        {
            var bw = GetCurriculum(fk10, "WIF");

            if (bw != null)
                return;

            // Studienprogramme
            bw = new Curriculum
            {
                ShortName = "WIF",
                Name = "Bachelor Wirtschaftsinformatik",
                Organiser = fk10,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(bw);

            AddCapacityGroup(bw, "1", "A", true, true, new string[] { "WI-1A" });
            AddCapacityGroup(bw, "1", "B", true, true, new string[] { "WI-1B" });
            AddCapacityGroup(bw, "1", "C", true, true, new string[] { "WI-1C" });

            AddCapacityGroup(bw, "2", "A", true, true, new string[] { "WI-2A" });
            AddCapacityGroup(bw, "2", "B", true, true, new string[] { "WI-2B" });
            AddCapacityGroup(bw, "2", "C", true, true, new string[] { "WI-2C" });

            AddCapacityGroup(bw, "3", "A", true, true, new string[] { "WI-3A" });
            AddCapacityGroup(bw, "3", "B", true, true, new string[] { "WI-3B" });
            AddCapacityGroup(bw, "3", "C", true, true, new string[] { "WI-3C" });

            AddCapacityGroup(bw, "4", "A", true, true, new string[] { "WI-4A" });
            AddCapacityGroup(bw, "4", "B", true, true, new string[] { "WI-4B" });
            AddCapacityGroup(bw, "4", "C", true, true, new string[] { "WI-4C" });

            AddCapacityGroup(bw, "6", "A", true, true, new string[] { "WI-6A" });
            AddCapacityGroup(bw, "6", "B", true, true, new string[] { "WI-6B" });
            AddCapacityGroup(bw, "6", "C", true, true, new string[] { "WI-6C" });

            AddCapacityGroup(bw, "7", "", true, true, new string[] { "WI-7" });

            _db.SaveChanges();
        }

        private void InitSimpleCurriculum(ActivityOrganiser fk10, string name, string shortName, string alias)
        {
            var bw = GetCurriculum(fk10, shortName);

            if (bw != null)
                return;

            // Studienprogramme
            bw = new Curriculum
            {
                ShortName = shortName,
                Name = name,
                Organiser = fk10,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = shortName, IsSubscribable = true},
                 },
            };

            _db.Curricula.Add(bw);

            AddCapacityGroup(bw, shortName, "", true, true, new string[] { alias });

            _db.SaveChanges();

        }

        public void FK10_InitCurriculumMisc(ActivityOrganiser fk10)
        {
            InitSimpleCurriculum(fk10, "Bachelor International Business Administration", "IB", "IB");
            InitSimpleCurriculum(fk10, "Bachelor Betriebswirtschaft und Unternehmensführung", "BAUF", "BAUF");
            InitSimpleCurriculum(fk10, "Master Business Innovation and Management Consulting", "ME", "ME");
            InitSimpleCurriculum(fk10, "Master Finance and Controlling", "MF", "MF");
            InitSimpleCurriculum(fk10, "Master Marketingmanagement", "MM", "MM");
            InitSimpleCurriculum(fk10, "Master Business Entrepreneurship and Digital Technolgy Management", "MD", "MD");
            InitSimpleCurriculum(fk10, "Master Betriebliche Steuerlehre", "MOT", "MOT");
            InitSimpleCurriculum(fk10, "Master Wirtschaftinformatik", "WI-M", "WI-M");
            InitSimpleCurriculum(fk10, "Master Personalmanagement", "HRM", "HRM");
            InitSimpleCurriculum(fk10, "Räume für FK11", "FK11", "FK11");
        }


    }
}

