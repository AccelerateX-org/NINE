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
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumDto Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Ects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double UsCredits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Sws { get; set; }

    }

    public class AccreditatedModuleDto : NamedDto
    {
        public AccreditatedModuleDto()
        {
            ModuleAccounts = new List<LecturerDto>();
            Lecturers = new List<LecturerDto>();
        }

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

    public class ExamDto
    {
        public string Category { get; set; }

        public double CalcFactor { get; set; }

        public string Info { get; set; }
    }

    public class ExamAidDto
    {
        public string Category { get; set; }

        public string Info { get; set; }
    }

}