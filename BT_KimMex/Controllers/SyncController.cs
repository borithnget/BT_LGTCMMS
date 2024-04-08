using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using Spire.Xls;
using EasyXLS;

namespace BT_KimMex.Controllers
{
    public class SyncController : Controller
    {
        // GET: Sync
        public ActionResult MaterailRequestPOComplete()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<string> productIds = new List<string>();

                var poReports = db.tb_po_report.ToList();
                foreach(var opor in poReports)
                {
                    //Get PO product id
                    var poProducts = (from por in db.tb_po_report
                                      join pod in db.tb_purchase_order_detail on por.po_ref_id equals pod.purchase_order_id
                                      join pos in db.tb_po_supplier on pod.po_detail_id equals pos.po_detail_id
                                      where string.Compare(pos.supplier_id, por.po_supplier_id) == 0 && pos.is_selected == true && string.Compare(opor.po_report_id,por.po_report_id)==0
                                      select pod).ToList();
                    foreach (var pro in poProducts)
                    {
                        productIds.Add(pro.item_id);
                    }

                    //Update Materail Request Item relate with PO
                    string mrId = (from q in db.tb_purchase_order
                                   join pr in db.tb_purchase_requisition on q.item_request_id equals pr.purchase_requisition_id

                                   where string.Compare(q.purchase_order_id, opor.po_ref_id) == 0
                                   select pr.material_request_id).FirstOrDefault();
                    var ir_detail1 = db.tb_ir_detail1.Where(s => string.Compare(s.ir_id, mrId) == 0).FirstOrDefault();
                    foreach (var proId in productIds)
                    {
                        var mrDetail = db.tb_ir_detail2.Where(s => string.Compare(s.ir_detail1_id, ir_detail1.ir_detail1_id) == 0 && string.Compare(s.ir_item_id, proId) == 0).FirstOrDefault();
                        if (mrDetail != null)
                        {
                            mrDetail.is_po = true;
                            db.SaveChanges();
                        }
                    }

                    var MRItemNotYetPO = db.tb_ir_detail2.Where(s => string.Compare(s.ir_detail1_id, ir_detail1.ir_detail1_id) == 0 && s.is_po == false).Count();
                    tb_item_request mr = db.tb_item_request.Find(mrId);
                    mr.is_po_completed = MRItemNotYetPO == 0 ? true : false;
                    db.SaveChanges();

                }
                
            }
            return View();
        }
        public ActionResult SyncPOReportWithGRN()
        {
            return View();
        }
        public ActionResult SyncPOReportWithGRNAJAX(string dateRange,string poReportType)
        {
            AJAXResultModel result = new AJAXResultModel();
            List<tb_po_report> models = new List<tb_po_report>();
            List<tb_po_report> results = new List<tb_po_report>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1);
                
                if (string.Compare(poReportType, "0") == 0)
                {
                    models = db.tb_po_report.Where(s => s.created_date >= startDate && s.created_date <= endDate).ToList();
                }else 
                {
                    bool poType = Convert.ToBoolean(poReportType);
                    models = db.tb_po_report.Where(s => s.created_date >= startDate && s.created_date <= endDate && s.vat_status == poType).ToList();
                }

                foreach(var obj in models)
                {
                    tb_po_report poReport = db.tb_po_report.Find(obj.po_report_id);
                    if (poReport != null)
                    {
                        var poItems = (from purd in db.tb_purchase_order_detail
                                       join purs in db.tb_po_supplier on purd.po_detail_id equals purs.po_detail_id
                                       where string.Compare(purd.purchase_order_id, poReport.po_ref_id) == 0 && string.Compare(purd.item_status, "approved") == 0
                                       && purs.is_selected == true && string.Compare(purs.supplier_id, poReport.po_supplier_id) == 0 && purd.item_vat == poReport.vat_status
                                       select new { purd, purs }).ToList();
                        bool isPOCompleteReceived = poItems.Sum(s => s.purd.remain_quantity) > 0 ? false : true;
                        poReport.is_completed = isPOCompleteReceived;
                        db.SaveChanges();
                        results.Add(poReport);

                    }
                }

            }
            catch(Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.Message;
            }
            return Json(new { result = result,data=results }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReStockByRemoveGRNItem(string id)
        {
            List<InventoryViewModel> models = new List<InventoryViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var results = (from inv in db.tb_inventory
                               join grnd in db.tb_received_item_detail on inv.product_id equals grnd.ri_item_id
                               join grn in db.tb_receive_item_voucher on grnd.ri_ref_id equals grn.receive_item_voucher_id
                               join prod in db.tb_product on inv.product_id equals prod.product_id
                               orderby inv.product_id, inv.inventory_date descending
                               where string.Compare(grn.receive_item_voucher_id, id) == 0 && string.Compare(inv.warehouse_id, grnd.ri_warehouse_id) == 0 && inv.inventory_date >= grn.created_date
                               select new { inv, grnd, grn,prod}).ToList();

                foreach(var rs in results)
                {
                    InventoryViewModel model = new InventoryViewModel();
                    model.inventory_id = rs.inv.inventory_id;
                    model.inventory_date = rs.inv.inventory_date;
                    model.inventory_status_id = rs.inv.inventory_status_id;
                    model.product_id = rs.inv.product_id;
                    model.itemCode = string.Format("{0} {1}", rs.prod.product_code, rs.prod.product_name);
                    model.warehouse_id = rs.inv.warehouse_id;
                    model.total_quantity = rs.inv.total_quantity;
                    model.in_quantity = rs.inv.in_quantity;
                    model.out_quantity = rs.inv.out_quantity;
                    if (string.Compare(model.inventory_status_id, "7") == 0)
                    {
                        var grn = db.tb_receive_item_voucher.Find(rs.inv.ref_id);
                        model.ref_id =string.Format("{0}/{1}",grn.received_number,grn.po_report_number);
                    }
                    models.Add(model);
                }
                         

            }catch(Exception ex)
            {

            }
            return View(models);
        }
        public ActionResult DeleteInventoryItem(string inv_id)
        {
            AJAXResultModel result = new AJAXResultModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var deletedItem = db.tb_inventory.Find(inv_id);
                if (deletedItem != null)
                {
                    var itemlatests = db.tb_inventory.OrderBy(s => s.inventory_date)
                        .Where(s=>string.Compare(s.product_id,deletedItem.product_id)==0 && string.Compare(s.warehouse_id,deletedItem.warehouse_id)==0 && s.inventory_date>deletedItem.inventory_date).ToList();
                    tb_inventory itemBeforeDeletedItem = db.tb_inventory.OrderByDescending(s => s.inventory_date).Where(s => string.Compare(s.product_id, deletedItem.product_id) == 0 && string.Compare(s.warehouse_id, deletedItem.warehouse_id) == 0 && s.inventory_date < deletedItem.inventory_date).FirstOrDefault();
                    if(itemlatests.Count()>0 && itemBeforeDeletedItem != null)
                    {
                        foreach(var rs in itemlatests)
                        {
                            string currentInventoryId = rs.inventory_id;
                            tb_inventory currentInventory = db.tb_inventory.Find(currentInventoryId);
                            decimal totalQty =Convert.ToDecimal((itemBeforeDeletedItem.total_quantity + rs.in_quantity) - rs.out_quantity);
                            currentInventory.total_quantity = totalQty;
                            db.SaveChanges();

                            itemBeforeDeletedItem = currentInventory;
                        }
                    }
                    db.tb_inventory.Remove(deletedItem);
                    db.SaveChanges();
                }
                

            }catch(Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.Message;
            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveDuplicateItemInventoryMainstock(string ref_id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var results = (from inv in db.tb_inventory
                               orderby inv.product_id, inv.inventory_date descending
                               where string.Compare(ref_id, inv.ref_id) == 0 && string.Compare(inv.warehouse_id, "1") == 0
                               select inv).ToList();
                var groupResults = results.GroupBy(s => s.product_id).Select(s => new { key = s.Key, list = s.OrderByDescending(ss => ss.inventory_date).ToList() }).ToList();
                foreach(var grs in groupResults)
                {
                    if (grs.list.Count() > 1)
                    {
                        for(int i = 0; i < grs.list.Count() - 1; i++)
                        {
                            tb_inventory inv = grs.list[i];
                            tb_inventory_deleted inventory_Deleted = new tb_inventory_deleted();
                            inventory_Deleted.inventory_id = inv.inventory_id;
                            inventory_Deleted.inventory_date = inv.inventory_date;
                            inventory_Deleted.inventory_status_id = inv.inventory_status_id;
                            inventory_Deleted.warehouse_id = inv.warehouse_id;
                            inventory_Deleted.product_id = inv.product_id;
                            inventory_Deleted.total_quantity = inv.total_quantity;
                            inventory_Deleted.in_quantity = inv.in_quantity;
                            inventory_Deleted.out_quantity = inv.out_quantity;
                            inventory_Deleted.ref_id = inv.ref_id;
                            inventory_Deleted.remark = inv.remark;
                            inventory_Deleted.unit = inv.unit;
                            db.tb_inventory_deleted.Add(inventory_Deleted);
                            db.SaveChanges();

                            tb_inventory dInv = db.tb_inventory.Find(inv.inventory_id);
                            db.tb_inventory.Remove(dInv);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return View();
        }

        public ActionResult ConvertXLStoXLSX()
        {
            var file_path = Server.MapPath("~/Documents/POReport/test1.xls");
            var new_file_name = string.Format("test{0}{1}{2}{3}{4}.xlsx", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            var new_file_path = Server.MapPath("~/Documents/POReport/") + new_file_name;

            Workbook workbook = new Workbook();

            workbook.LoadFromFile(file_path);
            workbook.SaveToFile(new_file_path, ExcelVersion.Version2016);

            // Create an instance of the class used to import/export Excel files
            //ExcelDocument workbook = new ExcelDocument();

            //// Import XLS file
            //workbook.easy_LoadXLSFile(file_path);

            //// Export XLSX file
            //workbook.easy_WriteXLSXFile(new_file_path) ;

            return View();
        }

        /// <summary>
        /// Using Microsoft.Office.Interop to convert XLS to XLSX format, to work with EPPlus library
        /// </summary>
        /// <param name="filesFolder"></param>
        //public static string ConvertXLS_XLSX(FileInfo file)
        //{
        //    var app = new Microsoft.Office.Interop.Excel.Application();
        //    var xlsFile = file.FullName;
        //    var wb = app.Workbooks.Open(xlsFile);
        //    var xlsxFile = xlsFile + "x";
        //    wb.SaveAs(Filename: xlsxFile, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
        //    wb.Close();
        //    app.Quit();
        //    return xlsxFile;
        //}

        public ActionResult AdjustmentNegativeBalance(string id)
        {
            try
            {
                List<tb_inventory> results = new List<tb_inventory>();

                kim_mexEntities db = new kim_mexEntities();
                var inventories = db.tb_inventory.OrderByDescending(s => s.inventory_date).Where(s => string.Compare(s.warehouse_id, id) == 0).GroupBy(s=>s.product_id).Select(s=>new {product_id=s.Key, list = s.ToList() }).ToList();
                foreach(var inv in inventories)
                {
                    var latest_qty = inv.list.OrderByDescending(s => s.inventory_date).Where(s => s.total_quantity < 0).FirstOrDefault();
                    if (latest_qty != null)
                    {
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = string.Empty;
                        inventory.inventory_status_id = "1";
                        inventory.warehouse_id = "1";
                        inventory.product_id = latest_qty.product_id;
                        inventory.out_quantity = 0;
                        inventory.in_quantity = 0;
                        inventory.total_quantity = 0;
                        inventory.remark = "Reset negative qty to zero by system.";
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                        results.Add(latest_qty);
                    }

                }
            }catch(Exception ex)
            {

            }
            return View();
        }

    }
}