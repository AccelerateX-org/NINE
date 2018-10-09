using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class PlanerGroupViewModel
    {
        public PlanerGroupViewModel()
        {
            Courses = new List<CourseSummaryModel>();
        }

        public SemesterGroup SemesterGroup { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public Semester Semester { get; set; }

        public Curriculum Curriculum { get; set; }

        public CurriculumTopic Topic { get; set; }

        public CurriculumChapter Chapter { get; set; }

        public CurriculumGroup CurriculumGroup { get; set; }

        public CapacityGroup CapacityGroup { get; set; }

        public ICollection<CourseSummaryModel> Courses { get; set; }

        public ICollection<SemesterGroup> SemesterGroups { get; set; }

        public ICollection<SemesterTopic> SemesterTopics { get; set; }
    }

    public class CourseSearchModel
    {
        [Display(Name = "Wochentag")]
        public int DayOfWeek { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beginn nach")]
        public string NewBegin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende vor")]
        public string NewEnd { get; set; }

        [Display(Name = "Reichweite")]
        public int Option { get; set; }

        public Semester Semester { get; set; }
    }

    public class StudentPlanerModel
    {
        public StudentPlanerModel()
        {
            Semester = new List<StudentSemesterPlanerModel>();
        }

        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        public List<StudentSemesterPlanerModel> Semester { get; set; }

        public Semester LatestSemester { get; set; }
    }

    public class StudentSemesterPlanerModel
    {
        public StudentSemesterPlanerModel()
        {
            Courses = new List<Course>();
        }

        public Semester Semester { get; set; }

        public List<Course> Courses { get; set; }
    }
}