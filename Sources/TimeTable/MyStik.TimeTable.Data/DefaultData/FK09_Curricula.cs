using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        private void InitCurriculumWI(ActivityOrganiser fk09)
        {
            var wi = GetCurriculum(fk09, "WI");

            if (wi != null)
                return;

            // Studienprogramme
            wi = new Curriculum
            {
                ShortName = "WI",
                Name = "Bachelor Wirtschaftsingenieurwesen",
                Organiser = fk09,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3 TEC", IsSubscribable = true},
                    new CurriculumGroup { Name = "3 BIO", IsSubscribable = true},
                    new CurriculumGroup { Name = "3 INF", IsSubscribable = true},
                    new CurriculumGroup { Name = "4 TEC", IsSubscribable = true},
                    new CurriculumGroup { Name = "4 BIO", IsSubscribable = true},
                    new CurriculumGroup { Name = "4 INF", IsSubscribable = true},
                    new CurriculumGroup { Name = "5 TEC", IsSubscribable = true},
                    new CurriculumGroup { Name = "5 BIO", IsSubscribable = true},
                    new CurriculumGroup { Name = "5 INF", IsSubscribable = true},
                    new CurriculumGroup { Name = "6 TEC", IsSubscribable = true},
                    new CurriculumGroup { Name = "6 BIO", IsSubscribable = true},
                    new CurriculumGroup { Name = "6 INF", IsSubscribable = true},
                    new CurriculumGroup { Name = "7 TEC", IsSubscribable = true},
                    new CurriculumGroup { Name = "7 BIO", IsSubscribable = true},
                    new CurriculumGroup { Name = "7 INF", IsSubscribable = true},
                    new CurriculumGroup { Name = "WPM", IsSubscribable = false},        // nicht sonderlich gut
                    new CurriculumGroup { Name = "AW", IsSubscribable = false},         // nicht sonderlich gut
                },
            };

            _db.Curricula.Add(wi);

            AddCapacityGroup(wi, "1", "A", true, true, new[] { "1A" });
            AddCapacityGroup(wi, "1", "B", true, true, new[] { "1B" });
            AddCapacityGroup(wi, "1", "C", true, false, new[] { "1C" });

            AddCapacityGroup(wi, "2", "A", true, true, new[] { "2A" });
            AddCapacityGroup(wi, "2", "B", true, true, new[] { "2B" });
            AddCapacityGroup(wi, "2", "C", false, true, new[] { "2C" });

            AddCapacityGroup(wi, "3 BIO", "", true, true, new[] { "3Bio", "3Bio/Inf", "3Bio/Tec", "3Bio/Tec-G2" });
            AddCapacityGroup(wi, "4 BIO", "", true, true, new[] { "4Bio", "4Bio/Inf", "4Bio/Tec", "4Bio/Tec-G2" });
            AddCapacityGroup(wi, "5 BIO", "", true, true, new[] { "5Bio", "5Bio/Inf", "5Bio/Tec", "5Bio/Tec-G2" });
            AddCapacityGroup(wi, "6 BIO", "", true, true, new[] { "6Bio", "6Bio/Inf", "6Bio/Tec", "6Bio/Tec-G2" });
            AddCapacityGroup(wi, "7 BIO", "", true, true, new[] { "7Bio", "7Bio/Inf", "7Bio/Tec", "7Bio/Tec-G2" });

            AddCapacityGroup(wi, "3 INF", "", true, true, new[] { "3Inf", "3Inf/Bio", "3Inf/Tec", "3Inf/Tec-G2" });
            AddCapacityGroup(wi, "4 INF", "", true, true, new[] { "4Inf", "4Inf/Bio", "4Inf/Tec", "4Inf/Tec-G2" });
            AddCapacityGroup(wi, "5 INF", "", true, true, new[] { "5Inf", "5Inf/Bio", "5Inf/Tec", "5Inf/Tec-G2" });
            AddCapacityGroup(wi, "6 INF", "", true, true, new[] { "6Inf", "6Inf/Bio", "6Inf/Tec", "6Inf/Tec-G2" });
            AddCapacityGroup(wi, "7 INF", "", true, true, new[] { "7Inf", "7Inf/Bio", "7Inf/Tec", "7Inf/Tec-G2" });

            AddCapacityGroup(wi, "3 TEC", "G1", true, true, new[] { "3Tec", "3Tec/Bio", "3Tec/Inf" });
            AddCapacityGroup(wi, "4 TEC", "G1", true, true, new[] { "4Tec", "4Tec/Bio", "4Tec/Inf" });
            AddCapacityGroup(wi, "5 TEC", "G1", true, true, new[] { "5Tec", "5Tec/Bio", "5Tec/Inf" });
            AddCapacityGroup(wi, "6 TEC", "G1", true, true, new[] { "6Tec", "6Tec/Bio", "6Tec/Inf" });
            AddCapacityGroup(wi, "7 TEC", "G1", true, true, new[] { "7Tec", "7Tec/Bio", "7Tec/Inf" });

            AddCapacityGroup(wi, "3 TEC", "G2", true, true, new[] { "3Tec-G2", "3Tec-G2/Bio", "3Tec-G2/Inf" });
            AddCapacityGroup(wi, "4 TEC", "G2", true, true, new[] { "4Tec-G2", "4Tec-G2/Bio", "4Tec-G2/Inf" });
            AddCapacityGroup(wi, "5 TEC", "G2", true, true, new[] { "5Tec-G2", "5Tec-G2/Bio", "5Tec-G2/Inf" });
            AddCapacityGroup(wi, "6 TEC", "G2", true, true, new[] { "6Tec-G2", "6Tec-G2/Bio", "6Tec-G2/Inf" });
            AddCapacityGroup(wi, "7 TEC", "G2", true, true, new[] { "7Tec-G2", "7Tec-G2/Bio", "7Tec-G2/Inf" });

            AddCapacityGroup(wi, "WPM", "", true, true, new[] { "WPM WI" });

            _db.SaveChanges();
        }

        public void InitCurriculumLM(ActivityOrganiser fk09)
        {
            var wi = GetCurriculum(fk09, "LM");

            if (wi != null)
                return;

            // Studienprogramme
            wi = new Curriculum
            {
                ShortName = "LM",
                Name = "Bachelor Wirtschaftsingenieurwesen Logistik",
                Organiser = fk09,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "WPM", IsSubscribable = false},        // nicht sonderlich gut
                    new CurriculumGroup { Name = "AW", IsSubscribable = false},         // nicht sonderlich gut
                },
            };

            _db.Curricula.Add(wi);

            AddCapacityGroup(wi, "1", "", true, false, new[] { "1LM" });
            AddCapacityGroup(wi, "2", "", false, true, new[] { "2LM" });
            AddCapacityGroup(wi, "3", "", true, false, new[] { "3LM" });
            AddCapacityGroup(wi, "4", "", false, true, new[] { "4LM" });
            AddCapacityGroup(wi, "5", "", true, false, new[] { "5LM" });
            AddCapacityGroup(wi, "6", "", false, true, new[] { "6LM" });
            AddCapacityGroup(wi, "7", "", true, false, new[] { "7LM" });
            AddCapacityGroup(wi, "WPM", "", true, true, new[] { "WPM LM" });

            _db.SaveChanges();
        }

        public void InitCurriculumAU(ActivityOrganiser fk09)
        {
            var wi = GetCurriculum(fk09, "AU");

            if (wi != null)
                return;

            // Studienprogramme
            wi = new Curriculum
            {
                ShortName = "AU",
                Name = "Bachelor Wirtschaftsingenieurwesen Automobilindustrie",
                Organiser = fk09,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "WPM", IsSubscribable = false},        // nicht sonderlich gut
                    new CurriculumGroup { Name = "AW", IsSubscribable = false},         // nicht sonderlich gut
                },
            };

            _db.Curricula.Add(wi);

            AddCapacityGroup(wi, "1", "", false, true, new[] { "1AU" });
            AddCapacityGroup(wi, "2", "", true, false, new[] { "2AU" });
            AddCapacityGroup(wi, "3", "", false, true, new[] { "3AU" });
            AddCapacityGroup(wi, "4", "", true, false, new[] { "4AU" });
            AddCapacityGroup(wi, "5", "", false, true, new[] { "5AU" });
            AddCapacityGroup(wi, "6", "", true, false, new[] { "6AU" });
            AddCapacityGroup(wi, "7", "", false, true, new[] { "7AU" });
            AddCapacityGroup(wi, "WPM", "", true, true, new[] { "WPM AU" });

            _db.SaveChanges();
        }

        public void InitCurriculumWIM(ActivityOrganiser fk09)
        {
            var wi = GetCurriculum(fk09, "WIM");

            if (wi != null)
                return;

            // Studienprogramme
            wi = new Curriculum
            {
                ShortName = "WIM",
                Name = "Master Wirtschaftsingenieurwesen",
                Organiser = fk09,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "WPM", IsSubscribable = false}
                },
            };

            _db.Curricula.Add(wi);

            AddCapacityGroup(wi, "1", "G1", true, true, new[] { "WI M1" });
            AddCapacityGroup(wi, "1", "G2", true, true, new[] { "WI M1 G2" });
            AddCapacityGroup(wi, "2", "G1", true, true, new[] { "WI M2" });
            AddCapacityGroup(wi, "2", "G2", true, true, new[] { "WI M2 G2" });
            AddCapacityGroup(wi, "3", "G1", true, true, new[] { "WI M3" });
            AddCapacityGroup(wi, "3", "G2", true, true, new[] { "WI M3 G2" });
            AddCapacityGroup(wi, "WPM", "", true, true, new[] { "WPM WI-M" });

            _db.SaveChanges();
        }

        public void InitCurriculumMBA(ActivityOrganiser fk09)
        {
            var wi = GetCurriculum(fk09, "MBA");

            if (wi != null)
                return;

            // Studienprogramme
            wi = new Curriculum
            {
                ShortName = "MBA",
                Name = "Master of Business Administration and Engineering",
                Organiser = fk09,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1 BAU", IsSubscribable = true},
                    new CurriculumGroup { Name = "1 ING / NW", IsSubscribable = true},
                    new CurriculumGroup { Name = "1 WI", IsSubscribable = true},
                    new CurriculumGroup { Name = "2 BAU", IsSubscribable = true},
                    new CurriculumGroup { Name = "2 ING / NW", IsSubscribable = true},
                    new CurriculumGroup { Name = "2 WI", IsSubscribable = true},
                    new CurriculumGroup { Name = "3 BAU", IsSubscribable = true},
                    new CurriculumGroup { Name = "3 ING / NW", IsSubscribable = true},
                    new CurriculumGroup { Name = "3 WI", IsSubscribable = true},
                    new CurriculumGroup { Name = "4 BAU", IsSubscribable = true},
                    new CurriculumGroup { Name = "4 ING / NW", IsSubscribable = true},
                    new CurriculumGroup { Name = "4 WI", IsSubscribable = true},
                    new CurriculumGroup { Name = "5 BAU", IsSubscribable = true},
                    new CurriculumGroup { Name = "5 ING / NW", IsSubscribable = true},
                    new CurriculumGroup { Name = "5 WI", IsSubscribable = true},
                    new CurriculumGroup { Name = "WPM", IsSubscribable = false}
                },
            };

            _db.Curricula.Add(wi);

            AddCapacityGroup(wi, "1 BAU", "", true, false, new[] { "WW M1 Bau" });
            AddCapacityGroup(wi, "1 ING / NW", "G1", true, true, new[] { "WW M1 NW" });
            AddCapacityGroup(wi, "1 ING / NW", "G2", true, true, new[] { "WW M1 NW-G2" });
            AddCapacityGroup(wi, "1 WI", "", true, false, new[] { "WW M1 WI" });
            AddCapacityGroup(wi, "2 BAU", "", true, false, new[] { "WW M2 Bau" });
            AddCapacityGroup(wi, "2 ING / NW", "G1", true, true, new[] { "WW M2 NW" });
            AddCapacityGroup(wi, "2 ING / NW", "G2", true, true, new[] { "WW M2 NW-G2" });
            AddCapacityGroup(wi, "2 WI", "", true, false, new[] { "WW M2 WI" });
            AddCapacityGroup(wi, "3 BAU", "", true, false, new[] { "WW M3 Bau" });
            AddCapacityGroup(wi, "3 ING / NW", "G1", true, true, new[] { "WW M3 NW" });
            AddCapacityGroup(wi, "3 ING / NW", "G2", true, true, new[] { "WW M3 NW-G2" });
            AddCapacityGroup(wi, "3 WI", "", true, false, new[] { "WW M3 WI" });
            AddCapacityGroup(wi, "4 BAU", "", true, false, new[] { "WW M4 Bau" });
            AddCapacityGroup(wi, "4 ING / NW", "G1", true, true, new[] { "WW M4 NW" });
            AddCapacityGroup(wi, "4 ING / NW", "G2", true, true, new[] { "WW M4 NW-G2" });
            AddCapacityGroup(wi, "4 WI", "", true, false, new[] { "WW M4 WI" });
            AddCapacityGroup(wi, "5 BAU", "", true, false, new[] { "WW M5 Bau" });
            AddCapacityGroup(wi, "5 ING / NW", "G1", true, true, new[] { "WW M5 NW" });
            AddCapacityGroup(wi, "5 ING / NW", "G2", true, true, new[] { "WW M5 NW-G2" });
            AddCapacityGroup(wi, "5 WI", "", true, false, new[] { "WW M5 WI" });
            AddCapacityGroup(wi, "WPM", "", true, true, new[] { "WPM WW-M" });

            _db.SaveChanges();
        }

    }
}
   
