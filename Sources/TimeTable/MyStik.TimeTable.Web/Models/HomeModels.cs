using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public HomeViewModel()
        {
            Faculties = new List<FacultyViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<FacultyViewModel> Faculties { get; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

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
    }
}