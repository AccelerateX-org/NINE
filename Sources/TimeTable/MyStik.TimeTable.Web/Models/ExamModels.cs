using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisListViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumModule Module { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StudentExam Exam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ThesisCriteriaListModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumCriteria Criteria { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumModule Module { get; set; }
    }
}