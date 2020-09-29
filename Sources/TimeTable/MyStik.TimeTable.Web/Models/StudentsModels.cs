using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class StudentViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public StudentViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterSubscription CurrentSubscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterSubscription LastSubscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Course> AllCourses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Course> CoursesFit { get; set; }

        public ICollection<Course> CurrentCourses { get; set; }

        public ICollection<Course> LastCourses { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public Semester Semester { get; set; }

    }


    /// <summary>
    /// 
    /// </summary>
    public class InvitationFileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<HttpPostedFileBase> Attachments { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class StudentInvitationModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Invite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Invited { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup SemGroup { get; set; }

        public string Course { get; set; }

        public string State { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvitationCheckModel
    {
        /// <summary>
        /// 
        /// </summary>
        public InvitationCheckModel()
        {
            Invitations = new List<StudentInvitationModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StudentInvitationModel> Invitations { get; private set; }
    }

    public class StudentStatisticsModel
    {
        public Curriculum Curriculum { get; set; }

        public Semester Semester { get; set; }

        public int Count { get; set; }
    }

    public class CieInvitationCheckModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CieInvitationCheckModel()
        {
            Invitations = new List<CieInvitationModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CieInvitationModel> Invitations { get; private set; }
    }

    public class CieInvitationModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        public Course Course { get; set; }

        public bool OnWaitinglist { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Invite { get; set; }

    }


    public class StudentSubscriptionModel
    {
        public Student Student { get; set; }

        public ApplicationUser User { get; set; }

        [Display(Name = "Bezeichnung Semester")]
        public string SemesterName { get; set; }

        [Display(Name="Kurzname der Lehrveranstaltung")]
        public string CourseShortName { get; set; }
    }

    public class StudentDetailViewModel
    {
        public StudentDetailViewModel()
        {
            Semester = new List<StudentSemesterViewModel>();
            Students = new List<Student>();
        }

        public Student Student { get; set; }

        public ApplicationUser User { get; set; }

        public List<StudentSemesterViewModel> Semester { get; set; }

        public List<Student> Students { get; set; }

    }

    public class StudentSemesterViewModel
    {
        public StudentSemesterViewModel()
        {
            Courses = new List<Course>();
        }

        public Semester Semester { get; set; }

        public List<Course> Courses { get; set; }
    }

    public class StudentsByCurriculumViewModel
    {
        public Curriculum Curriculum { get; set; }

        public List<Student> Students { get; set; }

        public List<Alumnus> Alumnae { get; set; }
    }

    public class StudentsOrgViewModel
    {
        public StudentsOrgViewModel()
        {
            StudentsByCurriculum = new List<StudentsByCurriculumViewModel>();
            Semesters = new List<Semester>();
        }


        public ActivityOrganiser Organiser { get; set; }

        public List<StudentsByCurriculumViewModel> StudentsByCurriculum { get; private set; }

        public List<Semester> Semesters { get; set; }
    }


    public class StudentSummaryModel
    {
        public Student Student { get; set; }

        public Thesis Thesis { get; set; }

        public Internship Internship { get; set; }

        public Semester Semester { get; set; }
        public Semester PrevSemester { get; set; }
        public Semester NextSemester { get; set; }

        public ICollection<CourseSummaryModel> Courses { get; set; }

    }


}