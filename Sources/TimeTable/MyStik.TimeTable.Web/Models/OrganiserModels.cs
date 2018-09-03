using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganiserViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OrganiserViewModel()
        {
            Members = new List<MemberViewModel>();
            Courses = new List<CourseSummaryModel>();
            Events = new List<EventViewModel>();
            SemesterGroups = new List<SemesterGroupViewModel>();
            ActiveSemesters = new List<Semester>();
            ActiveLotteries = new List<Lottery>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester PreviousSemester { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Semester NextSemester { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<MemberViewModel> Members { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseSummaryModel> Courses { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<EventViewModel> Events { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int MemberCount { get { return Organiser.Members.Count; } }

        /// <summary>
        /// 
        /// </summary>
        public int CourseCount
        {
            get
            {
                return Organiser.Activities.OfType<Course>().Count(c => c.SemesterGroups.Any(g => g.Semester.Name.Equals("SS14")));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int EventCount { get { return Organiser.Activities.OfType<Event>().Count(); } }

        /// <summary>
        /// 
        /// </summary>
        public int NewsletterCount { get { return Organiser.Activities.OfType<Newsletter>().Count(); } }

        public ICollection<IGrouping<string, OrganiserMember>> Roles { get; set; }
        public ICollection<IGrouping<Tag, MemberResponsibility>> Responsibilities { get; set; }

        public List<SemesterGroupViewModel> SemesterGroups { get; private set; }

        public List<Semester> ActiveSemesters { get; }

        public List<Lottery> ActiveLotteries { get; }

        public List<IGrouping<Curriculum, SemesterGroup>> Groups { get; set; }


    }

    public class SemesterGroupViewModel
    {

        public SemesterGroup Group { get; set; }

        public List<string> UserIds { get; set; }
    }


    public class SemesterOverviewModel
    {
        public SemesterOverviewModel()
        {
            SemesterGroups = new List<SemesterGroupViewModel>();
        }

        public Semester Semester { get; set; }

        public List<SemesterGroupViewModel> SemesterGroups { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class MemberViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Member { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ItsMe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool WasActiveLastSemester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool WasActiveLastYear { get; set; }

        public List<MemberExportViewModel> Exports { get; set; }

        public List<Course> ActiveCourses { get; set; }
    }


    public class MemberExportViewModel
    {
        public MemberExport Export { get; set; }

        public Activity Activity { get; set; }

        public OrganiserMember ExternalMember { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MemberUserViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Benutzername")]
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Rolle")]
        public string Role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Kurzname, Identifikation")]
        [Required]
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "URL HM-Profil oder Homepage")]
        public string UrlProfile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "LB, Gast, etc.")]
        public bool IsAssociated { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Personen")]
        public bool IsMemberAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Räume")]
        public bool IsRoomAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Termine")]
        public bool IsSemesterAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Studiengänge")]
        public bool IsCurriculumAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Kurse")]
        public bool IsCourseAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Studenten")]
        public bool IsStudentAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Alumni")]
        public bool IsAlumniAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Veranstaltungen")]
        public bool IsEventAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Administration Newsletter")]
        public bool IsNewsAdmin { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserRight
    {
        /// <summary>
        /// 
        /// </summary>
        public UserRight()
        {
            IsHost = false;
            IsSubscriber = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOrgMember
        {
            get
            {
                return (Member != null || IsSysAdmin);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public bool IsMemberOf(string orgName)
        {
            if (Member != null && Member.Organiser.ShortName.Equals(orgName))
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOrgAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMemberAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsMemberAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCourseAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsCourseAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRoomAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsRoomAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCurriculumAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsCurriculumAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEventAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsEventAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSemesterAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsSemesterAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNewsAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsNewsAdmin;
                return IsSysAdmin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsExamAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsExamAdmin;
                return IsSysAdmin;
                ;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsStudentAdmin
        {
            get
            {
                if (Member != null)
                    return Member.IsStudentAdmin;
                return IsSysAdmin;
                ;
            }
        }


        public bool IsStaff
        {
            get
            {
                if (User != null)
                    return User.MemberState == MemberState.Staff || IsSysAdmin;
                return IsSysAdmin;
            }
        }

        public bool IsStudent
        {
            get
            {
                if (User != null)
                    return User.MemberState == MemberState.Student || IsSysAdmin;
                return IsSysAdmin;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsHost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSubscriber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Member { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSysAdmin { get; set; }
    }

    public class MemberProfileViewModel
    {
        public Guid MemberId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [Display(Name = "Titel")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Profilbeschreibung")]
        public string Description { get; set; }

        [Display(Name = "Profilseite HM")]
        public string UrlProfile { get; set; }

        [Display(Name = "Profilbeschreibung öffentlich sichtbar")]
        public bool ShowDescription { get; set; }
    }
}