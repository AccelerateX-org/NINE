using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalLoginConfirmationViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExternalLoginListViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SendCodeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string SelectedProvider { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VerifyCodeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Provider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ForgotViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Email", ResourceType = typeof(Resources))]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType =typeof(Resources))]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "RememberMe", ResourceType =typeof(Resources))]
        public bool RememberMe { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "RegisterEmailField", ResourceType =typeof(Resources))]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessageResourceType=typeof(Resources),
                           ErrorMessageResourceName = "RegisterPasswordLengthErrorMessage", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType =typeof(Resources))]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "RegisterPasswordConfirmField", ResourceType =typeof(Resources))]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources), 
                             ErrorMessageResourceName = "RegisterPasswordConfirmErrorMessage")]
        public string ConfirmPassword { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public string DatenschutzGelesen { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RegisterSuccessViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// E-Mail Adresse oder bei alten Systemen auch der Benutzername
        /// </summary>
        [Required]
        [Display(Name = "Email", ResourceType =typeof(Resources))]
        public string Email { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CheckEMailViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string EMail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VerifyEMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DeleteUserModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string EMail { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ChangeEMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string EMail { get; set; }
    }

}
