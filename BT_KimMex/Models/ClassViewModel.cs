using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ClassViewModel
    {
        [Key]
        public string class_id { get; set; }
        [Required(ErrorMessage = "Class Type is required.")]
        [Display(Name = "Class Type : ")]
        public string class_type_id { get; set; }
        [Required(ErrorMessage = "Class is required.")]
        [Display(Name = "Class Name : ")]
        public string class_name { get; set; }
        public Nullable<bool> active { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string class_type_name { get; set; }
        public string class_code { get; set; }

        public static string GenerateGroupCode()
        {
            string code = string.Empty;
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                int count=db.tb_class.Where(s=>s.active==true).Count();
                string codeNumber = (count + 1).ToString();
                var isCodeExist = db.tb_class.Where(s => s.active == true && string.Compare(s.class_code, codeNumber) == 0).FirstOrDefault();
                if (isCodeExist != null)
                {
                    codeNumber = (Convert.ToInt32(codeNumber) + 1).ToString();
                }
                code = codeNumber;
            }
            return code;
        }

    }
}