using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyStik.TimeTable.Data;

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

        public List<Semester> ActiveSemesters { get; set; }

        public List<Assessment> Assessments { get; set; }
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

}