using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CourseSummaryModel> Courses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<LecturerViewModel> Lecturers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester NextSemester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CourseSummaryModel> NextCourses { get; set; }

    }
}