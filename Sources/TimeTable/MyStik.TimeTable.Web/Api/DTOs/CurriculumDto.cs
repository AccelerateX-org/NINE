using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumDto
    {
        public string Curriculum_id { get; set; }

        public string Name { get; set; }    
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
            Modules = new List<ModuleDto>();
        }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ModuleDto> Modules { get; }

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
        public ModuleDto Module { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumSubjectDto> Subjects { get; }
    }


    #endregion

    #region Studienplan

    public class CurriculumSchemeSemesterDto
    {
        public CurriculumSchemeSemesterDto()
        {
            Modules = new List<CurriculumSchemeModuleDto>();
        }

        public int Term { get; set; }

        public List<CurriculumSchemeModuleDto> Modules { get; }
    }

    public class CurriculumSchemeModuleDto : NamedDto
    {
        public CurriculumSchemeModuleDto()
        {
        }

        /// <summary>
        /// Summe der Ects im Semester
        /// </summary>
        public double Ects { get; set; }
    }

    public class CurriculumSchemeSubjectDto : NamedDto
    {
        public CurriculumSchemeSubjectDto()
        {
            Options = new List<CurriculumSchemeOptionDto>();
        }

        public double ECTS { get; set; }

        public List<CurriculumSchemeOptionDto> Options { get; }

    }

    /// <summary>
    /// Hier steckt dann die ModulID drin, die zur Modulbeschreibung führt
    /// </summary>
    public class CurriculumSchemeOptionDto : NamedDto
    {
        public string Number { get; set; }
        public bool IsMandatory { get; set; }
    }


    public class CurriculumSlotDto
    {
        public Guid id { get; set; }

        public string tag { get; set; }
        
        public int semester { get; set; }

        public string name { get; set; }
        
        public string description { get; set; }
        
        public double ects { get; set; }

        public List<ModuleDto> modules { get; set; }
    }

    #endregion
}