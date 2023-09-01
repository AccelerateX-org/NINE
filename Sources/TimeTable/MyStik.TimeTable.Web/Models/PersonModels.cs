using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class PersonViewModel
    {
        public ApplicationUser User { get; set; }

        public List<OrganiserMember> Members { get; set; }

        public List<Course> Courses { get; set; }

        public List<OfficeHour> OfficeHours { get; set; }

        public List<CurriculumModule> Modules { get; set; }


        public string FullName
        {
            get
            {
                if (User != null)
                    return User.FullName;

                if (Members != null && Members.Any())
                {
                    return Members.First().FullName;
                }

                return "#NA";
            }
        }


        public string UrlProfile
        {
            get
            {
                return null;
            }
        }
    }
}