using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Level Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Stockwerke { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Plan { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Räume { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class LevelCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Stockwerk { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Plan { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Räume { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class LevelEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Stockwerk { get; set; }
    }
}