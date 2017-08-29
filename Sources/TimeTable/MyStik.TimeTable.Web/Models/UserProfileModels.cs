using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserProfileViewModel
    {
        /// <summary>
        /// 
        /// </summary>
       public ProfileViewModel Profile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ResetPasswordViewModel Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserMsgProfileViewModel MsgProfile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserSemesterViewModel SemesterProfile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MemberState MemberState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup SemesterGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<UserDeviceViewModel> UserDevices { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserSemesterViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "FacultyField", ResourceType =typeof(Resources))]
        public string Faculty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "CurriculumField", ResourceType =typeof(Resources))]
        public string Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SemGroupField", ResourceType =typeof(Resources))]
        public string Group { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProfileViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "RegisterFirstNameField", ResourceType =typeof(Resources))]
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "RegisterLastNameField", ResourceType =typeof(Resources))]
        public string LastName { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class UserDeviceViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DeviceName", ResourceType =typeof(Resources))]
        public string DeviceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Activated", ResourceType =typeof(Resources))]
        public bool Activated { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DeviceId", ResourceType =typeof(Resources))]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "UserId", ResourceType =typeof(Resources))]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceType { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class UserMsgProfileViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool LikeEmailOnGlobalLevel { get; set; }
    }

}