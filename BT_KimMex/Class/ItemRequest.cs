using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Models;

namespace BT_KimMex.Class
{
    public class ItemRequest
    {
        private kim_mexEntities db = new kim_mexEntities();
        public void addItemBOQ(string project_id,List<ItemRequestDetail1ViewModel> ir1, List<ItemRequestDetail2ViewModel> ir2, List<IRTypeViewModel> irType)
        {
            try
            {
                string boqId = this.getBoqId(project_id);
                for(int i = 0; i < ir1.Count(); i++)
                {
                    string dBoq = this.GetBoqDetail1ID(boqId, ir1[i].ir_job_category_id);
                    for(var j = 0; j < irType.Count(); j++)
                    {
                        string ddBoq = this.GetBoqDetail2Id(dBoq, irType[j].item_type_id);
                        for(var k = 0; k < ir2.Count(); k++)
                        {
                            this.CheckItemInBoq(ddBoq, ir2[k].ir_item_id);
                        }
                    }
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "addItemBOQ", ex.StackTrace, ex.Message);
            }
        }
        public void addItemBOQ(string itemRequestId)
        {
            string projectId = db.tb_item_request.Where(x => string.Compare(x.ir_id, itemRequestId) == 0).Select(x => x.ir_project_id).FirstOrDefault();
            string boqId = this.getBoqId(projectId);
            var boqJobCategories = db.tb_boq_detail1.Where(x => string.Compare(x.boq_id, boqId) == 0).ToList();
            var irJobCategories = db.tb_ir_detail1.Where(x => string.Compare(x.ir_id, itemRequestId) == 0).ToList();

            foreach(var irJobCategory in irJobCategories)
            {
                #region check job category in boq
                string dBoqId = string.Empty;
                var isFound = boqJobCategories.Where(x => string.Compare(x.job_category_id, irJobCategory.ir_job_category_id) == 0).FirstOrDefault();
                if (isFound == null)
                    dBoqId = this.CreateBOQJobCategory(boqId, irJobCategory.ir_job_category_id);
                else
                    dBoqId = isFound.boq_detail1_id;
                #endregion

                #region check item in boq
                var ddIrs = db.tb_ir_detail2.Where(x => string.Compare(x.ir_detail1_id, irJobCategory.ir_detail1_id) == 0&&x.is_approved==true).ToList();
                if (ddIrs.Any())
                {
                    foreach(var ddIr in ddIrs)
                    {
                        checkItemInBOQ(dBoqId, irJobCategory.ir_job_category_id, ddIr.ir_item_id);
                    }
                }
                #endregion

            }
        }

        internal static List<ItemRequestDetail2ViewModel> GetAllAvailableItem(object item_request_id)
        {
            throw new NotImplementedException();
        }

        private string getBoqId(string project_id)
        {
            string boqId = string.Empty;
            try
            {
               boqId = db.tb_build_of_quantity.Where(m => m.project_id == project_id && m.status == true).Select(m => m.boq_id).FirstOrDefault();
            }
            catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "addItemBOQ", ex.StackTrace, ex.Message);
            }
            return boqId;
        }
        private string GetBoqDetail1ID(string boqId,string jobId)
        {
            string dBoq = string.Empty;
            try
            {
                dBoq = db.tb_boq_detail1.Where(m => m.boq_id == boqId && m.job_category_id == jobId).Select(m => m.boq_detail1_id).FirstOrDefault();
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "GetBoqDetail1ID", ex.StackTrace, ex.Message);
            }
            return dBoq;
        }
        private string GetBoqDetail2Id(string dBoq,string typeId)
        {
            string ddBoq = string.Empty;
            try
            {
                var d2 = db.tb_boq_detail2.Where(m => m.boq_detail1_id == dBoq && m.item_type_id == typeId).FirstOrDefault();
                if (d2 != null)
                {
                    ddBoq = d2.boq_detail2_id;
                }
                else
                {
                    ddBoq = this.CreateItemTypeInBoq(dBoq, typeId);
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "GetBoqDetail2Id", ex.StackTrace, ex.Message);
            }
            return ddBoq;
        }
        public void CheckItemInBoq(string ddBoq,string itemId)
        {
            try
            {
                var d3 = db.tb_boq_detail3.Where(m => m.boq_detail2_id == ddBoq && m.item_id == itemId).FirstOrDefault();
                if (d3 == null)
                {
                    this.CreateItemInBoq(ddBoq,itemId);
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "CheckItemInBoq", ex.StackTrace, ex.Message);
            }
        }
        public void checkItemInBOQ(string dboqId,string jobId,string itemId)
        {
            try
            {
                var d3 = (from item in db.tb_boq_detail3
                         join type in db.tb_boq_detail2 on item.boq_detail2_id equals type.boq_detail2_id
                         join job in db.tb_boq_detail1 on type.boq_detail1_id equals job.boq_detail1_id
                         where job.boq_detail1_id == dboqId && item.item_id == itemId
                         select item).FirstOrDefault();
                if (d3 == null)
                {
                    string ddBoq = string.Empty;
                    string itemTypeId = db.tb_product.Where(x => x.product_id == itemId).Select(x => x.brand_id).FirstOrDefault();
                    var isTypeInBoq = (from type in db.tb_boq_detail2
                                       join job in db.tb_boq_detail1 on type.boq_detail1_id equals job.boq_detail1_id
                                       where job.boq_detail1_id == dboqId && type.item_type_id==itemTypeId
                                       select type).FirstOrDefault();
                    if (isTypeInBoq == null)
                        ddBoq = this.CreateItemTypeInBoq(dboqId, itemTypeId);
                    else
                        ddBoq = isTypeInBoq.boq_detail2_id;
                    this.CreateItemInBoq(ddBoq, itemId);
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "checkItemInBOQ", ex.StackTrace, ex.Message);
            }
        }
        public string CreateBOQJobCategory(string boqId,string jobCategoryId)
        {
            string boqDetail1Id = Guid.NewGuid().ToString();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_boq_detail1 dBoq = new tb_boq_detail1();
                dBoq.boq_detail1_id = boqDetail1Id;
                dBoq.boq_id = boqId;
                dBoq.job_category_id = jobCategoryId;
                dBoq.job_category_code = this.GetJobCategoryCode(dBoq.boq_id);
                dBoq.amount = 0;
                dBoq.remark = "In Source";
                db.tb_boq_detail1.Add(dBoq);
                db.SaveChanges();
            }
            return boqDetail1Id;
        }
        private string CreateItemTypeInBoq(string dBoq,string typeId)
        {
            string ddBoq = Guid.NewGuid().ToString();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_boq_detail2 boq = new tb_boq_detail2();
                boq.boq_detail2_id = ddBoq;
                boq.boq_detail1_id = dBoq;
                boq.item_type_id = typeId;
                boq.amount = 0;
                boq.remark = "In Source";
                db.tb_boq_detail2.Add(boq);
                db.SaveChanges();
            }
            return ddBoq;
        }
        public void CreateItemInBoq(string ddBoq,string itemId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_boq_detail3 boq = new tb_boq_detail3();
                boq.boq_detail3_id = Guid.NewGuid().ToString();
                boq.boq_detail2_id = ddBoq;
                boq.item_id = itemId;
                boq.item_qty = 0;
                boq.item_unit_price = 0;
                db.tb_boq_detail3.Add(boq);
                db.SaveChanges();
            }
        }
        public static double GetBoqItemQty(string projectId,string jobId,string itemId)
        {
            double boqItemQty = 0;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                boqItemQty =Convert.ToDouble((from tbl in db.tb_boq_detail3
                                  join item in db.tb_product on tbl.item_id equals item.product_id
                                  join boq2 in db.tb_boq_detail2 on tbl.boq_detail2_id equals boq2.boq_detail2_id
                                  join boq1 in db.tb_boq_detail1 on boq2.boq_detail1_id equals boq1.boq_detail1_id
                                  join boq in db.tb_build_of_quantity on boq1.boq_id equals boq.boq_id
                                  where boq.project_id == projectId && boq.status==true && tbl.item_id==itemId && boq1.job_category_id==jobId
                                  select tbl.item_qty).FirstOrDefault());
            }
            catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequest.cs", "GetBoqItemQty", ex.StackTrace, ex.Message);
            }
            return boqItemQty;
        }
        public static string checkJobCategory(string job)
        {
            string jobCategoryId = string.Empty;
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var j = db.tb_job_category.Where(x => string.Compare(job, x.j_category_id) == 0).FirstOrDefault();
                if (j != null)
                    jobCategoryId = job;
                else
                {
                    /*
                    tb_job_category jobCategory = new tb_job_category();
                    jobCategory.j_category_id = Guid.NewGuid().ToString();
                    jobCategory.j_category_name = job;
                    jobCategory.j_status = true;
                    jobCategory.created_by = "System";
                    jobCategory.created_date = DateTime.Now;
                    db.tb_job_category.Add(jobCategory);
                    db.SaveChanges();
                    jobCategoryId = jobCategory.j_category_id;
                    */
                    Importing obj = new Importing();
                    jobCategoryId = obj.getJobCategoryID(job);
                }
            }
            return jobCategoryId;
        }
        public static string GetBOQId(string id)
        {
            string boqID = string.Empty;
            using(kim_mexEntities db=new kim_mexEntities())
            {
                boqID = (from tbl in db.tb_build_of_quantity
                         where tbl.project_id == id && tbl.boq_status != "Reject" && tbl.status == true
                         select tbl.boq_id
                     ).FirstOrDefault();
            }    
            return boqID;
        }
        private string GetJobCategoryCode(string boqId)
        {
            string jobCategoryCode = string.Empty;
            using(kim_mexEntities db=new kim_mexEntities())
            {
                string lastJobCategoryCode = db.tb_boq_detail1.OrderByDescending(x => x.job_category_code).Where(x => string.Compare(x.boq_id, boqId) == 0).Select(x => x.job_category_code).FirstOrDefault();
                string[] splitCode = lastJobCategoryCode.Split('-');
                int code = Convert.ToInt32(splitCode[splitCode.Count() - 1]) + 1;
                string ccode = code.ToString().Length == 1 ? "00" + code : code.ToString().Length == 2 ? "0" + code : code.ToString();
                jobCategoryCode = splitCode[0]+"-" + splitCode[1]+"-" + ccode;
            }
            return jobCategoryCode;
        }
        public static bool ValidationOnSave(List<ItemRequestDetail1ViewModel> jobcategories, List<ItemRequestDetail2ViewModel> items)
        {
            bool result=false;
            foreach(var jobcategory in jobcategories)
            {
                var requestItems = items.Where(x => string.Compare(jobcategory.job_group, x.job_group) == 0).ToList();
                var dupItems = requestItems.GroupBy(x => x.ir_item_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItems.Count() > 0) { return true; }
            }
            return result;
        }
        public static bool ValidationOnSave(List<ItemRequestDetail2ViewModel> items)
        {
            bool result = false;
            if (items == null)
            {
                return result;
            }
            var dupItems = items.GroupBy(x => x.ir_item_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (dupItems.Count() > 0) { return true; }
            return result;
        }
        public static List<Models.ItemRequestViewModel> GetAllItemRequestDropdownList(string id=null)
        {
            List<Models.ItemRequestViewModel> models = new List<ItemRequestViewModel>();
            using(kim_mexEntities context=new kim_mexEntities())
            {
                models = context.tb_item_request.OrderByDescending(x => x.created_date).Where(x => x.status == true && x.is_completed == false && string.Compare(x.ir_status, "Approved") == 0).Select(x => new Models.ItemRequestViewModel() { ir_id = x.ir_id, ir_no = x.ir_no,created_date=x.created_date,ir_project_id=x.ir_project_id }).ToList();
                if (!string.IsNullOrEmpty(id))
                {
                    var isExits = models.Where(m => string.Compare(m.ir_id, id) == 0).FirstOrDefault() != null ? true : false;
                    if (!isExits)
                    {
                        var item = context.tb_item_request.Find(id);
                        models.Add(new ItemRequestViewModel() { ir_id = item.ir_id, ir_no = item.ir_no, created_date = item.created_date,ir_project_id=item.ir_project_id });
                        models = models.OrderByDescending(x => x.created_date).ToList();
                    }
                }
            }
            return models;
        }
        public static List<Models.ItemRequestDetail2ViewModel> GetAllAvailableItem(string id,string poId=null)
        {
            List<Models.ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(poId))
                {
                    var itemRequests = (from item in db.tb_ir_detail2
                             join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                             join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                             join pro in db.tb_product on item.ir_item_id equals pro.product_id
                             join u in db.tb_unit on pro.product_unit equals u.Id
                             orderby pro.product_code
                             where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true && item.remain_qty > 0 && item.remain_qty != null
                             select new Models.ItemRequestDetail2ViewModel()
                             {
                                 ir_detail2_id = item.ir_detail2_id,
                                 ir_item_id = item.ir_item_id,
                                 product_code = pro.product_code,
                                 product_name = pro.product_name,
                                 product_unit = u.Name,
                                 unit_id=pro.product_unit,
                                 ir_qty = item.ir_qty,
                                 ir_item_unit = item.ir_item_unit,
                                 approved_qty = item.approved_qty,
                                 remain_qty = item.remain_qty
                             }).ToList();
                    if (itemRequests.Any())
                    {
                        var transferworkorders = db.tb_stock_transfer_voucher.Where(s => string.Compare(s.item_request_id, id) == 0 && s.status == true).ToList();
                        foreach (var item in itemRequests)
                        {
                            ItemRequestDetail2ViewModel ir = new ItemRequestDetail2ViewModel();
                            ir.ir_detail2_id = item.ir_detail2_id;
                            ir.ir_item_id = item.ir_item_id;
                            ir.product_code = item.product_code;
                            ir.product_name = item.product_name;
                            ir.product_unit = item.product_unit;
                            ir.unit_id = item.unit_id;
                            ir.ir_qty = item.ir_qty;
                            ir.ir_item_unit = db.tb_unit.Where(w => string.Compare(item.ir_item_unit, w.Id) == 0).Select(s => s.Name).FirstOrDefault();
                            ir.approved_qty = item.approved_qty;
                            ir.requested_unit_id = item.ir_item_unit;
                            ir.remain_qty = item.remain_qty;

                            //get from stock transfer workorder
                            //if (transferworkorders.Any())
                            //{
                            //    foreach (var transfer in transferworkorders)
                            //    {
                            //        var transferDetails = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, transfer.stock_transfer_id) == 0 && string.Compare(s.st_item_id,ir.ir_item_id)==0 && s.status==true).FirstOrDefault();
                            //        if (transferDetails != null)
                            //            ir.remain_qty = ir.remain_qty + Convert.ToDecimal(transferDetails.quantity);
                            //    }

                            //}

                            items.Add(ir);
                        }
                    }
                }
                else
                {
                    var itemRequests= (from item in db.tb_ir_detail2
                                       join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                                       join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                                       join pro in db.tb_product on item.ir_item_id equals pro.product_id
                                       join u in db.tb_unit on pro.product_unit equals u.Id
                                       orderby pro.product_code
                                       where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true
                                       select new Models.ItemRequestDetail2ViewModel()
                                       {
                                           ir_detail2_id = item.ir_detail2_id,
                                           ir_item_id = item.ir_item_id,
                                           product_code = pro.product_code,
                                           product_name = pro.product_name,
                                           product_unit = u.Name,
                                           unit_id=pro.product_unit,
                                           ir_qty = item.ir_qty,
                                           ir_item_unit = item.ir_item_unit,
                                           approved_qty = item.approved_qty,
                                           remain_qty = item.remain_qty
                                       }).ToList();
                    var purchaseOrderItems = (from pod in db.tb_purchase_order_detail where string.Compare(pod.purchase_order_id, poId) == 0 select new { pod }).ToList();
                    foreach(var item in itemRequests)
                    {
                        decimal poqty = 0;
                        var itempo =purchaseOrderItems.Where(x => string.Compare(x.pod.item_id, item.ir_item_id) == 0).Select(x => x.pod.quantity).FirstOrDefault();
                        if (itempo != null) poqty = Convert.ToDecimal(itempo);
                        string ir_unit_name = db.tb_unit.Where(w => string.Compare(w.Id, item.ir_item_unit) == 0).Select(s => s.Name).FirstOrDefault();
                        items.Add(new ItemRequestDetail2ViewModel()
                        {
                            ir_detail2_id = item.ir_detail2_id,
                            ir_item_id = item.ir_item_id,
                            product_code = item.product_code,
                            product_name = item.product_name,
                            product_unit = item.product_unit,
                            unit_id=item.unit_id,
                            ir_qty = item.ir_qty,
                            ir_item_unit = ir_unit_name,
                            requested_unit_id=item.ir_item_unit,
                            approved_qty = item.approved_qty,
                            remain_qty = item.remain_qty+poqty
                        });
                    }
                }
            }
            return items;
        }
        public static List<Models.ItemRequestDetail2ViewModel> GetMRRemainQuantityItemByPRId(string prId)
        {
            List<Models.ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var itemRequests = (from item in db.tb_ir_detail2
                                    join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                                    join mr in db.tb_item_request on job.ir_id equals mr.ir_id
                                    join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                    join pro in db.tb_product on item.ir_item_id equals pro.product_id
                                    join u in db.tb_unit on pro.product_unit equals u.Id
                                    orderby pro.product_code
                                    where string.Compare(pr.purchase_requisition_id, prId) == 0 && item.is_approved == true && item.remain_qty > 0 && item.remain_qty != null
                                    select new Models.ItemRequestDetail2ViewModel()
                                    {
                                        ir_detail2_id = item.ir_detail2_id,
                                        ir_item_id = item.ir_item_id,
                                        product_code = pro.product_code,
                                        product_name = pro.product_name,
                                        product_unit = u.Name,
                                        unit_id = pro.product_unit,
                                        ir_qty = item.ir_qty,
                                        ir_item_unit = item.ir_item_unit,
                                        approved_qty = item.approved_qty,
                                        remain_qty = item.remain_qty
                                    }).ToList();
                if (itemRequests.Any())
                {
                    foreach (var item in itemRequests)
                    {
                        ItemRequestDetail2ViewModel ir = new ItemRequestDetail2ViewModel();
                        ir.ir_detail2_id = item.ir_detail2_id;
                        ir.ir_item_id = item.ir_item_id;
                        ir.product_code = item.product_code;
                        ir.product_name = item.product_name;
                        ir.product_unit = item.product_unit;
                        ir.unit_id = item.unit_id;
                        ir.ir_qty = item.ir_qty;
                        ir.ir_item_unit = db.tb_unit.Where(w => string.Compare(item.ir_item_unit, w.Id) == 0).Select(s => s.Name).FirstOrDefault();
                        ir.approved_qty = item.approved_qty;
                        ir.requested_unit_id = item.ir_item_unit;
                        ir.remain_qty = item.remain_qty;

                        items.Add(ir);
                    }
                }
            }
            return items;
        }
        public static List<Models.ItemRequestDetail2ViewModel> GetAllAvailableItembyStockTransfer(string id,string stId = null)
        {
            List<Models.ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(stId))
                {
                    items = (from item in db.tb_ir_detail2
                             join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                             join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                             join pro in db.tb_product on item.ir_item_id equals pro.product_id
                             orderby pro.product_code
                             where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true && item.remain_qty > 0 && item.remain_qty != null
                             select new Models.ItemRequestDetail2ViewModel()
                             {
                                 ir_detail2_id = item.ir_detail2_id,
                                 ir_item_id = item.ir_item_id,
                                 product_code = pro.product_code,
                                 product_name = pro.product_name,
                                 product_unit = pro.product_unit,
                                 ir_qty = item.ir_qty,
                                 ir_item_unit = item.ir_item_unit,
                                 approved_qty = item.approved_qty,
                                 remain_qty = item.remain_qty
                             }).ToList();
                }
                else
                {
                    var itemRequests = (from item in db.tb_ir_detail2
                                        join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                                        join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                                        join pro in db.tb_product on item.ir_item_id equals pro.product_id
                                        orderby pro.product_code
                                        where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true
                                        select new Models.ItemRequestDetail2ViewModel()
                                        {
                                            ir_detail2_id = item.ir_detail2_id,
                                            ir_item_id = item.ir_item_id,
                                            product_code = pro.product_code,
                                            product_name = pro.product_name,
                                            product_unit = pro.product_unit,
                                            ir_qty = item.ir_qty,
                                            ir_item_unit = item.ir_item_unit,
                                            approved_qty = item.approved_qty,
                                            remain_qty = item.remain_qty
                                        }).ToList();
                    var stockTransferItems = (from std in db.tb_stock_transfer_detail where string.Compare(std.st_ref_id, stId) == 0 select new { std }).ToList();
                    foreach (var item in itemRequests)
                    {
                        decimal poqty = 0;
                        var itempo = stockTransferItems.Where(x => string.Compare(x.std.st_item_id, item.ir_item_id) == 0).Select(x => x.std.quantity).FirstOrDefault();
                        if (itempo != null) poqty = Convert.ToDecimal(itempo);
                        items.Add(new ItemRequestDetail2ViewModel()
                        {
                            ir_detail2_id = item.ir_detail2_id,
                            ir_item_id = item.ir_item_id,
                            product_code = item.product_code,
                            product_name = item.product_name,
                            product_unit = item.product_unit,
                            ir_qty = item.ir_qty,
                            ir_item_unit = item.ir_item_unit,
                            approved_qty = item.approved_qty,
                            remain_qty = item.remain_qty + poqty
                        });
                    }
                }
                items = items.Where(m => m.remain_qty > 0).ToList();
            }
            return items;
        }
        //GetAllAvailableItembyTransferFromMainStock
        public static List<Models.ItemRequestDetail2ViewModel> GetAllAvailableItembyTransferFromMainStock(string id, string stId = null)
        {
            List<Models.ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(stId))
                {
                    items = (from item in db.tb_ir_detail2
                             join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                             join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                             join pro in db.tb_product on item.ir_item_id equals pro.product_id
                             orderby pro.product_code
                             where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true && item.remain_qty > 0 && item.remain_qty != null
                             select new Models.ItemRequestDetail2ViewModel()
                             {
                                 ir_detail2_id = item.ir_detail2_id,
                                 ir_item_id = item.ir_item_id,
                                 product_code = pro.product_code,
                                 product_name = pro.product_name,
                                 product_unit = pro.product_unit,
                                 ir_qty = item.ir_qty,
                                 ir_item_unit = item.ir_item_unit,
                                 approved_qty = item.approved_qty,
                                 remain_qty = item.remain_qty
                             }).ToList();
                }
                else
                {
                    var itemRequests = (from item in db.tb_ir_detail2
                                        join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                                        join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                                        join pro in db.tb_product on item.ir_item_id equals pro.product_id
                                        orderby pro.product_code
                                        where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true
                                        select new Models.ItemRequestDetail2ViewModel()
                                        {
                                            ir_detail2_id = item.ir_detail2_id,
                                            ir_item_id = item.ir_item_id,
                                            product_code = pro.product_code,
                                            product_name = pro.product_name,
                                            product_unit = pro.product_unit,
                                            ir_qty = item.ir_qty,
                                            ir_item_unit = item.ir_item_unit,
                                            approved_qty = item.approved_qty,
                                            remain_qty = item.remain_qty
                                        }).ToList();
                    var transferFromMainStockItems = (from std in db.tb_transfer_frommain_stock_detail where string.Compare(std.st_ref_id, stId) == 0 select new { std }).ToList();
                    foreach (var item in itemRequests)
                    {
                        decimal poqty = 0;
                        var itempo = transferFromMainStockItems.Where(x => string.Compare(x.std.st_item_id, item.ir_item_id) == 0).Select(x => x.std.quantity).FirstOrDefault();
                        if (itempo != null) poqty = Convert.ToDecimal(itempo);
                        items.Add(new ItemRequestDetail2ViewModel()
                        {
                            ir_detail2_id = item.ir_detail2_id,
                            ir_item_id = item.ir_item_id,
                            product_code = item.product_code,
                            product_name = item.product_name,
                            product_unit = item.product_unit,
                            ir_qty = item.ir_qty,
                            ir_item_unit = item.ir_item_unit,
                            approved_qty = item.approved_qty,
                            remain_qty = item.remain_qty + poqty
                        });
                    }
                }
                items = items.Where(m => m.remain_qty > 0).ToList();
            }
            return items;
        }
        public static void UpdateItemRemainQuantity(string ir_detail2_id,decimal quantity)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_ir_detail2 irdd = db.tb_ir_detail2.Where(x => string.Compare(x.ir_detail2_id, ir_detail2_id) == 0).FirstOrDefault();
                decimal oldRemainQuantity = Convert.ToDecimal(irdd.remain_qty);
                decimal newRemainQuantity = oldRemainQuantity - quantity;
                irdd.remain_qty = newRemainQuantity;
                db.SaveChanges();
            }
        }
        public static void UpdateCompletedItemRequest(string id,bool isST,bool isReturn)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countNotCompleteItem = 0;
                var items = (from item in db.tb_ir_detail2
                             join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                             join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                             where string.Compare(pr.ir_id, id) == 0
                             select new { item }).ToList();
                foreach (var item in items)
                    if (item.item.remain_qty > 0) countNotCompleteItem++;
                tb_item_request ir = db.tb_item_request.Find(id);
                ir.is_completed = countNotCompleteItem == 0 && isReturn==false ? true : false;
                
                db.SaveChanges();
            }
        }
        public static void UpdateCompletedItemRequest(string id,string userid=null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countNotCompleteItem = 0;
                var ppr = db.tb_purchase_requisition.Find(id);
                var items = (from item in db.tb_ir_detail2
                             join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                             join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                             where string.Compare(pr.ir_id, ppr.material_request_id) == 0
                             select new { item }).ToList();
                foreach (var item in items)
                    if (item.item.remain_qty > 0) countNotCompleteItem++;
                tb_item_request ir = db.tb_item_request.Find(ppr.material_request_id);
                ir.is_completed = countNotCompleteItem == 0 ? true : false;
                ir.ir_status = "Completed";
                ir.checked_by = userid;
                ir.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                ppr.is_quote_complete = countNotCompleteItem == 0 ? true : false;
                db.SaveChanges();
            }
        }
        public static void RollbackItemRequestRemainQuantity(string prid,string poId,bool isRollback,bool isApproval,string status=null)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<Models.ItemRequestDetail2ViewModel> itemRequests = new List<ItemRequestDetail2ViewModel>();
                List<Models.PurchaseOrderDetailViewModel> purchaseOrderItems = new List<PurchaseOrderDetailViewModel>();
                string id = db.tb_purchase_requisition.Find(prid).material_request_id;

                itemRequests = (from item in db.tb_ir_detail2
                         join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                         join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                         where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true
                         select new Models.ItemRequestDetail2ViewModel(){ir_detail2_id = item.ir_detail2_id,ir_item_id=item.ir_item_id}).ToList();
                if (string.IsNullOrEmpty(status))
                {
                    purchaseOrderItems = db.tb_purchase_order_detail.Where(x => string.Compare(x.purchase_order_id, poId) == 0).Select(x => new PurchaseOrderDetailViewModel()
                    {
                        item_id = x.item_id,
                        quantity = x.quantity,
                        po_quantity = x.po_quantity,
                        po_unit = x.po_unit,
                    }).ToList();
                }
                else
                {
                    purchaseOrderItems = db.tb_purchase_order_detail.Where(x => string.Compare(x.purchase_order_id, poId) == 0 && string.Compare(x.item_status,status)==0).Select(x => new PurchaseOrderDetailViewModel()
                    {
                        item_id = x.item_id,
                        quantity = x.quantity,
                        po_quantity = x.po_quantity,
                        po_unit = x.po_unit,
                    }).ToList();
                }
                foreach(PurchaseOrderDetailViewModel purchaseOrderItem in purchaseOrderItems)
                {
                    string irDetail2Id = itemRequests.Where(x => string.Compare(x.ir_item_id, purchaseOrderItem.item_id) == 0).Select(x => x.ir_detail2_id).FirstOrDefault();
                    if (irDetail2Id != null)
                    {
                        tb_ir_detail2 irDetail2 = db.tb_ir_detail2.Find(irDetail2Id);
                        if (isRollback)
                        {
                            if(isApproval)
                                irDetail2.remain_qty = irDetail2.remain_qty + Class.CommonClass.ConvertMultipleUnit(purchaseOrderItem.item_id, purchaseOrderItem.po_unit, Convert.ToDecimal(purchaseOrderItem.po_quantity),irDetail2.ir_item_unit);
                            else 
                                irDetail2.remain_qty = irDetail2.remain_qty + purchaseOrderItem.quantity;
                        }
                        else
                        {
                            if (isApproval)
                                irDetail2.remain_qty = irDetail2.remain_qty - Class.CommonClass.ConvertMultipleUnit(purchaseOrderItem.item_id,purchaseOrderItem.po_unit,Convert.ToDecimal(purchaseOrderItem.po_quantity),irDetail2.ir_item_unit);
                            else 
                                irDetail2.remain_qty = irDetail2.remain_qty - purchaseOrderItem.quantity;
                        }
                        db.SaveChanges();
                    }
                }
                UpdateCompletedItemRequest(prid);
            }
        }
        internal static void RollbackItemRequestRemainQuantity(object item_request_id, string id, bool v1, bool v2, string v3)
        {
            throw new NotImplementedException();
        }
        public static void RollbackRemainQuantity(string id,string poID,bool isRollback)
        {

        }
        public static string GetPurchaseRequisitionNumberbyMonth()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                string purchaseRequisitionNumber = string.Empty;
                string yy = CommonClass.ToLocalTime(DateTime.Now).Year.ToString().Substring(2, 2);
                string mm = CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? string.Format("0{0}", CommonClass.ToLocalTime(DateTime.Now).Month.ToString()) : CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                string strPRCompare = string.Format("PR-{0}-{1}-", yy, mm);
                var lastPR = db.tb_item_request.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.ir_no.Contains(strPRCompare)).Select(s => s.ir_no).FirstOrDefault();
                int numLastPR = 0;
                if (lastPR==null)
                    numLastPR = 1;
                else
                {
                    string strPRLastNumber = lastPR.ToString();
                    string[] splitPRNumber = strPRLastNumber.Split('-');
                    numLastPR = Convert.ToInt32(splitPRNumber[splitPRNumber.Length - 1]) + 1;
                }
                string strPR = numLastPR.ToString().Length == 1 ? string.Format("00{0}", numLastPR.ToString()) : numLastPR.ToString().Length == 2 ? string.Format("0{0}", numLastPR.ToString()) : numLastPR.ToString();
                purchaseRequisitionNumber = string.Format("PR-{0}-{1}-{2}", yy, mm,strPR);
                return purchaseRequisitionNumber;
            }
        }
        public static List<ItemRequestDetail2ViewModel> GetMaterialRequestListItems(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                Controllers.ItemRequestController_202108031747 obj = new Controllers.ItemRequestController_202108031747(); 
                List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
                List<ItemRequestDetail2ViewModel> materialRequestItems = obj.GetItemRequestDetail(id).ir1[0].ir2.Where(s=>s.is_approved==true).ToList();
                var transferworkorders = db.tb_stock_transfer_voucher.Where(s => string.Compare(s.item_request_id, id) == 0 && s.status == true).ToList();
                var transferworkshops = db.transferformmainstocks.Where(s => string.Compare(s.item_request_id, id) == 0 && s.status == true).ToList();
                foreach (ItemRequestDetail2ViewModel requestitem in materialRequestItems)
                {
                    ItemRequestDetail2ViewModel item = new ItemRequestDetail2ViewModel();
                    item = requestitem;
                    item.remain_qty = item.approved_qty;

                    //get from stock transfer workorder
                    if (transferworkorders.Any())
                    {
                        foreach (var transfer in transferworkorders)
                        {
                            var transferDetails = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, transfer.stock_transfer_id) == 0 && string.Compare(s.st_item_id, item.ir_item_id) == 0 && s.status == false).FirstOrDefault();
                            if (transferDetails != null)
                                item.remain_qty = item.remain_qty - Convert.ToDecimal(transferDetails.quantity);
                        }

                    }

                    if (transferworkshops.Any())
                    {
                        foreach (var tranfer in transferworkshops)
                        {
                            var transferDetail = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, tranfer.stock_transfer_id) == 0 && string.Compare(s.st_item_id, item.ir_item_id) == 0).FirstOrDefault();
                            if (transferDetail != null)
                                item.remain_qty = item.remain_qty - Convert.ToDecimal(transferDetail.quantity);
                        }
                    }

                    items.Add(item);
                }
                return items;
            }
        }
    }
}