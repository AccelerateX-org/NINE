using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public OrganiserDto Organiser { get; set; }
    }

    #region structure
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumPackageDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumPackageDto()
        {
            Options = new List<CurriculumOptionDto>();
        }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumOptionDto> Options { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumOptionDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumOptionDto()
        {
            Modules = new List<CurriculumModuleDto>();
        }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumModuleDto> Modules { get; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumModuleDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumModuleDto()
        {
            Sections = new List<CurriculumModuleSectionDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        public double Ects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumModuleSectionDto> Sections { get; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumModuleSectionDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumModuleSectionDto()
        {
            Subjects = new List<CurriculumSubjectDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumSubjectDto> Subjects { get; }

    }

    /// <summary>
    /// Das Fach
    /// </summary>
    public class CurriculumSubjectDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string CourseTypes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ExamTypes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMandatory { get; set; }
    }
    #endregion

    #region schedule

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumTermDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumTermDto()
        {
            Sections = new List<CurriculumTermSectionDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumTermSectionDto> Sections { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumTermSectionDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumTermSectionDto()
        {
            Subjects = new List<CurriculumSubjectDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        public double Ects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumPackageDto Package { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CurriculumOptionDto Option { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CurriculumModuleDto Module { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumSubjectDto> Subjects { get; }
    }


    #endregion



}