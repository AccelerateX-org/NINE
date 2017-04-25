using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OrganiserViewModel
    {
        public OrganiserViewModel()
        {
            Members = new List<MemberViewModel>();
            Courses = new List<CourseSummaryModel>();
            Events = new List<EventViewModel>();
        }
        public ActivityOrganiser Organiser { get; set; }

        public List<MemberViewModel> Members { get; private set; }

        public List<CourseSummaryModel> Courses { get; private set; }

        public List<EventViewModel> Events { get; private set; }

        public int MemberCount { get { return Organiser.Members.Count; } }

        public int CourseCount
        {
            get
            {
                return Organiser.Activities.OfType<Course>().Count(c => c.SemesterGroups.Any(g => g.Semester.Name.Equals("SS14")));
            }
        }

        public int EventCount { get { return Organiser.Activities.OfType<Event>().Count(); } }

        public int NewsletterCount { get { return Organiser.Activities.OfType<Newsletter>().Count(); } }
    }

    public class MemberViewModel
    {
        public OrganiserMember Member { get; set; }

        public ApplicationUser User { get; set; }

        public bool ItsMe { get; set; }

        public bool IsActive { get; set; }

        public bool WasActiveLastSemester { get; set; }

        public bool WasActiveLastYear { get; set; }
    }

    public class MemberUserViewModel
    {
        public Guid OrganiserId { get; set; }

        public Guid MemberId { get; set; }

        [Display(Name = "Benutzername")]
        public string UserName { get; set; }

        [Display(Name = "Rolle")]
        public string Role { get; set; }

        [Display(Name = "Administrator des Veranstalters")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Display(Name = "Kurzname")]
        public string ShortName { get; set; }


        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "URL HM-Profil oder Homepage")]
        public string UrlProfile { get; set; }
    }

    public class UserRight
    {
        public UserRight()
        {
            IsHost = false;
            IsOrgAdmin = false;
            IsOrgMember = false;
            IsSubscriber = false;
        }

        public UserRight(bool isSysAdmin)
        {
            IsHost = isSysAdmin;
            IsOrgAdmin = isSysAdmin;
            IsOrgMember = isSysAdmin;
            IsSubscriber = isSysAdmin;
        }


        public bool IsOrgMember { get; set; }
        public bool IsOrgAdmin { get; set; }
        public bool IsHost { get; set; }
        public bool IsSubscriber { get; set; }

        public ApplicationUser User { get; set; }
    }
}