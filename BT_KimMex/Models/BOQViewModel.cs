using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Models;

namespace BT_KimMex.Models
{
    public class BOQViewModel
    {
        [Key]
        public string boq_id { get; set; }
        public string boq_no { get; set; }
        public string project_id { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string updated_by { get; set; }
        public string checked_date { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string approved_by { get; set; }
        public string boq_status { get; set; }
        public Nullable<bool> status { get; set; }
        public string project_short_name { get; set; }
        public string project_full_name { get; set; }
        public string cutomer_id { get; set; }
        public string customer_name { get; set; }
        public string cutomer_project_manager { get; set; }
        public string cutomer_signatory { get; set; }
        public string project_telephone { get; set; }
        public List<BOQDetail1> boq_details_1 { get; set; }
        public List<AttachmentViewModel> attachments { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public BOQViewModel()
        {
            boq_details_1 = new List<BOQDetail1>();
            attachments = new List<AttachmentViewModel>();
            rejects = new List<RejectViewModel>();
        }
    }
    public class BOQProductViewModel
    {
        [Key]
        public string boq_detail_id { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string updated_by { get; set; }
        public string product_id { get; set; }
        public string boq_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
    }
    public class BOQDetail1
    {
        public string boq_detail1_id { get; set; }
        public string boq_id { get; set; }
        public string job_category_id { get; set; }
        public string job_category_code { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string remark { get; set; }
        public string job_group { get; set; }
        public string j_category_name { get; set; }
        public string j_description { get; set; }
        public List<BOQDetail2> boq_details_2 { get; set; }
        public BOQDetail1()
        {
            boq_details_2 = new List<BOQDetail2>();
        }
    }
    public class BOQDetail2
    {
        public string boq_detail2_id { get; set; }
        public string boq_detail1_id { get; set; }
        public string item_type_id { get; set; }
        public string job_group { get; set; }
        public string type_group { get; set; }
        public string type_group_letter { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string remark { get; set; }
        public string p_category_code { get; set; }
        public string p_category_name { get; set; }
        public string p_category_address { get; set; }
        public string chart_account { get; set; }
        List<BOQDetail3> boq_details_3 { get; set; }
        public BOQDetail2()
        {
            boq_details_3 = new List<BOQDetail3>();
        }
    }
    public class BOQDetail3
    {
        public string boq_detail3_id { get; set; }
        public string boq_detail2_id { get; set; }
        public string item_id { get; set; }
        public Nullable<decimal> item_qty { get; set; }
        public Nullable<decimal> item_unit_price { get; set; }
        public string type_group { get; set; }
        public string p_category_id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string product_unit { get; set; }
        public Nullable<decimal> unit_price { get; set; }

    }
}