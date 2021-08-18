using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ScriptCreatetModel
    {
        public string Title { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase ScriptDoc { get; set; }


        public string Module { get; set; }

        public bool AllCourses { get; set; }

        public Semester Semester { get; set; }

        public Guid SemesterId { get; set; }
    }
}