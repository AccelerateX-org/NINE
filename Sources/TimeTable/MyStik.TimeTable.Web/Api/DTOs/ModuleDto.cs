using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

}