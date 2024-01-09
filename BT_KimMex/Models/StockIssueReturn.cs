using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class StockIssueReturnViewModel
    {
        [Key]
        public string stock_issue_return_id { get; set; }
        [Display(Name ="Stock Issue Ref.:")]
        public string stock_issue_ref { get; set; }
        [Display(Name ="Stock Issue Return No.:")]
        public string issue_return_number { get; set; }
        public string issue_return_status { get; set; }
        [Display(Name = "Stock Issue Ref.:")]
        public string stock_issue_number { get; set; }
        public Nullable<bool> status { get; set; }
        [Display(Name ="Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
        public string strWarehouse { get; set; }
        public string strInvoiceNumber { get; set; }
        public string strInvoiceDate { get; set; }
        public string created_by { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public List<InventoryDetailViewModel> inventoryDetails { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public string received_status { get; set; }
        public string project_short_name { get; set; }
        public string created_by_text { get; set; }
        public string created_at_text { get; set; }
        public string show_status { get; set; }
        public List<GoodReceivedNoteStatusViewModel> receivedStatus { get; set; }
        public List<ProcessWorkflowModel> processWorkflows { get; set; }
        public StockIssueReturnViewModel()
        {
            inventories = new List<InventoryViewModel>();
            inventoryDetails = new List<InventoryDetailViewModel>();
            rejects = new List<RejectViewModel>();
            receivedStatus = new List<GoodReceivedNoteStatusViewModel>();
        }
    }

    public class StockReturnFilterResultModel
    {
        public tb_stock_issue_return isr { get; set; }
        public tb_stock_transfer_voucher iss { get; set; }

    }
}