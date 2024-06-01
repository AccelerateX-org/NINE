using System.Collections.Generic;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeViewModel
    {
        public List<SemesterActiveViewModel> ActiveSemester { get; private set; } = new List<SemesterActiveViewModel>();

        public List<ActivityOrganiser> Organisers { get; private set; } = new List<ActivityOrganiser>();

        public List<Institution> Institutions { get; private set; } = new List<Institution>();
        public Semester CurrentSemester { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FacultyViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StudentCount { get; set; }

        public bool HasCurrentSchedule { get; set; }

        public bool HasNextSchedule { get; set; }

    }

    public class SemesterActiveViewModel
    {
        public SemesterActiveViewModel()
        {
            Topics = new List<TopicSummaryModel>();
            Courses = new List<CourseSummaryModel>();
            Events = new List<Event>();
        }

        public Semester Semester { get; set; }

        public SemesterDate Segment { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<ActivityOrganiser> Organisers { get; set; }

        public ICollection<Curriculum> Curricula { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public Curriculum Curriculum { get; set; }

        public CurriculumModuleCatalog Catalog { get; set; }

        public CapacityGroup CapacityGroup { get; set; }

        public SemesterGroup SemesterGroup { get; set; }

        public List<TopicSummaryModel> Topics { get; set; }

        public List<CourseSummaryModel> Courses { get; set; }

        public List<Event> Events { get; set; }

        public ItemLabel Label { get; set; }

        public List<Lottery> Lotteries { get; set; }
    }

    public class SemesterScheduleViewModel
    {
        public SemesterScheduleViewModel()
        {
            Courses = new List<CourseSummaryModel>();
        }

        public Semester Semester { get; set; }

        public Curriculum Curriculum { get; set; }

        public List<CourseSummaryModel> Courses { get; private set; }
    }

    public class CourseScheduleViewModel
    {
        public Semester Semester { get; set; }

        public Curriculum Curriculum { get; set; }


        public Course Course { get; set; }
    }

    public class SlotSemesterModel
    {
        public Semester Semester { get; set; }

        public Curriculum Curriculum { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public AreaOption Option { get; set; }

        public int NUmberSemester { get; set; }

        public CurriculumSlot Slot { get; set; }

        public List<CurriculumSlot> Slots { get; set; }
    }

}