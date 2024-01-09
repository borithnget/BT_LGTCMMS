using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BT_KimMex.Class;
using BT_KimMex.Models;
using BT_KimMex.Entities;

namespace BT_KimMex.Controllers
{
    public class GoodReceiveNoteController : Controller
    {
        // GET: GoodReceiveNote
        public ActionResult Index()
        {
            return View();
        }

        // GET: GoodReceiveNote/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GoodReceiveNote/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GoodReceiveNote/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GoodReceiveNote/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GoodReceiveNote/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GoodReceiveNote/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GoodReceiveNote/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region Json Function
        public ActionResult GetTransferFromWorkshopListItemsJSON()
        {
            return Json(new { data = this.GetTransferFormWorkshopListItems() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetReturnToWorkshopListItemsJSON()
        {
            return Json(new { data = this.GetReturnToWorkshopListItems() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTransferFromWorkshopItemDetailJSON(string id)
        {
            return Json(new { data = this.GetTransferFromWorkshopItem(id) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockReturnItemDetailJSON(string id)
        {
            return Json(new { data = this.GetStockReturnItem(id) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockReturnListItemsJSON()
        {
            return Json(new { data = this.GetStockReturnListitems() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetReturnWorkshopListItemsJSON()
        {
            return Json(new { data = this.GetReturnToWorkshopListItems() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetReturnWorkshopItemJSON(string id)
        {
            return Json(new { data = this.GetReturnWorkshopItem(id) }, JsonRequestBehavior.AllowGet); 
        }
        #endregion

        #region Commom Function
        public List<TransferFromMainStockViewModel> GetTransferFormWorkshopListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<TransferFromMainStockViewModel> models = new List<TransferFromMainStockViewModel>();
                models = (from transfer in db.transferformmainstocks
                          orderby transfer.created_date descending
                          where transfer.status==true && string.Compare(transfer.stock_transfer_status,Status.Completed)==0 && transfer.is_receive_completed==false
                          select new TransferFromMainStockViewModel()
                          {
                              stock_transfer_id=transfer.stock_transfer_id,
                              stock_transfer_no=transfer.stock_transfer_no,
                          }).ToList();
                if (User.IsInRole(Role.SiteStockKeeper))
                {
                    string userid = User.Identity.GetUserId();
                    models= (from transfer in db.transferformmainstocks
                            join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                            join keeper in db.tb_stock_keeper_warehouse on wh.warehouse_id equals keeper.warehouse_id
                            orderby transfer.created_date descending
                            where transfer.status == true && string.Compare(transfer.stock_transfer_status, Status.Completed) == 0 && transfer.is_receive_completed == false && string.Compare(keeper.stock_keeper,userid)==0
                             select new TransferFromMainStockViewModel()
                             {
                                stock_transfer_id = transfer.stock_transfer_id,
                              stock_transfer_no = transfer.stock_transfer_no,
                          }).ToList();
                }
                return models;
            }
        }
        public List<ItemReceiveViewModel> GetStockReturnListitems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                models = (from sreturn in db.tb_stock_issue_return
                          orderby sreturn.created_date descending
                          where sreturn.status == true && string.Compare(sreturn.issue_return_status, Status.Completed) == 0 && sreturn.is_receive_completed==false
                          select new ItemReceiveViewModel()
                          {
                              receive_item_voucher_id=sreturn.stock_issue_return_id,
                              received_number=sreturn.issue_return_number
                          }).ToList();
                if (User.IsInRole(Role.SiteStockKeeper))
                {
                    string userid = User.Identity.GetUserId();
                    models=( from sreturn in db.tb_stock_issue_return
                            join transfer in db.tb_stock_transfer_voucher on sreturn.stock_issue_ref equals transfer.stock_transfer_id
                            join keeper in db.tb_stock_keeper_warehouse on transfer.from_warehouse_id equals keeper.warehouse_id
                            where sreturn.status == true && string.Compare(sreturn.issue_return_status, Status.Completed) == 0 && sreturn.is_receive_completed == false && string.Compare(keeper.stock_keeper,userid)==0
                             select new ItemReceiveViewModel()
                             {
                                 receive_item_voucher_id = sreturn.stock_issue_return_id,
                                 received_number = sreturn.issue_return_number
                             }).ToList();
                }
                return models;
            }
        }
        public List<ReturnMainStockViewModel> GetReturnToWorkshopListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ReturnMainStockViewModel> models = new List<ReturnMainStockViewModel>();
                models = (from wreturn in db.tb_return_main_stock
                          orderby wreturn.create_date descending
                          where wreturn.status == true && string.Compare(wreturn.return_main_stock_status, Status.Completed) == 0
                          select new ReturnMainStockViewModel()
                          {
                              return_main_stock_id=wreturn.return_main_stock_id,
                              return_main_stock_no=wreturn.return_main_stock_no,
                          }).ToList();
                return models;
            }
        }
        public TransferFromMainStockViewModel GetTransferFromWorkshopItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                TransferFromMainStockViewModel model = new TransferFromMainStockViewModel();

                model = (from transfer in db.transferformmainstocks
                         join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                         join project in db.tb_project on mr.ir_project_id equals project.project_id
                         //join site in db.tb_site on project.site_id equals site.site_id
                         //join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                         join wh in db.tb_warehouse on project.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                         where string.Compare(transfer.stock_transfer_id, id) == 0
                         select new TransferFromMainStockViewModel()
                         {
                             item_request_id=transfer.item_request_id,
                             item_request_no=mr.ir_no,
                             project_id=project.project_id,
                             project_fullname=project.project_full_name,
                             warehouse_id=wh.warehouse_id,
                             warehouse_name=wh.warehouse_name,
                         }).FirstOrDefault();

                model.inventoryDetails = (from detail in db.tb_transfer_frommain_stock_detail
                                          join item in db.tb_product on detail.st_item_id equals item.product_id
                                          join tunit in db.tb_unit on detail.unit equals tunit.Id
                                          join unit in db.tb_unit on item.product_unit equals unit.Id
                                          join wh in db.tb_warehouse on detail.st_warehouse_id equals wh.warehouse_id
                                          where string.Compare(detail.st_ref_id, id) == 0
                                          select new InventoryViewModel()
                                          {
                                              inventory_id=detail.st_detail_id,
                                              product_id=detail.st_item_id,
                                              itemCode=item.product_code,
                                              itemName=item.product_name,
                                              itemUnit=item.product_unit,
                                                itemUnitName=unit.Name,
                                                warehouse_id=detail.st_warehouse_id,
                                                total_quantity=detail.quantity,
                                                remain_quantity=detail.received_remain_quantity,
                                                unit=detail.unit,
                                                unitName=tunit.Name,
                                                warehouseName=wh.warehouse_name,
                                                from_warehouse_id=EnumConstants.WORKSHOP,
                                                from_warehouse_name="Workshop",

                                          }).ToList();
                return model;
            }
        }
        public StockIssueReturnViewModel GetStockReturnItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                StockIssueReturnViewModel model = new StockIssueReturnViewModel();
                model = (from sreturn in db.tb_stock_issue_return
                         join transfer in db.tb_stock_transfer_voucher on sreturn.stock_issue_ref equals transfer.stock_transfer_id
                         join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                         join project in db.tb_project on mr.ir_project_id equals project.project_id
                         //join site in db.tb_site on project.site_id equals site.site_id
                         //join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                         join wh in db.tb_warehouse on project.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                         where string.Compare(sreturn.stock_issue_return_id, id) == 0
                         select new StockIssueReturnViewModel()
                         {
                             stock_issue_return_id=sreturn.stock_issue_return_id,
                             stock_issue_ref=mr.ir_no,
                             stock_issue_number=transfer.stock_transfer_no,
                             project_id = project.project_id,
                             project_fullname = project.project_full_name,
                             warehouse_id = wh.warehouse_id,
                             warehouse_name = wh.warehouse_name,
                         }).FirstOrDefault();
                model.inventories = (from detail in db.tb_inventory_detail
                                          join product in db.tb_product on detail.inventory_item_id equals product.product_id
                                          join punit in db.tb_unit on product.product_unit equals punit.Id
                                          join iunit in db.tb_unit on detail.unit equals iunit.Id
                                          join wh in db.tb_warehouse on detail.inventory_warehouse_id equals wh.warehouse_id
                                          where string.Compare(detail.inventory_ref_id, id) == 0
                                          select new InventoryViewModel()
                                          {
                                              inventory_id = detail.inventory_detail_id,
                                              product_id = detail.inventory_item_id,
                                              itemCode = product.product_code,
                                              itemName = product.product_name,
                                              itemUnit = product.product_unit,
                                              itemUnitName = punit.Name,
                                              warehouse_id = detail.inventory_warehouse_id,
                                              total_quantity = detail.quantity,
                                              remain_quantity = detail.remain_quantity,
                                              unit = detail.unit,
                                              unitName = iunit.Name,
                                              warehouseName = wh.warehouse_name,
                                          }).ToList();
                return model;
            }
            
        }
        public ReturnMainStockViewModel GetReturnWorkshopItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                ReturnMainStockViewModel model = new ReturnMainStockViewModel();
                model = (from wreturn in db.tb_return_main_stock
                         join wtranfer in db.transferformmainstocks on wreturn.return_ref_id equals wtranfer.stock_transfer_id
                         join mr in db.tb_item_request on wtranfer.item_request_id equals mr.ir_id
                         join project in db.tb_project on mr.ir_project_id equals project.project_id
                         join site in db.tb_site on project.site_id equals site.site_id
                         join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                         where string.Compare(wreturn.return_main_stock_id, id) == 0
                         select new ReturnMainStockViewModel()
                         {
                            mr_id=mr.ir_id,
                            mr_number=mr.ir_no,
                             project_id = project.project_id,
                             project_fullname = project.project_full_name,
                             warehouse_id = wh.warehouse_id,
                             warehouse_name = wh.warehouse_name,
                         }).FirstOrDefault();
                model.inventoryDetails= (from detail in db.tb_return_mainstock_detail
                                        join item in db.tb_product on detail.main_stock_detail_item equals item.product_id
                                        join tunit in db.tb_unit on detail.unit equals tunit.Id
                                        join unit in db.tb_unit on item.product_unit equals unit.Id
                                        join wh in db.tb_warehouse on detail.main_stock_warehouse_id equals wh.warehouse_id
                                        where string.Compare(detail.main_stock_detail_ref,id)==0
                                        select new InventoryViewModel()
                                        {
                                            inventory_id = detail.main_stock_detail_id,
                                            product_id = detail.main_stock_detail_item,
                                            itemCode = item.product_code,
                                            itemName = item.product_name,
                                            itemUnit = item.product_unit,
                                            itemUnitName = unit.Name,
                                            warehouse_id = detail.main_stock_warehouse_id,
                                            total_quantity = detail.quantity,
                                            remain_quantity = detail.remain_quantity,
                                            unit = detail.unit,
                                            unitName = tunit.Name,
                                            warehouseName = wh.warehouse_name,
                                            from_warehouse_id = EnumConstants.WORKSHOP,
                                            from_warehouse_name = "Workshop",

                                        }).ToList();
                return model;
            }
        }
        #endregion
    }
}
