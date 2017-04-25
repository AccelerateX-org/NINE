using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class StudentViewModel
    {
        public StudentViewModel()
        {
        }

        public ApplicationUser User { get; set; }

        public SemesterSubscription CurrentSubscription { get; set; }
        public SemesterSubscription LastSubscription { get; set; }

        public ICollection<Course> AllCourses { get; set; }

        public ICollection<Course> CoursesFit { get; set; }
    }



    public class InvitationFileModel
    {
        public ICollection<HttpPostedFileBase> Attachments { get; set; }

    }


    public class StudentInvitationModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Curriculum { get; set; }
        public string Group { get; set; }

        public string Semester { get; set; }

        public string Remark { get; set; }

        public bool Invite { get; set; }

        public bool Invited { get; set; }

        public ApplicationUser User { get; set; }

        public SemesterGroup SemGroup { get; set; }
    }

    public class InvitationCheckModel
    {
        public InvitationCheckModel()
        {
            Invitations = new List<StudentInvitationModel>();
        }

        public string Error { get; set; }

        public List<StudentInvitationModel> Invitations { get; private set; }
    }
}