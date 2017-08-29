using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK05_InitCurriculumEG(ActivityOrganiser fk05)
        {
            var eg = GetCurriculum(fk05, "EG");

            if (eg != null)
                return;

            // Studienprogramme
            eg = new Curriculum
            {
                ShortName = "EG",
                Name = "Bachelor Energie- und Gebäudetechnik",
                Organiser = fk05,
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

            _db.Curricula.Add(eg);

            AddCapacityGroup(eg, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(eg, "1", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(eg, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(eg, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(eg, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(eg, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(eg, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(eg, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(eg, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(eg, "5", "B", true, true, new string[] { "5B" });

            AddCapacityGroup(eg, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(eg, "6", "B", true, true, new string[] { "6B" });

            AddCapacityGroup(eg, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(eg, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }

        public void FK05_InitCurriculumPV(ActivityOrganiser fk05)
        {
            var pv = GetCurriculum(fk05, "PV");

            if (pv != null)
                return;

            // Studienprogramme
            pv = new Curriculum
            {
                ShortName = "PV",
                Name = "Bachelor Papier- und Verpackungstechnik",
                Organiser = fk05,
                CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    }
            };
            _db.Curricula.Add(pv);

            AddCapacityGroup(pv, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(pv, "1", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(pv, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(pv, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(pv, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(pv, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(pv, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(pv, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(pv, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(pv, "5", "B", true, true, new string[] { "5B" });

            AddCapacityGroup(pv, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(pv, "6", "B", true, true, new string[] { "6B" });

            AddCapacityGroup(pv, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(pv, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }

        public void FK05_InitCurriculumDM(ActivityOrganiser fk05)
        {
            var dm = GetCurriculum(fk05, "DM");

            if (dm != null)
                return;

            // Studienprogramme
            dm = new Curriculum
            {
                ShortName = "DM",
                Name = "Bachelor Druck- und Medientechnik",
                Organiser = fk05,
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

            _db.Curricula.Add(dm);

            AddCapacityGroup(dm, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(dm, "1", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(dm, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(dm, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(dm, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(dm, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(dm, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(dm, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(dm, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(dm, "5", "B", true, true, new string[] { "5B" });

            AddCapacityGroup(dm, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(dm, "6", "B", true, true, new string[] { "6B" });

            AddCapacityGroup(dm, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(dm, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }
        public void FK05_InitCurriculumTRK(ActivityOrganiser fk05)
        {
            var trk = GetCurriculum(fk05, "TRK");

            if (trk != null)
                return;

            // Studienprogramme
            trk = new Curriculum
            {
                ShortName = "TRK",
                Name = " Bachelor Technische Redaktion und Kommunikation",
                Organiser = fk05,
                CurriculumGroups = new HashSet<CurriculumGroup>
                    {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    }


            };
            _db.Curricula.Add(trk);

            AddCapacityGroup(trk, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(trk, "1", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(trk, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(trk, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(trk, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(trk, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(trk, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(trk, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(trk, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(trk, "5", "B", true, true, new string[] { "5B" });

            AddCapacityGroup(trk, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(trk, "6", "B", true, true, new string[] { "6B" });

            AddCapacityGroup(trk, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(trk, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }


    }

}



