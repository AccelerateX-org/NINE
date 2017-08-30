using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        public void FK12_InitCurriculumDE(ActivityOrganiser fk12)
        {

                var DE = GetCurriculum(fk12, "DE");

                if (DE != null)
                    return;

                // Studienprogramme
                DE = new Curriculum
                {
                    ShortName = "DE",
                    Name = "Bachelor Design",
                    Organiser = fk12,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},

                },
                };

                _db.Curricula.Add(DE);

                AddCapacityGroup(DE, "1", "", true, true, new string[] { "1" });
                AddCapacityGroup(DE, "3", "", true, true, new string[] { "3" });
                AddCapacityGroup(DE, "5", "", true, true, new string[] { "5" });
                AddCapacityGroup(DE, "7", "", true, true, new string[] { "7" });

                _db.SaveChanges();
        }
        public void FK12_InitCurriculumMDE(ActivityOrganiser fk12)
        {
                var MDE = GetCurriculum(fk12, "MDE");

                if (MDE != null)
                    return;

                // Studienprogramme
                MDE = new Curriculum
                {
                    ShortName = "MDE",
                    Name = "Master Design",
                    Organiser = fk12,
                    CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},

                },
                };

                _db.Curricula.Add(MDE);

                AddCapacityGroup(MDE, "1", "", true, true, new string[] { "1" });
                AddCapacityGroup(MDE, "2", "", true, true, new string[] { "3" });
                AddCapacityGroup(MDE, "3", "", true, true, new string[] { "5" });

                _db.SaveChanges();
        }
    }
}
