using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ItemBlockingViewModel
    {
        [Key]
        public string item_blocking_id { get; set; }
        public string item_blocking_number { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public string project_id { get; set; }
        public string project_name { get; set; }
        public Nullable<bool> active { get; set; }
        public string item_blocking_status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string updated_by { get; set; }
        public List<ItemBlockingDetailViewModel> itemBlockingDetails { get; set; }
        public ItemBlockingViewModel()
        {
            itemBlockingDetails = new List<ItemBlockingDetailViewModel>();
        }
    }
    public class ItemBlockingDetailViewModel
    {
        [Key]
        public string item_blocking_detail_id { get; set; }
        public string item_blocking_id { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public Nullable<System.DateTime> item_blocking_date { get; set; }
        public string item_id { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string item_unit { get; set; }
        public Nullable<bool> is_block { get; set; }
        public Nullable<decimal> block_qty { get; set; }
        public Nullable<bool> active { get; set; }
        public string remark { get; set; }
    }
}