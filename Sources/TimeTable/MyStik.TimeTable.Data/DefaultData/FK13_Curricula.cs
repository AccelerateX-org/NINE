using System.Collections.Generic;

namespace MyStik.TimeTable.Data.DefaultData
{
    public partial class InfrastructureData
    {
        private void FK13_InitCurriculumAW(ActivityOrganiser fk13)
        {
            var AW = GetCurriculum(fk13, "AW");

            if (AW != null)
                return;

            // Studienprogramme
            AW = new Curriculum
            {
                ShortName = "AW",
                Name = "FAKULTÄT STUDIUM GENERALE UND INTERDISZIPLINÄRE STUDIEN - GENERAL STUDIES",
                Organiser = fk13,
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

            _db.Curricula.Add(AW);

    
        }
    }
}

