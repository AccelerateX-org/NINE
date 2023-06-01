using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ExaminationEditModel
    {
        public Guid orgId { get; set; }

        public Guid moduleId { get; set; }

        public Guid examinationId { get; set; }

        public Guid accredidationId { get; set; }

        public Guid semesterId { get; set; }

        public Guid examOptId { get; set; }

        public Guid firstMemberId { get; set; }

        public Guid secondMemberId { get; set;}

        public string Conditions { get; set; }

        public string Utilities { get; set; }

        public Semester Semester { get; set; }

        public CurriculumModule Module { get; set; }

        public ModuleAccreditation Accreditation { get; set; }

        public OrganiserMember FirstMember { get; set; }

        public OrganiserMember SecondMember { get; set; }
    }

    public class ModuleSemesterView
    {
        public Curriculum Curriculum { get; set; }

        public CurriculumModule CurriculumModule { get; set; }

        public Semester Semester { get; set; }

        public ModuleDescription ModuleDescription { get; set; }

        public List<CurriculumModule> Modules { get; set; }
        
    }

    public class ModuleDescriptionsViewModel
    {
        public CurriculumModule Module { get; set; }
        public Semester Semester { get; set; }
        public List<ModuleDescription> ModuleDescriptions { get; set; }
        public List<ModuleDescription> BadModuleDescriptions { get; set; }
    }

}