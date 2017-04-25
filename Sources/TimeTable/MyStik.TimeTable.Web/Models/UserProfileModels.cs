using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class UserProfileViewModel
    {
       public ProfileViewModel Profile { get; set; }

        public ResetPasswordViewModel Password { get; set; }

        public UserMsgProfileViewModel MsgProfile { get; set; }

        public UserSemesterViewModel SemesterProfile { get; set; }

        public MemberState MemberState { get; set; }

        public SemesterGroup SemesterGroup { get; set; }

        public List<UserDeviceViewModel> UserDevices { get; set; }
    }


    public class UserSemesterViewModel
    {
        [Required]
        [Display(Name = "Fakultät")]
        public string Faculty { get; set; }

        [Required]
        [Display(Name = "Studienprogramm")]
        public string Curriculum { get; set; }

        [Required]
        [Display(Name = "Semestergruppe")]
        public string Group { get; set; }
    }

    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nachname")]
        public string LastName { get; set; }

    }

    public class UserDeviceViewModel
    {
        [Required]
        [Display(Name = "Gerätename")]
        public string DeviceName { get; set; }

        [Required]
        [Display(Name = "Aktiviert")]
        public bool Activated { get; set; }

        [Required]
        [Display(Name = "GeräteId")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "UserId")]
        public string UserId { get; set; }

    }

    public class UserMsgProfileViewModel
    {
        public bool LikeEmailOnGlobalLevel { get; set; }
    }

}