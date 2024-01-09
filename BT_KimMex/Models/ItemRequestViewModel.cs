using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using BT_KimMex.Class;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class ItemRequestViewModel
    {
        [Display(Name ="PR ID:")]
        public string ir_id { get; set; }
        [Display(Name ="PR No.:")]
        public string ir_no { get; set; }
        [Display(Name ="Project Name:")]
        public string ir_project_id { get; set; }
        [Display(Name ="Purpose of Purchase:")]
        public string ir_purpose_id { get; set; }
        public string ir_status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string project_short_name { get; set; }
        public string project_full_name { get; set; }
        public string purpose_description { get; set; }      
        public string att_id { get; set; }
        public string ir_attachment_name { get; set; }
        public string ir_attachment_extension { get; set; }
        public string file_path { get; set; }
        public string brand_id { get; set; }
        public string product_size_id { get; set; }
        public string boq_id { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string checked_by { get; set; }
        public string approved_by { get; set; }
        public bool isCompleted { get; set; }
        public bool isMR { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public string status { get; set; }
        public Nullable<bool> is_mr { get; set; }
        public string po_status { get; set; }
        public string st_status { get; set; }
        public string tw_status { get; set; }
        public string created_by_id { get; set; }
        public List<ItemRequestDetail1ViewModel> ir1 { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public string[] ir_detail2_id { get; set; }
        public string[] remark { get; set; }
        public string[] ir_qty { get; set; }
        public string so_number { get; set; }
        public Nullable<System.DateTime> expected_delivery_date { get; set; }
        public string created_at_text { get; set; }
        public string show_status_html { get; set; }
        public string po_show_status_html { get; set; }
        public string st_show_status_html { get; set; }
        public string tw_show_status_html { get; set; }
        public string expected_delivery_date_text { get; set; }
        public string created_by_signature { get; set; }
        public string approved_by_signature { get; set; }
        public Nullable<bool> is_grn { get; set; }
        public Nullable<bool> is_po_completed { get; set; }
        public Nullable<bool> is_po_partial { get; set; }
        public List<tb_material_request_history> materialRequestHistories { get; set; }
        public List<ProcessWorkflowModel> workFlowHistories { get; set; }
        public int count { get; set; }
        public ItemRequestViewModel()
        {
            ir1 = new List<ItemRequestDetail1ViewModel>();
            rejects = new List<RejectViewModel>();
            materialRequestHistories = new List<tb_material_request_history>();
        }

        public static bool SaveMaterailRequestHistory(tb_item_request entity,string user_id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_material_request_history model = new tb_material_request_history();
                model.mr_history_id = Guid.NewGuid().ToString();
                model.mr_ref_id = entity.ir_id;
                model.ir_number = entity.ir_no;
                model.ir_project_id = entity.ir_project_id;
                model.ir_purpose_id = entity.ir_purpose_id;
                model.ir_status = entity.ir_status;
                model.created_by = entity.created_by;
                model.created_date = entity.created_date;
                model.updated_by = entity.updated_by;
                model.updated_date = entity.updated_date;
                model.checked_by = entity.checked_by;
                model.checked_date = entity.checked_date;
                model.approved_by = entity.approved_by;
                model.approved_date = entity.approved_date;
                model.status = entity.status;
                model.is_completed = entity.is_completed;
                model.is_mr = entity.is_mr;
                model.po_status = entity.po_status;
                model.st_status = entity.st_status;
                model.tw_status = entity.tw_status;
                model.is_cut_off = entity.is_cut_off;
                model.expected_delivery_date = entity.expected_delivery_date;
                model.new_created_at = Class.CommonClass.ToLocalTime(DateTime.Now);
                model.new_created_by = user_id;

                db.tb_material_request_history.Add(model);
                db.SaveChanges();

                //update mr detail
                var dIrs = db.tb_ir_detail1.Where(m => m.ir_id == entity.ir_id).ToList();
                if (dIrs.Any())
                {
                    foreach(var dIr in dIrs)
                    {
                        var ir_detail1_id = Convert.ToString(dIr.ir_detail1_id);
                        //var ddIrs = db.tb_ir_detail2.Where(m => m.ir_detail1_id == ir_detail1_id).ToList();
                        //foreach(var ddIr in ddIrs)
                        //{
                        //    string ir_detail2_id = ddIr.ir_detail2_id;
                        //    tb_ir_detail2 detail2 = db.tb_ir_detail2.Find(ir_detail2_id);
                        //    if (detail2 != null)
                        //    {
                        //        detail2.ir_detail1_id = model.mr_history_id;
                        //        db.SaveChanges();
                        //    }
                        //}
                        tb_ir_detail1 detail1 = db.tb_ir_detail1.Find(ir_detail1_id);
                        if (detail1 != null)
                        {
                            detail1.ir_id = model.mr_history_id;
                            db.SaveChanges();
                        }
                    }
                }

                //attachment
                var attachments = db.tb_ir_attachment.Where(s => string.Compare(s.ir_id, entity.ir_id) == 0).ToList();
                foreach(var att in attachments)
                {
                    tb_ir_attachment irAtt = new tb_ir_attachment();
                    irAtt.ir_id = model.mr_history_id;
                    irAtt.ir_attachment_id = Guid.NewGuid().ToString();
                    irAtt.ir_attachment_name = att.ir_attachment_name;
                    irAtt.ir_attachment_extension = att.ir_attachment_extension;
                    irAtt.file_path = att.file_path;
                    db.tb_ir_attachment.Add(irAtt);
                    db.SaveChanges();

                }

                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
        public static List<tb_material_request_history> GetMaterailRequestHistory(string mr_id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_material_request_history.OrderByDescending(s => s.new_created_at).Where(s => string.Compare(s.mr_ref_id, mr_id) == 0).ToList();
            }
        }

        public static ItemRequestViewModel GetItemRequestDetail(string id, bool isEdit = false)
        {
            ItemRequestViewModel itemRequest = new ItemRequestViewModel();
            kim_mexEntities db = new kim_mexEntities();
            
            var request = (from i in db.tb_item_request
                           join pj in db.tb_project on i.ir_project_id equals pj.project_id
                           join purpose in db.tb_purpose on i.ir_purpose_id equals purpose.purpose_id into pp
                           from purpose in pp.DefaultIfEmpty()
                           where i.status == true && i.ir_id == id
                           select new { i, pj, purpose }).FirstOrDefault();
            if (request != null)
            {
                itemRequest = new ItemRequestViewModel();
                itemRequest.ir_id = request.i.ir_id;
                itemRequest.ir_no = request.i.ir_no;
                itemRequest.ir_project_id = request.i.ir_project_id;
                itemRequest.project_full_name = request.pj.project_full_name;
                itemRequest.ir_purpose_id = request.i.ir_purpose_id;
                itemRequest.purpose_description = request.purpose == null ? string.Empty : request.purpose.purpose_name;
                itemRequest.ir_status = request.i.ir_status;
                itemRequest.created_date = request.i.created_date;
                itemRequest.created_by = CommonFunctions.GetUserFullnamebyUserId(request.i.created_by);
                itemRequest.updated_by = CommonFunctions.GetUserFullnamebyUserId(request.i.updated_by);
                itemRequest.checked_by = CommonFunctions.GetUserFullnamebyUserId(request.i.checked_by);
                itemRequest.approved_by = CommonFunctions.GetUserFullnamebyUserId(request.i.approved_by);
                itemRequest.created_by_id = request.i.created_by;
                itemRequest.approved_date = request.i.approved_date;
                itemRequest.so_number = request.pj.project_no;
                itemRequest.is_mr = request.i.is_mr;
                itemRequest.expected_delivery_date = request.i.expected_delivery_date;
                itemRequest.po_status = request.i.po_status;
                //itemRequest.ir_status = request.i.ir_status;
                itemRequest.tw_status = request.i.tw_status;
                itemRequest.st_status = request.i.st_status;
                itemRequest.is_po_completed = request.i.is_po_completed;
                string createdBySignature = CommonClass.GetUserSignature(request.i.created_by);
                string approvedBySignature = CommonClass.GetUserSignature(request.i.approved_by);
                string host = HttpContext.Current.Request.Url.Authority;
                itemRequest.created_by_signature= string.IsNullOrEmpty(createdBySignature) ? string.Empty : string.Format("{0}{1}", EnumConstants.domainName, createdBySignature); 
                itemRequest.approved_by_signature=string.IsNullOrEmpty(approvedBySignature)?string.Empty : string.Format("{0}{1}", EnumConstants.domainName, approvedBySignature);
            }

            List<ItemRequestDetail1ViewModel> ir1 = new List<ItemRequestDetail1ViewModel>();
            var dIrs = (from ir in db.tb_ir_detail1
                        join jc in db.tb_job_category on ir.ir_job_category_id equals jc.j_category_id
                        orderby jc.created_date
                        where ir.ir_id == itemRequest.ir_id
                        select new
                        {
                            ir_detail1_id = ir.ir_detail1_id,
                            ir_id = ir.ir_id,
                            ir_job_category_id = ir.ir_job_category_id,
                            job_category_description = jc.j_category_name
                        }).ToList();
            if (dIrs.Any())
            {
                int dCount = 1;
                foreach (var dIr in dIrs)
                {

                    List<ItemRequestDetail2ViewModel> ir2 = new List<ItemRequestDetail2ViewModel>();
                    List<FilterMaterialRequestProductDetailModel> ddIrs = new List<FilterMaterialRequestProductDetailModel>();
                    if (isEdit)
                    {
                        ddIrs = (from ir in db.tb_ir_detail2
                                 join it in db.tb_product on ir.ir_item_id equals it.product_id
                                 join u in db.tb_unit on it.product_unit equals u.Id
                                 orderby ir.ordering_number, it.product_code
                                 where ir.ir_detail1_id == dIr.ir_detail1_id && ir.is_po == false
                                 select new FilterMaterialRequestProductDetailModel()
                                 {
                                     ir = ir,
                                     it = it,
                                     u = u
                                 }).ToList();
                    }
                    else
                    {
                        ddIrs = (from ir in db.tb_ir_detail2
                                 join it in db.tb_product on ir.ir_item_id equals it.product_id
                                 join u in db.tb_unit on it.product_unit equals u.Id
                                 orderby ir.ordering_number, it.product_code
                                 where ir.ir_detail1_id == dIr.ir_detail1_id
                                 select new FilterMaterialRequestProductDetailModel()
                                 {
                                     ir = ir,
                                     it = it,
                                     u = u
                                 }).ToList();
                    }

                    if (ddIrs.Any())
                    {
                        foreach (var ddIr in ddIrs)
                        {
                            ItemRequestDetail2ViewModel irItem = new ItemRequestDetail2ViewModel();
                            irItem.ir_detail2_id = ddIr.ir.ir_detail2_id;
                            irItem.ir_detail1_id = ddIr.ir.ir_detail1_id;
                            irItem.ir_item_id = ddIr.ir.ir_item_id;
                            irItem.ir_item_unit = ddIr.ir.ir_item_unit;
                            irItem.product_code = ddIr.it.product_code;
                            irItem.product_name = ddIr.it.product_name;
                            irItem.unit_id = ddIr.u.Id;
                            irItem.product_unit = Regex.Replace(ddIr.u.Name.Trim(), @"\t|\n|\r", "");
                            //irItem.product_unit =​Regex.Replace(ddIr.product_unit.Trim(), @"\t|\n|\r", "");
                            irItem.ir_qty = ddIr.ir.ir_qty;
                            //irItem.requested_unit =​Regex.Replace(ddIr.requested_unit.Trim(), @"\t|\n|\r", ""); 
                            //irItem.requested_unit = Regex.Replace(ddIr.requested_unit.Trim(), @"\t|\n|\r", "");
                            irItem.requested_unit_id = ddIr.ir.ir_item_unit;
                            irItem.requested_unit = db.tb_unit.Where(w => string.Compare(w.Id, ddIr.ir.ir_item_unit) == 0).Select(s => s.Name).FirstOrDefault();
                            irItem.remark = ddIr.ir.remark;
                            irItem.reason = ddIr.ir.reason;
                            irItem.is_approved = ddIr.ir.is_approved;
                            irItem.approved_qty = ddIr.ir.approved_qty;
                            irItem.boq_qty = ItemRequest.GetBoqItemQty(itemRequest.ir_project_id, dIr.ir_job_category_id, ddIr.ir.ir_item_id);
                            irItem.job_group = dCount.ToString();
                            irItem.uom = db.tb_multiple_uom.Where(x => x.product_id == irItem.ir_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            irItem.item_status = ddIr.ir.item_status;
                            irItem.is_po = ddIr.ir.is_po;
                            ir2.Add(irItem);
                        }
                    }
                    ir1.Add(new ItemRequestDetail1ViewModel() { ir_detail1_id = dIr.ir_detail1_id, ir_id = dIr.ir_id, ir_job_category_id = dIr.ir_job_category_id, job_category_description = dIr.job_category_description, job_group = dCount.ToString(), ir2 = ir2 });
                    dCount++;
                }
            }
            itemRequest.ir1 = ir1;
            var att = db.tb_ir_attachment.Where(m => m.ir_id == id).FirstOrDefault();
            if (att != null)
            {
                itemRequest.att_id = att.ir_attachment_id;
                itemRequest.ir_attachment_name = att.ir_attachment_name;
                itemRequest.ir_attachment_extension = att.ir_attachment_extension;
                itemRequest.file_path = att.file_path;
            }
            itemRequest.rejects = CommonClass.GetRejectByRequest(id);
            itemRequest.boq_id = ItemRequest.GetBOQId(itemRequest.ir_project_id);
            itemRequest.workFlowHistories = ProcessWorkflowModel.GetProcessWorkflowByRefId(itemRequest.ir_id);
            return itemRequest;
        }
    }
    public class ItemRequestDetail1ViewModel
    {
        public string ir_detail1_id { get; set; }
        public string ir_id { get; set; }
        public string ir_job_category_id { get; set; }
        public string job_category_description { get; set; }
        public string job_group { get; set; }
        public List<ItemRequestDetail2ViewModel> ir2 { get; set; }
        public ItemRequestDetail1ViewModel()
        {
            ir2 = new List<ItemRequestDetail2ViewModel>();
        }
    }
    public class ItemRequestDetail2ViewModel
    {
        public string ir_detail2_id { get; set; }
        public string ir_detail1_id { get; set; }
        public string ir_item_id { get; set; }
        public Nullable<decimal> ir_qty { get; set; }
        public string ir_item_unit { get; set; }
        public double boq_qty { get; set; }
        public string p_category_id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string product_unit { get; set; }
        public string requested_unit { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public string job_group { get; set; }
        public string type_group { get; set; }
        public string reason { get; set; }
        public string remark { get; set; }
        public Nullable<bool> is_approved { get; set; }
        public Nullable<decimal> approved_qty { get; set; }
        public decimal remain_approved_qty { get; set; }
        public ProductViewModel uom { get; set; }
        public Nullable<decimal> remain_qty { get; set; }
        public string unit_id { get; set; }
        public string requested_unit_id { get; set; }
        public Nullable<int> ordering_number { get; set; }
        public string item_status { get; set; }
        public Nullable<bool> is_po { get; set; }
        public ItemRequestDetail2ViewModel()
        {
            uom = new ProductViewModel();
        }
    }
    public class FilterMaterialRequestProductDetailModel
    {
        public tb_ir_detail2 ir { get; set; }
        public tb_product it { get; set; }
        public tb_unit u { get; set; }
        public FilterMaterialRequestProductDetailModel()
        {
            ir = new tb_ir_detail2();
            it = new tb_product();
            u = new tb_unit();
        }
    }
    public class IRTypeViewModel
    {
        public string boq_detail2_id { get; set; }
        public string boq_detail1_id { get; set; }
        public string item_type_id { get; set; }
        public string job_group { get; set; }
        public string type_group { get; set; }
        public string remark { get; set; }
        public string p_category_code { get; set; }
        public string p_category_name { get; set; }
        public string p_category_address { get; set; }
        public string chart_account { get; set; }
    }
    public class MRFilterResultModel
    {
        public tb_item_request tbl { get; set; }
        public tb_project project { get; set; }
        public tb_purpose purpose
        { get; set; }
    }
    public class ItemRequestPostModel
    {

        public ItemRequestViewModel itemRequest { get; set; }
        public List<ItemRequestDetail1ViewModel> ir1 { get; set; }
        public List<ItemRequestDetail2ViewModel> ir2 { get; set; }
        public List<IRTypeViewModel> irType { get; set; }

    }
    public class MaterialRequestHistoryModel
    {
        public string mr_history_id { get; set; }
        public string mr_ref_id { get; set; }
        public string ir_number { get; set; }
        public string ir_project_id { get; set; }
        public string ir_purpose_id { get; set; }
        public string ir_status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<bool> is_completed { get; set; }
        public Nullable<bool> is_mr { get; set; }
        public string po_status { get; set; }
        public string st_status { get; set; }
        public string tw_status { get; set; }
        public Nullable<bool> is_cut_off { get; set; }
        public Nullable<System.DateTime> expected_delivery_date { get; set; }
        public string new_created_by { get; set; }
        public Nullable<System.DateTime> new_created_at { get; set; }
    }
}