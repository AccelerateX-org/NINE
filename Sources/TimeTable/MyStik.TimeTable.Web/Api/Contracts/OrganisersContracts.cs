using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    public class OrganiserApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string Organiser_Id { get; set; }

        public string Name { get; set; }
    }

    public class OrganiserLecturerApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string Lecturer_Id { get; set; }
        public string Name { get; set; }
    }

    public class OrganiserCurriculumApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string Curriculum_Id { get; set; }
        public string Name { get; set; }
    }


    public class OrganiserModuleApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string Module_Id { get; set; }
        public string Name_de { get; set; }
        public string Name_en { get; set; }
    }

}