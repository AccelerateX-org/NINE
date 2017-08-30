using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        [Display(Name = "Kurzname")]
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
            get { return IsCurriculumAdmin; }
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
}