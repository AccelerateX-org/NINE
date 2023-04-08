namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SemesterStatisticsDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Subscriptions { get; set; }
    }
}