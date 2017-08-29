using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK07_InitCurriculumInfoMath(ActivityOrganiser FK07)
        {
            var InfoMath = GetCurriculum(FK07, "InfoMath");

            if (InfoMath != null)
                return;

            // Studienprogramme
            InfoMath = new Curriculum
            {
                ShortName = "InfoMath",
                Name = "Bachelor Informatik und Mathematik",
                Organiser = FK07,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "G", IsSubscribable = true},
                    new CurriculumGroup { Name = "IB", IsSubscribable = true},
                    new CurriculumGroup { Name = "IC", IsSubscribable = true},
                    new CurriculumGroup { Name = "IF", IsSubscribable = true},
                    new CurriculumGroup { Name = "IG", IsSubscribable = true},
                    new CurriculumGroup { Name = "IN", IsSubscribable = true},
                    new CurriculumGroup { Name = "IS", IsSubscribable = true},
                    new CurriculumGroup { Name = "ZD", IsSubscribable = false},        // nicht sonderlich gut
                },
            };

            _db.Curricula.Add(InfoMath);
            AddCapacityGroup(InfoMath, "G", "0", true, true, new string[] { "G0" });
            AddCapacityGroup(InfoMath, "G", "01", true, true, new string[] { "G01" });
            AddCapacityGroup(InfoMath, "G", "06", true, false, new string[] { "G02" });
            AddCapacityGroup(InfoMath, "G", "03", true, true, new string[] { "G03" });
            AddCapacityGroup(InfoMath, "G", "04", true, false, new string[] { "G04" });
            AddCapacityGroup(InfoMath, "G", "05", true, false, new string[] { "G05" });
            AddCapacityGroup(InfoMath, "G", "06", true, false, new string[] { "G06" });
            AddCapacityGroup(InfoMath, "G", "07", true, false, new string[] { "G07" });

            AddCapacityGroup(InfoMath, "IB", "", true, true, new string[] { "IB" });
            AddCapacityGroup(InfoMath, "IB", "1A", true, true, new string[] { "IB1A" });
            AddCapacityGroup(InfoMath, "IB", "1B", true, true, new string[] { "IB1B" });
            AddCapacityGroup(InfoMath, "IB", "1C", true, true, new string[] { "IB1C" });

            AddCapacityGroup(InfoMath, "IB", "2A", true, true, new string[] { "IB2A" });
            AddCapacityGroup(InfoMath, "IB", "2B", true, true, new string[] { "IB2B" });
            AddCapacityGroup(InfoMath, "IB", "2C", true, true, new string[] { "IB2C" });

            AddCapacityGroup(InfoMath, "IB", "3A", true, true, new string[] { "IB3A" });
            AddCapacityGroup(InfoMath, "IB", "3B", true, true, new string[] { "IB3B" });
            AddCapacityGroup(InfoMath, "IB", "3C", true, false, new string[] { "IB3C" });

            AddCapacityGroup(InfoMath, "IF", "", true, true, new string[] { "IF" });
            AddCapacityGroup(InfoMath, "IF", "1A", true, true, new string[] { "IF1A" });
            AddCapacityGroup(InfoMath, "IF", "1B", true, true, new string[] { "IF1B" });
            AddCapacityGroup(InfoMath, "IF", "1C", true, true, new string[] { "IF1C" });

            AddCapacityGroup(InfoMath, "IF", "3A", true, true, new string[] { "IF3A" });
            AddCapacityGroup(InfoMath, "IF", "3B", true, true, new string[] { "IF3B" });

            AddCapacityGroup(InfoMath, "IF", "5", true, true, new string[] { "IF5" });
            AddCapacityGroup(InfoMath, "IF", "6", true, true, new string[] { "IF6" });
            AddCapacityGroup(InfoMath, "IF", "7", true, true, new string[] { "IF7" });

            AddCapacityGroup(InfoMath, "IG", "", true, true, new string[] { "IG" });

            AddCapacityGroup(InfoMath, "IN", "", true, true, new string[] { "IN" });
            AddCapacityGroup(InfoMath, "IN", "1", true, true, new string[] { "IN1" });
            AddCapacityGroup(InfoMath, "IN", "2", false, true, new string[] { "IN2" });
            AddCapacityGroup(InfoMath, "IN", "3", false, true, new string[] { "IN3" });

            AddCapacityGroup(InfoMath, "IS", "", true, true, new string[] { "IS" });
            AddCapacityGroup(InfoMath, "IS", "1", true, true, new string[] { "IS1" });
            AddCapacityGroup(InfoMath, "IS", "2", true, false, new string[] { "IS2" });
            AddCapacityGroup(InfoMath, "IS", "3", true, false, new string[] { "IS3" });

            AddCapacityGroup(InfoMath, "ZD", "", true, true, new string[] { "ZD" });


            _db.SaveChanges();
            ;


        }
        public void FK07_InitCurriculumWirtschaftsinformatik(ActivityOrganiser FK07)
        {
            var Wirtschaftsinformatik = GetCurriculum(FK07, "Wirtschaftsinformatik");

            if (Wirtschaftsinformatik != null)
                return;

            // Studienprogramme
            Wirtschaftsinformatik = new Curriculum
            {
                ShortName = "Wirtschaftsinformatik",
                Name = "Bachelor Wirtschaftsinformatik",
                Organiser = FK07,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "WIF1", IsSubscribable = true},
                    new CurriculumGroup { Name = "WIF2", IsSubscribable = true},
                    new CurriculumGroup { Name = "WIF3", IsSubscribable = true},
                    new CurriculumGroup { Name = "WIF4", IsSubscribable = true},
                    new CurriculumGroup { Name = "WIF5", IsSubscribable = true},
                    new CurriculumGroup { Name = "WIF6", IsSubscribable = true},
                    new CurriculumGroup { Name = "WIF7", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(Wirtschaftsinformatik);

            AddCapacityGroup(Wirtschaftsinformatik, "WIF1", "A", true, true, new string[] { "WIF1A" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF1", "B", true, true, new string[] { "WIF1B" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF1", "C", true, false, new string[] { "WIF1C" });

            AddCapacityGroup(Wirtschaftsinformatik, "WIF2", "A", true, true, new string[] { "WIF2A" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF2", "B", true, true, new string[] { "WIF2B" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF2", "C", false, true, new string[] { "WIF2C" });

            AddCapacityGroup(Wirtschaftsinformatik, "WIF3", "A", true, true, new string[] { "WIF3A" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF3", "B", true, true, new string[] { "WIF3B" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF3", "C", true, false, new string[] { "WIF3C" });

            AddCapacityGroup(Wirtschaftsinformatik, "WIF4", "A", true, true, new string[] { "WIF4A" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF4", "B", true, true, new string[] { "WIF4B" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF4", "C", false, true, new string[] { "WIF4C" });

            AddCapacityGroup(Wirtschaftsinformatik, "WIF5", "A", true, true, new string[] { "WIF5A" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF5", "B", true, true, new string[] { "WIF5B" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF5", "C", true, false, new string[] { "WIF5C" });

            AddCapacityGroup(Wirtschaftsinformatik, "WIF6", "A", true, true, new string[] { "WIF6A" });
            AddCapacityGroup(Wirtschaftsinformatik, "WIF6", "B", true, true, new string[] { "WIF6B" });
            AddCapacityGroup(Wirtschaftsinformatik, "6", "C", false, true, new string[] { "WIF6C" });

            AddCapacityGroup(Wirtschaftsinformatik, "WIF7", "", true, false, new string[] { "WIF7" });

            _db.SaveChanges();
        }
        public void FK07_InitCurriculumKART(ActivityOrganiser FK07)
        {
            var KART = GetCurriculum(FK07, "Bachelor Kartographie");

            if (KART != null)
                return;

            // Studienprogramme
            KART = new Curriculum
            {
                ShortName = "KART",
                Name = "Bachelor Kartographie ",
                Organiser = FK07,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(KART);

            AddCapacityGroup(KART, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(KART, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(KART, "1", "C", true, false, new string[] { "1C" });
            AddCapacityGroup(KART, "3", "", true, true, new string[] { "3" });
            AddCapacityGroup(KART, "5", "", true, false, new string[] { "5" });
            AddCapacityGroup(KART, "7", "", true, true, new string[] { "7" });


            _db.SaveChanges();
        }
        public void FK07_InitCurriculumMWINF(ActivityOrganiser FK07)
        {
            var MWINF = GetCurriculum(FK07, "Master Wirtschaftsinformatik");

            if (MWINF != null)
                return;

            // Studienprogramme
            MWINF = new Curriculum
            {
                ShortName = "MWINF",
                Name = "Master Wirtschaftsinformatik",
                Organiser = FK07,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(MWINF);

            AddCapacityGroup(MWINF, "1", "Vollzeit", true, true, new string[] { "3AVollzeit" });
            AddCapacityGroup(MWINF, "1", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MWINF, "2", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MWINF, "2", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MWINF, "3", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MWINF, "3", "Teilzeit", true, true, new string[] { "Teilzeit" });


            _db.SaveChanges();
        }
        public void FK07_InitCurriculumMINFO(ActivityOrganiser FK07)
        {
            var MINF = GetCurriculum(FK07, "Master Informatik" );

            if (MINF != null)
                return;

            // Studienprogramme
            MINF = new Curriculum
            {
                ShortName = "MINF",
                Name = "Master Informatik ",
                Organiser = FK07,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(MINF);

            AddCapacityGroup(MINF, "1", "Vollzeit", true, true, new string[] { "3AVollzeit" });
            AddCapacityGroup(MINF, "1", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MINF, "2", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MINF, "2", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(MINF, "3", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(MINF, "3", "Teilzeit", true, true, new string[] { "Teilzeit" });


            _db.SaveChanges();
        }
        public void FK07_InitCurriculumANMATH(ActivityOrganiser FK07)
        {
            var ANMATH = GetCurriculum(FK07, "Master angewandte Mathematik");

            if (ANMATH != null)
                return;

            // Studienprogramme
            ANMATH = new Curriculum
            {
                ShortName = "ANMATH",
                Name = "Master angewandte Mathematik",
                Organiser = FK07,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(ANMATH);

            AddCapacityGroup(ANMATH, "1", "Vollzeit", true, true, new string[] { "3AVollzeit" });
            AddCapacityGroup(ANMATH, "1", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(ANMATH, "2", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(ANMATH, "2", "Teilzeit", true, true, new string[] { "Teilzeit" });
            AddCapacityGroup(ANMATH, "3", "Vollzeit", true, false, new string[] { "Vollzeit" });
            AddCapacityGroup(ANMATH, "3", "Teilzeit", true, true, new string[] { "Teilzeit" });


            _db.SaveChanges();

        }
    }
}
      