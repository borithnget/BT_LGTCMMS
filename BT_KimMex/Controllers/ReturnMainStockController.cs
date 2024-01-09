using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class ReturnMainStockController : Controller
    {
        // GET: ReturnMainStock
        public ActionResult Index()
        {
            return View(this.GetReturntoWorkshopListItems());
        }
        public ActionResult Create()
        {
            BOQ obj = new BOQ();
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            //projects = obj.GetBOQProjects();
            //projects = CommonClass.GetAllProject();
            //ViewBag.ProjectID = new SelectList(projects, "project_id", "project_full_name");
            ViewBag.Projects = CommonFunctions.GetReturnToWorkshopProjectListitems(User.IsInRole(Role.SystemAdmin), User.Identity.GetUserId().ToString());
            ViewBag.Transfers = this.GetTransferReferenceListItems();
            ViewBag.PurchaseRequisitionNumber = Class.CommonClass.GenerateProcessNumber("RW");
            return View();
        }
        public ActionResult GetReturnToMainStock(string status)
        {
            List<ReturnMainStockViewModel> stockDamages = new List<ReturnMainStockViewModel>();
           // stockDamages = this.GetReturnToMainStock1(status);
            return Json(new { data = stockDamages }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWarehouse(string id)
        {
            string warehouseId = GetWarehouseIDbyPurchaseRequisition(id);
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(warehouseId))
                {
                    var getWarehouse = db.tb_warehouse.Where(x => x.warehouse_id == warehouseId).FirstOrDefault();
                    if (getWarehouse == null)
                    {
                        return Json(JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { result = "success", data = getWarehouse }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { result = "error", message = "No warehouse available for this request." }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetItemFromWarehouse(string id, string stId = null)
        {

            List<STItemViewModel> stItems = new List<STItemViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
         
                var warehouses =(from pro in db.tb_project join site in db.tb_site on pro.site_id equals site.site_id join war in db.tb_warehouse 
                                 on site.site_id equals war.warehouse_site_id where pro.project_id == id && pro.project_status == true
                                 select war.warehouse_id                  
                                 ).ToList();

                    foreach (var wh in warehouses)
                    {
                    
                        var irItem = (from inv in db.tb_inventory
                                      join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                      join pro in db.tb_product on inv.product_id equals pro.product_id     
                                      orderby inv.inventory_date descending
                                      where inv.warehouse_id == wh 
                                      select new { inv, wah, pro }
                                  ).ToList();
                      
                        var query = irItem.GroupBy(x => x.inv.product_id)
                              .Where(g => g.Count() > 1)
                              .Select(y => y.Key)
                              .ToList();
                        var query1 = irItem.GroupBy(x => x.inv.product_id)
                             .Where(g => g.Count() == 1)
                             .Select(y => y.Key)
                             .ToList();

                    foreach (var fd in query)
                    {
                        var dup = (from inv in db.tb_inventory
                                   join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                   join pro in db.tb_product on inv.product_id equals pro.product_id                                   
                                   where inv.warehouse_id == wh && inv.product_id == fd
                                   orderby inv.inventory_date descending
                                   select new { inv, wah, pro }).FirstOrDefault();                      
                        if(dup != null)
                        {
                            STItemViewModel item = new STItemViewModel();
                            
                            item.itemID = dup.inv.product_id;
                            item.itemCode = dup.pro.product_code;
                            item.itemName = dup.pro.product_name;
                            item.itemUnit = dup.pro.product_unit;
                            item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;

                            item.warehouseID = dup.inv.warehouse_id;
                            item.warehouseName = dup.wah.warehouse_name;
                            item.stockBalance = dup.inv.total_quantity;

                            item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                            item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                            item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                            item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                            item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                            item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                            item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                            item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                            item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                            item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == dup.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                            stItems.Add(item);
                        }

                    }
                    foreach(var search in query1)
                    {
                        var dup1 = (from inv in db.tb_inventory
                                   join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                   join pro in db.tb_product on inv.product_id equals pro.product_id
                                   orderby inv.inventory_date descending
                                   where inv.warehouse_id == wh && inv.product_id == search
                                   select new { inv, wah, pro }).FirstOrDefault();
                        if(dup1 != null)
                        {
                            STItemViewModel item = new STItemViewModel();

                            item.itemID = dup1.inv.product_id;
                            item.itemCode = dup1.pro.product_code;
                            item.itemName = dup1.pro.product_name;
                            item.itemUnit = dup1.pro.product_unit;
                            item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;

                            item.warehouseID = dup1.inv.warehouse_id;
                            item.warehouseName = dup1.wah.warehouse_name;
                            item.stockBalance = dup1.inv.total_quantity;

                            item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                            item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                            item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                            item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                            item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                            item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                            item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                            item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                            item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                            item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == dup1.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                            stItems.Add(item);
                        }

                    }
             
                }
                return Json(new { result = "success", data = stItems }, JsonRequestBehavior.AllowGet);
            }
        }
        public static string GetWarehouseIDbyPurchaseRequisition(string puchaseRequisitionID)
        {
            string warehouseId = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                warehouseId = (from pr in db.tb_item_request
                               join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                               join site in db.tb_site on pro.site_id equals site.site_id
                               join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                               where string.Compare(pro.project_id, puchaseRequisitionID) == 0
                               select warehouse.warehouse_id).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetWarehouseIDbyPurchaseRequisition", ex.StackTrace, ex.Message);
            }
            return warehouseId;
        }
        public ActionResult CreateReturnMainStock(ReturnMainStockViewModel model, List<InventoryViewModel> inventories)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_return_main_stock rms = new tb_return_main_stock();
                rms.return_main_stock_id = Guid.NewGuid().ToString();
                rms.return_type = model.return_type;
                rms.return_ref_id = model.return_ref_id;
                rms.return_main_stock_no = Class.CommonClass.GenerateProcessNumber("RW");
                rms.return_main_stock_status = Status.Pending;
                rms.status = true;
                rms.create_by = User.Identity.GetUserId();
                rms.create_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                rms.update_by = User.Identity.GetUserId();
                rms.update_date = DateTime.Now;
                rms.is_receive_completed = false;
                db.tb_return_main_stock.Add(rms);
                db.SaveChanges();
                //Generate invoice number by warehouse
                foreach (InventoryViewModel inv in inventories)
                {
                    tb_return_mainstock_detail rmsDetail = new tb_return_mainstock_detail();
                    rmsDetail.main_stock_detail_id = Guid.NewGuid().ToString();
                    rmsDetail.main_stock_detail_ref = rms.return_main_stock_id;
                    rmsDetail.main_stock_detail_item = inv.product_id;
                    rmsDetail.main_stock_warehouse_id = inv.warehouse_id;
                    rmsDetail.quantity = inv.in_quantity;
                    rmsDetail.remain_quantity = inv.in_quantity;
                    rmsDetail.unit = inv.unit;
                    rmsDetail.item_status = Status.Pending;
                    rmsDetail.remark = inv.remark;
                    db.tb_return_mainstock_detail.Add(rmsDetail);
                    db.SaveChanges();
                }

                //Class.ReturnToWorkshop.RollBackItemQuantitybyWorkshopTransfer(rms.return_ref_id, rms.return_main_stock_id, false);
                //update pending completed projec
                //tb_project project = db.tb_project.Find(rms.return_ref_id);
                //project.p_status = ProjectStatus.PeningComplete;
                //db.SaveChanges();

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Detail(string id)
        {
            ReturnMainStockViewModel sTransfer = new ReturnMainStockViewModel();
            sTransfer = this.GetReturntoWorkshopItemDetail(id);
            return View(sTransfer);
        }
        public ActionResult ApproveFeedback(string id)
        {
            return View(this.GetReturntoWorkshopItemDetail(id));
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<InventoryViewModel> models)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                int countItemApproved = 0;
                foreach(InventoryViewModel item in models)
                {
                    string iid = item.inventory_id;
                    tb_return_mainstock_detail dreturn = db.tb_return_mainstock_detail.Find(iid);
                    dreturn.item_status = item.item_status;
                    dreturn.remark = item.remark;
                    db.SaveChanges();
                    if (string.Compare(dreturn.item_status, Status.Approved) == 0)
                        countItemApproved++;
                }
                tb_return_main_stock mreturn = db.tb_return_main_stock.Find(id);
                mreturn.return_main_stock_status = countItemApproved == models.Count() ? Status.Approved : Status.PendingFeedback;
                mreturn.approved_by = User.Identity.GetUserId();
                mreturn.approved_date = DateTime.Now;
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approval(string id,string status,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_return_main_stock returnMainStock = db.tb_return_main_stock.Find(id);
                returnMainStock.return_main_stock_status = string.Compare(status, Status.Approved) == 0 ? Status.Checked : Status.CheckRejected;
                returnMainStock.checked_by = User.Identity.GetUserId();
                returnMainStock.checked_date = DateTime.Now;
                returnMainStock.checked_comment = comment;
                db.SaveChanges();
                if (string.Compare(status, Status.Rejected) == 0)
                {
                    //Class.ReturnToWorkshop.RollBackItemQuantitybyWorkshopTransfer(returnMainStock.return_ref_id, returnMainStock.return_main_stock_id, true);
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = returnMainStock.return_main_stock_id;
                    reject.ref_type = "Return to Workshop";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.GetUserId();
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Completed(string id, string status, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_return_main_stock returnMainStock = db.tb_return_main_stock.Find(id);
                returnMainStock.return_main_stock_status = string.Compare(status, Status.Approved) == 0 ? Status.Completed : Status.Rejected;
                returnMainStock.completed_by = User.Identity.GetUserId();
                returnMainStock.completed_date = DateTime.Now;
                returnMainStock.completed_comment = comment;
                db.SaveChanges();

                string warehouseID = db.tb_warehouse.Where(s => string.Compare(s.warehouse_project_id, returnMainStock.return_ref_id) == 0).Select(s => s.warehouse_id).FirstOrDefault().ToString();

                if (string.Compare(returnMainStock.return_main_stock_status, Status.Completed) == 0)
                {
                    //insert to inventory here

                    var inventories = db.tb_return_mainstock_detail.Where(m => m.main_stock_detail_ref == id).ToList();
                    foreach (var inv in inventories)
                    {
                        //add item balance to workshop
                        decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.main_stock_detail_item && m.warehouse_id == EnumConstants.WORKSHOP)
                            .Select(m => m.total_quantity).FirstOrDefault());
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = id;
                        inventory.inventory_status_id = "9";
                        inventory.warehouse_id = EnumConstants.WORKSHOP;
                        inventory.product_id = inv.main_stock_detail_item;
                        inventory.out_quantity = 0;
                        inventory.in_quantity = CommonClass.ConvertMultipleUnit(inv.main_stock_detail_item, inv.unit, Convert.ToDecimal(inv.quantity));
                        inventory.total_quantity = totalQty +inventory.in_quantity;
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                        //remove item balance from warehouse
                        decimal totalQty1 = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.main_stock_detail_item && m.warehouse_id == warehouseID)
                            .Select(m => m.total_quantity).FirstOrDefault());
                        tb_inventory inventory1 = new tb_inventory();
                        inventory1.inventory_id = Guid.NewGuid().ToString();
                        inventory1.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory1.ref_id = id;
                        inventory1.inventory_status_id = "9";
                        inventory1.warehouse_id = warehouseID;
                        inventory1.product_id = inv.main_stock_detail_item;
                        inventory1.out_quantity = CommonClass.ConvertMultipleUnit(inv.main_stock_detail_item, inv.unit, Convert.ToDecimal(inv.quantity));
                        inventory1.in_quantity = 0;
                        inventory1.total_quantity = totalQty1 - inventory1.out_quantity;
                        db.tb_inventory.Add(inventory1);
                        db.SaveChanges();
                    }

                    //tb_project project = db.tb_project.Find(returnMainStock.return_ref_id);
                    //project.p_status = ProjectStatus.Completed;
                    //db.SaveChanges();

                }
                else
                {
                    //Class.ReturnToWorkshop.RollBackItemQuantitybyWorkshopTransfer(returnMainStock.return_ref_id, returnMainStock.return_main_stock_id, true);
                    //update project status to active
                    tb_project project = db.tb_project.Find(returnMainStock.return_ref_id);
                    project.p_status = ProjectStatus.Active;
                    db.SaveChanges();

                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = returnMainStock.return_main_stock_id;
                    reject.ref_type = "Return to Workshop";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.GetUserId();
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                }

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CancelRequest(string id,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_return_main_stock returnMainStock = db.tb_return_main_stock.Find(id);
                returnMainStock.return_main_stock_status = Status.RequestCancelled;
                returnMainStock.last_approved_by = comment;
                returnMainStock.update_date = CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                tb_project project = db.tb_project.Find(returnMainStock.return_ref_id);
                project.p_status = ProjectStatus.Active;
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        #region json function
        public ActionResult GetTransferItemDetailsJson(string id)
        {
            return Json(new { data = this.GetAvialableItemToTransfer(id) }, JsonRequestBehavior.AllowGet); 
        }
        public ActionResult GetWarehouseItembyTransferWorkshopJson(string transferId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var item = (from transfer in db.transferformmainstocks
                            join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            join site in db.tb_site on pro.site_id equals site.site_id
                            join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                            where string.Compare(transfer.stock_transfer_id, transferId) == 0
                            select new { warehouse_id = wh.warehouse_id, warehouse_name = wh.warehouse_name }).FirstOrDefault();
                return Json(new { data = item }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetReturnInformationbyProject(string projectId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                ReturnMainStockViewModel model = new ReturnMainStockViewModel();
                List<ReturnMainStockItemViewModel> pendingItemss = new List<ReturnMainStockItemViewModel>();
                model.warehouse = db.tb_warehouse.Where(s => string.Compare(s.warehouse_project_id, projectId) == 0).Select(s => new WareHouseViewModel() { warehouse_id = s.warehouse_id, warehouse_name = s.warehouse_name }).FirstOrDefault();
                if(model.warehouse!=null)
                    model.inventories = this.GetAllItembyWarehouse(model.warehouse.warehouse_id);
                //check item with pending
                var pendingRequests = db.tb_return_main_stock.Where(s => s.status == true && string.Compare(s.return_ref_id, projectId) == 0 && ( string.Compare(s.return_main_stock_status,Status.Pending)==0 || string.Compare(s.return_main_stock_status,Status.PendingFeedback)==0 ||string.Compare(s.return_main_stock_status,Status.Feedbacked)==0 || string.Compare(s.return_main_stock_status,Status.Approved)==0 || string.Compare(s.return_main_stock_status,Status.Checked)==0)).ToList();
                if (pendingRequests.Any())
                {
                    foreach(var item in pendingRequests)
                    {
                        var pendingItems = db.tb_return_mainstock_detail.Where(w => string.Compare(w.main_stock_detail_ref, item.return_main_stock_id) == 0 && string.Compare(w.item_status, Status.Pending) == 0).ToList();
                        foreach (var iitem in pendingItems)
                        {
                            var isexist = pendingItemss.Where(s => string.Compare(s.st_item_id, iitem.main_stock_detail_item) == 0).FirstOrDefault();
                            if(isexist==null)
                                pendingItemss.Add(new ReturnMainStockItemViewModel
                                {
                                    st_item_id = iitem.main_stock_detail_item,
                                    quantity = iitem.quantity,
                                    unit = iitem.unit
                                });
                            else
                            {
                                isexist.quantity = isexist.quantity + iitem.quantity;
                            }
                        }
                            
                    }
                }

                //return Json(new { data = model,pendingItems=pendingItemss }, JsonRequestBehavior.AllowGet);
                var jsonResult = Json(new { data = model, pendingItems = pendingItemss }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        #endregion

        #region common function form Return to Workshop
        public List<TransferFromMainStockViewModel> GetTransferReferenceListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<TransferFromMainStockViewModel> models = new List<TransferFromMainStockViewModel>();
                models = db.transferformmainstocks.OrderByDescending(s => s.created_date).Where(s => s.status == true && string.Compare(s.stock_transfer_status, Status.Completed) == 0 && s.is_completed == false)
                    .Select(s=>new TransferFromMainStockViewModel()
                    {
                        stock_transfer_id=s.stock_transfer_id,
                        stock_transfer_no=s.stock_transfer_no,
                    }).ToList();
                return models;
            }
        }
        public List<TransferFromMainStockItemViewModel> GetAvialableItemToTransfer(string transferId,string returnId=null)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<TransferFromMainStockItemViewModel> models = new List<TransferFromMainStockItemViewModel>();
                if (string.IsNullOrEmpty(returnId))
                {
                    models = (from dTransfer in db.tb_transfer_frommain_stock_detail
                              join item in db.tb_product on dTransfer.st_item_id equals item.product_id
                              join tunit in db.tb_unit on dTransfer.unit equals tunit.Id
                              join unit in db.tb_unit on item.product_unit equals unit.Id
                              orderby item.product_code descending
                              where dTransfer.status == true && string.Compare(dTransfer.st_ref_id, transferId) == 0 && dTransfer.remain_quantity > 0
                              select new TransferFromMainStockItemViewModel()
                              {
                                  st_item_id = dTransfer.st_item_id,
                                  itemCode = item.product_code,
                                  itemName = item.product_name,
                                  itemUnit = item.product_unit,
                                  itemUnitName = unit.Name,
                                  quantity = dTransfer.quantity,
                                  remain_quantity = dTransfer.remain_quantity,
                                  unit = dTransfer.unit,
                                  unitName = tunit.Name,
                              }).ToList();
                }else
                {
                    List<TransferFromMainStockItemViewModel> transfers= (from dTransfer in db.tb_transfer_frommain_stock_detail
                                                            join item in db.tb_product on dTransfer.st_item_id equals item.product_id
                                                            join tunit in db.tb_unit on dTransfer.unit equals tunit.Id
                                                            join unit in db.tb_unit on item.product_unit equals unit.Id
                                                            orderby item.product_code descending
                                                            where dTransfer.status == true && string.Compare(dTransfer.st_ref_id, transferId) == 0 
                                                            select new TransferFromMainStockItemViewModel()
                                                            {
                                                                st_item_id = dTransfer.st_item_id,
                                                                itemCode = item.product_code,
                                                                itemName = item.product_name,
                                                                itemUnit = item.product_unit,
                                                                itemUnitName = unit.Name,
                                                                quantity = dTransfer.quantity,
                                                                remain_quantity = dTransfer.remain_quantity,
                                                                unit = dTransfer.unit,
                                                                unitName = tunit.Name,
                                                            }).ToList();
                    var returns = db.tb_return_mainstock_detail.Where(s => string.Compare(s.main_stock_detail_ref, returnId) == 0).ToList();
                    foreach(TransferFromMainStockItemViewModel transfer in transfers)
                    {
                        TransferFromMainStockItemViewModel model = new TransferFromMainStockItemViewModel();
                        model = transfer;
                        var rreturn = returns.Where(s => string.Compare(s.main_stock_detail_item, transfer.st_item_id) == 0).FirstOrDefault();
                        if (rreturn != null)
                            model.remain_quantity = model.remain_quantity + rreturn.quantity;
                        models.Add(model);
                    }
                }
                
                return models;
            }
        }
        public List<ReturnMainStockViewModel> GetReturntoWorkshopListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ReturnMainStockViewModel> models = new List<ReturnMainStockViewModel>();
                List<ReturnMainStockViewModel> objs = new List<ReturnMainStockViewModel>();
                if (User.IsInRole(Role.SystemAdmin))
                {
                    models = (from rws in db.tb_return_main_stock
                              join pro in db.tb_project on rws.return_ref_id equals pro.project_id
                              join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                              where rws.status == true
                              select new ReturnMainStockViewModel()
                              {
                                  return_main_stock_id=rws.return_main_stock_id,
                                  return_main_stock_no=rws.return_main_stock_no,
                                  return_main_stock_status=rws.return_main_stock_status,
                                  return_ref_id=rws.return_ref_id,
                                  create_by=rws.create_by,
                                  create_date=rws.update_date,
                                  project_fullname=pro.project_full_name,
                                  warehouse_id=wh.warehouse_id,
                                  warehouse_name=wh.warehouse_name,
                              }).ToList();
                    return models;
                }else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        models = (from rws in db.tb_return_main_stock
                                  join pro in db.tb_project on rws.return_ref_id equals pro.project_id
                                  join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                  where rws.status == true && (string.Compare(rws.create_by,userid)==0 || string.Compare(rws.update_by,userid)==0)
                                  select new ReturnMainStockViewModel()
                                  {
                                      return_main_stock_id = rws.return_main_stock_id,
                                      return_main_stock_no = rws.return_main_stock_no,
                                      return_main_stock_status = rws.return_main_stock_status,
                                      return_ref_id = rws.return_ref_id,
                                      create_by = rws.create_by,
                                      create_date = rws.update_date,
                                      project_fullname = pro.project_full_name,
                                      warehouse_id = wh.warehouse_id,
                                      warehouse_name = wh.warehouse_name,
                                  }).ToList();
                    }
                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        objs = (from rws in db.tb_return_main_stock
                                  join pro in db.tb_project on rws.return_ref_id equals pro.project_id
                                  join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                  join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                  where rws.status == true && (string.Compare(rws.approved_by, userid) == 0 || (string.Compare(rws.return_main_stock_status,Status.Pending)==0 && string.Compare(qaqc.qaqc_id,userid)==0))
                                  select new ReturnMainStockViewModel()
                                  {
                                      return_main_stock_id = rws.return_main_stock_id,
                                      return_main_stock_no = rws.return_main_stock_no,
                                      return_main_stock_status = rws.return_main_stock_status,
                                      return_ref_id = rws.return_ref_id,
                                      create_by = rws.create_by,
                                      create_date = rws.update_date,
                                      project_fullname = pro.project_full_name,
                                      warehouse_id = wh.warehouse_id,
                                      warehouse_name = wh.warehouse_name,
                                  }).ToList();
                        foreach(var obj in objs)
                        {
                            var isexist = models.Where(w => string.Compare(w.return_main_stock_id, obj.return_main_stock_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.ProjectManager))
                    {
                        objs = (from rws in db.tb_return_main_stock
                                join pro in db.tb_project on rws.return_ref_id equals pro.project_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                join pm in db.tb_project_pm on pro.project_id equals pm.project_id
                                where rws.status == true && (string.Compare(rws.checked_by, userid) == 0 || (string.Compare(rws.return_main_stock_status, Status.Approved) == 0 && string.Compare(pm.project_manager_id, userid) == 0))
                                select new ReturnMainStockViewModel()
                                {
                                    return_main_stock_id = rws.return_main_stock_id,
                                    return_main_stock_no = rws.return_main_stock_no,
                                    return_main_stock_status = rws.return_main_stock_status,
                                    return_ref_id = rws.return_ref_id,
                                    create_by = rws.create_by,
                                    create_date = rws.update_date,
                                    project_fullname = pro.project_full_name,
                                    warehouse_id = wh.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isexist = models.Where(w => string.Compare(w.return_main_stock_id, obj.return_main_stock_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.WorkshopController))
                    {
                        objs = (from rws in db.tb_return_main_stock
                                join pro in db.tb_project on rws.return_ref_id equals pro.project_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                where rws.status == true && (string.Compare(rws.completed_by, userid) == 0 || string.Compare(rws.return_main_stock_status, Status.Checked) == 0)
                                select new ReturnMainStockViewModel()
                                {
                                    return_main_stock_id = rws.return_main_stock_id,
                                    return_main_stock_no = rws.return_main_stock_no,
                                    return_main_stock_status = rws.return_main_stock_status,
                                    return_ref_id = rws.return_ref_id,
                                    create_by = rws.create_by,
                                    create_date = rws.update_date,
                                    project_fullname = pro.project_full_name,
                                    warehouse_id = wh.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isexist = models.Where(w => string.Compare(w.return_main_stock_id, obj.return_main_stock_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                models.Add(obj);
                        }
                    }
                }
                
                return models;
            }
        }
        public ReturnMainStockViewModel GetReturntoWorkshopItemDetail(string id)
        {
            ReturnMainStockViewModel model = new ReturnMainStockViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                model = (from rtn in db.tb_return_main_stock
                         join pro in db.tb_project on rtn.return_ref_id equals pro.project_id
                         join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                             where string.Compare(rtn.return_main_stock_id,id)==0
                             select new ReturnMainStockViewModel()
                             {
                                 return_main_stock_id = rtn.return_main_stock_id,
                                 return_main_stock_no = rtn.return_main_stock_no,
                                 create_date = rtn.update_date,
                                 return_main_stock_status = rtn.return_main_stock_status,
                                 create_by=rtn.create_by,
                                 return_ref_id=rtn.return_ref_id,
                                 return_type=rtn.return_type,
                                project_fullname=pro.project_full_name,
                                warehouse_id=wh.warehouse_id,
                                warehouse_name=wh.warehouse_name,
                                last_approved_by=rtn.last_approved_by,
                             }).FirstOrDefault();

                var inventories = (from inv in db.tb_return_mainstock_detail
                                   join pro in db.tb_product on inv.main_stock_detail_item equals pro.product_id
                                   join iunit in db.tb_unit on inv.unit equals iunit.Id
                                   where string.Compare(inv.main_stock_detail_ref,model.return_main_stock_id)==0
                                   select new InventoryViewModel()
                                   {
                                       inventory_id = inv.main_stock_detail_id,
                                       product_id = inv.main_stock_detail_item,
                                       itemCode = pro.product_code,
                                       itemName = pro.product_name,
                                       itemUnit = pro.product_unit,
                                       in_quantity = 0,
                                       out_quantity = inv.quantity,
                                       unit = inv.unit,
                                       unitName=iunit.Name,
                                       //total_quantity = (from invv in db.tb_inventory orderby invv.inventory_date descending where invv.product_id == inv.main_stock_detail_item && invv.warehouse_id == inv.main_stock_warhouse_id select invv.total_quantity).FirstOrDefault(),
                                       item_status = inv.item_status,
                                       remark = inv.remark,
                                   }).ToList();
                model.inventoryDetails = inventories;
                model.itemTransfers = this.GetAvialableItemToTransfer(model.return_ref_id, model.return_main_stock_id);
                //sTransfer.rejects = Class.CommonClass.GetRejectByRequest(id);
                return model;
            }
        }
        public List<Models.InventoryViewModel> GetAllItembyWarehouse(string id)
        {
            List<Models.InventoryViewModel> models = new List<Models.InventoryViewModel>();
            List<Models.InventoryViewModel> inventories = new List<Models.InventoryViewModel>();
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<Models.InventoryViewModel> orginalItems = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items1 = new List<Models.InventoryViewModel>();
                items1 = db.tb_inventory.OrderBy(x => x.product_id).ThenByDescending(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, id) == 0).Select(x => new Models.InventoryViewModel()
                {
                    inventory_date = x.inventory_date,
                    product_id = x.product_id,
                    warehouse_id = x.warehouse_id,
                    total_quantity = x.total_quantity,
                }).ToList();
                var dup = items1.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dup.Any())
                {
                    foreach (var ditem in dup)
                    {
                        Models.InventoryViewModel item = new Models.InventoryViewModel();
                        item = items1.Where(x => string.Compare(x.product_id, ditem) == 0).First();
                        items.Add(item);
                    }
                    foreach (var item in items1)
                    {
                        var checkItem = dup.Where(x => string.Compare(x, item.product_id) == 0).ToList();
                        if (checkItem.Any()) { }
                        else
                            items.Add(new Models.InventoryViewModel() { inventory_date = item.inventory_date, product_id = item.product_id, warehouse_id = item.warehouse_id, total_quantity = item.total_quantity });
                    }
                }
                else
                    items = items1;

                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        if (item != null)
                        {
                            if (item.product_id != null)
                            {
                                Models.ProductViewModel product = Class.CommonClass.GetProductDetail(item.product_id);
                                if (product != null)
                                {
                                    if(item.total_quantity>0)
                                    orginalItems.Add(new Models.InventoryViewModel()
                                    {
                                        inventory_date = item.inventory_date,
                                        product_id = item.product_id,
                                        total_quantity = item.total_quantity,
                                        warehouse_id = item.warehouse_id,
                                        itemCode = Class.CommonClass.GetProductDetail(item.product_id).product_code,
                                        itemName = Class.CommonClass.GetProductDetail(item.product_id).product_name,
                                        itemUnit = Class.CommonClass.GetProductDetail(item.product_id).product_unit,
                                        itemUnitName = Class.CommonClass.GetProductDetail(item.product_id).unit_name,
                                    });
                                }
                            }

                        }

                    }
                }

                var duplicationItems = orginalItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (duplicationItems.Any())
                {
                    foreach (var dItem in duplicationItems)
                    {
                        decimal totalQuantity = 0;
                        var itemss = orginalItems.Where(x => string.Compare(x.product_id, dItem) == 0).Select(x => x.total_quantity).ToList();
                        foreach (var quantiy in itemss)
                            totalQuantity = totalQuantity + Convert.ToDecimal(quantiy);
                        foreach (var item in orginalItems)
                        {
                            Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                            iitem.inventory_date = item.inventory_date;
                            iitem.product_id = item.product_id;
                            iitem.itemCode = Class.CommonClass.GetProductDetail(iitem.product_id).product_code;
                            iitem.itemName = Class.CommonClass.GetProductDetail(iitem.product_id).product_name;
                            iitem.itemUnit = Class.CommonClass.GetProductDetail(iitem.product_id).product_unit;
                            iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                            iitem.warehouse_id = item.warehouse_id;
                            if (string.Compare(item.product_id, dItem) == 0)
                            {
                                iitem.total_quantity = totalQuantity;
                            }
                            else
                            {
                                iitem.total_quantity = item.total_quantity;
                            }
                            models.Add(iitem);
                        }
                    }
                }
                else
                    models = orginalItems;

                inventories = models.OrderBy(x => x.itemCode).ToList();
            }
            return inventories;
        }
        #endregion
    }
}