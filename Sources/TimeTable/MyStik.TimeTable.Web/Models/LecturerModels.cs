using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LecturerCharacteristicModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LecturerCharacteristicModel()
        {
            Courses = new List<CourseSummaryModel>();
            OfficeHours = new List<OfficeHourSummaryModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHourViewModel NextOfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OfficeHourSummaryModel> OfficeHours { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseSummaryModel> Courses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseHistoryModel> OldCourses { get; set; }

        public Semester Semester { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerUserModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MapLecturerUserModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string DozId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LecturerStateModel()
        {
            Conflicts = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasConflicts { get { return Conflicts.Any(); } }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ActivityDate> Conflicts { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerListStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LecturerListStateModel()
        {
            LecturerStates = new List<LecturerStateModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityDateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<LecturerStateModel> LecturerStates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LecturerViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Course> Courses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerViewModelMobil
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<LecturerStateViewMobilModel> myLecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<LecturerStateViewModelMobil2> Lecturer { get; set; }
    
    
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerStateViewMobilModel 
    {
        /// <summary>
        /// 
        /// </summary>
        public string DozentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Kontakt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Sprechstunde { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Room Raum { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerStateViewModelMobil2 
    {   
        /// <summary>
        /// 
        /// </summary>
        public string DozentName {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public string   Kontakt {get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string    Sprechstunde {get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public Room    Raum {get; set; }
    }

    public class LecturerOverviewModel
    {
        public LecturerOverviewModel()
        {
            Courses = new List<CourseOverviewModel>();
        }

        public OrganiserMember Lecturer { get; set; }

        public List<CourseOverviewModel> Courses { get; private set; }

        public List<Thesis> Thesis { get; set; }
    }

    public class CourseOverviewModel
    {
        public Course Course { get; set; }

        public Semester Semester { get; set; }
    }


    public class LecturerSummaryModel
    {
        public LecturerSummaryModel()
        {
            Courses = new List<LecturerCourseSummaryModel>();
            OfficeHours = new List<LecturerOfficehourSummaryModel>();
        }

        public ICollection<LecturerCourseSummaryModel> Courses { get; private set; }
        public ICollection<LecturerOfficehourSummaryModel> OfficeHours { get; private set; }

        public ICollection<OrganiserMember> Memberships { get; set; }

        public ICollection<CurriculumModule> Modules { get; set; }

        public Semester Semester { get; set; }

        public List<PersonalDate> Dates { get; set; }

        public Guid OrganiserId { get; set; }
        public Guid SegmentId { get; set; }
    }

    public class LecturerCourseSummaryModel
    {
        public Course Course { get; set; }

        public ActivityDate FirstDate { get; set; }

        public ActivityDate LastDate { get; set; }

        public ActivityOwner Owner { get; set; }

        public List<ActivityDate> HostingDates { get; set; }
    }

    public class LecturerOfficehourSummaryModel
    {
        public OfficeHour OfficeHour { get; set; }

        public ActivityDate FirstDate { get; set; }

        public ActivityDate LastDate { get; set; }

        public ActivityOwner Owner { get; set; }
    }

    public class PersonalDateCreateModel
    {
        public ActivityOrganiser Organiser { get; set; }
        public SemesterDate Segment { get; set; }

        public PersonalDate PersonalDate { get; set; }
    }

}


