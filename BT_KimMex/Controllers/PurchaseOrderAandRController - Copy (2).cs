//using BT_KimMex.Entities;
//using BT_KimMex.Models;
//using Microsoft.AspNet.Identity;
//using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Collections;
//using BT_KimMex.Class;

//namespace BT_KimMex.Controllers
//{
//    public class PurchaseOrderAandRController : Controller
//    {
        //private object purchase;
        //private IEnumerable posupplier;

        // GET: PurchaseOrderAandR
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: PurchaseOrderAandR/Details/5
        //public ActionResult Details(int id)
        //{
        //    var model = GetPurchseOrderAandRViewModel(id);
        //    var supplierName = string.Empty;
        //    var amount = 0m;
        //    if (model != null)
        //    {
        //        using(var db=  new kim_mexEntities())
        //        {
        //            supplierName = db.tb_supplier.FirstOrDefault(x => x.supplier_id == model.supplier_id)?.supplier_name ?? string.Empty;
        //            var po = db.tb_purchase_order.FirstOrDefault(x => x.purchase_oder_number == model.purchase_oder_number && x.status == true);
        //            var amo = from pod in db.tb_purchase_order_detail
        //                      join po_sup in db.tb_po_supplier on pod.po_detail_id equals po_sup.po_detail_id
        //                      where pod.purchase_order_id == po.purchase_order_id
        //                      select new
        //                      {
        //                          amount = (pod.quantity == null || pod.unit_price == null )? 0m : pod.quantity * pod.unit_price
        //                      };
        //            amount = amo?.FirstOrDefault()?.amount ?? 0m;
        //        }
        //    }
        //    ViewBag.supplier_name = supplierName;
        //    ViewBag.Amount = amount;
        //    return View(model);
        //}

        //public Purchase_OrderAandR GetPurchseOrderAandRViewModel(int id)
        //{
        //    using(var db  = new kim_mexEntities())
        //    {
        //        var pos = db.Purchase_OrderAandR.FirstOrDefault(x => x.Purchase_order_id == id);
        //        return pos;
        //    }
        //}

        // GET: PurchaseOrderAandR/Create
    
  //      public ActionResult Create ()
  //      {
  //          var supplierName = string.Empty;
  //          var ponumber = this.GetponumberDropdownList();
  //          ViewBag.ApprovealNumber = this.GenerateApprovalNumber();
  //          ViewBag.PONumberID = new SelectList(ponumber, "purchase_order_id", "purchase_oder_number");
  //          return View();
  //      }

  //      [HttpPost]
  //      public ActionResult CreateNew(string purchase_order_number, string purchase_number,
  //          List<string> approves, List<string> rejects)
  //      {
  //          try
  //          {
  //              if (!string.IsNullOrEmpty(purchase_order_number))
  //              {
  //                  using (var db = new kim_mexEntities())
  //                  {
  //                      var minDate = new DateTime(1900, 1, 1);
  //                      var po = db.tb_purchase_order.FirstOrDefault(x => x.purchase_order_id == purchase_order_number);

  //                      if (approves != null)
  //                      {
  //                          foreach (var appr in approves)
  //                          {
  //                              var poApprove = new Purchase_OrderAandR()
  //                              {
  //                                  Purchar_Number = purchase_number,
  //                                  purchase_oder_number = po?.purchase_oder_number ?? string.Empty,
  //                                  Create_date = DateTime.Now,
  //                                  status = "Approved",
  //                                  Action = "Approved",
  //                                  supplier_id = appr,
  //                                  checked_by = User.Identity.GetUserId(),
  //                                  checked_date = DateTime.Now,
  //                                  Update_by = string.Empty,
  //                                  Update_Date = minDate,
  //                                  Upprove_by = User.Identity.GetUserId(),
  //                                  Uprove_date = DateTime.Now,
  //                                  rejected_by = "",
  //                                  rejected_date = minDate,
  //                                  Item_request_id = "",
  //                              };
  //                              db.Purchase_OrderAandR.Add(poApprove);
  //                          }
  //                      }
  //                      if (rejects != null)
  //                      {
  //                          foreach (var rej in rejects)
  //                          {
  //                              var poReject = new Purchase_OrderAandR()
  //                              {
  //                                  Purchar_Number = purchase_number,
  //                                  purchase_oder_number = po?.purchase_oder_number ?? string.Empty,
  //                                  Create_date = DateTime.Now,
  //                                  status = "Rejected",
  //                                  Action = "Rejected",
  //                                  supplier_id = rej,
  //                                  checked_by = User.Identity.GetUserId(),
  //                                  checked_date = DateTime.Now,
  //                                  Update_by = string.Empty,
  //                                  Update_Date = minDate,
  //                                  Upprove_by = "",
  //                                  Uprove_date = minDate,
  //                                  rejected_by = User.Identity.GetUserId(),
  //                                  rejected_date = DateTime.Now,
  //                                  Item_request_id = "",
  //                              };
  //                              db.Purchase_OrderAandR.Add(poReject);
  //                          }
  //                      }
  //                      db.SaveChanges();
  //                  }
  //              }

  //              return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
  //          }
  //          catch(Exception ex)
  //          {
  //              return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
  //          }
  //      }


        
  //      private object GetSuppliersList()
  //      {
  //          List<tb_Purchase_OrderAandR> purchaseoder = new List<tb_Purchase_OrderAandR>();
  //          throw new NotImplementedException();
  //      }

  //      public List<tb_purchase_order> GetponumberDropdownList()
  //      {
  //          List<tb_purchase_order> purchaseorder = new List<tb_purchase_order>();
  //          using (kim_mexEntities db = new kim_mexEntities())
  //          {
  //              purchaseorder = db.tb_purchase_order.OrderBy(m => m.purchase_oder_number).Where(m => m.status == true).ToList();
  //          }
  //          return purchaseorder;
  //      }

  //      [HttpPost]
  //public ActionResult Create ( PurchaseOrderAandRViewModel model)
  //      {
  //         if (ModelState.IsValid)
  //          {
  //              using ( kim_mexEntities db = new kim_mexEntities())
  //              {
  //                  tb_Purchase_OrderAandR purchaseorder = new tb_Purchase_OrderAandR();
  //                  purchaseorder.purchase_order_id = Guid.NewGuid().ToString();
  //                  purchaseorder.purchase_order_number = model.purchase_oder_number;

  //              }
                
  //          }

  //          return View();
  //      }

      //  POST: PurchaseOrderAandR/Create
      //[HttpPost]
      //  public ActionResult Create(FormCollection collection)
      //  {
      //      try
      //      {
      //          //using (kim_mexEntities db = new kim_mexEntities)
      //          // TODO: Add insert logic here

      //          return RedirectToAction("Index");
      //      }
      //      catch
      //      {
      //          return View();
      //      }
      //  }

        // GET: PurchaseOrderAandR/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: PurchaseOrderAandR/Edit/5
        //[HttpPost]
        //public ActionResult Edit(string purchase_order_number, string purchase_number,
        //    List<string> approves, List<string> rejects)
        //{
        //    try
        //    {



        //        return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //public ActionResult Approve(string id)
        //{
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        return View();
        //    }
        //}



        //​​​      public ActionResult Reject(string id)
        //        {
        //            using (kim_mexEntities db = new kim_mexEntities())
        //            {

        //                tb_Purchase_OrderAandR purchaseorderItemId = new tb_Purchase_OrderAandR();
        //                purchaseorderItemId.supplier_element = "Rejected";
        //                purchaseorderItemId.Upprove_by = User.Identity.GetUserId();
        //                purchaseorderItemId.Uprove_date = Class.CommonClass.ToLocalTime(DateTime.Now);
        //                db.SaveChanges();
        //                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }

        // GET: PurchaseOrderAandR/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    //using (kim_mexEntities db = new kim_mexEntities()
            //{
               
            // })
        //    return View();
        //}

        // POST: PurchaseOrderAandR/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
                // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //public ActionResult GetPurchaseOrderApprovalDataTable()
        //{
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        List<PurchaseOrderAandRViewModel> purchaseorder = new List<PurchaseOrderAandRViewModel>();
        //        purchaseorder = this.GetPurchaseOrderApprovalList();
        //        return Json(new { result = "success", data = purchaseorder }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public List<PurchaseOrderAandRViewModel> GetPurchaseOrderApprovalList()
        //{
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        var q = from po in db.Purchase_OrderAandR
        //                join por in db.tb_purchase_order on po.purchase_oder_number equals por.purchase_oder_number
        //                join su in db.tb_supplier  on po.supplier_id equals  su. supplier_id
        //                select new PurchaseOrderAandRViewModel
        //                {
        //                    Purchase_order_id = po.Purchase_order_id,
                           // purchase_oder_number = po.purchase_oder_number,
                            //purchase_oder_number = por.purchase_oder_number,
                            //Purchar_Number = po.Purchar_Number,
                            //ref_id = po.ref_id,
        //                    checked_by = po.checked_by,
        //                    checked_date = po.checked_date,
        //                    Create_date = po.Create_date,
        //                    Action = po.Action,
        //                    rejected_by = po.rejected_by,
        //                    rejected_date = po.rejected_date,
        //                    status = po.status,
        //                    Update_by = po.Update_by,
        //                    Update_Date = po.Update_Date,
        //                    Upprove_by = po.Upprove_by,
        //                    Uprove_date = po.Uprove_date,

        //                };
        //        return q.OrderByDescending(x => x.Create_date).ToList();
        //    }
        //}

        //public List<PurchaseOrderViewModel> GetPurchaseOrderViewModel()
        //{
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        var q = from de in db.tb_purchase_order_detail
        //                join po in db.tb_purchase_order on de.purchase_order_id equals po.purchase_order_id
        //                join su_po in db.tb_po_supplier on de.po_detail_id equals su_po.po_detail_id
        //                join su in db.tb_supplier on su_po.supplier_id equals su.supplier_id
        //                select new PurchaseOrderDetailViewModel
        //                {
        //                    supplier_name = su.supplier_name,

        //                };
                //return q.OrderByDescending(x=> x.created_date).ToList();  
        //        return new List<PurchaseOrderViewModel>();
        //    }
        //}
       
        //public string GenerateApprovalNumber()
        //{
        //    string quoteNo = string.Empty;
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        string last_no = "", quoteNum;
        //        string number = db.Purchase_OrderAandR.OrderByDescending(x => x.Purchar_Number).FirstOrDefault()?.Purchar_Number;
        //        if (string.IsNullOrEmpty(number))
        //            last_no = "001";
        //        else
        //        {
        //            number = number.Substring(number.Length - 3, 3);
        //            int num = Convert.ToInt32(number) + 1;
        //            if (num.ToString().Length == 1) last_no = "00" + num;
        //            else if (num.ToString().Length == 2) last_no = "0" + num;
        //            else if (num.ToString().Length == 3) last_no = num.ToString();
        //        }
        //        string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
        //        string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
        //        quoteNo = "POA-" + yy + "-" + mm + "-" + last_no;
        //    }
        //    return quoteNo;
        //}

        //public ActionResult GetPOSupplierDataTabel(string id)
        //{
        //    var po_supplier = GetPOSupplierItem(id);
        //    return Json(new { result = "success", data = po_supplier}, JsonRequestBehavior.AllowGet);
        //}

        //public PurchaseOrderViewModel GetPOSupplierItem(string id)
        //{
        //    PurchaseOrderViewModel model = new PurchaseOrderViewModel();
        //    List<PurchaseOrderItemSupplier> poSuppliers = new List<PurchaseOrderItemSupplier>();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        model = (from po in db.tb_purchase_order
        //                 join ir in db.tb_item_request on po.item_request_id equals ir.ir_id
        //                 join proj in db.tb_project on ir.ir_project_id equals proj.project_id
        //                 where po.purchase_order_id == id
        //                 select new PurchaseOrderViewModel()
        //                 {
        //                     purchase_order_id = po.purchase_order_id,
        //                     item_request_id = po.item_request_id,
        //                     ir_no = ir.ir_no,
        //                     project_full_name = proj.project_full_name,
        //                     purchase_oder_number = po.purchase_oder_number,
        //                     purchase_order_status = po.purchase_order_status,
        //                     created_date = po.created_date,
        //                     POLNumber = po.pol_project_short_name_number,
        //                     ShippingTo = po.shipping_to
        //                 }).FirstOrDefault();
        //        if (model != null)
        //        {
        //            if (string.Compare(model.purchase_order_status, "Completed") == 0)
        //            {
        //                List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
        //                poDetails = (from dPO in db.tb_purchase_order_detail
        //                             join item in db.tb_product on dPO.item_id equals item.product_id
        //                             where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "approved"
        //                             select new PurchaseOrderDetailViewModel() {
        //                                 po_detail_id = dPO.po_detail_id,
        //                                 purchase_order_id = dPO.purchase_order_id,
        //                                 item_id = dPO.item_id,
        //                                 quantity = dPO.quantity,
        //                                 product_code = item.product_code,
        //                                 product_name = item.product_name,
        //                                 product_unit = item.product_unit,
        //                                 unit_price = dPO.unit_price,
        //                                 item_status = dPO.item_status,
        //                                 po_quantity = dPO.po_quantity }).ToList();
        //                foreach (var pod in poDetails)
        //                {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
//                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
//                            pois = (from pos in db.tb_po_supplier
//                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
//                                    orderby pos.sup_number
//                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
//                                    select new PurchaseOrderItemSupplier() {
//                                        po_supplier_id = pos.po_supplier_id,
//                                        po_detail_id = pos.po_detail_id,
//                                        unit_price = pos.unit_price,
//                                        supplier_id = sup.supplier_id,
//                                        supplier_name = sup.supplier_name,
//                                        sup_number = pos.sup_number,
//                                        is_selected = pos.is_selected,
//                                        vat = pos.vat,
//                                        Reason = pos.Reason,                                     
//                                        po_quantity = pos.po_qty,
//                                       supplier_address = sup.supplier_address,
//                                        supplier_phone = sup.supplier_phone,
//                                        supplier_email = sup.supplier_email }).FirstOrDefault();
//                            pois.po_quantity = pod.quantity;
//                            //pod.poSuppliers = pois;
//                            poSuppliers.Add(pois);
//                            pod.poSuppliers.Add(pois);
//                        }
//                        model.poDetails = poDetails;

//                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
//                        foreach (var sup in suppliers)
//                        {
//                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();
//                            var po_supplier = poSuppliers.FirstOrDefault(x => x.supplier_id == sup);
//                            if (supplier != null)
//                            {
//                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() {
//                                    supplier_id = sup,
//                                    supplier_name = supplier.supplier_name,
//                                    supplier_email = supplier.supplier_email,
//                                    supplier_address = supplier.supplier_address,
//                                    supplier_phone = supplier.supplier_phone,
//                                    po_quantity = po_supplier?.po_quantity,
//                                    unit_price = po_supplier?.unit_price,
//                                });
//                            }

//                        }
//                    }
//                    else if (string.Compare(model.purchase_order_status, "Approved") == 0)
//                    {
//                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
//                        poDetails = (from dPO in db.tb_purchase_order_detail
//                                     join item in db.tb_product on dPO.item_id equals item.product_id
//                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "Pending"
//                                     select new PurchaseOrderDetailViewModel() {
//                                         po_detail_id = dPO.po_detail_id,
//                                         purchase_order_id = dPO.purchase_order_id,
//                                         item_id = dPO.item_id,
                                         
//                                         quantity = dPO.quantity,
//                                         product_code = item.product_code,
//                                         product_name = item.product_name,
//                                         product_unit = item.product_unit,
//                                         unit_price = dPO.unit_price,
//                                         item_status = dPO.item_status,
//                                         po_quantity = dPO.po_quantity }).ToList();
//                        foreach (var pod in poDetails)
//                        {
//                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
//                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
//                            pois = (from pos in db.tb_po_supplier
//                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
//                                    orderby pos.sup_number
//                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
//                                    select new PurchaseOrderItemSupplier() {
//                                        po_supplier_id = pos.po_supplier_id,
//                                        po_detail_id = pos.po_detail_id,
//                                        unit_price = pos.unit_price,
//                                        supplier_id = sup.supplier_id,
//                                        supplier_name = sup.supplier_name,
//                                        sup_number = pos.sup_number,
//                                        is_selected = pos.is_selected,
//                                        Reason = pos.Reason,
//                                        po_quantity = pos.po_qty,
//                                        supplier_address = sup.supplier_address,
//                                        supplier_phone = sup.supplier_phone,
//                                        supplier_email = sup.supplier_email }).FirstOrDefault();
//                            //pod.poSuppliers = pois;
//                            pois.po_quantity = pod.quantity;
//                            poSuppliers.Add(pois);
//                            pod.poSuppliers.Add(pois);
//                        }
//                       // model.poDetails = poDetails;

//                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
//                        foreach (var sup in suppliers)
//                        {
//                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();
//                            var po_supplier = poSuppliers.FirstOrDefault(x => x.supplier_id == sup);
//                            if (supplier != null)
//                            {
//                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() {
//                                    supplier_id = sup,
//                                    supplier_name = supplier.supplier_name,
//                                    supplier_email = supplier.supplier_email,
//                                    supplier_address = supplier.supplier_address,
//                                    supplier_phone = supplier.supplier_phone ,
//                                    po_quantity = po_supplier?.po_quantity,
//                                    unit_price = po_supplier?.unit_price,
//                                });

//                            }

//                        }
//                    }

//                }
//            }
//            return model;
//        }

//        public ActionResult Cancel(string id)
//        {
//            PurchaseOrderViewModel model = new PurchaseOrderViewModel();          
//            model = this.GetPOSupplierItem(id);
//            return View(model);
//        }

//        public ActionResult Cancel(string id, string[] suppliers)
//        {
//            using (kim_mexEntities db = new kim_mexEntities())
//            {
//                if (suppliers == null || suppliers.Count() == 0)
//                {
//                    return Json(new { result = "error", message = "Please select at least one supplier to cancel." }, JsonRequestBehavior.AllowGet);
//                }
//                for (int i = 0; i < suppliers.Count(); i++)
//                {
//                    var poDetails = db.tb_purchase_order_detail.Where(x => x.purchase_order_id == id && string.Compare(x.item_status, "approved") == 0).ToList();
//                    foreach (var poDetail in poDetails)
//                    {
//                        var poSuppliers = db.tb_po_supplier.Where(x => x.po_detail_id == poDetail.po_detail_id && x.is_selected == true).ToList();
//                        foreach (var poSupplier in poSuppliers)
//                        {
//                            if (string.Compare(poSupplier.supplier_id, suppliers[i].ToString()) == 0)
//                            {
//                                tb_purchase_order_detail pDetail = db.tb_purchase_order_detail.Where(x => x.po_detail_id == poDetail.po_detail_id).FirstOrDefault();
//                                pDetail.item_status = "cancelled";
//                                db.SaveChanges();
//                            }
//                        }
//                    }
//                }

//                Purchase_OrderAandR po = db.Purchase_OrderAandR.Find(id);
//                Class.ItemRequest.RollbackItemRequestRemainQuantity(po.Item_request_id, id, true, true, "cancelled");
//                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
//            }
//        }


//    }
//}
