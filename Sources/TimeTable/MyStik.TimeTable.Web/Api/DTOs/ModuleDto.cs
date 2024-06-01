using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleDto : NamedDto
    {
        public string Tag { get; set; }

        public List<ModuleAccreditionDto> Accreditions { get; set; }


        public List<SubjectDto> Subjects { get; set; }

        public List<ExamOptionDto> ExamOptions { get; set; }
    }

    public class SubjectDto : NamedDto
    {
        public string TeachingFormat { get; set; }

        public double SWS { get; set; }
    }

    
    public class ModuleAccreditionDto : NamedDto
    {
        public string Curriculum { get; set; }

        public string Faculty { get; set; }

        public string Slot { get; set; }

        public double Ects { get; set; }
    }

    public class TeachingDto
    {
        public string Category { get; set; }
    }

    public class ExamOptionDto : NamedDto
    {
        public List<ExamDto> Exams { get; set; }
    }


    public class ExamDto : NamedDto
    {
        public double Weight { get; set; }
    }

    public class ExamAidDto
    {
        public string Category { get; set; }

        public string Info { get; set; }
    }

    public class ModuleDescriptionDto : NamedDto
    {
        public string DescriptionDe { get; set; }
        public string DescriptionEn { get; set; }

    }


    public class ModuleDescriptionTeachingDto
    {
        public ModuleDescriptionSubjectDto subject { get; set; }

        public List<ModuleDescriptionCourseDto> courses { get; set; }
    }

    public class ModuleDescriptionSubjectDto
    {
        public string tag { get; set; }
        public string name { get; set; }
        public double sws { get; set; }
        public string type { get; set; }
    }

    public class ModuleDescriptionCourseDto
    {
        public string tag { get; set; }
        public string url { get; set; }
    }

    public class ModuleDescriptionExamDto
    {
        public string first { get; set; }
        public string second { get; set; }
        public string conditions { get; set; }
        public string utilities { get; set; }

        public List<ModuleDescriptionExamFractionDto> fractions { get; set; }
    }

    public class ModuleDescriptionExamFractionDto
    {
        public string tag { get; set; }
        public double weight { get; set; }
    }

    public class ModuleSlotDto
    {
        public string ModuleTag { get; set; }
        public string ModuleName { get; set; }
        public string SubjectTag { get; set; }
        public string SubjectName { get; set; }

        public List<CourseSummaryDto> Courses { get; set; }
    }


    public class ModuleDtoVersion2
    {
        public string CatalogId { get; set; }

        public string Title { get; set; }

        public string UrlDescription { get; set; }

        public List<TeachingDtoVersion2> Teachings { get; set; }
    }

    public class TeachingDtoVersion2
    {
        public string CurriculumId { get; set; }

        public string CurriculumTitle { get; set; }

        public string UrlCurriculumPlan { get; set; }

        public int Semester { get; set; }

       
        public string SubjectTitle { get; set; }

        public string SubjectTeachingFormat { get; set; }
    }


}