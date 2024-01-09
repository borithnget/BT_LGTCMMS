using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class MRCutOffViewModel
    {
        [Key]
        public string mr_cut_off_id { get; set; }
        public string material_request_id { get; set; }
        public string material_request_number { get; set; }
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string mr_cut_off_number { get; set; }
        public string mr_cut_off_status { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> approved_at { get; set; }
        public string approved_by { get; set; }
        public string approved_comment { get; set; }
        public List<MRCutOffDetailViewModel> mrCutOffDetail { get; set; }
        public List<ItemRequestViewModel> materialRequests { get; set; }
        public List<ItemRequestDetail2ViewModel> materialRequestItems { get; set; }
        public MRCutOffViewModel()
        {
            mrCutOffDetail = new List<MRCutOffDetailViewModel>();
            materialRequests = new List<ItemRequestViewModel>();
            materialRequestItems = new List<ItemRequestDetail2ViewModel>();
        }
    }

    public class MRCutOffDetailViewModel
    {
        [Key]
        public string cut_off_detail_id { get; set; }
        public string cut_off_id { get; set; }
        public string item_id { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string item_unit_id { get; set; }
        public string item_unit_name { get; set; }
        public Nullable<decimal> material_request_qty { get; set; }
        public Nullable<decimal> cut_off_qty { get; set; }
        public string cut_off_reason { get; set; }
        public string item_status { get; set; }
        public string approval_comment { get; set; }
    }
}