using BT_KimMex.Class;
using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace BT_KimMex.Models
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
        [Display(Name ="User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Key]
        public string UserID { get; set; }
        //[Required]
        //[EmailAddress]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password:")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Login Name:")]
        public string UserName { get; set; }

        //[Required]
        //[Display(Name = "User Group:")]
        //public string UserRoles { get; set; }

        [Required]
        [Display(Name = "User Group:")]
        public string[] Role { get; set; }

        [Display(Name ="First Name:")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Display(Name = "Position:")]
        public string Position { get; set; }
        [Display(Name = "Telephone:")]
        public string Telephone { get; set; }
        [Display(Name ="Date:")]
        public string created_date { get; set; }
        public string user_signature { get; set; }
        public string[] user_signatures { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Key]
        public string UserID { get; set; }
        [Display(Name = "User Name:")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password: ")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    
    public class UserUpdateViewModel
    {
        [Key]
        public string UserID { get; set; }
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Login Name:")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "User Group:")]
        public string[] Role { get; set; }
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Display(Name = "Position:")]
        public string Position { get; set; }
        [Display(Name = "Telephone:")]
        public string Telephone { get; set; }
        [Display(Name = "Date:")]
        public string created_date { get; set; }
        public string user_signature { get; set; }
        public List<AttachmentViewModel> signatures { get; set; }
        public string[] user_signatures { get; set; }

        public static UserViewModel GetUserDetailById(string id)
        {
            UserViewModel user = new UserViewModel();
            user = GlobalMethod.GetUserInfomationDetail(id);
           
            return user;
        }
        public UserUpdateViewModel()
        {
            signatures = new List<AttachmentViewModel>();
        }
    }

    public class POSTUserSignatureModel
    {
        public string UserID { get; set; }
        public HttpPostedFileWrapper ImageFile { get; set; }
        public string SignatureURLPath { get; set; }

    }

}
