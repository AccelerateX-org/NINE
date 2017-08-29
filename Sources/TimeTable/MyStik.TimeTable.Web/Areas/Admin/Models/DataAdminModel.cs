using System.Collections.Generic;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Curriculum;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DataAdminModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DataAdminModel()
        {
            Organisers = new List<OrgState>();
            Curricula = new List<CurriculumState>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int OrgCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ModuleCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GroupCount { get; set; }




        
        /// <summary>
        /// 
        /// </summary>
        public Semester CurrentSemester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<ApplicationUser> MemberUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ApplicationUser> StudUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool MemberUserInitialized { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool StudUserInitialized { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ImportInitialized { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OrgState> Organisers { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumState> Curricula { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrgState
    {
        /// <summary>
        /// 
        /// </summary>
        public OrgState()
        {
            Users = new List<ApplicationUser>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ApplicationUser> Users { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumState
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IModuleCatalog ModuleCatalog { get; set; }
    }
}