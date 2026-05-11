using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    public class ModuleApiContract
    {
        public string InstitutionId { get; set; }

        public string OrganiserId { get; set; }

        public string CatalogId { get; set; }

        public string ModuleId { get; set; }

        public string Title { get; set; }

        public List<ModuleInstructionApiContract> Instructions { get; set; }

        public List<ModuleChallengeApiContract> Challenges { get; set; }

        public List<ModuleObjectiveApiContract> Objectives { get; set; }
    }

    public class ModuleInstructionApiContract
    {
        public string InstructionId { get; set; }

        public string Title { get; set; }
     
    }

    public class ModuleChallengeApiContract
    {
        public string Title { get; set; }
        public string ExaminationType { get; set; }
    }

    public class ModuleObjectiveApiContract
    {
        public string ObjectiveId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ModuleTeachingApiContract
    {
        public string SemesterId { get; set; }
        public string InstructionId { get; set; }
        public string CourseId { get; set; }
    }

    public class ModuleExaminationApiContract
    {
        public string SemesterId { get; set; }
        public string ChallengeId { get; set; }
        public string ExamId { get; set; }
    }
}
