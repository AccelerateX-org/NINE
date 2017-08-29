using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildingCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Institution { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class BuildingViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Building Building { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Institution { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BuildingEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LevelModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CreateLevelModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}