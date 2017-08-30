using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        private void FK01_InitCurriculumAR(ActivityOrganiser fk01)
        {
            var ar = GetCurriculum(fk01, "AR");

            if (ar != null)
                return;

            //Studienprogramme
            ar = new Curriculum
            {
                ShortName = "AR",
                Name = "Bachelor Architektur",
                Organiser = fk01,
                CurriculumGroups = new HashSet<CurriculumGroup>
                {
                    new CurriculumGroup { Name = "1", IsSubscribable = true},
                    new CurriculumGroup { Name = "2", IsSubscribable = true},
                    new CurriculumGroup { Name = "3", IsSubscribable = true},
                    new CurriculumGroup { Name = "4", IsSubscribable = true},
                    new CurriculumGroup { Name = "5", IsSubscribable = true},
                    new CurriculumGroup { Name = "6", IsSubscribable = true},
                    new CurriculumGroup { Name = "7", IsSubscribable = true},
                    new CurriculumGroup { Name = "WPM", IsSubscribable = true},
                    new CurriculumGroup { Name = "AW", IsSubscribable = true},
                },
            };

            _db.Curricula.Add(ar);


            AddCapacityGroup(ar, "1", "A", true, true, new string[] { "1A" });

            AddCapacityGroup(ar, "2", "A", true, true, new string[] { "2A" });
 
            AddCapacityGroup(ar, "3", "A", true, true, new string[] { "3A" });

            AddCapacityGroup(ar, "4", "A", true, true, new string[] { "4A" });

            AddCapacityGroup(ar, "5 ARC", "", true, true, new[] { "5ARC", "5ARC/KON", "5ARC/STB" });
            AddCapacityGroup(ar, "5 KON", "", true, true, new[] { "5KON", "5KON/ARC", "5KON/STB" });
            AddCapacityGroup(ar, "5 STB", "", true, true, new[] { "5STB", "5STB/KON", "5STB/ARC" });

            AddCapacityGroup(ar, "6", "A", true, true, new string[] { "6A" });

            AddCapacityGroup(ar, "7", "A", true, true, new string[] { "7A" });

            AddCapacityGroup(ar, "WPM", "", true, true, new string[] { "WPM" });

            _db.SaveChanges();
        }

    }
}
