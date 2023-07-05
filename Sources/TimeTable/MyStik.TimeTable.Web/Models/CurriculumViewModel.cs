using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Controllers;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumViewModel
    {
        public CurriculumViewModel()
        {
            ActiveSemesters = new List<Semester>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<SemesterGroup> SemesterGroups { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Curriculum> Curricula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<SemesterTopic> Topics { get; set; }

        public Semester Semester { get; set; }

        public Semester NextSemester { get; set; }

        public Semester PreviousSemester { get; set; }


        public List<Semester> ActiveSemesters { get; set; }

        public List<Assessment> Assessments { get; set; }

        public ItemLabel FilterLabel { get; set; }

        public List<AreaSelectViewModel> Areas { get; set; }
    }

    public class AreaSelectViewModel
    {
        public CurriculumArea Area { get; set; }

        public AreaOption Option { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class GroupSelectionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "FacultyField", ResourceType =typeof(Resources))]
        public String Faculty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "CurriculumField", ResourceType=typeof(Resources))]
        public string Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "SemGroupField", ResourceType =typeof(Resources))]
        public string Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="SemesterString", ResourceType =typeof(Resources))]
        public string Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterSubscription Subscription { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumCreateAliasModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CurriculumId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Aliasname")]
        public string AliasName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name Curriculumsgruppe")]
        public string CurrGroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Zusatz Semestergruppe")]
        public string SemGroupName { get; set; }
    }

    public class CurriculumTermViewModel
    {
        public Curriculum Curriculum { get; set; }


        //public ICollection<IGrouping<int, CurriculumCriteria>> Terms { get; set; }

    }

    public class CurriculumTransferModel
    {
        public Curriculum Curriculum { get; set; }

        public Guid TargetCurrId { get; set; }

    }

    public class CurriculumSummaryModel
    {
        public Curriculum Curriculum { get; set; }

        public List<Student> Students { get; set; }
    }


    public class CurriculumAreaCreateModel
    {
        public Guid CurrId { get; set; }
        public Guid AreaId { get; set; }
        public Guid OptionId { get; set; }
        public Guid SlotId { get; set; }

        public string Tag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
        public double Ects { get; set; }

    }


    public class MoveSlotModel
    {
        public Curriculum Curriculum { get; set; }
    }

    public class CurriculumDeleteModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DeleteCancelled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DeleteHosting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool KeepOwnership { get; set; }

        public string Code { get; set; }
    }

    public class CurriculumEditModel
    {
        public Guid CurriculumId { get; set; }

        public string Tag { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public string Version { get; set; }

        public int ThesisDuration { get; set; }

        public bool IsDeprecated { get; set; }

        public Guid DegreeId { get; set; }

        public int EctsTarget { get; set; }
    }

}