using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        //[EmailAddress] => muss raus wegen Abwärtskompatibilität, Anmeldung mit Benutzernamen der keine EMail Adresse ist muss gehen
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail(=Benutzername)")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Das {0} muss mindests {2} lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Wiederholung Passwort")]
        [Compare("Password", ErrorMessage = "Passwort und Passwortwiederholung sind nicht identisch.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nachname")]
        public string LastName { get; set; }
    }


    public class RegisterSuccessViewModel
    {
        public string Email { get; set; }

        public DateTime ExpiryDate { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        //[EmailAddress] kann auch der Benutzername sein
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class CheckEMailViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string EMail { get; set; }

        public string UserId { get; set; }
    }

    public class VerifyEMailModel
    {
        public string EMail { get; set; }

        public string Message { get; set; }
    }

    public class DeleteUserModel
    {
        public string EMail { get; set; }
    }

}
