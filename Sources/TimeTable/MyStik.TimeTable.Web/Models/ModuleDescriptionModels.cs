using System;
using System.Collections.Generic;
using System.Web.Mvc;
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

        [AllowHtml]
        public string Conditions { get; set; }

        [AllowHtml]
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
        
        public List<ExaminationDescription> Exams { get; set; }
    }

    public class ModuleDescriptionsViewModel
    {
        public ActivityOrganiser Organiser { get; set; }
        public CurriculumModule Module { get; set; }
        public Semester Semester { get; set; }
        public List<ModuleDescription> ModuleDescriptions { get; set; }
        public List<ModuleDescription> BadModuleDescriptions { get; set; }

        public List<ExaminationDescription> Exams { get; set; }
        public List<ActivityOrganiser> Organisers { get; set; }
    }


    public class ModuleDescriptionEditModel
    {
        public ModuleDescription ModuleDescription { get; set; }

        [AllowHtml]
        public string DescriptionText { get; set; }

        [AllowHtml]
        public string DescriptionTextEn { get; set; }
    }

    public class ExaminationFractionViewModel
    {
        public ExaminationOption Option { get; set; }

        public Guid FractionId { get; set; }

        public Guid ExaminationTypeId { get; set; }

        public int Weight { get; set; }

        public int MinDuration { get; set; }

        public int MaxDuration { get; set; }
    }

    public class SubjectViewModel
    {
        public CurriculumModule Module { get; set; }

        public Guid SubjectId { get; set; }

        public Guid TeachingTypeId { get; set; }

        public string Tag { get; set; }

        public string Name { get; set; }

        public double SWS { get; set; }
    }

}