using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK06_InitCurriculumAOB(ActivityOrganiser fk06)
        {
            var aob = GetCurriculum(fk06, "AOB");

            if (aob != null)
                return;

            // Studienprogramme
            aob = new Curriculum
            {
                ShortName = "AOB",
                Name = "Bachelor Augenoptik und Optometrie",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = false},
                 },
            };

            _db.Curricula.Add(aob);

            AddCapacityGroup(aob, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(aob, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(aob, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(aob, "1", "D", true, true, new string[] { "1D" });

            AddCapacityGroup(aob, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(aob, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(aob, "3", "C", true, true, new string[] { "3C" });
            AddCapacityGroup(aob, "3", "D", true, true, new string[] { "3D" });

            AddCapacityGroup(aob, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(aob, "5", "B", true, true, new string[] { "5B" });
            AddCapacityGroup(aob, "5", "C", true, true, new string[] { "5C" });
            AddCapacityGroup(aob, "5", "D", true, true, new string[] { "5D" });

            AddCapacityGroup(aob, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(aob, "7", "B", true, true, new string[] { "7B" });
            AddCapacityGroup(aob, "7", "C", true, true, new string[] { "7C" });
            AddCapacityGroup(aob, "7", "D", true, true, new string[] { "7D" });

            _db.SaveChanges();
        }
            public void FK06_InitCurriculumPHT(ActivityOrganiser fk06)
        {
            var pht = GetCurriculum(fk06, "pht");

            if (pht!= null)
                return;

            pht = new Curriculum
            {
                ShortName = "PHT",
                Name = "Bachelor Physikalische Technik",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "6", IsSubscribable = true },
                new CurriculumGroup { Name = "7", IsSubscribable = true },
            },
            };

            _db.Curricula.Add(pht);


            AddCapacityGroup(pht, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(pht, "1", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(pht, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(pht, "2", "B", true, true, new string[] { "2B" });

            AddCapacityGroup(pht, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(pht, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(pht, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(pht, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(pht, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(pht, "5", "B", true, true, new string[] { "5B" });

            AddCapacityGroup(pht, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(pht, "6", "B", true, true, new string[] { "6B" });

            AddCapacityGroup(pht, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(pht, "7", "B", true, true, new string[] { "7B" });

            _db.SaveChanges();
        }

        public void FK06_InitCurriculumBBM(ActivityOrganiser fk06)
        {
            var bbm = GetCurriculum(fk06, "BBM");

            if (bbm != null)
                return;

            // Studienprogramme
            bbm = new Curriculum
            {
                ShortName = "BBM",
                Name = "Master  Biotechnologie und Bioingenieurwesen",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = false},

                 },
            };

            _db.Curricula.Add(bbm);

            AddCapacityGroup(bbm, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(bbm, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(bbm, "1", "C", true, true, new string[] { "1C" });


            _db.SaveChanges();
        }
        public void FK06_InitCurriculumBOB(ActivityOrganiser fk06)
        {
            var bob = GetCurriculum(fk06, "BOB");

            if (bob != null)
                return;

            // Studienprogramme
            bob = new Curriculum
            {
                ShortName = "BOB",
                Name = "Bachelor Bioingenieurwesen",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = false},
                    new CurriculumGroup { Name = "UWP", IsSubscribable = false},
                    new CurriculumGroup { Name = "SP", IsSubscribable = false},
                },
            };

            _db.Curricula.Add(bob);

            AddCapacityGroup(bob, "1", "", true, true, new string[] { "1" });
            AddCapacityGroup(bob, "2", "", true, true, new string[] { "2" });
            AddCapacityGroup(bob, "3", "", true, true, new string[] { "3" });
            AddCapacityGroup(bob, "4", "", true, true, new string[] { "4" });
            AddCapacityGroup(bob, "5", "", true, true, new string[] { "5" });
            AddCapacityGroup(bob, "6", "", true, true, new string[] { "6" });
            AddCapacityGroup(bob, "7", "", true, true, new string[] { "7" });
            AddCapacityGroup(bob, "UWP", "", true, true, new string[] { "UWP" });
            AddCapacityGroup(bob, "SP", "1", true, true, new string[] { "SP1" });
            AddCapacityGroup(bob, "SP", "2", true, true, new string[] { "SP2" });
            _db.SaveChanges();
        }
        public void FK06_InitCurriculumBOR(ActivityOrganiser fk06)
        {
            var chb = GetCurriculum(fk06, "CHB");

            if (chb != null)
                return;

            // Studienprogramme
            chb = new Curriculum
            {
                ShortName = "CHB",
                Name = "Bachelor Chemische Technik",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = false},

                 },
            };

            _db.Curricula.Add(chb);

            AddCapacityGroup(chb, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(chb, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(chb, "1", "C", true, true, new string[] { "1C" });

            AddCapacityGroup(chb, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(chb, "3", "B", true, true, new string[] { "3B" });
            AddCapacityGroup(chb, "3", "C", true, true, new string[] { "3C" });

            AddCapacityGroup(chb, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(chb, "4", "B", true, true, new string[] { "4B" });
            AddCapacityGroup(chb, "4", "C", true, true, new string[] { "4C" });

            AddCapacityGroup(chb, "5", "A", true, true, new string[] { "5A" });
            AddCapacityGroup(chb, "5", "B", true, true, new string[] { "5B" });
            AddCapacityGroup(chb, "5", "C", true, true, new string[] { "5C" });

            AddCapacityGroup(chb, "6", "A", true, true, new string[] { "6A" });
            AddCapacityGroup(chb, "6", "B", true, true, new string[] { "6B" });
            AddCapacityGroup(chb, "6", "C", true, true, new string[] { "6C" });

            AddCapacityGroup(chb, "7", "A", true, true, new string[] { "7A" });
            AddCapacityGroup(chb, "7", "B", true, true, new string[] { "7B" });
            AddCapacityGroup(chb, "7", "C", true, true, new string[] { "7C" });
            _db.SaveChanges();
        }

        public void FK06_InitCurriculumMFB(ActivityOrganiser fk06)
        {
            var mfb = GetCurriculum(fk06, "MFB");

            if (mfb != null)
                return;

            // Studienprogramme
            mfb = new Curriculum
            {
                ShortName = "MFB",
                Name = "Bachelor  Mechatronik und Feinwerktechnik",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = false},


                 },
            };

            _db.Curricula.Add(mfb);

            AddCapacityGroup(mfb, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(mfb, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(mfb, "1", "C", true, true, new string[] { "1C" });

            AddCapacityGroup(mfb, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(mfb, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(mfb, "4", "G", true, true, new string[] { "3G" });
            AddCapacityGroup(mfb, "4", "M", true, true, new string[] { "3M" });

            AddCapacityGroup(mfb, "5", "G", true, true, new string[] { "5G" });
            AddCapacityGroup(mfb, "5", "M", true, true, new string[] { "5M" });

            AddCapacityGroup(mfb, "7", "G", true, true, new string[] { "7G" });
            AddCapacityGroup(mfb, "7", "M", true, true, new string[] { "7M" });

            _db.SaveChanges();
        }
        public void FK06_InitCurriculumMFM(ActivityOrganiser fk06)
        {
            var mfm = GetCurriculum(fk06, "MFM");

            if (mfm != null)
                return;

            // Studienprogramme
            mfm = new Curriculum
            {
                ShortName = "MFM",
                Name = "Master  Mechatronik und Feinwerktechnik",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = false},

                 },
            };

            _db.Curricula.Add(mfm);

            AddCapacityGroup(mfm, "1", "", true, true, new string[] { "1" });


            _db.SaveChanges();
        }
        public void FK06_InitCurriculumMNM(ActivityOrganiser fk06)
        {
            var mnm = GetCurriculum(fk06, "MNM");

            if (mnm != null)
                return;

            // Studienprogramme
            mnm = new Curriculum
            {
                ShortName = "MNM",
                Name = "Master  Mikotechnik und Nanotechnik",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = false},

                 },
            };

            _db.Curricula.Add(mnm);

            AddCapacityGroup(mnm, "1", "", true, true, new string[] { "1" });


            _db.SaveChanges();
        }
        public void FK06_InitCurriculumPAB(ActivityOrganiser fk06)
        {
            var pab = GetCurriculum(fk06, "PAB");

            if (pab != null)
                return;

            // Studienprogramme
            pab = new Curriculum
            {
                ShortName = "PAB",
                Name = "Bachelor Produktion und Automatisierung",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = false},
                 },
            };

            _db.Curricula.Add(pab);

            AddCapacityGroup(pab, "1", "", true, true, new string[] { "1" });


            AddCapacityGroup(pab, "3", "", true, true, new string[] { "3" });


            AddCapacityGroup(pab, "5", "", true, true, new string[] { "5" });


            AddCapacityGroup(pab, "7", "", true, true, new string[] { "7" });


            _db.SaveChanges();
        }

        public void FK06_InitCurriculumPHB(ActivityOrganiser fk06)
        {
            var phb = GetCurriculum(fk06, "PHB");

            if (phb != null)
                return;

            // Studienprogramme
            phb = new Curriculum
            {
                ShortName = "PAA",
                Name = "Bachelor Produktion und Automatisierung (D/F)",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = false},
                 },
            };

            _db.Curricula.Add(phb);

            AddCapacityGroup(phb, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(phb, "1", "B", true, true, new string[] { "1B" });

            AddCapacityGroup(phb, "3", "", true, true, new string[] { "3" });


            AddCapacityGroup(phb, "5", "", true, true, new string[] { "5" });


            AddCapacityGroup(phb, "7", "", true, true, new string[] { "7" });


            _db.SaveChanges();
        }

        public void FK06_InitCurriculumPNB(ActivityOrganiser fk06)
        {
            var pnb = GetCurriculum(fk06, "PNB");

            if (pnb != null)
                return;

            // Studienprogramme
            pnb = new Curriculum
            {
                ShortName = "PNB",
                Name = "Bachelor Produktion und Automatisierung (national)",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = false},
                 },
            };

            _db.Curricula.Add(pnb);

            AddCapacityGroup(pnb, "1", "", true, true, new string[] { "1" });


            AddCapacityGroup(pnb, "3", "", true, true, new string[] { "3" });


            AddCapacityGroup(pnb, "5", "", true, true, new string[] { "5" });


            AddCapacityGroup(pnb, "7", "", true, true, new string[] { "7" });


            _db.SaveChanges();
        }

        public void FK06_InitCurriculumPOM(ActivityOrganiser fk06)
        {
            var pom = GetCurriculum(fk06, "POM");

            if (pom != null)
                return;

            // Studienprogramme
            pom = new Curriculum
            {
                ShortName = "POM",
                Name = "Master Photonik",
                Organiser = fk06,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {

                    new CurriculumGroup { Name = "1", IsSubscribable = false},

                 },
            };

            _db.Curricula.Add(pom);

            AddCapacityGroup(pom, "1", "", true, true, new string[] { "1" });


            _db.SaveChanges();
        }
    }
}

