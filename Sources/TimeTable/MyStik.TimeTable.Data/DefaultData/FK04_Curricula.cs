using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK04_InitCurriculumBA(ActivityOrganiser fk04)
        {
            var ba = GetCurriculum(fk04, "BA");

            if (ba != null)
                return;

            ba = new Curriculum
            {
                ShortName = "BA",
                Name = "Bachelor Erstsemestergruppen",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
            },
            };

            _db.Curricula.Add(ba);

            AddCapacityGroup(ba, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(ba, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(ba, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(ba, "1", "D", true, true, new string[] { "1D" });

            _db.SaveChanges();
        }
        public void FK04_InitCurriculumEIB(ActivityOrganiser fk04)
        {
            var eib = GetCurriculum(fk04, "eib");

            if (eib != null)
                return;

            eib = new Curriculum
            {
                ShortName = "EIB",
                Name = "Bachelor Elektro- & Informationtechnik",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "6", IsSubscribable = true },
                new CurriculumGroup { Name = "7", IsSubscribable = true },
                new CurriculumGroup { Name = "WPM", IsSubscribable = false },
                new CurriculumGroup { Name = "AW", IsSubscribable = false },
            },
            };

            _db.Curricula.Add(eib);

            AddCapacityGroup(eib, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(eib, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(eib, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(eib, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(eib, "4", "A", true, true, new string[] { "4A" });

            AddCapacityGroup(eib, "5", "", true, true, new string[] { "5" });

            AddCapacityGroup(eib, "6", "", true, true, new string[] { "6" });

            AddCapacityGroup(eib, "7", "", true, true, new string[] { "7" });

            _db.SaveChanges();

        }
        public void FK04_InitCurriculumREB(ActivityOrganiser fk04)
        {
            var reb = GetCurriculum(fk04, "reb");

            if (reb != null)
                return;

            reb = new Curriculum
            {
                ShortName = "REB",
                Name = "Bachelor Regenerative Energien",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "6", IsSubscribable = true },
                new CurriculumGroup { Name = "7", IsSubscribable = true },
                new CurriculumGroup { Name = "WPM", IsSubscribable = false },
                new CurriculumGroup { Name = "AW", IsSubscribable = false },
            },
            };

            _db.Curricula.Add(reb);

            AddCapacityGroup(reb, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(reb, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(reb, "3", "A", true, true, new string[] { "3A" });

            AddCapacityGroup(reb, "4", "A", true, true, new string[] { "4A" });

            AddCapacityGroup(reb, "5", "", true, true, new string[] { "5" });

            AddCapacityGroup(reb, "6", "", true, true, new string[] { "6" });

            AddCapacityGroup(reb, "7", "", true, true, new string[] { "7" });

            _db.SaveChanges();
        }
        public void FK04_InitCurriculumEMB(ActivityOrganiser fk04)
        {
            var emb = GetCurriculum(fk04, "emb");

            if (emb != null)
                return;

            emb = new Curriculum
            {
                ShortName = "EMB",
                Name = "Bachelor Elektromobilität",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "6", IsSubscribable = true },
                new CurriculumGroup { Name = "7", IsSubscribable = true },
                new CurriculumGroup { Name = "WPM", IsSubscribable = false },
                new CurriculumGroup { Name = "AW", IsSubscribable = false },
            },
            };

            _db.Curricula.Add(emb);

            AddCapacityGroup(emb, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(emb, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(emb, "3", "A", true, true, new string[] { "3A" });

            AddCapacityGroup(emb, "4", "A", true, true, new string[] { "4A" });

            AddCapacityGroup(emb, "5", "", true, true, new string[] { "5" });

            AddCapacityGroup(emb, "6", "", true, true, new string[] { "6" });

            AddCapacityGroup(emb, "7", "", true, true, new string[] { "7" });

            _db.SaveChanges();

        }
        public void FK04_InitCurriculumELM(ActivityOrganiser fk04)
        {
            var elm = GetCurriculum(fk04, "elm");

            if (elm != null)
                return;

            elm = new Curriculum
            {
                ShortName = "ELM",
                Name = "Master Elektrotechnik",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
            },
            };

            _db.Curricula.Add(elm);


            AddCapacityGroup(elm, "1", "", true, true, new string[] { "1" });

            AddCapacityGroup(elm, "2", "", true, true, new string[] { "2" });

            AddCapacityGroup(elm, "3", "", true, true, new string[] { "3" });

            _db.SaveChanges();

        }
        public void FK04_InitCurriculumEEM(ActivityOrganiser fk04)
        {
            var eem = GetCurriculum(fk04, "eem");

            if (eem != null)
                return;

            eem = new Curriculum
            {
                ShortName = "EEM",
                Name = "Master Electrical Engineering",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "2", IsSubscribable = true },      
            },
            };

            _db.Curricula.Add(eem);

            AddCapacityGroup(eem, "1", "", true, true, new string[] { "1" });

            AddCapacityGroup(eem, "2", "", true, true, new string[] { "2" });

            AddCapacityGroup(eem, "3", "", true, true, new string[] { "3" });

            _db.SaveChanges();
        }
        public void FK04_InitCurriculumSMM(ActivityOrganiser fk04)
        {
            var smm = GetCurriculum(fk04, "smm");

            if (smm != null)
                return;

            smm = new Curriculum
            {
                ShortName = "SSM",
                Name = "Master Systems Engineering",
                Organiser = fk04,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
            },
            };

            _db.Curricula.Add(smm);


            AddCapacityGroup(smm, "1", "", true, true, new string[] { "1" });

            AddCapacityGroup(smm, "2", "", true, true, new string[] { "2" });

            AddCapacityGroup(smm, "3", "", true, true, new string[] { "3" });

            _db.SaveChanges();

        }
    }
}
