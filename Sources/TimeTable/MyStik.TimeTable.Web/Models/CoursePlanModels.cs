using MyStik.TimeTable.Data;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CoursePlanViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<MyStik.TimeTable.Data.CurriculumModule> Semester7 { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CoursePlanPlanningViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CoursePlanPlanningViewModel()
        {
            SemesterModules = new List<CoursePlanSemesterViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public CoursePlan CoursePlan {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public List<CoursePlanSemesterViewModel> SemesterModules { get; set; }



    }

    /// <summary>
    /// 
    /// </summary>
    public class CoursePlanSemesterViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CoursePlanSemesterViewModel()
        {
            Modules = new List<ModuleMapping>();
            CurriculumModules = new List<ModuleMapping>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public Semester NextSemester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester PrevSemester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CoursePlan Plan { get; set; }

        /// <summary>
        /// Ist (Plan) Module
        /// </summary>
        public List<ModuleMapping> Modules {get; set;}

        /// <summary>
        /// Module nach SPO
        /// </summary>
        public List<ModuleMapping> CurriculumModules { get; set; }
    }
}