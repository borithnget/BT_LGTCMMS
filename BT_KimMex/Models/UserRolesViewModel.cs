using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class UserRolesViewModel
    {
        [Key]
        public string UserId { get; set; }
        [Display(Name ="Login Name:")]
        public string Username { get; set; }
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Display(Name = "User Group:")]
        public string Role { get; set; }
        [Display(Name = "First Name:")]
        public string user_first_name { get; set; }
        [Display(Name = "Last Name:")]
        public string user_last_name { get; set; }
        [Display(Name = "Position:")]
        public string user_position_id { get; set; }
        [Display(Name = "Position:")]
        public string position_name { get; set; }
        [Display(Name = "Telephone:")]
        public string user_telephone { get; set; }
        [Display(Name = "Email:")]
        public string user_email { get; set; }
        [Display(Name = "Date:")]
        public string created_date { get; set; }
        public string user_signature { get; set; }
        public UserViewModel userDetail { get; set; }
        public UserRolesViewModel()
        {
            userDetail = new UserViewModel();
        }
    }
    public class PositionViewModel
    {
        [Key]
        public string position_id { get; set; }
        [Display(Name ="Position")]
        [Required(ErrorMessage ="Position is required.")]
        public string position_name { get; set; }
    }

    public class UserViewModel
    {
        public string user_detail_id { get; set; }
        public string user_id { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public string user_position_id { get; set; }
        public string user_telephone { get; set; }
        public string user_email { get; set; }
        public string position_name { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string user_signature { get; set; }
    }

}