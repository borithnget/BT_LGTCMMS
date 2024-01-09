using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class PurchaseRequisitionViewModel
    {
        [Key]
        public string purchase_requisition_id { get; set; }
        public string purchase_requisition_number { get; set; }
        [Required(ErrorMessage = "Material Request Reference is required.")]
        public string material_request_id { get; set; }
        public string purchase_requisition_status { get; set; }
        public Nullable<bool> status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_at { get; set; }
        public string approved_comment { get; set; }
        public string materail_request_number { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public string project_short_name { get; set; }
        public Nullable<bool> is_quote_complete { get; set; }
        public string created_at_text { get; set; }
        public string created_by_text { get; set; }
        public string show_status_text { get; set; }
        
        public List<ItemRequestViewModel> materialRequests { get; set; }
        public List<ItemRequestDetail2ViewModel> materialRequestItems { get; set; }
        public List<PurchaseRequisitionDetailViewModel> purchaseRequisitionDetails { get; set; }
        public List<ProcessWorkflowModel> processWorkflow { get; set; }
        public List<tb_purchase_requisition> PRHistories { get; set; }
        public PurchaseRequisitionViewModel()
        {
            materialRequests = new List<ItemRequestViewModel>();
            materialRequestItems = new List<ItemRequestDetail2ViewModel>();
            processWorkflow = new List<ProcessWorkflowModel>();
        }
        public static List<PurchaseRequisitionViewModel> GetPurchaseRequisitionCompletedListItems(string id = null)
        {
            List<PurchaseRequisitionViewModel> models = new List<PurchaseRequisitionViewModel>();
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                models = (from pr in db.tb_purchase_requisition
                          join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                          orderby pr.created_at
                          where pr.status == true && pr.is_quote_complete == false && string.Compare(pr.purchase_requisition_status, BT_KimMex.Class.Status.Approved) == 0
                          select new PurchaseRequisitionViewModel()
                          {
                              purchase_requisition_id=pr.purchase_requisition_id,
                              purchase_requisition_number=pr.purchase_requisition_number,
                              created_at=pr.updated_at,
                              project_id=mr.ir_project_id
                          }).ToList();
                if (!string.IsNullOrEmpty(id))
                {
                    var isExist = models.Where(s => string.Compare(s.purchase_requisition_id, id) == 0).FirstOrDefault() != null ? true : false;
                    if (!isExist)
                    {
                        PurchaseRequisitionViewModel item = (from pr in db.tb_purchase_requisition
                                  join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                                             orderby pr.created_at
                                                             where string.Compare(pr.purchase_requisition_id,id)==0
                                  select new PurchaseRequisitionViewModel()
                                  {
                                      purchase_requisition_id = pr.purchase_requisition_id,
                                      purchase_requisition_number = pr.purchase_requisition_number,
                                      created_at = pr.updated_at,
                                      project_id = mr.ir_project_id
                                  }).FirstOrDefault();
                        models.Add(item);
                        models = models.OrderByDescending(s => s.created_at).ToList();
                    }
                }
            }
            return models;
        }
        public static PurchaseRequisitionViewModel GetPurchaseRequisitionItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                PurchaseRequisitionViewModel model = new PurchaseRequisitionViewModel();
                model = (from pr in db.tb_purchase_requisition
                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                         join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                         where string.Compare(pr.purchase_requisition_id, id) == 0
                         select new PurchaseRequisitionViewModel()
                         {
                             purchase_requisition_id = pr.purchase_requisition_id,
                             purchase_requisition_number = pr.purchase_requisition_number,
                             material_request_id = pr.material_request_id,
                             materail_request_number = mr.ir_no,
                             created_at = pr.updated_at,
                             project_id = mr.ir_project_id,
                             project_fullname = pro.project_full_name,
                             created_by = pr.created_by,
                             purchase_requisition_status = pr.purchase_requisition_status,
                             is_quote_complete = pr.is_quote_complete,
                         }).FirstOrDefault();
                model.purchaseRequisitionDetails = (from prd in db.tb_purchase_requisition_detail
                                                    join item in db.tb_product on prd.item_id equals item.product_id
                                                    join unit in db.tb_unit on prd.item_unit equals unit.Id
                                                    orderby item.product_code
                                                    where string.Compare(prd.purchase_requisition_id, model.purchase_requisition_id) == 0
                                                    select new PurchaseRequisitionDetailViewModel()
                                                    {
                                                        purchase_requisition_detail_id = prd.purchase_requisition_detail_id,
                                                        purchase_requisition_id = prd.purchase_requisition_id,
                                                        item_id = prd.item_id,
                                                        item_code = item.product_code,
                                                        item_name = item.product_name,
                                                        item_unit = prd.item_unit,
                                                        item_unit_name = unit.Name,
                                                        approved_qty = prd.approved_qty,
                                                        remain_qty = prd.remain_qty,
                                                        reason = prd.reason,
                                                        remark = prd.remark,
                                                        item_status = prd.item_status,
                                                    }).ToList();

                return model;
            }
        }
        public static List<tb_purchase_requisition> GetPurchaseRequisitionHistorybyMRID(string mrId,string currentPRId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_purchase_requisition.OrderByDescending(s => s.created_at).Where(s => s.status == true && string.Compare(s.purchase_requisition_id, currentPRId) != 0 && string.Compare(s.material_request_id, mrId) == 0).ToList();

            }
        }

    }

    public class PRFilterRequestModel
    {
        public tb_purchase_requisition pr { get; set; }
        public tb_item_request mr { get; set; }
        public tb_project pro { get; set; }
    }
}