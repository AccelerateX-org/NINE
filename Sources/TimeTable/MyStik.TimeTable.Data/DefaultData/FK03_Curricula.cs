using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{

    public partial class InfrastructureData
    {

        // hier wird der Studiengang Fahrzeugtechnik Bachelor angelegt
        public void FK03_InitCurriculumFAB(ActivityOrganiser fk03)
        {
            var fab = GetCurriculum(fk03, "FAB");

            if (fab != null)
                return;

            fab = new Curriculum
            {
                ShortName = "FAB",
                Name = "Bachelor Fahrzeugtechnik",
                Organiser = fk03,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
                new CurriculumGroup { Name = "1 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "2 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "3 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "4 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "7 AS", IsSubscribable = true },
                new CurriculumGroup { Name = "7 KF", IsSubscribable = true },
                new CurriculumGroup { Name = "7 SA", IsSubscribable = true },
                new CurriculumGroup { Name = "7 SV", IsSubscribable = true },
                new CurriculumGroup { Name = "WPM", IsSubscribable = false },
                new CurriculumGroup { Name = "AW", IsSubscribable = false },
                },
            };

            _db.Curricula.Add(fab);

            AddCapacityGroup(fab, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(fab, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(fab, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(fab, "1 AU", "A", true, true, new string[] { "1AU" });

            AddCapacityGroup(fab, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(fab, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(fab, "2 AU", "A", true, true, new string[] { "2AU" });

            AddCapacityGroup(fab, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(fab, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(fab, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(fab, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(fab, "5", "", true, true, new string[] { "5A" });

            AddCapacityGroup(fab, "6 AS", "", true, true, new string[] { "6AS", "6AS/KF", "6AS/SA", "6AS/SV"});


            AddCapacityGroup(fab, "6 KF", "", true, true, new string[] { "6KF", "6KF/AS", "6KF/SA", "6KF/SV"});


            AddCapacityGroup(fab, "6 SA", "", true, true, new string[] { "6SA", "6SA/KF", "6SA/AS", "6SA/SV"});


            AddCapacityGroup(fab, "6 SV", "", true, true, new string[] { "6SV", "6SV/KF", "6SV/SA", "6SV/AS"});

            _db.SaveChanges();
        }

        // hier wird der Studiengang Luft -und Rauhmfahrttechnik angelegt
        public void FK03_InitCurriculumLRB(ActivityOrganiser fk03)
        {
            var lrb = GetCurriculum(fk03, "LRB");

            if (lrb != null)
                return;

            lrb = new Curriculum
            {
                ShortName = "LRB",
                Name = "Bachelor Luft- und Raumfahrtechnik",
                Organiser = fk03,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
                new CurriculumGroup { Name = "1 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "2 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "6", IsSubscribable = true },
                new CurriculumGroup { Name = "7", IsSubscribable = true },
                new CurriculumGroup { Name = "WPM", IsSubscribable = false },
                new CurriculumGroup { Name = "AW", IsSubscribable = false },
            },
            };

            _db.Curricula.Add(lrb);

            AddCapacityGroup(lrb, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(lrb, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(lrb, "1 AU", "A", true, true, new string[] { "1AU" });

            AddCapacityGroup(lrb, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(lrb, "2 AU", "A", true, true, new string[] { "2AU" });

            AddCapacityGroup(lrb, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(lrb, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(lrb, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(lrb, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(lrb, "5", "", true, true, new string[] { "5A" });

            AddCapacityGroup(lrb, "6", "", true, true, new string[] { "6A" });

            AddCapacityGroup(lrb, "7", "", true, true, new string[] { "57" });

            _db.SaveChanges();

        }

	
        public void FK03_InitCurriculumMBB(ActivityOrganiser fk03)
        {
            var mbb = GetCurriculum(fk03, "MBB");

            if (mbb != null)
                return;

            mbb = new Curriculum
            {
                ShortName = "MBB",
                Name = "Bachelor Maschinenbau",
                Organiser = fk03,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1", IsSubscribable = true },
                new CurriculumGroup { Name = "1 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "2", IsSubscribable = true },
                new CurriculumGroup { Name = "2 DU", IsSubscribable = true },
                new CurriculumGroup { Name = "3", IsSubscribable = true },
                new CurriculumGroup { Name = "4", IsSubscribable = true },
                new CurriculumGroup { Name = "5", IsSubscribable = true },
                new CurriculumGroup { Name = "6", IsSubscribable = true },
                new CurriculumGroup { Name = "7", IsSubscribable = true },
                new CurriculumGroup { Name = "WPM", IsSubscribable = false },
                new CurriculumGroup { Name = "AW", IsSubscribable = false },
            },
            };

            _db.Curricula.Add(mbb);

            AddCapacityGroup(mbb, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(mbb, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(mbb, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(mbb, "1 AU", "A", true, true, new string[] { "1AU" });

            AddCapacityGroup(mbb, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(mbb, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(mbb, "2 AU", "A", true, true, new string[] { "2AU" });

            AddCapacityGroup(mbb, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(mbb, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(mbb, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(mbb, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(mbb, "5", "", true, true, new string[] { "5A" });

            AddCapacityGroup(mbb, "6", "", true, true, new string[] { "6A" });

            AddCapacityGroup(mbb, "7", "", true, true, new string[] { "57" });

            _db.SaveChanges();
        }

        public void FK03_InitCurriculumFEM_TBM_FAM_LRM_MBM(ActivityOrganiser fk03)
        {
            var fem_tbm_fam_lrm_mbm = GetCurriculum(fk03, "FEM_TBM_FAM_LRM_MBM");

            if (fem_tbm_fam_lrm_mbm != null)
                return;

            fem_tbm_fam_lrm_mbm = new Curriculum
            {
                ShortName = "FEM_TBM_FAM_LRM_MBM",
                Name = "Master FEM_TBM_FAM_LRM_MBM",
                Organiser = fk03,
                CurriculumGroups = new HashSet<CurriculumGroup>
            {
                new CurriculumGroup { Name = "1 FAM", IsSubscribable = true },
                new CurriculumGroup { Name = "2 FAM", IsSubscribable = true },
                new CurriculumGroup { Name = "1 FEM", IsSubscribable = true },
                new CurriculumGroup { Name = "2 FEM", IsSubscribable = true },
                new CurriculumGroup { Name = "3 FEM", IsSubscribable = true },
                new CurriculumGroup { Name = "1 LRM", IsSubscribable = true },
                new CurriculumGroup { Name = "2 LRM", IsSubscribable = true },
                new CurriculumGroup { Name = "1 MBM", IsSubscribable = true },
                new CurriculumGroup { Name = "2 MBM", IsSubscribable = true },
                new CurriculumGroup { Name = "1 TBM", IsSubscribable = true },
                new CurriculumGroup { Name = "2 TBM", IsSubscribable = true },
            },
            };

            _db.Curricula.Add(fem_tbm_fam_lrm_mbm);

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "1", "A", true, true, new string[] { "1A" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "1", "B", true, true, new string[] { "1B" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "1", "C", true, true, new string[] { "1C" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "1 AU", "A", true, true, new string[] { "1AU" });

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "2", "A", true, true, new string[] { "2A" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "2", "B", true, true, new string[] { "2B" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "2 AU", "A", true, true, new string[] { "2AU" });

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "3", "A", true, true, new string[] { "3A" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "3", "B", true, true, new string[] { "3B" });

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "4", "A", true, true, new string[] { "4A" });
            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "4", "B", true, true, new string[] { "4B" });

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "5", "", true, true, new string[] { "5A" });

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "6", "", true, true, new string[] { "6A" });

            AddCapacityGroup(fem_tbm_fam_lrm_mbm, "7", "", true, true, new string[] { "57" });

            _db.SaveChanges();
        }
    }

    
}

        

        
