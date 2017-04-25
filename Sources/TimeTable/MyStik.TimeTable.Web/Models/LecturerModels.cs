using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class LecturerCharacteristicModel
    {
        public LecturerCharacteristicModel()
        {
            Courses = new List<CourseSummaryModel>();
            OfficeHours = new List<OfficeHourSummaryModel>();
        }
        public OrganiserMember Lecturer { get; set; }

        public OfficeHourViewModel NextOfficeHour { get; set; }

        public List<OfficeHourSummaryModel> OfficeHours { get; set; }

        public List<CourseSummaryModel> Courses { get; set; }

        public List<CourseHistoryModel> OldCourses { get; set; }
    }


    public class LecturerUserModel
    {
        public OrganiserMember Lecturer { get; set; }

        public ApplicationUser User { get; set; }
    }

    public class MapLecturerUserModel
    {
        public string DozId { get; set; }
        public string UserName { get; set; }
    }


    public class LecturerStateModel
    {
        public LecturerStateModel()
        {
            Conflicts = new List<ActivityDate>();
        }

        public OrganiserMember Lecturer { get; set; }

        public bool HasConflicts { get { return Conflicts.Any(); } }

        public IEnumerable<ActivityDate> Conflicts { get; set; }
    }

    public class LecturerListStateModel
    {
        public LecturerListStateModel()
        {
            LecturerStates = new List<LecturerStateModel>();
        }

        public Guid ActivityDateId { get; set; }

        public List<LecturerStateModel> LecturerStates { get; set; }
    }

    public class LecturerViewModel
    {
        public LecturerViewModel()
        {
        }
        public OrganiserMember Lecturer { get; set; }

        public OfficeHour OfficeHour { get; set; }

        public List<Course> Courses { get; set; }

        public ApplicationUser User { get; set; }
    }

public class LecturerViewModelMobil
{

    public ICollection<LecturerStateViewMobilModel> myLecturer { get; set; }
    public ICollection<LecturerStateViewModelMobil2> Lecturer { get; set; }
    
    
}

public class LecturerStateViewMobilModel 
{
    public string DozentName { get; set; }

    public string Kontakt { get; set; }

    public string Sprechstunde { get; set; }

    public Room Raum { get; set; }
}

        public class LecturerStateViewModelMobil2 
        {   
            public string DozentName {get; set;}

            public string   Kontakt {get; set; }

            public string    Sprechstunde {get; set; } 

            public Room    Raum {get; set; }
      
}

}


