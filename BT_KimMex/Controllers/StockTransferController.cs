using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using BT_KimMex.Class;
using Microsoft.Ajax.Utilities;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class StockTransferController : Controller
    {
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        // GET: StockTransfer
        public ActionResult Index()
        {
            //string userid = User.Identity.GetUserId().ToString();
            //kim_mexEntities db = new kim_mexEntities();
            //List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
            //if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.OperationDirector) ||User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.ProjectManager)||User.IsInRole(Role.SiteSupervisor)||User.IsInRole(Role.SiteAdmin)|| User.IsInRole(Role.ProjectManager))
            //{
            //    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
            //                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                             join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                             //join site in db.tb_site on pro.site_id equals site.site_id
            //                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
            //                             join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh from fwh in ffwh.DefaultIfEmpty()
            //                             where st.status == true
            //                             select new { st, ir,pro,wh,fwh }).ToList();
            //    if (allStockTransfers.Any())
            //    {
            //        foreach (var item in allStockTransfers)
            //        {

            //            StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //            stocktransfer.item_request_id = item.st.item_request_id;
            //            stocktransfer.item_request_no = item.ir.ir_no;
            //            stocktransfer.created_date = item.st.created_date;
            //            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //            stocktransfer.create_by = item.st.created_by;
            //            stocktransfer.project_id = item.pro.project_id;
            //            stocktransfer.project_fullname = item.pro.project_full_name;
            //            if (item.wh != null)
            //            {
            //                stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                stocktransfer.warehouse_name = item.wh.warehouse_name;
            //            }
            //            stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //            if(item.fwh!=null)
            //            stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //            stocktransfer.sr_status = item.st.sr_status;
            //            stocktransfer.mr_status = item.st.mr_status;
            //            stocktransfer.received_status = item.st.received_status;
            //            stockTransfers.Add(stocktransfer);
            //        }
            //    }
            //}else{

            //    if (User.IsInRole(Role.SiteStockKeeper))
            //    {
            //        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
            //                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
            //                                 from wh in pwh.DefaultIfEmpty()
            //                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
            //                                 from fwh in ffwh.DefaultIfEmpty()
            //                                 join stockkeeper in db.tb_stock_keeper_warehouse on fwh.warehouse_id equals stockkeeper.warehouse_id
            //                                 where st.status == true && string.Compare(stockkeeper.stock_keeper,userid)==0
            //                                 select new { st, ir, pro, wh, fwh }).ToList();
            //        if (allStockTransfers.Any())
            //        {
            //            foreach (var item in allStockTransfers)
            //            {
            //                var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
            //                if (!isexist)
            //                {
            //                    StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //                    stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //                    stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //                    stocktransfer.item_request_id = item.st.item_request_id;
            //                    stocktransfer.item_request_no = item.ir.ir_no;
            //                    stocktransfer.created_date = item.st.created_date;
            //                    stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //                    stocktransfer.create_by = item.st.created_by;
            //                    stocktransfer.project_id = item.pro.project_id;
            //                    stocktransfer.project_fullname = item.pro.project_full_name;
            //                    if (item.wh != null)
            //                    {
            //                        stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                        stocktransfer.warehouse_name = item.wh.warehouse_name;
            //                    }
            //                    stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //                    if (item.fwh != null)
            //                        stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //                    stocktransfer.sr_status = item.st.sr_status;
            //                    stocktransfer.mr_status = item.st.mr_status;
            //                    stocktransfer.received_status = item.st.received_status;
            //                    stockTransfers.Add(stocktransfer);
            //                }

            //            }
            //        }
            //    }

            //    if (User.IsInRole(Role.Purchaser))
            //    {
            //        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
            //                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                                 //join site in db.tb_site on pro.site_id equals site.site_id
            //                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
            //                                 from wh in pwh.DefaultIfEmpty()
            //                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
            //                                 from fwh in ffwh.DefaultIfEmpty()
            //                                 where st.status == true && (string.Compare(st.created_by,userid)==0 || string.Compare(st.updated_by,userid)==0)
            //                                 select new { st, ir, pro, wh,fwh }).ToList();
            //        foreach (var item in allStockTransfers)
            //        {
            //            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
            //            if (!isexist)
            //            {
            //                StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //                stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //                stocktransfer.item_request_id = item.st.item_request_id;
            //                stocktransfer.item_request_no = item.ir.ir_no;
            //                stocktransfer.created_date = item.st.created_date;
            //                stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //                stocktransfer.create_by = item.st.created_by;
            //                stocktransfer.project_id = item.pro.project_id;
            //                stocktransfer.project_fullname = item.pro.project_full_name;
            //                if (item.wh != null)
            //                {
            //                    stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                    stocktransfer.warehouse_name = item.wh.warehouse_name;
            //                }
            //                stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //                if (item.fwh != null)
            //                    stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //                stocktransfer.sr_status = item.st.sr_status;
            //                stocktransfer.mr_status = item.st.mr_status;
            //                stocktransfer.received_status = item.st.received_status;
            //                stockTransfers.Add(stocktransfer);
            //            }

            //        }
            //    }
            //    if (User.IsInRole(Role.QAQCOfficer))
            //    {
            //        //Pending and Feedback Request
            //        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
            //                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                                 //join site in db.tb_site on pro.site_id equals site.site_id
            //                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
            //                                 from wh in pwh.DefaultIfEmpty()
            //                                 join qaqc in db.tb_warehouse_qaqc on st.from_warehouse_id equals qaqc.warehouse_id
            //                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
            //                                 from fwh in ffwh.DefaultIfEmpty()
            //                                 where st.status == true && string.Compare(qaqc.qaqc_id, userid) == 0 && (string.Compare(st.stock_transfer_status,Status.Pending)==0 || string.Compare(st.stock_transfer_status,Status.Feedbacked)==0)
            //                                 select new { st, ir, pro, wh,fwh }).ToList();
            //        foreach (var item in allStockTransfers)
            //        {
            //            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
            //            if (!isexist)
            //            {


            //                StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //                stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //                stocktransfer.item_request_id = item.st.item_request_id;
            //                stocktransfer.item_request_no = item.ir.ir_no;
            //                stocktransfer.created_date = item.st.created_date;
            //                stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //                stocktransfer.create_by = item.st.created_by;
            //                stocktransfer.project_id = item.pro.project_id;
            //                stocktransfer.project_fullname = item.pro.project_full_name;
            //                if (item.wh != null)
            //                {
            //                    stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                    stocktransfer.warehouse_name = item.wh.warehouse_name;
            //                }
            //                stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //                if (item.fwh != null)
            //                    stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //                stocktransfer.sr_status = item.st.sr_status;
            //                stocktransfer.mr_status = item.st.mr_status;
            //                stocktransfer.received_status = item.st.received_status;
            //                stockTransfers.Add(stocktransfer);
            //            }
            //        }
            //        //Approved History
            //        var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
            //                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                                  //join site in db.tb_site on pro.site_id equals site.site_id
            //                                  join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
            //                                  from wh in pwh.DefaultIfEmpty()
            //                                      //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
            //                                  join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
            //                                  from fwh in ffwh.DefaultIfEmpty()
            //                                  where st.status == true && string.Compare(st.approved_by, userid) == 0
            //                                 select new { st, ir, pro, wh,fwh }).ToList();
            //        foreach (var item in allStockTransfers1)
            //        {
            //            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
            //            if (!isexist)
            //            {
            //                StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //                stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //                stocktransfer.item_request_id = item.st.item_request_id;
            //                stocktransfer.item_request_no = item.ir.ir_no;
            //                stocktransfer.created_date = item.st.created_date;
            //                stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //                stocktransfer.create_by = item.st.created_by;
            //                stocktransfer.project_id = item.pro.project_id;
            //                stocktransfer.project_fullname = item.pro.project_full_name;
            //                if (item.wh != null)
            //                {
            //                    stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                    stocktransfer.warehouse_name = item.wh.warehouse_name;
            //                }
            //                stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //                if (item.fwh != null)
            //                    stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //                stocktransfer.sr_status = item.st.sr_status;
            //                stocktransfer.mr_status = item.st.mr_status;
            //                stocktransfer.received_status = item.st.received_status;
            //                stockTransfers.Add(stocktransfer);
            //            }
            //        }
            //    }
            //    if (User.IsInRole(Role.SiteManager))
            //    {
            //        //Pending and Feedback Request
            //        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
            //                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                                 //join site in db.tb_site on pro.site_id equals site.site_id
            //                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
            //                                 from wh in pwh.DefaultIfEmpty()

            //                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
            //                                 from fwh in ffwh.DefaultIfEmpty()
            //                                 join fsite in db.tb_site on fwh.warehouse_site_id equals fsite.site_id
            //                                 join fpro in db.tb_project on fsite.site_id equals fpro.site_id
            //                                 join sitem in db.tb_site_manager_project on fpro.project_id equals sitem.project_id


            //                                 where st.status == true && string.Compare(sitem.site_manager, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Approved) == 0)
            //                                 select new { st, ir, pro, wh,fwh }).ToList();
            //        foreach (var item in allStockTransfers)
            //        {
            //            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
            //            if (!isexist)
            //            {
            //                StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //                stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //                stocktransfer.item_request_id = item.st.item_request_id;
            //                stocktransfer.item_request_no = item.ir.ir_no;
            //                stocktransfer.created_date = item.st.created_date;
            //                stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //                stocktransfer.create_by = item.st.created_by;
            //                stocktransfer.project_id = item.pro.project_id;
            //                stocktransfer.project_fullname = item.pro.project_full_name;
            //                if (item.wh != null)
            //                {
            //                    stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                    stocktransfer.warehouse_name = item.wh.warehouse_name;
            //                }
            //                stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //                if (item.fwh != null)
            //                    stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //                stocktransfer.sr_status = item.st.sr_status;
            //                stocktransfer.mr_status = item.st.mr_status;
            //                stocktransfer.received_status = item.st.received_status;
            //                stockTransfers.Add(stocktransfer);
            //            }
            //        }


            //        //Approved History
            //        var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
            //                                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
            //                                  join pro in db.tb_project on ir.ir_project_id equals pro.project_id
            //                                  //join site in db.tb_site on pro.site_id equals site.site_id
            //                                  join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
            //                                  from wh in pwh.DefaultIfEmpty()
            //                                  join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
            //                                  from fwh in ffwh.DefaultIfEmpty()
            //                                  //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
            //                                  where st.status == true && string.Compare(st.checked_by, userid) == 0
            //                                  select new { st, ir, pro, wh,fwh }).ToList();
            //        foreach (var item in allStockTransfers1)
            //        {
            //            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
            //            if (!isexist)
            //            {
            //                StockTransferViewModel stocktransfer = new StockTransferViewModel();
            //                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
            //                stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
            //                stocktransfer.item_request_id = item.st.item_request_id;
            //                stocktransfer.item_request_no = item.ir.ir_no;
            //                stocktransfer.created_date = item.st.created_date;
            //                stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
            //                stocktransfer.create_by = item.st.created_by;
            //                stocktransfer.project_id = item.pro.project_id;
            //                stocktransfer.project_fullname = item.pro.project_full_name;
            //                if (item.wh != null)
            //                {
            //                    stocktransfer.warehouse_id = item.wh.warehouse_id;
            //                    stocktransfer.warehouse_name = item.wh.warehouse_name;
            //                }
            //                stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
            //                if (item.fwh != null)
            //                    stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
            //                stocktransfer.sr_status = item.st.sr_status;
            //                stocktransfer.mr_status = item.st.mr_status;
            //                stocktransfer.received_status = item.st.received_status;
            //                stockTransfers.Add(stocktransfer);
            //            }
            //        }
            //    }
            //}

            //stockTransfers = stockTransfers.OrderByDescending(s => s.created_date).ToList();
            //return View(stockTransfers);
            return View();
        }
        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Create(string id=null,string warehouse=null)
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            itemRequests = this.GetItemRequestDropdownList();
            
            List<ItemRequestViewModel> itemRequestss = new List<ItemRequestViewModel>();
            //itemRequestss = Class.CommonClass.GetAvailableItemRequestList();
            itemRequestss = Class.ItemRequest.GetAllItemRequestDropdownList();

            ViewBag.STDate = Class.CommonClass.ToLocalTime(DateTime.Now).ToString("dd/MM/yyyy");
            //ViewBag.STID = this.GetStockTransferNo();
            ViewBag.STID = Class.CommonClass.GenerateProcessNumber("ST");
            ViewBag.IRID = new SelectList(itemRequestss, "ir_id", "ir_no");
            StockTransferViewModel model = new StockTransferViewModel();
            model.item_request_id = id;
            model.warehouse_id = warehouse;
            return View(model);
        }
        public ActionResult CreateStockTransfer(StockTransferViewModel model,List<InventoryViewModel> inventories)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                int countInvalid = 0;
                string message = string.Empty;
                //List<InventoryViewModel> remainRequestItems = new List<InventoryViewModel>();
                //remainRequestItems = this.getRemainItems(model.item_request_id);
                List<Models.ItemRequestDetail2ViewModel> remainRequestItems = new List<ItemRequestDetail2ViewModel>();
                remainRequestItems = Class.ItemRequest.GetAllAvailableItem(model.item_request_id);
                
                #region check reamin transfer balance
                foreach (Models.ItemRequestDetail2ViewModel item in remainRequestItems)
                {
                    foreach(InventoryViewModel transfer in inventories)
                    {
                        if(item.ir_item_id==transfer.product_id && item.remain_qty < Class.CommonClass.ConvertMultipleUnit(transfer.product_id, transfer.unit, Convert.ToDecimal(transfer.out_quantity)))
                        {
                            countInvalid++;
                            if (item.remain_qty < 0)
                                message = message + string.Format("{0} is completed to tranfer.\n", item.product_name);
                            else
                                message = message + string.Format("{0} remain quantity {1} to transfer.\n", item.product_name,string.Format("{0:G29}",Double.Parse(item.remain_qty.ToString())));
                            break;
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                #endregion
                #region check with total transfer quantity
                decimal requestQty = 0, totalTransferQty = 0;
                var dupId = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                foreach (var Id in dupId)
                {
                    totalTransferQty = 0;
                    //requestQty =Convert.ToDecimal(inventories.Where(x => x.product_id == Id).Select(x => x.total_quantity).FirstOrDefault());
                    requestQty = Convert.ToDecimal(remainRequestItems.Where(m => m.ir_item_id == Id).Select(m => m.remain_qty).FirstOrDefault());
                    //var transferQty =inventories.Where(x => x.product_id == Id).Select(x => x.out_quantity).ToList();
                    var transferQty = inventories.Where(x => x.product_id == Id).ToList();
                    foreach (var tQty in transferQty)
                    {
                        //totalTransferQty = totalTransferQty + Convert.ToDecimal(tQty);
                        totalTransferQty = totalTransferQty + Class.CommonClass.ConvertMultipleUnit(tQty.product_id, tQty.unit, Convert.ToDecimal(tQty.out_quantity));
                    }
                    if (totalTransferQty > requestQty)
                    {
                        return Json(new { result = "fail", message = "Total transfer quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                int countReturn = 0;
                tb_stock_transfer_voucher sTransfer = new tb_stock_transfer_voucher();
                sTransfer.stock_transfer_id = Guid.NewGuid().ToString();
                sTransfer.stock_transfer_no = Class.CommonClass.GenerateProcessNumber("ST");
                sTransfer.item_request_id = model.item_request_id;
                sTransfer.stock_transfer_status = Status.Pending;
                sTransfer.from_warehouse_id = model.from_warehouse_id;
                sTransfer.status = true;
                sTransfer.created_by = User.Identity.GetUserId();
                sTransfer.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                sTransfer.is_return_complete = false;
                db.tb_stock_transfer_voucher.Add(sTransfer);
                db.SaveChanges();
                //Generate invoice number by warehouse
                //Class.StockTransfer.GenerateWarehouseInvoiceNumber(sTransfer.stock_transfer_id, inventories);
                Class.CommonClass.AutoGenerateStockInvoiceNumber(sTransfer.stock_transfer_id, inventories);
                foreach (var inv in inventories)
                {
                    #region updated when have workflow
                    //decimal totalQty =Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.product_id && m.warehouse_id == inv.warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    //tb_inventory inventory = new tb_inventory();
                    //inventory.inventory_id = Guid.NewGuid().ToString();
                    //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //inventory.ref_id = sTransfer.stock_transfer_id;
                    //inventory.inventory_status_id = "6";
                    //inventory.warehouse_id = inv.warehouse_id;
                    //inventory.product_id = inv.product_id;
                    //inventory.out_quantity = inv.out_quantity;
                    //inventory.in_quantity = 0;
                    //inventory.total_quantity = totalQty - inventory.out_quantity;
                    //db.tb_inventory.Add(inventory);
                    //db.SaveChanges();
                    #endregion
                    if (inv.status == "true")
                    {
                        inv.status = "true";
                    }
                    else
                    {
                        if (inv.status == null)
                        {
                            inv.status = "false";
                        }
                    }
                   
                    tb_stock_transfer_detail stDetail = new tb_stock_transfer_detail();
                    stDetail.st_detail_id = Guid.NewGuid().ToString();
                    stDetail.st_ref_id = sTransfer.stock_transfer_id;
                    stDetail.st_item_id = inv.product_id;
                    stDetail.status = Convert.ToBoolean(inv.status);
                    stDetail.st_warehouse_id = inv.warehouse_id;
                    stDetail.quantity = inv.out_quantity;
                    stDetail.unit = inv.unit;                
                    stDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now):inv.inventory_date;
                    stDetail.invoice_number = Class.CommonClass.GetInvoiceNumber(sTransfer.stock_transfer_id,stDetail.st_warehouse_id,stDetail.invoice_date); //string.IsNullOrEmpty(inv.invoice_number)?Class.StockTransfer.GenerateInvoiceNumber(stDetail.st_warehouse_id,Convert.ToDateTime(stDetail.invoice_date)):inv.invoice_number;
                    stDetail.remain_quantity = Class.CommonClass.ConvertMultipleUnit(stDetail.st_item_id, stDetail.unit, Convert.ToDecimal(inv.out_quantity));
                    stDetail.return_remain_quantity = Class.CommonClass.ConvertMultipleUnit(stDetail.st_item_id, stDetail.unit, Convert.ToDecimal(inv.out_quantity));
                    stDetail.item_status =Status.Pending;
                    db.tb_stock_transfer_detail.Add(stDetail);
                    db.SaveChanges();
                    if (Convert.ToBoolean(stDetail.status))
                        countReturn++;
                }

                Class.StockTransfer.RollBackItemQuantitybyStockTransfer(model.item_request_id, sTransfer.stock_transfer_id, false);
                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), sTransfer.stock_transfer_id, sTransfer.stock_transfer_status, sTransfer.created_by, sTransfer.created_date, string.Empty);

                if (countReturn ==0)
                {
                    tb_item_request materialRequest = db.tb_item_request.Find(sTransfer.item_request_id);
                    materialRequest.st_status = ShowStatus.STCreated;
                    db.SaveChanges();
                }

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Detail(string id)
        {
            StockTransferViewModel sTransfer = new StockTransferViewModel();
            sTransfer = this.GetStockTransferDetail(id);
            return View(sTransfer);
        }
        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Edit(string id)
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            StockTransferViewModel sTransfer = new StockTransferViewModel();
            sTransfer = this.GetStockTransferDetail(id);
            itemRequests = this.GetItemRequestDropdownList();
            List<ItemRequestViewModel> itemRequestss = new List<ItemRequestViewModel>();
            //itemRequestss = Class.CommonClass.GetAvailableItemRequestList();
            //ViewBag.IRID = new SelectList(itemRequestss, "ir_id", "ir_no");
            itemRequestss = Class.ItemRequest.GetAllItemRequestDropdownList(sTransfer.item_request_id);
            ViewBag.IRID = itemRequestss;
            return View(sTransfer);
        }
        public ActionResult EditStockTransfer(StockTransferViewModel model, List<InventoryViewModel> inventories)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countInvalid = 0;
                string message = string.Empty;
                //List<InventoryViewModel> remainRequestItems = new List<InventoryViewModel>();
                //remainRequestItems = this.getRemainItems(model.item_request_id);
                List<Models.ItemRequestDetail2ViewModel> remainRequestItems = new List<ItemRequestDetail2ViewModel>();
                remainRequestItems = Class.ItemRequest.GetAllAvailableItembyStockTransfer(model.item_request_id,model.stock_transfer_id);
                #region check reamin transfer balance
                foreach (ItemRequestDetail2ViewModel item in remainRequestItems)
                {
                    foreach (InventoryViewModel transfer in inventories)
                    {
                        if (item.ir_item_id == transfer.product_id && item.remain_qty < Class.CommonClass.ConvertMultipleUnit(transfer.product_id, transfer.unit, Convert.ToDecimal(transfer.out_quantity)))
                        {
                            countInvalid++;
                            if (item.remain_qty < 0)
                                message = message + string.Format("{0} is completed to tranfer.\n", item.product_name);
                            else
                                message = message + string.Format("{0} is remain quantity {1} to transfer.", item.product_name, string.Format("{0:G29}", Double.Parse(item.remain_qty.ToString())));
                            break;
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                #endregion
                #region check with total transfer quantity
                decimal requestQty = 0, totalTransferQty = 0;
                var dupId = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                foreach (var Id in dupId)
                {
                    totalTransferQty = 0;
                    //requestQty =Convert.ToDecimal(inventories.Where(x => x.product_id == Id).Select(x => x.total_quantity).FirstOrDefault());
                    requestQty = Convert.ToDecimal(remainRequestItems.Where(m => m.ir_item_id == Id).Select(m => m.remain_qty).FirstOrDefault());
                    //var transferQty =inventories.Where(x => x.product_id == Id).Select(x => x.out_quantity).ToList();
                    var transferQty = inventories.Where(x => x.product_id == Id).ToList();
                    foreach (var tQty in transferQty)
                    {
                        //totalTransferQty = totalTransferQty + Convert.ToDecimal(tQty);
                        totalTransferQty = totalTransferQty + Class.CommonClass.ConvertMultipleUnit(tQty.product_id, tQty.unit, Convert.ToDecimal(tQty.out_quantity));
                    }
                    if (totalTransferQty > requestQty)
                    {
                        return Json(new { result = "fail", message = "Total transfer quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                #region update inventory
                if (!string.IsNullOrEmpty(model.stock_transfer_id))
                {
                    string stockTransferID = model.stock_transfer_id;
                    //this.UpdateInventory(stockTransferID);
                    //this.DeleteInventory(stockTransferID);
                    Class.StockTransfer.RollBackItemQuantitybyStockTransfer(model.item_request_id, model.stock_transfer_id, true);
                    this.DeleteStockTransferDetail(stockTransferID);
                    Class.CommonClass.DeleteInvoiceNumber(stockTransferID);
                    Class.CommonClass.AutoGenerateStockInvoiceNumber(stockTransferID, inventories);
                }
                #endregion
                

                tb_stock_transfer_voucher sTransfer = db.tb_stock_transfer_voucher.Where(m => m.stock_transfer_id == model.stock_transfer_id).FirstOrDefault();

                
                sTransfer.stock_transfer_no = model.stock_transfer_no;
                sTransfer.item_request_id = model.item_request_id;
                sTransfer.stock_transfer_status = "Pending";
                sTransfer.status = true;
                sTransfer.updated_by = User.Identity.GetUserId();
                sTransfer.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                foreach (var inv in inventories)
                {
                    //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.product_id && m.warehouse_id == inv.warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    //tb_inventory inventory = new tb_inventory();
                    //inventory.inventory_id = Guid.NewGuid().ToString();
                    //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //inventory.ref_id = sTransfer.stock_transfer_id;
                    //inventory.inventory_status_id = "6";
                    //inventory.warehouse_id = inv.warehouse_id;
                    //inventory.product_id = inv.product_id;
                    //inventory.out_quantity = inv.out_quantity;
                    //inventory.in_quantity = 0;
                    //inventory.total_quantity = totalQty - inventory.out_quantity;
                    //db.tb_inventory.Add(inventory);
                    //db.SaveChanges();

                    tb_stock_transfer_detail stDetail = new tb_stock_transfer_detail();
                    if (inv.status == "true")
                    {
                        inv.status = "true";
                    }
                    else
                    {
                        inv.status = "false";
                    }

                    stDetail.st_detail_id = Guid.NewGuid().ToString();
                    stDetail.st_ref_id = sTransfer.stock_transfer_id;
                    stDetail.st_item_id = inv.product_id;
                    stDetail.st_warehouse_id = inv.warehouse_id;
                    stDetail.quantity = inv.out_quantity;
                    stDetail.unit = inv.unit;
                    stDetail.status = Convert.ToBoolean(inv.status);
                    stDetail.invoice_date = inv.invoice_date != null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.inventory_date;
                    stDetail.invoice_number = Class.CommonClass.GetInvoiceNumber(sTransfer.stock_transfer_id,stDetail.st_warehouse_id,stDetail.invoice_date); //string.IsNullOrEmpty(inv.invoice_number)?Class.StockTransfer.GenerateInvoiceNumber(stDetail.st_warehouse_id,Convert.ToDateTime(stDetail.invoice_date)):inv.invoice_number;
                    stDetail.item_status = "pending";
                    db.tb_stock_transfer_detail.Add(stDetail);
                    db.SaveChanges();
                }
                Class.StockTransfer.RollBackItemQuantitybyStockTransfer(sTransfer.item_request_id, sTransfer.stock_transfer_id, false);
            }
            return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(id);
                    stockTransfer.status = false;
                    stockTransfer.updated_by = User.Identity.Name;
                    stockTransfer.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    //this.UpdateInventory(id);
                    //this.DeleteInventory(id);
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult Approve(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int countInvalid = 0;
                    string message = string.Empty;
                    List<InventoryViewModel> remainRequestItems = new List<InventoryViewModel>();
                    //List<InventoryViewModel> inventories = new List<InventoryViewModel>();
                    remainRequestItems = this.getRemainItems(id);
                    var inventories= db.tb_stock_transfer_detail.Where(m => m.st_ref_id == id).ToList();
                    #region check reamin transfer balance
                    foreach (InventoryViewModel item in remainRequestItems)
                    {
                        foreach (var transfer in inventories)
                        {
                            if (item.product_id == transfer.st_item_id && item.total_quantity < Class.CommonClass.ConvertMultipleUnit(transfer.st_item_id, transfer.unit, Convert.ToDecimal(transfer.quantity)))
                            {
                                countInvalid++;
                                if (item.total_quantity < 0)
                                    message = message + string.Format("{0} is completed to tranfer.\n", item.itemName);
                                else
                                    message = message + string.Format("{0} is remain quantity {1} to transfer.", item.itemName, string.Format("{0:G29}", Double.Parse(item.total_quantity.ToString())));
                                break;
                            }
                        }
                    }
                    if (countInvalid > 0)
                        return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                    #endregion
                    
                    tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(id);
                    stockTransfer.stock_transfer_status = Status.Completed;
                    stockTransfer.checked_by = User.Identity.GetUserId();
                    stockTransfer.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    InsertItemInventory(id);
                    string irId = stockTransfer.item_request_id;
                    if (Convert.ToBoolean(stockTransfer.is_return)==false)
                    {
                        tb_item_request materialRequest = db.tb_item_request.Find(irId);
                        materialRequest.st_status = ShowStatus.GRNPending;
                        db.SaveChanges();
                    }
                    updateItemRequestStatus(stockTransfer.item_request_id, stockTransfer.stock_transfer_id);
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), stockTransfer.stock_transfer_id, stockTransfer.stock_transfer_status, stockTransfer.checked_by, stockTransfer.checked_date, string.Empty);
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult Reject(string id,string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(id);
                    stockTransfer.stock_transfer_status = Status.Rejected;
                    stockTransfer.checked_by = User.Identity.GetUserId();
                    stockTransfer.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();

                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = stockTransfer.stock_transfer_id;
                    reject.ref_type = "Stock Transfer";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.GetUserId();
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();

                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), stockTransfer.stock_transfer_id, stockTransfer.stock_transfer_status, stockTransfer.checked_by, stockTransfer.checked_date, comment);
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            StockTransferViewModel model = new StockTransferViewModel();
            model = this.GetStockTransferDetail(id);
            return View(model);
        }
        [HttpPost]
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult ApproveFeedback(string id, List<Models.InventoryViewModel> models)
        {
            try
            {
                int countReturn = 0;
                int countItemApproved = 0;
                Entities.kim_mexEntities db = new kim_mexEntities();
                foreach(InventoryViewModel item in models)
                {
                    string idd = item.inventory_id;
                    Entities.tb_stock_transfer_detail transferDetail = db.tb_stock_transfer_detail.Find(idd);
                    transferDetail.item_status = item.item_status;
                    transferDetail.remark = item.remark;
                    transferDetail.invoice_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    transferDetail.remain_quantity = transferDetail.quantity;
                    db.SaveChanges();
                    if (Convert.ToBoolean(transferDetail.status))
                        countReturn++;
                    if (string.Compare(transferDetail.item_status, Status.Approved) == 0)
                    {
                        countItemApproved++;
                        decimal stockBalance = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => string.Compare(m.product_id, transferDetail.st_item_id) == 0 && string.Compare(m.warehouse_id, transferDetail.st_warehouse_id) == 0).Select(m => m.total_quantity).FirstOrDefault());
                        //if(stockBalance >= 0)
                        //{
                        //    return Json(new { result = "error", message = "Your stockbalance is not enough." }, JsonRequestBehavior.AllowGet);
                        //}
                        //tb_inventory inventory = new tb_inventory();
                        //inventory.inventory_id = Guid.NewGuid().ToString();
                        //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inventory.ref_id = id;
                        //inventory.inventory_status_id = "6";
                        //inventory.warehouse_id = transferDetail.st_warehouse_id;
                        //inventory.product_id = transferDetail.st_item_id;
                        //inventory.out_quantity = Class.CommonClass.ConvertMultipleUnit(inventory.product_id, transferDetail.unit, Convert.ToDecimal(transferDetail.quantity));
                        //inventory.in_quantity = 0;
                        //inventory.total_quantity = stockBalance - inventory.out_quantity;
                        //db.tb_inventory.Add(inventory);
                        //db.SaveChanges();
                    }
                }
                tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(id);
                if(stockTransfer.stock_transfer_status == Status.Pending || stockTransfer.stock_transfer_status == Status.Feedbacked)
                {
                    var pendingItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "pending") == 0).ToList();
                    if (pendingItems.Count() == 0)
                    {
                        var totalItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0).ToList();
                        var approvedItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "approved") == 0).ToList();
                        if (totalItems.Count() == approvedItems.Count())
                            stockTransfer.stock_transfer_status = Status.Approved;
                        else
                            stockTransfer.stock_transfer_status = Status.PendingFeedback;
                    }
                    stockTransfer.approved_by = User.Identity.GetUserId();
                    stockTransfer.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    //return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), stockTransfer.stock_transfer_id, stockTransfer.stock_transfer_status, stockTransfer.approved_by, stockTransfer.approved_date, string.Empty);
                }
                else if(stockTransfer.stock_transfer_status == Status.Approved)
                {
                    var pendingItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "pending") == 0).ToList();
                    if (pendingItems.Count() == 0)
                    {
                        var totalItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0).ToList();
                        var approvedItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "approved") == 0).ToList();
                        if (totalItems.Count() == approvedItems.Count())
                            stockTransfer.stock_transfer_status = Status.Completed;
                        else
                            stockTransfer.stock_transfer_status = Status.PendingFeedback;
                    }
                    stockTransfer.last_approved_by = User.Identity.GetUserId();
                    stockTransfer.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), stockTransfer.stock_transfer_id, stockTransfer.stock_transfer_status, stockTransfer.last_approved_by, stockTransfer.approved_date, string.Empty);

                }
                //stockTransfer.stock_transfer_status = countItemApproved == models.Count() ? "Completed" : "Pending Feedback";
                if(countReturn==0)
                {
                    tb_item_request materialRequest = db.tb_item_request.Find(stockTransfer.item_request_id);
                    materialRequest.st_status = ShowStatus.STChecked;
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RequestCancel(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_transfer_voucher sTransfer = db.tb_stock_transfer_voucher.Find(id);
                sTransfer.stock_transfer_status = Status.cancelled;
                sTransfer.updated_by = User.Identity.GetUserId();
                sTransfer.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                Class.StockTransfer.RollBackItemQuantitybyStockTransfer(sTransfer.item_request_id, sTransfer.stock_transfer_id, true);
                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), sTransfer.stock_transfer_id, sTransfer.stock_transfer_status, sTransfer.updated_by, sTransfer.updated_date, string.Empty);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult PrepareFeedback(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            StockTransferViewModel model = new StockTransferViewModel();
            model = this.GetStockTransferDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id,List<InventoryViewModel> inventories)
        {
            try
            {
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    int countInvalid = 0;
                    string message = string.Empty;
                    tb_stock_transfer_voucher model = db.tb_stock_transfer_voucher.Find(id);
                    List<Models.ItemRequestDetail2ViewModel> remainRequestItems = new List<ItemRequestDetail2ViewModel>();
                    remainRequestItems = Class.ItemRequest.GetAllAvailableItembyStockTransfer( model.item_request_id,id);
                    #region check reamin transfer balance
                    foreach (ItemRequestDetail2ViewModel item in remainRequestItems)
                    {
                        foreach (InventoryViewModel transfer in inventories)
                        {
                            if (item.ir_item_id == transfer.product_id && item.remain_qty < Class.CommonClass.ConvertMultipleUnit(transfer.product_id, transfer.unit, Convert.ToDecimal(transfer.out_quantity)))
                            {
                                countInvalid++;
                                if (item.remain_qty < 0)
                                    message = message + string.Format("{0} is completed to tranfer.\n", item.product_name);
                                else
                                    message = message + string.Format("{0} is remain quantity {1} to transfer.", item.product_name, string.Format("{0:G29}", Double.Parse(item.remain_qty.ToString())));
                                break;
                            }
                        }
                    }
                    if (countInvalid > 0)
                        return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                    #endregion
                    #region check with total transfer quantity
                    decimal requestQty = 0, totalTransferQty = 0;
                    var dupId = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    foreach (var Id in dupId)
                    {
                        totalTransferQty = 0;
                        //requestQty =Convert.ToDecimal(inventories.Where(x => x.product_id == Id).Select(x => x.total_quantity).FirstOrDefault());
                        requestQty = Convert.ToDecimal(remainRequestItems.Where(m => m.ir_item_id == Id).Select(m => m.remain_qty).FirstOrDefault());
                        //var transferQty =inventories.Where(x => x.product_id == Id).Select(x => x.out_quantity).ToList();
                        var transferQty = inventories.Where(x => x.product_id == Id).ToList();
                        foreach (var tQty in transferQty)
                        {
                            //totalTransferQty = totalTransferQty + Convert.ToDecimal(tQty);
                            totalTransferQty = totalTransferQty + Class.CommonClass.ConvertMultipleUnit(tQty.product_id, tQty.unit, Convert.ToDecimal(tQty.out_quantity));
                        }
                        if (totalTransferQty > requestQty)
                        {
                            return Json(new { result = "fail", message = "Total transfer quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion
                    Class.StockTransfer.RollBackItemQuantitybyStockTransfer(model.item_request_id, id, true, "feedbacked");
                    var stockTransferDetail = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, id) == 0 && string.Compare(x.item_status, "feedbacked") == 0).Select(x => x.st_detail_id).ToList();
                    foreach(var idd in stockTransferDetail)
                    {
                        tb_stock_transfer_detail transferDetails = db.tb_stock_transfer_detail.Find(idd);
                        db.tb_stock_transfer_detail.Remove(transferDetails);
                        db.SaveChanges();
                    }
                    model.stock_transfer_status = Status.Feedbacked;
                    model.updated_by = User.Identity.GetUserId();
                    model.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    foreach(var inv in inventories)
                    {
                        tb_stock_transfer_detail stDetail = new tb_stock_transfer_detail();
                        stDetail.st_detail_id = Guid.NewGuid().ToString();
                        stDetail.st_ref_id = id;
                        stDetail.st_item_id = inv.product_id;
                        stDetail.st_warehouse_id = inv.warehouse_id;
                        stDetail.quantity = inv.out_quantity;
                        stDetail.remain_quantity = Class.CommonClass.ConvertMultipleUnit(inv.product_id, inv.unit, Convert.ToDecimal(inv.out_quantity));
                        stDetail.status = Convert.ToBoolean(inv.status);
                        stDetail.unit = inv.unit;
                        stDetail.invoice_date = inv.invoice_date != null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.inventory_date;
                        stDetail.invoice_number = inv.invoice_number; //string.IsNullOrEmpty(inv.invoice_number)?Class.StockTransfer.GenerateInvoiceNumber(stDetail.st_warehouse_id,Convert.ToDateTime(stDetail.invoice_date)):inv.invoice_number;
                        stDetail.item_status = "pending";
                        db.tb_stock_transfer_detail.Add(stDetail);
                        db.SaveChanges();
                    }
                    Class.StockTransfer.RollBackItemQuantitybyStockTransfer(model.item_request_id, id, false, "pending");
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), model.stock_transfer_id, model.stock_transfer_status, model.updated_by, model.updated_date, string.Empty);
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStockTransferDataTable(string status)
        {
            List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (string.Compare(status, "All")==0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true 
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach(var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach(var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count()>0?true:false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if(!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if(!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if(!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                    //stockTransfers = (from st in db.tb_stock_transfer_voucher
                    //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                    //                  where st.status == true
                    //                  select new StockTransferViewModel()
                    //                  {
                    //                      stock_transfer_id = st.stock_transfer_id,
                    //                      stock_transfer_no = st.stock_transfer_no,
                    //                      item_request_id = st.item_request_id,
                    //                      item_request_no = ir.ir_no,
                    //                      created_date = st.created_date,
                    //                      stock_transfer_status = st.stock_transfer_status,
                                          
                    //                  }).ToList();
                }else if (string.Compare(status, "AllQA") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.stock_transfer_status == "Pending" || st.stock_transfer_status== "Feedbacked"
                                             || st.stock_transfer_status == "Approved" || st.stock_transfer_status == "Pending Feedback"
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }else if (string.Compare(status, "AllCFO") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where
                                             st.stock_transfer_status == "Approved" || st.stock_transfer_status == "Completed" || st.stock_transfer_status == "Pending Feedback"
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }else if (string.Compare(status, "AllCQ") == 0)
                {
                    
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.stock_transfer_status == "Pending" || st.stock_transfer_status == "Feedbacked"
                                            || st.stock_transfer_status == "Approved" || st.stock_transfer_status == "Pending Feedback"
                                            || st.stock_transfer_status == "Completed"
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }

                }else if (string.Compare(status, "AllPQ") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.stock_transfer_status == "Pending" || st.stock_transfer_status == "Feedbacked"
                                            || st.stock_transfer_status == "Approved" || st.stock_transfer_status == "Pending Feedback"
                                        
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }else if (string.Compare(status, "AllSP") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.stock_transfer_status == "Pending" || st.stock_transfer_status == "Completed"
                                            || st.stock_transfer_status == "Approved"

                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }
                
                else
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && string.Compare(st.stock_transfer_status,status)==0
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate +invdate==null?"": Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate==null?"": Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }
                
                return Json(new { data = stockTransfers },JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStockTransferDataTableForQA(string status)
        {
            List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Pending" || st.stock_transfer_status == "Complete"
                                             || st.stock_transfer_status == "Feedbacked" || st.stock_transfer_status == "Pending Feedback"
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0 
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                    //stockTransfers = (from st in db.tb_stock_transfer_voucher
                    //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                    //                  where st.status == true
                    //                  select new StockTransferViewModel()
                    //                  {
                    //                      stock_transfer_id = st.stock_transfer_id,
                    //                      stock_transfer_no = st.stock_transfer_no,
                    //                      item_request_id = st.item_request_id,
                    //                      item_request_no = ir.ir_no,
                    //                      created_date = st.created_date,
                    //                      stock_transfer_status = st.stock_transfer_status,

                    //                  }).ToList();
                }
                else
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }

                return Json(new { data = stockTransfers }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStockTransferDataTableForPCH(string status)
        {
            List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Pending"                                   
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                    //stockTransfers = (from st in db.tb_stock_transfer_voucher
                    //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                    //                  where st.status == true
                    //                  select new StockTransferViewModel()
                    //                  {
                    //                      stock_transfer_id = st.stock_transfer_id,
                    //                      stock_transfer_no = st.stock_transfer_no,
                    //                      item_request_id = st.item_request_id,
                    //                      item_request_no = ir.ir_no,
                    //                      created_date = st.created_date,
                    //                      stock_transfer_status = st.stock_transfer_status,

                    //                  }).ToList();
                }
                else
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Pending" && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }

                return Json(new { data = stockTransfers }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStockTransferDataTableForCFO(string status)
        {
            List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Complete" || st.stock_transfer_status == "Completed"
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                    //stockTransfers = (from st in db.tb_stock_transfer_voucher
                    //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                    //                  where st.status == true
                    //                  select new StockTransferViewModel()
                    //                  {
                    //                      stock_transfer_id = st.stock_transfer_id,
                    //                      stock_transfer_no = st.stock_transfer_no,
                    //                      item_request_id = st.item_request_id,
                    //                      item_request_no = ir.ir_no,
                    //                      created_date = st.created_date,
                    //                      stock_transfer_status = st.stock_transfer_status,

                    //                  }).ToList();
                }
                else
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allStockTransfers.Any())
                    {
                        foreach (var item in allStockTransfers)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var stockTransferDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in stockTransferDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            StockTransferViewModel stocktransfer = new StockTransferViewModel();
                            stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                            stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                            stocktransfer.item_request_id = item.st.item_request_id;
                            stocktransfer.item_request_no = item.ir.ir_no;
                            stocktransfer.created_date = item.st.created_date;
                            stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                            stocktransfer.strWarehouse = warehouse;
                            stocktransfer.strInvoiceDate = invoiceDate;
                            stocktransfer.strInvoiceNo = invoiceNo;
                            stockTransfers.Add(stocktransfer);
                        }
                    }
                }

                return Json(new { data = stockTransfers }, JsonRequestBehavior.AllowGet);
            }
        }


  //public ActionResult GetStockTransferDataTableForQA(string status)
  //      {
  //          List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
  //          using (kim_mexEntities db = new kim_mexEntities())
  //          {
  //              if (string.Compare(status, "All") == 0)
  //              {
  //                  var allStockTransfers = (from st in db.tb_stock_transfer_voucher
  //                                           join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
  //                                           where st.status == true && st.stock_transfer_status == "Pending" || st.stock_transfer_status == "Complete"
  //                                           || st.stock_transfer_status == "Feedbacked" || st.stock_transfer_status == "Pending Feedback"
  //                                           select new { st, ir }).ToList();
  //                  if (allStockTransfers.Any())
  //                  {
  //                      foreach (var item in allStockTransfers)
  //                      {
  //                          string warehouse = string.Empty;
  //                          string invoiceNo = string.Empty;
  //                          string invoiceDate = string.Empty;
  //                          var stockTransferDetails = (from std in db.tb_stock_transfer_detail
  //                                                      join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
  //                                                      where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0 
  //                                                      select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
  //                          var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          if (dupwarehouses.Any())
  //                          {
  //                              foreach (var wh in dupwarehouses)
  //                                  warehouse = warehouse + wh + ", ";
  //                          }
  //                          var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invno in dupinvoiceno)
  //                              invoiceNo = invoiceNo + invno + ", ";
  //                          var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invdate in dupinvoicedate)
  //                              invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

  //                          foreach (var std in stockTransferDetails)
  //                          {
  //                              bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
  //                              if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
  //                              if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
  //                              if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
  //                          }
  //                          StockTransferViewModel stocktransfer = new StockTransferViewModel();
  //                          stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
  //                          stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
  //                          stocktransfer.item_request_id = item.st.item_request_id;
  //                          stocktransfer.item_request_no = item.ir.ir_no;
  //                          stocktransfer.created_date = item.st.created_date;
  //                          stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
  //                          stocktransfer.strWarehouse = warehouse;
  //                          stocktransfer.strInvoiceDate = invoiceDate;
  //                          stocktransfer.strInvoiceNo = invoiceNo;
  //                          stockTransfers.Add(stocktransfer);
  //                      }
  //                  }
  //                  //stockTransfers = (from st in db.tb_stock_transfer_voucher
  //                  //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
  //                  //                  where st.status == true
  //                  //                  select new StockTransferViewModel()
  //                  //                  {
  //                  //                      stock_transfer_id = st.stock_transfer_id,
  //                  //                      stock_transfer_no = st.stock_transfer_no,
  //                  //                      item_request_id = st.item_request_id,
  //                  //                      item_request_no = ir.ir_no,
  //                  //                      created_date = st.created_date,
  //                  //                      stock_transfer_status = st.stock_transfer_status,

  //                  //                  }).ToList();
  //              }
  //              else
  //              {
  //                  var allStockTransfers = (from st in db.tb_stock_transfer_voucher
  //                                           join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
  //                                           where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
  //                                           select new { st, ir }).ToList();
  //                  if (allStockTransfers.Any())
  //                  {
  //                      foreach (var item in allStockTransfers)
  //                      {
  //                          string warehouse = string.Empty;
  //                          string invoiceNo = string.Empty;
  //                          string invoiceDate = string.Empty;
  //                          var stockTransferDetails = (from std in db.tb_stock_transfer_detail
  //                                                      join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
  //                                                      where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
  //                                                      select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
  //                          var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          if (dupwarehouses.Any())
  //                          {
  //                              foreach (var wh in dupwarehouses)
  //                                  warehouse = warehouse + wh + ", ";
  //                          }
  //                          var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invno in dupinvoiceno)
  //                              invoiceNo = invoiceNo + invno + ", ";
  //                          var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invdate in dupinvoicedate)
  //                              invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

  //                          foreach (var std in stockTransferDetails)
  //                          {
  //                              bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
  //                              if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
  //                              if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
  //                              if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
  //                          }
  //                          StockTransferViewModel stocktransfer = new StockTransferViewModel();
  //                          stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
  //                          stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
  //                          stocktransfer.item_request_id = item.st.item_request_id;
  //                          stocktransfer.item_request_no = item.ir.ir_no;
  //                          stocktransfer.created_date = item.st.created_date;
  //                          stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
  //                          stocktransfer.strWarehouse = warehouse;
  //                          stocktransfer.strInvoiceDate = invoiceDate;
  //                          stocktransfer.strInvoiceNo = invoiceNo;
  //                          stockTransfers.Add(stocktransfer);
  //                      }
  //                  }
  //              }

  //              return Json(new { data = stockTransfers }, JsonRequestBehavior.AllowGet);
  //          }
  //      }

  //      public ActionResult GetStockTransferDataTableForPCH(string status)
  //      {
  //          List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
  //          using (kim_mexEntities db = new kim_mexEntities())
  //          {
  //              if (string.Compare(status, "All") == 0)
  //              {
  //                  var allStockTransfers = (from st in db.tb_stock_transfer_voucher
  //                                           join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
  //                                           where st.status == true && st.stock_transfer_status == "Pending"                                   
  //                                           select new { st, ir }).ToList();
  //                  if (allStockTransfers.Any())
  //                  {
  //                      foreach (var item in allStockTransfers)
  //                      {
  //                          string warehouse = string.Empty;
  //                          string invoiceNo = string.Empty;
  //                          string invoiceDate = string.Empty;
  //                          var stockTransferDetails = (from std in db.tb_stock_transfer_detail
  //                                                      join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
  //                                                      where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
  //                                                      select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
  //                          var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          if (dupwarehouses.Any())
  //                          {
  //                              foreach (var wh in dupwarehouses)
  //                                  warehouse = warehouse + wh + ", ";
  //                          }
  //                          var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invno in dupinvoiceno)
  //                              invoiceNo = invoiceNo + invno + ", ";
  //                          var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invdate in dupinvoicedate)
  //                              invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

  //                          foreach (var std in stockTransferDetails)
  //                          {
  //                              bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
  //                              if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
  //                              if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
  //                              if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
  //                          }
  //                          StockTransferViewModel stocktransfer = new StockTransferViewModel();
  //                          stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
  //                          stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
  //                          stocktransfer.item_request_id = item.st.item_request_id;
  //                          stocktransfer.item_request_no = item.ir.ir_no;
  //                          stocktransfer.created_date = item.st.created_date;
  //                          stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
  //                          stocktransfer.strWarehouse = warehouse;
  //                          stocktransfer.strInvoiceDate = invoiceDate;
  //                          stocktransfer.strInvoiceNo = invoiceNo;
  //                          stockTransfers.Add(stocktransfer);
  //                      }
  //                  }
  //                  //stockTransfers = (from st in db.tb_stock_transfer_voucher
  //                  //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
  //                  //                  where st.status == true
  //                  //                  select new StockTransferViewModel()
  //                  //                  {
  //                  //                      stock_transfer_id = st.stock_transfer_id,
  //                  //                      stock_transfer_no = st.stock_transfer_no,
  //                  //                      item_request_id = st.item_request_id,
  //                  //                      item_request_no = ir.ir_no,
  //                  //                      created_date = st.created_date,
  //                  //                      stock_transfer_status = st.stock_transfer_status,

  //                  //                  }).ToList();
  //              }
  //              else
  //              {
  //                  var allStockTransfers = (from st in db.tb_stock_transfer_voucher
  //                                           join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
  //                                           where st.status == true && st.stock_transfer_status == "Pending" && string.Compare(st.stock_transfer_status, status) == 0
  //                                           select new { st, ir }).ToList();
  //                  if (allStockTransfers.Any())
  //                  {
  //                      foreach (var item in allStockTransfers)
  //                      {
  //                          string warehouse = string.Empty;
  //                          string invoiceNo = string.Empty;
  //                          string invoiceDate = string.Empty;
  //                          var stockTransferDetails = (from std in db.tb_stock_transfer_detail
  //                                                      join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
  //                                                      where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
  //                                                      select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
  //                          var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          if (dupwarehouses.Any())
  //                          {
  //                              foreach (var wh in dupwarehouses)
  //                                  warehouse = warehouse + wh + ", ";
  //                          }
  //                          var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invno in dupinvoiceno)
  //                              invoiceNo = invoiceNo + invno + ", ";
  //                          var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
  //                          foreach (var invdate in dupinvoicedate)
  //                              invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

  //                          foreach (var std in stockTransferDetails)
  //                          {
  //                              bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
  //                              bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
  //                              if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
  //                              if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
  //                              if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
  //                          }
  //                          StockTransferViewModel stocktransfer = new StockTransferViewModel();
  //                          stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
  //                          stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
  //                          stocktransfer.item_request_id = item.st.item_request_id;
  //                          stocktransfer.item_request_no = item.ir.ir_no;
  //                          stocktransfer.created_date = item.st.created_date;
  //                          stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
  //                          stocktransfer.strWarehouse = warehouse;
  //                          stocktransfer.strInvoiceDate = invoiceDate;
  //                          stocktransfer.strInvoiceNo = invoiceNo;
  //                          stockTransfers.Add(stocktransfer);
  //                      }
  //                  }
  //              }

  //              return Json(new { data = stockTransfers }, JsonRequestBehavior.AllowGet);
  //          }
  //      }

        //public ActionResult GetStockTransferDataTableForCFO(string status)
        //{
        //    List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        if (string.Compare(status, "All") == 0)
        //        {
        //            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
        //                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
        //                                     where st.status == true && st.stock_transfer_status == "Complete" || st.stock_transfer_status == "Completed"
        //                                     select new { st, ir }).ToList();
        //            if (allStockTransfers.Any())
        //            {
        //                foreach (var item in allStockTransfers)
        //                {
        //                    string warehouse = string.Empty;
        //                    string invoiceNo = string.Empty;
        //                    string invoiceDate = string.Empty;
        //                    var stockTransferDetails = (from std in db.tb_stock_transfer_detail
        //                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
        //                                                where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
        //                                                select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
        //                    var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                    if (dupwarehouses.Any())
        //                    {
        //                        foreach (var wh in dupwarehouses)
        //                            warehouse = warehouse + wh + ", ";
        //                    }
        //                    var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                    foreach (var invno in dupinvoiceno)
        //                        invoiceNo = invoiceNo + invno + ", ";
        //                    var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                    foreach (var invdate in dupinvoicedate)
        //                        invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

        //                    foreach (var std in stockTransferDetails)
        //                    {
        //                        bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
        //                        bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
        //                        bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
        //                        if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
        //                        if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
        //                        if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
        //                    }
        //                    StockTransferViewModel stocktransfer = new StockTransferViewModel();
        //                    stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
        //                    stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
        //                    stocktransfer.item_request_id = item.st.item_request_id;
        //                    stocktransfer.item_request_no = item.ir.ir_no;
        //                    stocktransfer.created_date = item.st.created_date;
        //                    stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
        //                    stocktransfer.strWarehouse = warehouse;
        //                    stocktransfer.strInvoiceDate = invoiceDate;
        //                    stocktransfer.strInvoiceNo = invoiceNo;
        //                    stockTransfers.Add(stocktransfer);
        //                }
        //            }
        //            //stockTransfers = (from st in db.tb_stock_transfer_voucher
        //            //                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
        //            //                  where st.status == true
        //            //                  select new StockTransferViewModel()
        //            //                  {
        //            //                      stock_transfer_id = st.stock_transfer_id,
        //            //                      stock_transfer_no = st.stock_transfer_no,
        //            //                      item_request_id = st.item_request_id,
        //            //                      item_request_no = ir.ir_no,
        //            //                      created_date = st.created_date,
        //            //                      stock_transfer_status = st.stock_transfer_status,

        //            //                  }).ToList();
        //        }
        //        else
        //        {
        //            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
        //                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
        //                                     where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
        //                                     select new { st, ir }).ToList();
        //            if (allStockTransfers.Any())
        //            {
        //                foreach (var item in allStockTransfers)
        //                {
        //                    string warehouse = string.Empty;
        //                    string invoiceNo = string.Empty;
        //                    string invoiceDate = string.Empty;
        //                    var stockTransferDetails = (from std in db.tb_stock_transfer_detail
        //                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
        //                                                where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
        //                                                select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
        //                    var dupwarehouses = stockTransferDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                    if (dupwarehouses.Any())
        //                    {
        //                        foreach (var wh in dupwarehouses)
        //                            warehouse = warehouse + wh + ", ";
        //                    }
        //                    var dupinvoiceno = stockTransferDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                    foreach (var invno in dupinvoiceno)
        //                        invoiceNo = invoiceNo + invno + ", ";
        //                    var dupinvoicedate = stockTransferDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                    foreach (var invdate in dupinvoicedate)
        //                        invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

        //                    foreach (var std in stockTransferDetails)
        //                    {
        //                        bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
        //                        bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
        //                        bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
        //                        if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
        //                        if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
        //                        if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
        //                    }
        //                    StockTransferViewModel stocktransfer = new StockTransferViewModel();
        //                    stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
        //                    stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
        //                    stocktransfer.item_request_id = item.st.item_request_id;
        //                    stocktransfer.item_request_no = item.ir.ir_no;
        //                    stocktransfer.created_date = item.st.created_date;
        //                    stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
        //                    stocktransfer.strWarehouse = warehouse;
        //                    stocktransfer.strInvoiceDate = invoiceDate;
        //                    stocktransfer.strInvoiceNo = invoiceNo;
        //                    stockTransfers.Add(stocktransfer);
        //                }
        //            }
        //        }

        //        return Json(new { data = stockTransfers }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public List<tb_item_request> GetItemRequestDropdownList()
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                itemRequests = db.tb_item_request.OrderBy(m=>m.ir_no).Where(m => m.status == true && m.ir_status == "Approved").ToList();
            }
            return itemRequests;
        }
        public string GetStockTransferNo()
        {
            string stockTransferNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.tb_stock_transfer_voucher orderby tbl.created_date descending select tbl.stock_transfer_no).FirstOrDefault();
                if (number == null)
                    last_no = "001";
                else
                {
                    number = number.Substring(number.Length - 3, 3);
                    int num = Convert.ToInt32(number) + 1;
                    if (num.ToString().Length == 1) last_no = "00" + num;
                    else if (num.ToString().Length == 2) last_no = "0" + num;
                    else if (num.ToString().Length == 3) last_no = num.ToString();
                }
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                stockTransferNo = "ST-" + yy + "-" + mm + "-" + last_no;
            }
            return stockTransferNo;
        }

        #region added by terd apr 16 2020
        public ActionResult GetAvailbleWarehouseToTransferbyMR(string id,string stId = null)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<WareHouseViewModel> models = new List<WareHouseViewModel>();
                var materialRequest = (from pr in db.tb_item_request
                                       join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                                       //join site in db.tb_site on pro.site_id equals site.site_id
                                       join warehouse in db.tb_warehouse on pro.project_id equals warehouse.warehouse_project_id
                                       where string.Compare(pr.ir_id, id) == 0 
                                       select new ItemRequestViewModel
                                       {
                                           ir_project_id = pro.project_id,
                                           project_full_name = pro.project_full_name,
                                           warehouse_id = warehouse.warehouse_id,
                                           warehouse_name = warehouse.warehouse_name,
                                       }).FirstOrDefault();
                List<Models.ItemRequestDetail2ViewModel> itemIDs = Class.ItemRequest.GetAllAvailableItembyStockTransfer(id, stId);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true && string.Compare(m.warehouse_id, materialRequest.warehouse_id) != 0).ToList();
                foreach (var it in itemIDs)
                {
                    foreach (var wh in warehouses)
                    {

                        var irItem = (from inv in db.tb_inventory
                                      join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                      join pro in db.tb_product on inv.product_id equals pro.product_id
                                      join proj in db.tb_project on wah.warehouse_project_id equals proj.project_id
                                      orderby inv.inventory_date descending
                                      where inv.warehouse_id == wh.warehouse_id && inv.product_id == it.ir_item_id && string.Compare(proj.p_status, ProjectStatus.Active) == 0
                                      select new { inv, wah, pro }
                                  ).FirstOrDefault();
                        if (irItem != null)
                        {
                            var isExits = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).FirstOrDefault()==null?true:false;
                            if(isExits)
                                models.Add(new WareHouseViewModel() { warehouse_id = wh.warehouse_id, warehouse_name = wh.warehouse_name });
                            
                        }

                    }
                    }
                    return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetItemByIRID1(string id,string warehouseId,string stId = null)
        {
            List<STItemViewModel> stItems = new List<STItemViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {

                var materialRequest = (from pr in db.tb_item_request
                                       join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                                       join site in db.tb_site on pro.site_id equals site.site_id
                                       join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                                       where string.Compare(pr.ir_id, id) == 0
                                       select new ItemRequestViewModel
                                       {
                                           ir_project_id = pro.project_id,
                                           project_full_name = pro.project_full_name,
                                           warehouse_id = warehouse.warehouse_id,
                                           warehouse_name = warehouse.warehouse_name,
                                       }).FirstOrDefault();

                //itemIDs = Class.CommonClass.GetAvailableRequestItemDetails(id);
                List<Models.ItemRequestDetail2ViewModel> itemIDs = Class.ItemRequest.GetAllAvailableItembyStockTransfer(id, stId);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true && string.Compare(m.warehouse_id, materialRequest.warehouse_id) != 0).Select(m => m.warehouse_id).ToList();
                foreach (var it in itemIDs)
                {
                    var irItem = (from inv in db.tb_inventory
                                  join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                  join pro in db.tb_product on inv.product_id equals pro.product_id
                                  orderby inv.inventory_date descending
                                  where inv.warehouse_id == warehouseId && inv.product_id == it.ir_item_id
                                  select new { inv, wah, pro }

                                  ).FirstOrDefault();
                    if (irItem != null)
                    {
                        decimal? stockBalance = 0;
                        var isItemBlock = db.tb_item_blocking_detail.OrderByDescending(s => s.item_blocking_date).Where(s => string.Compare(s.warehouse_id, warehouseId) == 0 && string.Compare(s.item_id, it.ir_item_id) == 0 && s.active == true).FirstOrDefault();

                        STItemViewModel item = new STItemViewModel();
                        item.itemID = irItem.inv.product_id;
                        item.itemCode = irItem.pro.product_code;
                        item.itemName = irItem.pro.product_name;
                        item.itemUnit = irItem.pro.product_unit;
                        item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                        item.Unit = it.ir_item_unit;//add new
                        item.unitName = db.tb_unit.Find(item.Unit).Name;
                        item.warehouseID = irItem.inv.warehouse_id;
                        item.warehouseName = irItem.wah.warehouse_name;

                        if (isItemBlock == null)
                        {
                            item.stockBalance = irItem.inv.total_quantity;
                            stockBalance = irItem.inv.total_quantity;
                        }
                        else
                        {
                            stockBalance = irItem.inv.total_quantity - isItemBlock.block_qty;
                            item.stockBalance = stockBalance;
                        }

                        //item.requestQty = Class.CommonClass.ConvertMultipleUnit(irItem.inv.product_id, it.ir_item_unit, Convert.ToDecimal(it.remain_qty));
                        // item.requestQty = it.remain_qty;
                        item.requestQty = it.approved_qty;
                        item.requestUnit = it.ir_item_unit;
                        item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                        item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                        item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                        item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                        item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                        item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                        item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                        item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                        item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                        item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                        item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                        if(stockBalance>0)
                            stItems.Add(item);
                    }
                }
                return Json(new { result = "success", data = stItems, detail = materialRequest }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        public ActionResult GetItemByIRID(string id,string stId=null)
        {
            List<STItemViewModel> stItems = new List<STItemViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //List<ItemRequestDetail2ViewModel> itemIDs = new List<ItemRequestDetail2ViewModel>();
                //var items = (from ir in db.tb_item_request
                //             join dir in db.tb_ir_detail1 on ir.ir_id equals dir.ir_id
                //             join ddir in db.tb_ir_detail2 on dir.ir_detail1_id equals ddir.ir_detail1_id
                //             where ir.ir_id == id
                //             select new {itemID= ddir.ir_item_id,irQTY=ddir.ir_qty}).ToList();
                /*
                var items = (from ir in db.tb_item_request
                             join dir in db.tb_ir_detail1 on ir.ir_id equals dir.ir_id
                             join ddir in db.tb_ir_detail2 on dir.ir_detail1_id equals ddir.ir_detail1_id
                             where ir.ir_id == id && ddir.is_approved==true
                             select new { itemID = ddir.ir_item_id, irQTY = ddir.approved_qty,irUnit=ddir.ir_item_unit }).ToList();
                
                if (items.Any())
                {
                    foreach (var item in items)
                        itemIDs.Add(new ItemRequestDetail2ViewModel() { ir_item_id = item.itemID, ir_qty = item.irQTY,ir_item_unit=item.irUnit } );
                }
                */

                var materialRequest = (from pr in db.tb_item_request
                                       join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                                       join site in db.tb_site on pro.site_id equals site.site_id
                                       join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                                       where string.Compare(pr.ir_id, id) == 0
                                       select new ItemRequestViewModel{
                                           ir_project_id=pro.project_id,
                                           project_full_name=pro.project_full_name,
                                          warehouse_id=warehouse.warehouse_id,
                                          warehouse_name=warehouse.warehouse_name,
                                       }).FirstOrDefault();

                 //itemIDs = Class.CommonClass.GetAvailableRequestItemDetails(id);
                 List < Models.ItemRequestDetail2ViewModel > itemIDs = Class.ItemRequest.GetAllAvailableItembyStockTransfer(id, stId);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true && string.Compare(m.warehouse_id,materialRequest.warehouse_id)!=0).Select(m=>m.warehouse_id).ToList();
                foreach (var it in itemIDs)
                {
                    foreach (var wh in warehouses)
                    {
                        
                        var irItem= (from inv in db.tb_inventory
                                join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                join pro in db.tb_product on inv.product_id equals pro.product_id
                                orderby inv.inventory_date descending
                                    where inv.warehouse_id == wh && inv.product_id == it.ir_item_id
                                    select new {inv,wah,pro}
                                    //select new STItemViewModel()
                                    //{
                                    //    itemID=inv.product_id,
                                    //    itemCode=pro.product_code,
                                    //    itemName=pro.product_name,
                                    //    itemUnit=pro.product_unit,
                                    //    warehouseID=inv.warehouse_id,
                                    //    warehouseName=wah.warehouse_name,
                                    //    stockBalance=inv.total_quantity,
                                    //    //requestQty=Class.CommonClass.ConvertMultipleUnit(pro.product_id,it.ir_item_unit,Convert.ToDecimal(it.ir_qty)),
                                    //    //requestQty=it.ir_qty,
                                    //    quantity=it.ir_qty,
                                    //    requestUnit=it.ir_item_unit,
                                    //    uom1_id=db.tb_multiple_uom.Where(x=>x.product_id==inv.product_id).Select(x=>x.uom1_id).FirstOrDefault(),
                                    //    uom2_id = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom2_id).FirstOrDefault(),
                                    //    uom3_id = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom3_id).FirstOrDefault(),
                                    //    uom4_id = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom4_id).FirstOrDefault(),
                                    //    uom5_id = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom5_id).FirstOrDefault(),
                                    //    uom1_qty= db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom1_qty).FirstOrDefault(),
                                    //    uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom2_qty).FirstOrDefault(),
                                    //    uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom3_qty).FirstOrDefault(),
                                    //    uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom4_qty).FirstOrDefault(),
                                    //    uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => x.uom5_qty).FirstOrDefault(),
                                    //}
                                  ).FirstOrDefault();
                        if (irItem != null)
                        {
                            STItemViewModel item = new STItemViewModel();
                            item.itemID = irItem.inv.product_id;
                            item.itemCode = irItem.pro.product_code;
                            item.itemName = irItem.pro.product_name;
                            item.itemUnit = irItem.pro.product_unit;
                            item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                            item.Unit = it.ir_item_unit;//add new
                            item.unitName = db.tb_unit.Find(item.Unit).Name;
                            item.warehouseID = irItem.inv.warehouse_id;
                            item.warehouseName = irItem.wah.warehouse_name;
                            item.stockBalance = irItem.inv.total_quantity;
                            //item.requestQty = Class.CommonClass.ConvertMultipleUnit(irItem.inv.product_id, it.ir_item_unit, Convert.ToDecimal(it.remain_qty));
                            // item.requestQty = it.remain_qty;
                            item.requestQty = it.approved_qty;
                            item.requestUnit = it.ir_item_unit;
                            item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                            item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                            item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                            item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                            item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                            item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                            item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                            item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                            item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                            item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                            item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                            stItems.Add(item);
                        }
                        
                        
                    }
                }
                return Json(new { result = "success",data= stItems,detail= materialRequest }, JsonRequestBehavior.AllowGet);
            }
        }
        public StockTransferViewModel GetStockTransferDetail(string id)
        {
            StockTransferViewModel sTransfer = new StockTransferViewModel();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                sTransfer = (from str in db.tb_stock_transfer_voucher
                             join ir in db.tb_item_request on str.item_request_id equals ir.ir_id
                             join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                             where str.stock_transfer_id == id
                             select new StockTransferViewModel() { stock_transfer_id = str.stock_transfer_id,
                                 stock_transfer_no = str.stock_transfer_no,
                                 item_request_id = str.item_request_id,
                                 item_request_no = ir.ir_no,
                                 created_date = str.created_date,
                                 stock_transfer_status =str.stock_transfer_status,
                                 project_id=pro.project_id,
                                 project_fullname=pro.project_full_name,
                                 warehouse_id=wh.warehouse_id,
                                 warehouse_name=wh.warehouse_name
                             }).FirstOrDefault();
                /*  updated when have workflow
                var inventories = (from inv in db.tb_inventory
                                   join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                   join pro in db.tb_product on inv.product_id equals pro.product_id
                                   where inv.ref_id == sTransfer.stock_transfer_id
                                   select new InventoryViewModel()
                                   {
                                       product_id=inv.product_id,
                                       itemCode=pro.product_code,
                                       itemName=pro.product_name,
                                       itemUnit=pro.product_unit,
                                       warehouse_id=inv.warehouse_id,
                                       warehouseName=wah.warehouse_name,
                                       in_quantity=inv.in_quantity,
                                       out_quantity=inv.out_quantity,
                                       total_quantity=inv.total_quantity+inv.out_quantity
                                   }).ToList();
                */
                var inventories = (from inv in db.tb_stock_transfer_detail
                                   join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                   join pro in db.tb_product on inv.st_item_id equals pro.product_id
                                   where inv.st_ref_id == sTransfer.stock_transfer_id
                                   select new InventoryViewModel()
                                   {
                                       inventory_id=inv.st_detail_id,
                                       product_id = inv.st_item_id,
                                       itemCode = pro.product_code,
                                       itemName = pro.product_name,
                                       itemUnit = pro.product_unit,
                                       warehouse_id = inv.st_warehouse_id,
                                       warehouseName = wah.warehouse_name,
                                       in_quantity =0,
                                       out_quantity = inv.quantity,
                                       unit=inv.unit,
                                       total_quantity = (from invv in db.tb_inventory orderby invv.inventory_date descending where invv.product_id==inv.st_item_id&&invv.warehouse_id==inv.st_warehouse_id select invv.total_quantity).FirstOrDefault(),
                                       invoice_number=inv.invoice_number,
                                       invoice_date=inv.invoice_date,
                                       item_status=inv.item_status,
                                       status = inv.status.ToString(),
                                       //status = inv.status.ToString(),
                                       remark=inv.remark,
                                   }).ToList();
                sTransfer.inventoryDetails = inventories;
                sTransfer.itemTransfers = this.GetItemRequest(sTransfer.item_request_id,sTransfer.stock_transfer_id);
                sTransfer.rejects = Class.CommonClass.GetRejectByRequest(id);
                sTransfer.processWorkFlows = ProcessWorkflowModel.GetProcessWorkflowByRefId(sTransfer.stock_transfer_id);
                return sTransfer;
            }
        }
        public List<STItemViewModel> GetItemRequest(string id,string stID=null)
        {
            List<STItemViewModel> stItems = new List<STItemViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemRequestDetail2ViewModel> itemIDs = new List<ItemRequestDetail2ViewModel>();
                /*
                var items = (from ir in db.tb_item_request
                             join dir in db.tb_ir_detail1 on ir.ir_id equals dir.ir_id
                             join ddir in db.tb_ir_detail2 on dir.ir_detail1_id equals ddir.ir_detail1_id
                             where ir.ir_id == id && ddir.remain_qty>0
                             select new { itemID = ddir.ir_item_id, irQTY = ddir.ir_qty,irUnit=ddir.ir_item_unit,approvedQty=ddir.approved_qty,remainQty=ddir.remain_qty }).ToList();
                if (items.Any())
                {
                    foreach (var item in items)
                        itemIDs.Add(new ItemRequestDetail2ViewModel() { ir_item_id = item.itemID, ir_qty = item.irQTY,ir_item_unit=item.irUnit,approved_qty=item.approvedQty,remain_qty=item.remainQty });
                }
                */
                itemIDs = Class.ItemRequest.GetAllAvailableItembyStockTransfer(id, stID);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true).Select(m => m.warehouse_id).ToList();
                foreach (var it in itemIDs)
                {
                    foreach (var wh in warehouses)
                    {
                        //STItemViewModel item = new STItemViewModel();
                        var irItem = (from inv in db.tb_inventory
                                      join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                      join pro in db.tb_product on inv.product_id equals pro.product_id
                                      orderby inv.inventory_date descending
                                      where inv.warehouse_id == wh && inv.product_id == it.ir_item_id
                                      select new { inv, wah, pro }).FirstOrDefault();
                        if (irItem != null)
                        {
                            STItemViewModel item = new STItemViewModel();
                            item.itemID = irItem.inv.product_id;
                            item.itemCode = irItem.pro.product_code;
                            item.itemName = irItem.pro.product_name;
                            item.itemUnit = irItem.pro.product_unit;
                            item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                            item.warehouseID = irItem.inv.warehouse_id;
                            item.warehouseName = irItem.wah.warehouse_name;
                            item.stockBalance = irItem.inv.total_quantity;
                            //  item.requestQty = Class.CommonClass.ConvertMultipleUnit(irItem.inv.product_id, it.ir_item_unit, Convert.ToDecimal(it.approved_qty));
                            item.requestQty = Convert.ToDecimal(it.approved_qty);
                            item.requestUnit = it.ir_item_unit;
                            item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                            item.remain_qty = it.remain_qty;
                            item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                            item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                            item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                            item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                            item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                            item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                            item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                            item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                            item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                            item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                            item.projectSiteManagers = (from dwh in db.tb_warehouse
                                              join dsite in db.tb_site on dwh.warehouse_site_id equals dsite.site_id
                                              join dpro in db.tb_project on dsite.site_id equals dpro.site_id
                                              join dprosm in db.tb_site_manager_project on dpro.project_id equals dprosm.project_id
                                              where string.Compare(dwh.warehouse_id, item.warehouseID) == 0
                                              select dprosm.site_manager).ToList();
                            stItems.Add(item);
                        }

                        /*
                        item = (from inv in db.tb_inventory
                                join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                join pro in db.tb_product on inv.product_id equals pro.product_id
                                orderby inv.inventory_date descending
                                where inv.warehouse_id == wh && inv.product_id == it.ir_item_id
                                select new STItemViewModel()
                                {
                                    itemID = inv.product_id,
                                    itemCode = pro.product_code,
                                    itemName = pro.product_name,
                                    itemUnit = pro.product_unit,
                                    warehouseID = inv.warehouse_id,
                                    warehouseName = wah.warehouse_name,
                                    stockBalance = inv.total_quantity,
                                    requestQty = it.ir_qty,
                                    approved_qty=it.approved_qty
                                }
                                  ).FirstOrDefault();
                        if (item != null)
                        {
                            stItems.Add(item);
                        }
                        */
                    }
                }
            }
            return stItems;
        }
        public void UpdateInventory(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory.Where(m => m.ref_id == id).ToList();
                foreach(var inventory in inventories)
                {
                    var item = (from tbl in db.tb_inventory orderby tbl.inventory_date descending where tbl.product_id == inventory.product_id && tbl.warehouse_id == inventory.warehouse_id select tbl).ToList();
                    if (string.Compare(inventory.inventory_id, item[0].inventory_id) != 0)
                    {
                        string inventoryID = item[0].inventory_id;
                        tb_inventory inv = db.tb_inventory.Where(m => m.inventory_id == inventoryID).FirstOrDefault();
                        //inv.total_quantity = inv.total_quantity + inventory.in_quantity;
                        inv.total_quantity = (inv.total_quantity - inventory.in_quantity) + inventory.out_quantity;
                        db.SaveChanges();
                    }
                }
            }
        }
        public void DeleteInventory(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var inventories = db.tb_inventory.Where(m => m.ref_id == id).ToList();
                foreach(var inventory in inventories)
                {
                    var inventoryID = inventory.inventory_id;
                    tb_inventory inv = db.tb_inventory.Where(m => m.inventory_id == inventoryID).FirstOrDefault();
                    db.tb_inventory.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public void DeleteStockTransferDetail(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var inventories = db.tb_stock_transfer_detail.Where(m => m.st_ref_id == id).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.st_detail_id;
                    tb_stock_transfer_detail inv = db.tb_stock_transfer_detail.Where(m => m.st_detail_id == inventoryID).FirstOrDefault();
                    db.tb_stock_transfer_detail.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public void InsertItemInventory(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var inventories = db.tb_stock_transfer_detail.Where(m => m.st_ref_id == id).ToList();
                foreach(var inv in inventories)
                {
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.st_item_id && m.warehouse_id == inv.st_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.ref_id = id;
                    inventory.inventory_status_id = "6";
                    inventory.warehouse_id = inv.st_warehouse_id;
                    inventory.product_id = inv.st_item_id;
                    inventory.out_quantity = Class.CommonClass.ConvertMultipleUnit(inv.st_item_id,inv.unit,Convert.ToDecimal(inv.quantity));
                    inventory.in_quantity = 0;
                    inventory.total_quantity = totalQty - inventory.out_quantity;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }
            }
        }
        public void updateItemRequestStatus(string irId,string stId)
        {
            kim_mexEntities db = new kim_mexEntities();
            List<InventoryViewModel> invs = new List<InventoryViewModel>();
            int count = 0;
            var requestItems = (from ir in db.tb_item_request
                                join dIr in db.tb_ir_detail1 on ir.ir_id equals dIr.ir_id
                                join ddIr in db.tb_ir_detail2 on dIr.ir_detail1_id equals ddIr.ir_detail1_id
                                where ir.ir_id == irId && ddIr.is_approved == true
                                select new { itemId = ddIr.ir_item_id, approvedQty = ddIr.approved_qty }).ToList();
            var inventories = (from inv in db.tb_inventory
                               where inv.ref_id == stId
                               select new { itemId = inv.product_id, qty = inv.out_quantity }).ToList();
            var dupItem = inventories.GroupBy(x => x.itemId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            double totalInventoryQty = 0;
            if (dupItem.Any())
            {
                foreach (var item in dupItem)
                {
                    totalInventoryQty = 0;
                    var dd = inventories.Where(m => m.itemId == item).ToList();
                    if (dd.Any())
                    {
                        foreach (var d in dd)
                        {
                            totalInventoryQty = totalInventoryQty + Convert.ToDouble(d.qty);
                        }
                    }
                    invs.Add(new InventoryViewModel() { product_id = item, total_quantity = Convert.ToDecimal(totalInventoryQty) });
                }
            }
            else
            {
                foreach(var inv in inventories)
                {
                    invs.Add(new InventoryViewModel() { product_id = inv.itemId, total_quantity = inv.qty });
                }
            }
            
            foreach(var ri in requestItems)
            {
                foreach(var inv in invs)
                {
                    if (string.Compare(ri.itemId, inv.product_id) == 0 && ri.approvedQty == inv.total_quantity)
                        count++;
                }
            }
            if(count==requestItems.Count())
            {
                tb_item_request itemRequest = db.tb_item_request.Where(m => m.ir_id == irId).FirstOrDefault();
                itemRequest.ir_status = "Completed";
                itemRequest.checked_by = User.Identity.Name;
                itemRequest.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
            }
        }
        public List<InventoryViewModel> getRemainItems(string requestId)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<InventoryViewModel> itemTransfers = new List<InventoryViewModel>();
                var itemRequests = (from request in db.tb_item_request
                                    join dRequest in db.tb_ir_detail1 on request.ir_id equals dRequest.ir_id
                                    join ddRequest in db.tb_ir_detail2 on dRequest.ir_detail1_id equals ddRequest.ir_detail1_id
                                    join product in db.tb_product on ddRequest.ir_item_id equals product.product_id
                                    where request.ir_id == requestId && ddRequest.is_approved == true
                                    select new { request, dRequest, ddRequest ,product}).ToList();
                
                var stIDs = (from st in db.tb_stock_transfer_voucher
                             where st.item_request_id == requestId && st.status == true && st.stock_transfer_status == "Approved"
                             select st.stock_transfer_id).ToList();
                if (stIDs.Any())
                {
                    foreach (var stId in stIDs)
                    {
                        var transers = (from inv in db.tb_inventory
                                          where inv.ref_id == stId
                                          select new InventoryViewModel() { product_id = inv.product_id, out_quantity = inv.out_quantity }).ToList();
                        if (transers.Any())
                        {
                            foreach(var t in transers)
                            {
                                itemTransfers.Add(new InventoryViewModel() { product_id = t.product_id, out_quantity = t.out_quantity });
                            }
                        }
                        
                    }
                }

                if (itemRequests.Any())
                {
                    foreach(var item in itemRequests)
                    {
                        decimal remainApprovedQty = Convert.ToDecimal(Class.CommonClass.ConvertMultipleUnit(item.ddRequest.ir_item_id, item.ddRequest.ir_item_unit, Convert.ToDecimal(item.ddRequest.approved_qty)));
                        if (itemTransfers.Count() > 0)
                        {
                            foreach(InventoryViewModel st in itemTransfers)
                            {
                                if (item.ddRequest.ir_item_id == st.product_id)
                                    remainApprovedQty = remainApprovedQty -Convert.ToDecimal(st.out_quantity);
                            }
                        }
                        inventories.Add(new InventoryViewModel() { product_id = item.ddRequest.ir_item_id,itemName=item.product.product_name, total_quantity = remainApprovedQty });
                    }
                }
            }
            return inventories;
        }
        public ActionResult GetWarehousFromPR(string id)
        {
            string warehouseId = Class.CommonClass.GetWarehouseIDbyPurchaseRequisition(id);
            using (kim_mexEntities db =new kim_mexEntities())
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
        public ActionResult GetStockTransferInfobyMRId(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var obj = (from mr in db.tb_item_request
                           join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                           join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                           from wh in pwh.DefaultIfEmpty()
                           where string.Compare(mr.ir_id, id) == 0
                           select new { pro, wh }).FirstOrDefault();
                if(obj==null)
                    return Json(new { result = "error", message = "No warehouse available for this request." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { result = "success", data = obj }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<StockTransferViewModel> GetStockTransferByDaterangeAndStatusListItems(string dateRange,string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                string userid = User.Identity.GetUserId().ToString();
                List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
                List<StockTransferFilterResult> results = new List<StockTransferFilterResult>();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                if (string.Compare(status, "0") == 0)
                {
                    if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.OperationDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.Purchaser))
                    {
                        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                 //join site in db.tb_site on pro.site_id equals site.site_id
                                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                 from wh in pwh.DefaultIfEmpty()
                                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                 from fwh in ffwh.DefaultIfEmpty()
                                                 where st.status == true && st.created_date>=startDate && st.created_date<=endDate
                                                 select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                        results.AddRange(allStockTransfers);
                    }
                    else
                    {

                        if (User.IsInRole(Role.SiteStockKeeper))
                        {
                            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                     join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                     join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                     from wh in pwh.DefaultIfEmpty()
                                                     join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                     from fwh in ffwh.DefaultIfEmpty()
                                                     join stockkeeper in db.tb_stock_keeper_warehouse on fwh.warehouse_id equals stockkeeper.warehouse_id
                                                     where st.status == true && string.Compare(stockkeeper.stock_keeper, userid) == 0 && st.created_date >= startDate && st.created_date <= endDate
                                                     select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers);
                        }
                        if (User.IsInRole(Role.QAQCOfficer))
                        {
                            //Pending and Feedback Request
                            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                     join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                     //join site in db.tb_site on pro.site_id equals site.site_id
                                                     join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                     from wh in pwh.DefaultIfEmpty()
                                                     join qaqc in db.tb_warehouse_qaqc on st.from_warehouse_id equals qaqc.warehouse_id
                                                     join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                     from fwh in ffwh.DefaultIfEmpty()
                                                     where st.status == true && string.Compare(qaqc.qaqc_id, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Pending) == 0 || string.Compare(st.stock_transfer_status, Status.Feedbacked) == 0)
                                                     && st.created_date >= startDate && st.created_date <= endDate
                                                     select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers);

                            //Approved History
                            var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
                                                      join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                      join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                      from wh in pwh.DefaultIfEmpty()
                                                          //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                                      join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                      from fwh in ffwh.DefaultIfEmpty()
                                                      where st.status == true && string.Compare(st.approved_by, userid) == 0 && st.created_date >= startDate && st.created_date <= endDate
                                                      select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers1);

                        }
                        if (User.IsInRole(Role.SiteManager))
                        {
                            //Pending and Feedback Request
                            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                     join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                     //join site in db.tb_site on pro.site_id equals site.site_id
                                                     join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                     from wh in pwh.DefaultIfEmpty()
                                                     join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                     from fwh in ffwh.DefaultIfEmpty()
                                                     join fsite in db.tb_site on fwh.warehouse_site_id equals fsite.site_id
                                                     join fpro in db.tb_project on fsite.site_id equals fpro.site_id
                                                     join sitem in db.tb_site_manager_project on fpro.project_id equals sitem.project_id
                                                     where st.status == true && string.Compare(sitem.site_manager, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Approved) == 0) && st.created_date >= startDate && st.created_date <= endDate
                                                     select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers);

                            //Approved History
                            var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
                                                      join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                      join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                      from wh in pwh.DefaultIfEmpty()
                                                      join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                      from fwh in ffwh.DefaultIfEmpty()
                                                          //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                                      where st.status == true && string.Compare(st.checked_by, userid) == 0 && st.created_date >= startDate && st.created_date <= endDate
                                                      select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers1);

                        }
                    }
                }else
                {
                    if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.OperationDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.Purchaser))
                    {
                        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                 //join site in db.tb_site on pro.site_id equals site.site_id
                                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                 from wh in pwh.DefaultIfEmpty()
                                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                 from fwh in ffwh.DefaultIfEmpty()
                                                 where st.status == true && st.created_date >= startDate && st.created_date <= endDate && string.Compare(st.stock_transfer_status,status)==0
                                                 select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                        results.AddRange(allStockTransfers);
                    }
                    else
                    {

                        if (User.IsInRole(Role.SiteStockKeeper))
                        {
                            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                     join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                     join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                     from wh in pwh.DefaultIfEmpty()
                                                     join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                     from fwh in ffwh.DefaultIfEmpty()
                                                     join stockkeeper in db.tb_stock_keeper_warehouse on fwh.warehouse_id equals stockkeeper.warehouse_id
                                                     where st.status == true && string.Compare(stockkeeper.stock_keeper, userid) == 0 && st.created_date >= startDate && st.created_date <= endDate && string.Compare(st.stock_transfer_status, status) == 0
                                                     select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers);
                        }
                        if (User.IsInRole(Role.QAQCOfficer))
                        {
                            //Pending and Feedback Request
                            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                     join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                     //join site in db.tb_site on pro.site_id equals site.site_id
                                                     join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                     from wh in pwh.DefaultIfEmpty()
                                                     join qaqc in db.tb_warehouse_qaqc on st.from_warehouse_id equals qaqc.warehouse_id
                                                     join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                     from fwh in ffwh.DefaultIfEmpty()
                                                     where st.status == true && string.Compare(qaqc.qaqc_id, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Pending) == 0 || string.Compare(st.stock_transfer_status, Status.Feedbacked) == 0)
                                                     && st.created_date >= startDate && st.created_date <= endDate && string.Compare(st.stock_transfer_status, status) == 0
                                                     select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers);

                            //Approved History
                            var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
                                                      join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                      join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                      from wh in pwh.DefaultIfEmpty()
                                                          //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                                      join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                      from fwh in ffwh.DefaultIfEmpty()
                                                      where st.status == true && string.Compare(st.approved_by, userid) == 0 && st.created_date >= startDate && st.created_date <= endDate && string.Compare(st.stock_transfer_status, status) == 0
                                                      select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers1);

                        }
                        if (User.IsInRole(Role.SiteManager))
                        {
                            //Pending and Feedback Request
                            var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                     join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                     join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                     //join site in db.tb_site on pro.site_id equals site.site_id
                                                     join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                     from wh in pwh.DefaultIfEmpty()
                                                     join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                     from fwh in ffwh.DefaultIfEmpty()
                                                     join fsite in db.tb_site on fwh.warehouse_site_id equals fsite.site_id
                                                     join fpro in db.tb_project on fsite.site_id equals fpro.site_id
                                                     join sitem in db.tb_site_manager_project on fpro.project_id equals sitem.project_id
                                                     where st.status == true && string.Compare(sitem.site_manager, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Approved) == 0) && st.created_date >= startDate && st.created_date <= endDate
                                                      && string.Compare(st.stock_transfer_status, status) == 0
                                                     select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers);

                            //Approved History
                            var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
                                                      join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                      join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                      from wh in pwh.DefaultIfEmpty()
                                                      join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                      from fwh in ffwh.DefaultIfEmpty()
                                                          //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                                      where st.status == true && string.Compare(st.checked_by, userid) == 0 && st.created_date >= startDate && st.created_date <= endDate && string.Compare(st.stock_transfer_status, status) == 0
                                                      select new StockTransferFilterResult() { st = st, ir = ir, pro = pro, wh = wh, fwh = fwh }).ToList();
                            results.AddRange(allStockTransfers1);

                        }
                    }
                }

                

                foreach(var item in results.DistinctBy(s=>s.st).ToList())
                {

                    StockTransferViewModel stocktransfer = new StockTransferViewModel();
                    stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                    stocktransfer.stock_transfer_no = item.st.stock_transfer_no;
                    stocktransfer.item_request_id = item.st.item_request_id;
                    stocktransfer.item_request_no = item.ir.ir_no;
                    stocktransfer.created_date = item.st.created_date;
                    stocktransfer.stock_transfer_status = item.st.stock_transfer_status;
                    stocktransfer.create_by = item.st.created_by;
                    stocktransfer.project_id = item.pro.project_id;
                    stocktransfer.project_fullname = item.pro.project_full_name;
                    if (item.wh != null)
                    {
                        stocktransfer.warehouse_id = item.wh.warehouse_id;
                        stocktransfer.warehouse_name = item.wh.warehouse_name;
                    }
                    stocktransfer.from_warehouse_id = item.st.from_warehouse_id;
                    if (item.fwh != null)
                        stocktransfer.from_warehouse_name = item.fwh.warehouse_name;
                    stocktransfer.sr_status = item.st.sr_status == null ? string.Empty:string.Format("<label class='label w3-red'>{0}</label>",item.st.sr_status);
                    stocktransfer.mr_status = item.st.mr_status==null?string.Empty:string.Format("<label class='label w3-blue'>{0}</label>",item.st.mr_status);
                    stocktransfer.received_status = item.st.received_status==null?string.Empty:string.Format("<label class='label w3-green'>{0}</label>",item.st.received_status);
                    stocktransfer.created_at_text = CommonClass.ToLocalTime(Convert.ToDateTime(stocktransfer.created_date)).ToString("dd/MM/yyyy");
                    stocktransfer.created_by_text = CommonClass.GetUserFullnameByUserId(stocktransfer.create_by);
                    stocktransfer.show_status = ShowStatus.GetStockTransfershowStatus(stocktransfer.stock_transfer_status);
                    stockTransfers.Add(stocktransfer);
                }

                stockTransfers = stockTransfers.DistinctBy(s=>s.stock_transfer_id).OrderByDescending(s => s.created_date).ToList();
                return stockTransfers;
            }
        }
        public ActionResult GetStockTransferByDaterangeAndStatusListItemsAJAX(string dateRange,string status)
        {
            return Json(new { data = GetStockTransferByDaterangeAndStatusListItems(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }
    }
}