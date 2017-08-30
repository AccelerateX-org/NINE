using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK14_InitCurriculumTourismus(ActivityOrganiser fk14)
        {
            var tourismus = GetCurriculum(fk14, "Tourismus");

            if (tourismus != null)
                return;

            // Studienprogramme
            tourismus = new Curriculum
            {
                ShortName = "Tourismus",
                Name = "Tourismus Management Bachelor",
                Organiser = fk14,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1B", IsSubscribable = true},
                    new CurriculumGroup { Name = "2B", IsSubscribable = true},
                    new CurriculumGroup { Name = "3B", IsSubscribable = true},
                    new CurriculumGroup { Name = "5B", IsSubscribable = true},
                    new CurriculumGroup { Name = "5SOC", IsSubscribable = true},
                    new CurriculumGroup { Name = "6B", IsSubscribable = true},
                    new CurriculumGroup { Name = "7B", IsSubscribable = true},

                },
            };

            _db.Curricula.Add(tourismus);

            AddCapacityGroup(tourismus, "1B", "-A", true, true, new string[] { "1B-A" });
            AddCapacityGroup(tourismus, "1B", "-B", true, true, new string[] { "1B-B" });
            AddCapacityGroup(tourismus, "1B", "-C", true, false, new string[] { "1B-C" });
            AddCapacityGroup(tourismus, "1B", "-D", true, true, new string[] { "1B-D" });


            AddCapacityGroup(tourismus, "2B", "-A", true, true, new string[] { "2B-A" });
            AddCapacityGroup(tourismus, "2B", "-B", true, true, new string[] { "2B-B" });
            AddCapacityGroup(tourismus, "2B", "-C", false, true, new string[] { "2B-C" });
            AddCapacityGroup(tourismus, "2B", "-D", true, true, new string[] { "2B-D" });


            AddCapacityGroup(tourismus, "3B", "-A", true, true, new string[] { "3B-A" });
            AddCapacityGroup(tourismus, "3B", "-B", true, true, new string[] { "3B-B" });
            AddCapacityGroup(tourismus, "3B", "-C", true, false, new string[] { "3B-C" });
            AddCapacityGroup(tourismus, "3B", "-D", true, true, new string[] { "3B-D" });


            AddCapacityGroup(tourismus, "5B", "-A", true, true, new string[] { "5B-A" });
            AddCapacityGroup(tourismus, "5B", "-B", true, true, new string[] { "5B-B" });
            AddCapacityGroup(tourismus, "5B", "-C", true, false, new string[] { "5B-C" });
            AddCapacityGroup(tourismus, "5B", "-D", true, true, new string[] { "5B-D" });


            AddCapacityGroup(tourismus, "6B", "-A", true, true, new string[] { "6B-A" });
            AddCapacityGroup(tourismus, "6B", "-B", true, true, new string[] { "6B-B" });
            AddCapacityGroup(tourismus, "6B", "-C", true, false, new string[] { "6B-C" });
            AddCapacityGroup(tourismus, "6B", "-D", true, true, new string[] { "6B-D" });

            AddCapacityGroup(tourismus, "7B", "-A", true, true, new string[] { "7B-A" });
            AddCapacityGroup(tourismus, "7B", "-B", true, true, new string[] { "7B-B" });
            AddCapacityGroup(tourismus, "7B", "-C", true, false, new string[] { "7B-C" });
            AddCapacityGroup(tourismus, "7B", "-D", true, true, new string[] { "7B-D" });

            _db.SaveChanges();
        }

        public void FK14_InitCurriculumTourismusM(ActivityOrganiser fk14)
        {
            var tourismus = GetCurriculum(fk14, "TourismusM");

            if (tourismus != null)
                return;

            // Studienprogramme

            tourismus = new Curriculum
            {
                ShortName = "TourismusM",
                Name = "Tourismus Management Master",
                Organiser = fk14,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "Master 1", IsSubscribable = true},
                    new CurriculumGroup { Name = "Master 2", IsSubscribable = true},
                    new CurriculumGroup { Name = "Master 3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(tourismus);

            AddCapacityGroup(tourismus, "Master 1", "T", true, true, new string[] { "Master 1T" });
            AddCapacityGroup(tourismus, "Master 2", "T", true, true, new string[] { "Master 2T" });
            AddCapacityGroup(tourismus, "Master 3", "T", true, true, new string[] { "Master 3T" });

            _db.SaveChanges();
        }
        public void FK14_InitCurriculumHospitalityM(ActivityOrganiser fk14)
        {
            var tourismus = GetCurriculum(fk14, "HospitalityM");

            if (tourismus != null)
                return;

            tourismus = new Curriculum
            {
                ShortName = "HospitalityM",
                Name = "Hospitality Management Master",
                Organiser = fk14,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "Master 1", IsSubscribable = true},
                    new CurriculumGroup { Name = "Master 2", IsSubscribable = true},
                    new CurriculumGroup { Name = "Master 3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(tourismus);

            AddCapacityGroup(tourismus, "Master 1", "H", true, true, new string[] { "Master 1H" });
            AddCapacityGroup(tourismus, "Master 2", "H", true, true, new string[] { "Master 2H" });
            AddCapacityGroup(tourismus, "Master 3", "H", true, true, new string[] { "Master 3H" });


            _db.SaveChanges();

        }
    }
}

