using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleDto : NamedDto
    {
        public List<SubjectDto> Subjects { get; set; }

        public List<ExamOptionDto> ExamOptions { get; set; }
    }

    public class SubjectDto : NamedDto
    {
        public string TeachingFormat { get; set; }

        public double SWS { get; set; }
    }


    public class CertificateModuleDto : NamedDto
    {
        public CertificateModuleDto()
        {
            Subjects = new List<AccreditatedModuleDto>();
        }

        public List<AccreditatedModuleDto> Subjects { get; set; }
    }

    public class AccreditatedModuleDto : NamedDto
    {
        public AccreditatedModuleDto()
        {
            ModuleAccounts = new List<LecturerDto>();
            Lecturers = new List<LecturerDto>();
        }

        public double Ects { get; set; }

        public int Term { get; set; }

        public string Number { get; set; }

        public bool isMandatory { get; set; }

        public List<LecturerDto> ModuleAccounts { get; set; }

        public List<LecturerDto> Lecturers { get; set; }

        public string Language { get; set; }

        public string Content { get; set; }

        public List<TeachingDto> TeachingMethods { get; set; }
        public List<ExamDto> Exams { get; set; }

        public List<ExamAidDto> Resources { get; set; }
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

    public class ModuleDescriptionDto
    {
        public Guid id { get; set; }

        public string tag { get; set; }
        public string name { get; set; }

        public List<ModuleDescriptionRespDto> responsible { get; set; }

        public List<ModuleDescriptionSlotDto> slots { get; set; }

        public List<ModuleDescriptionInstanceDto> instances { get; set; }

    }

    public class ModuleDescriptionRespDto
    {
        public string tag { get; set; }
    }

    public class ModuleDescriptionSlotDto
    {
        public string tag { get; set; }
    }

    public class ModuleDescriptionInstanceDto
    {
        public string semster { get; set; }

        public string description { get; set; }

        public List<ModuleDescriptionTeachingDto> teaching { get; set; }

        public List<ModuleDescriptionExamDto> exams { get; set; }
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
}
