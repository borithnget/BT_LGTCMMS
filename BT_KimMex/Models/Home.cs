using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Class;

namespace BT_KimMex.Models
{
    public class Home
    {
        public int PendingBOQ { get; set; }
        public int ApprovedBOQ { get; set; }
        public int CompletedBOQ { get; set; }
        public int pendingIR { get; set; }
        public int ApprovedIR { get; set; }
        public int PendingPO { get; set; }
        public int ApprovedPO { get; set; }
        public int CompletedPO { get; set; }
        public int PendingIRec { get; set; }
        public int CompletedIRec { get; set; }
        public int PendingIRet { get; set; }
        public int CompletedRet { get; set; }
        public int PendingST { get; set;}
        public int CompletedST { get; set; }
        public int PendingSD { get; set; }
        public int CompletedSD { get; set; }
        public int PendingSIss { get; set; }
        public int CompletedSIss { get; set; }
        public int PendingIssRe { get; set; }
        public int CompletedIssRe { get; set; }
        public int countPR { get; set; }
        public int countQuote { get; set; }
        public int countPO { get; set; }
        public int countSR { get; set; }
        public int countSA { get; set; }
        public int countTW { get; set; }
        public int countRW { get; set; }
        public int countApprovedMR { get; set; }
        public int countApprovedPR { get; set; }
        public int countApprovedPO { get; set; }
        public int countApprovedGRN { get; set; }
        public int countApprovedST { get; set; }
        public int countApprovedSR { get; set; }
        public int countApprovedTW { get; set; }
        public int countApprovedRW { get; set; }
        public List<ItemRequestViewModel> approvedMrListItems { get; set; }
        public List<PurchaseRequisitionViewModel> approvedPRListItems { get; set; }
        public List<PurchaseRequestViewModel> approvedPOListItems { get; set; }
        public List<StockTransferViewModel> approvedSTListItems { get; set; }
        public List<ItemReceiveViewModel> approvedGRNListItems { get; set; }
        public List<StockIssueReturnViewModel> approvedSRListItems { get; set; }
        public List<TransferFromMainStockViewModel> approvedTWListItems { get; set; }
        public List<ReturnMainStockViewModel> approvedRWListItems { get; set; }
        public Home()
        {
            approvedMrListItems = new List<ItemRequestViewModel>();
            approvedGRNListItems = new List<ItemReceiveViewModel>();
        }
      
    }
    public class RejectViewModel
    {
        public string reject_id { get; set; }
        public string ref_id { get; set; }
        public string ref_type { get; set; }
        public string comment { get; set; }
        public string rejected_by { get; set; }
    }

    public class ProcessWorkflowModel
    {
        [Key]
        public int id { get; set; }
        public Nullable<int> menu_id { get; set; }
        public string ref_id { get; set; }
        public string status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string remark { get; set; }
        public string show_status { get; set; }
        public string crated_by_name { get { return CommonClass.GetUserFullnameByUserId(created_by); } set { crated_by_name = value; } }

        public static List<ProcessWorkflowModel> GetProcessWorkflowByRefId(string ref_id)
        {
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                return db.tb_procress_workflow.OrderByDescending(s => s.created_at).Where(s => string.Compare(s.ref_id, ref_id) == 0).Select(s => new
                       ProcessWorkflowModel()
                {
                    id=s.id,
                    menu_id=s.menu_id,
                    ref_id=s.ref_id,
                    status=s.status,
                    created_by=s.created_by,
                    created_at=s.created_at,
                    remark=s.remark,
                    
                }).ToList();
            }
        }
    }

}