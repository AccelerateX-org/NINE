using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class CieSemesterModel
    {
        public CieSemesterModel()
        {
            Courses = new List<CieCourseModel>();
        }

        public Semester Semester { get; set; }

        public List<CieCourseModel> Courses { get; private set; }
    }


    public class CieCourseModel
    {
        public CieCourseModel()
        {
            CieGroups = new List<SemesterGroup>();
        }
        public CourseSummaryModel Course { get; set; }

        public List<SemesterGroup> CieGroups { get; private set; }

        public bool IsBachelor
        {
            get { return CieGroups.Any(x => x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE-B")); }
        }

        public bool IsMaster
        {
            get { return CieGroups.Any(x => x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE-M")); }
        }

        public List<ActivityOrganiser> Faculties
        {
            get
            {
                return CieGroups.Select(x => x.CapacityGroup.CurriculumGroup.Curriculum.Organiser).Distinct().ToList();
            }
        }

        public int AmpelState
        {
            get
            {
                if (Course.Course.Occurrence.IsCoterie)
                {
                    return 3;            // red
                }
                else
                {
                    if (Course.Course.Occurrence.HasHomeBias)
                    {
                        return 2;        // yellow
                    }
                    else
                    {
                        return 1;        // green
                    }
                }

            }
        }

    }
}