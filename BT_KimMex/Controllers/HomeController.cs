using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
//using Syncfusion.Pdf;
//using Syncfusion.Pdf.Graphics;
//using Syncfusion.Drawing;
using System.IO;
using System.Web.Hosting;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Class.Importing importCalss = new Class.Importing();
        public ActionResult Index()
        {
            Models.Home model = new Models.Home();
           ItemReceiveViewModel approvedGRNListItems = new ItemReceiveViewModel();
            using (BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                string userid = User.Identity.GetUserId();
                //tb_receive_item_voucher itemReceive = new tb_receive_item_voucher();
                model.PendingBOQ = db.tb_build_of_quantity.Where(x => x.status == true && x.boq_status == "Pending").Count();
                model.ApprovedBOQ = db.tb_build_of_quantity.Where(x => x.status == true && x.boq_status == "Approved").Count();
                model.pendingIR = db.tb_item_request.Where(x => x.status == true && x.ir_status == "Pending").Count();
                model.PendingPO = db.tb_purchase_order.Where(x => x.status == true && x.purchase_order_status == "Pending").Count();
                model.ApprovedPO = db.tb_purchase_order.Where(x => x.status == true && x.purchase_order_status == "Approved").Count();
                #region Count Pending Approval 
                model.PendingIRec = db.tb_receive_item_voucher.Where(x => x.status == true && x.received_status == Status.Pending).Count();
                model.PendingIRec = ItemReceiveViewModel.CountApproval(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.QAQCOfficer), User.IsInRole(Role.SiteManager), User.Identity.GetUserId());
                #endregion

                model.PendingIRet = db.tb_item_return.Where(x => x.status == true && (x.item_return_status == Status.Pending || string.Compare(x.item_return_status, Status.Approved) == 0)).Count();


                //model.PendingST = db.tb_stock_transfer_voucher.Where(x => x.status == true &&( x.stock_transfer_status ==Status.Pending || string.Compare(x.stock_transfer_status,Status.Approved)==0)).Count();
                model.PendingST = StockTransferViewModel.CountApproval(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.QAQCOfficer), User.IsInRole(Role.SiteManager), User.Identity.GetUserId());

                #region work order issue
                //model.PendingSIss = db.tb_stock_issue.Where(x => x.status == true && (x.stock_issue_status == Status.Pending || string.Compare(x.stock_issue_status,Status.Approved)==0)).Count();
                model.PendingSIss = StockIssueViewModel.CountStockIssueApproval(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.SiteStockKeeper), User.IsInRole(Role.SiteManager), userid);
                #endregion
                #region work order returned
                //model.PendingIssRe = db.tb_workorder_returned.Where(x => x.status == true && ( string.Compare(x.workorder_returned_status,Status.Pending)==0 || string.Compare(x.workorder_returned_status,Status.Approved)==0 || string.Compare(x.workorder_returned_status,Status.Checked)==0)).Count();
                model.PendingIssRe = WorkorderReturnedViewModel.CountWorkOrderReturnApproval(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.QAQCOfficer), User.IsInRole(Role.SiteStockKeeper), User.IsInRole(Role.SiteManager), User.Identity.GetUserId().ToString());
                #endregion

                #region Count Pending Approval MR
                if (User.IsInRole(Role.SystemAdmin))
                    model.pendingIR = db.tb_item_request.Where(x => x.status == true && x.ir_status == "Pending").Count();
                else if (User.IsInRole(Role.SiteManager))
                {
                    model.pendingIR = (from mr in db.tb_item_request
                                       join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                       join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                       where mr.status == true && string.Compare(mr.ir_status, "Pending") == 0 && string.Compare(sm.site_manager, userid) == 0
                                       select mr).Count();
                }
                #endregion
                // Purchase Requisition
                model.countPR = db.tb_purchase_requisition.Where(s => s.status == true && (string.Compare(s.purchase_requisition_status, Status.Pending) == 0 || string.Compare(s.purchase_requisition_status, Status.Feedbacked) == 0)).Count();

                #region Count Pending Approval Quote
                if (User.IsInRole(Role.SystemAdmin) || (User.IsInRole(Role.TechnicalDirector) && User.IsInRole(Role.ProjectManager) && User.IsInRole(Role.OperationDirector)))
                    model.countQuote = db.tb_purchase_order.Where(s => s.status == true && (string.Compare(s.purchase_order_status, "Pending") == 0 || string.Compare(s.purchase_order_status, "Approved") == 0 || string.Compare(s.purchase_order_status,Status.Checked)==0)).Count();
                else if (User.IsInRole(Role.TechnicalDirector))
                    model.countQuote = db.tb_purchase_order.Where(s => s.status == true && (string.Compare(s.purchase_order_status, "Approved") == 0)).Count();
                else if (User.IsInRole(Role.ProjectManager))
                {
                    model.countQuote = (from q in db.tb_purchase_order
                                        join pr in db.tb_purchase_requisition on q.item_request_id equals pr.purchase_requisition_id
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                        join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                        join pm in db.tb_project_pm on pro.project_id equals pm.project_id
                                        where string.Compare(q.purchase_order_status, "Pending") == 0 && q.status == true && string.Compare(pm.project_manager_id, userid) == 0
                                        select q).Count();
                }
                else if (User.IsInRole(Role.OperationDirector))
                {
                    model.countQuote = db.tb_purchase_order.Where(s => s.status == true && (string.Compare(s.purchase_order_status, Status.Checked) == 0)).Count();
                }
                #endregion
                #region Count Pending Approval Purchase Order
                if (User.IsInRole(Role.SystemAdmin) || (User.IsInRole(Role.OperationDirector) && (User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager))))
                    model.countPO = db.tb_purchase_request.Where(s => s.status == true && (string.Compare(s.purchase_request_status, Status.Pending) == 0 || string.Compare(s.purchase_request_status, Status.Approved) == 0)).Count();
                else if (User.IsInRole(Role.AccountingManager) || User.IsInRole(Role.FinanceManager))
                    model.countPO = db.tb_purchase_request.Where(s => s.status == true && string.Compare(s.purchase_request_status, Status.Pending) == 0).Count();
                else if (User.IsInRole(Role.OperationDirector))
                    model.countPO = db.tb_purchase_request.Where(s => s.status == true && string.Compare(s.purchase_request_status, Status.Approved) == 0).Count();

                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedPO = db.tb_purchase_request.Where(s => s.status == true && string.Compare(s.purchase_request_status, "Approved") == 0).Count();
                    model.approvedPOListItems = (from po in db.tb_purchase_request
                                                 join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id
                                                 join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                                 join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                                 join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                 orderby po.approved_date descending
                                                 where po.status == true && string.Compare(po.purchase_request_status, "Approved") == 0
                                                 select new Models.PurchaseRequestViewModel()
                                                 {
                                                     purchase_order_id = po.purchase_order_id,
                                                     purchase_request_number = po.purchase_request_number,
                                                     project_short_name = pro.project_no,
                                                 }).Take(5).ToList();
                }

                #endregion
                #region Count Pending Approval Stock Return
                if (User.IsInRole(Role.SystemAdmin))
                    model.countSR = db.tb_stock_issue_return.Where(s => s.status == true && (string.Compare(s.issue_return_status, Status.Pending) == 0 || string.Compare(s.issue_return_status, Status.Approved) == 0)).Count();
                else
                {
                    if (User.IsInRole(Role.QAQCOfficer))
                        model.countSR = (from sr in db.tb_stock_issue_return
                                         join st in db.tb_stock_transfer_voucher on sr.stock_issue_ref equals st.stock_transfer_id
                                         join mr in db.tb_item_request on st.item_request_id equals mr.ir_id
                                         join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                         join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                         join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                         where string.Compare(qaqc.qaqc_id, userid) == 0 && sr.status == true && string.Compare(sr.issue_return_status, Status.Pending) == 0
                                         select sr).Count();
                    if (User.IsInRole(Role.SiteManager))
                        model.countSR = model.countSR + (from sr in db.tb_stock_issue_return
                                                         join st in db.tb_stock_transfer_voucher on sr.stock_issue_ref equals st.stock_transfer_id
                                                         join mr in db.tb_item_request on st.item_request_id equals mr.ir_id
                                                         join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                         join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                                         where string.Compare(sm.site_manager, userid) == 0 && sr.status == true && string.Compare(sr.issue_return_status, Status.Approved) == 0
                                                         select sr).Count();

                }
                #endregion
                #region Count Pending SA
                //model.countSA = db.tb_stock_adjustment.Where(s => s.status == true && (string.Compare(s.stock_adjustment_status, Status.Pending) == 0 || string.Compare(s.stock_adjustment_status, Status.Approved) == 0 || string.Compare(s.stock_adjustment_status, Status.Checked) == 0)).Count();
                model.countSA = StockAdjustmentViewModel.CountStockAdjustmntApproval(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.QAQCOfficer), User.IsInRole(Role.SiteManager), User.IsInRole(Role.ProjectManager), User.Identity.GetUserId().ToString());
                #endregion
                model.countTW = db.transferformmainstocks.Where(s => s.status == true && string.Compare(s.stock_transfer_status, Status.Pending) == 0).Count();
                model.countRW = db.tb_return_main_stock.Where(s => s.status == true && (string.Compare(s.return_main_stock_status, Status.Pending) == 0 || string.Compare(s.return_main_stock_status, Status.Approved) == 0 || string.Compare(s.return_main_stock_status, Status.Checked) == 0)).Count();

                //Get Top 5 Items approval
                #region MR
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedMR = db.tb_item_request.Where(s => s.status == true&&string.Compare(s.ir_status, "Approved") == 0 && s.is_mr == false).Count();
                    model.approvedMrListItems = (from mr in db.tb_item_request
                                                 join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                 orderby mr.approved_date descending
                                                 where mr.status == true && string.Compare(mr.ir_status, "Approved") == 0 && mr.is_mr==false
                                                 select new Models.ItemRequestViewModel()
                                                 {
                                                     ir_id=mr.ir_id,
                                                     ir_no=mr.ir_no,
                                                     project_full_name=pro.project_full_name,
                                                     created_date=mr.approved_date,
                                                     project_short_name=pro.project_no,
                                                 }).Take(5).ToList();
                }
                #endregion
                #region PR
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedPR = db.tb_purchase_requisition.Where(s => s.status == true && string.Compare(s.purchase_requisition_status, "Approved")==0 ).Count();
                    model.approvedPRListItems = (from pr in db.tb_purchase_requisition
                                                 join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                                 join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                 orderby pr.approved_at descending
                                                 where pr.status == true && string.Compare(pr.purchase_requisition_status, "Approved") == 0 
                                                 select new Models.PurchaseRequisitionViewModel()
                                                 {
                                                     purchase_requisition_id = pr.purchase_requisition_id,
                                                     purchase_requisition_number=pr.purchase_requisition_number,
                                                     project_short_name=pro.project_no,
                                                    
                                                 }).Take(5).ToList();
                }
                #endregion
                #region PO
                
                #endregion
                #region GRN
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    //ItemReceiveViewModel itemReceive1 = new ItemReceiveViewModel();
                        model.countApprovedGRN = db.tb_receive_item_voucher.Where(s => s.status == true && string.Compare(s.received_status, "Approved") == 0).Count();
                    
                    
                        string ref_number = string.Empty;

                        model.approvedGRNListItems = (from ire in db.tb_receive_item_voucher
                                                          orderby ire.approved_date descending
                                                          where ire.status == true && string.Compare(ire.received_status, "Approved") == 0
                                                          select new Models.ItemReceiveViewModel()
                                                          {
                                                              receive_item_voucher_id = ire.receive_item_voucher_id,
                                                              received_number = ire.received_number,
                                                              received_status = ire.received_status,
                                                              received_type = ire.received_type,
                                                              po_report_number = ire.po_report_number,
                                                              supplier_id = ire.supplier_id,
                                                              ref_id = ire.ref_id,
                                                              created_date = ire.created_date

                                                          }).Take(5).ToList();
                   foreach (var itemReceive in model.approvedGRNListItems)
                        {
                        if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                        {
                            ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                            itemReceive.receivedItems = Inventory.GetStockTransferItems(itemReceive.ref_id);
                            itemReceive.mr_ref_number = (from mr in db.tb_item_request
                                                         join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                         where string.Compare(st.stock_transfer_id, itemReceive.ref_id) == 0
                                                         select mr.ir_no).FirstOrDefault().ToString();
                            itemReceive.project_full_name = (from pro in db.tb_project
                                                             join mr in db.tb_item_request on pro.project_id equals mr.ir_project_id
                                                             join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                             where string.Compare(st.stock_transfer_id, itemReceive.ref_id) == 0
                                                             select pro.project_no).FirstOrDefault().ToString();
                        }
                        else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                        {
                            ref_number = itemReceive.po_report_number;
                            itemReceive.mr_ref_number = (from mr in db.tb_item_request
                                                         join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                         join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                         join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                         where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                                         select mr.ir_no).FirstOrDefault();
                            itemReceive.project_full_name = (from mr in db.tb_item_request
                                                             join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                             join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                             join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                             join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                             where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                                             select pro.project_no).FirstOrDefault();
                            itemReceive.receivedItems = Inventory.ConvertFromPOtoInventory(itemReceive.ref_id, itemReceive.po_report_number, itemReceive.supplier_id);

                        }
                        else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                        {
                            var transferRef = (from transfer in db.transferformmainstocks
                                               join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                               join project in db.tb_project on mr.ir_project_id equals project.project_id
                                               join site in db.tb_site on project.site_id equals site.site_id
                                               join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                               where string.Compare(transfer.stock_transfer_id, itemReceive.ref_id) == 0
                                               select new
                                               {
                                                   transfer,
                                                   project,
                                                   mr,
                                                   wh
                                               }).FirstOrDefault();
                            ref_number = transferRef.transfer.stock_transfer_no;
                            itemReceive.mr_ref_number = transferRef.mr.ir_no;
                            itemReceive.project_full_name = transferRef.project.project_no;
                            itemReceive.receivedItems = Inventory.GetTransferfromWorkshopItems(itemReceive.ref_id);

                        }
                        else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                        {
                            var returnRef = (from sreturn in db.tb_stock_issue_return
                                             join transfer in db.tb_stock_transfer_voucher on sreturn.stock_issue_ref equals transfer.stock_transfer_id
                                             join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                             join project in db.tb_project on mr.ir_project_id equals project.project_id
                                             join site in db.tb_site on project.site_id equals site.site_id
                                             join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                             where string.Compare(sreturn.stock_issue_return_id, itemReceive.ref_id) == 0
                                             select new
                                             {
                                                 sreturn,
                                                 transfer,
                                                 mr,
                                                 project,
                                                 wh
                                             }).FirstOrDefault();
                            ref_number = returnRef.sreturn.issue_return_number;
                            itemReceive.mr_ref_number = returnRef.mr.ir_no;
                            itemReceive.project_full_name = returnRef.project.project_no;
                            itemReceive.receivedItems = Inventory.GetStockReturnItems(itemReceive.ref_id);
                        }
                        else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                        {
                            var returnRef = (
                                from wreturn in db.tb_return_main_stock
                                join transfer in db.transferformmainstocks on wreturn.return_ref_id equals transfer.stock_transfer_id
                                join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                join project in db.tb_project on mr.ir_project_id equals project.project_id
                                join site in db.tb_site on project.site_id equals site.site_id
                                join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                where string.Compare(wreturn.return_main_stock_id, itemReceive.ref_id) == 0
                                select new
                                {
                                    wreturn,
                                    transfer,
                                    project,
                                    mr,
                                    wh
                                }).FirstOrDefault();
                            ref_number = returnRef.wreturn.return_main_stock_no;
                            itemReceive.mr_ref_number = returnRef.mr.ir_no;
                            itemReceive.project_full_name = returnRef.project.project_no;
                            itemReceive.receivedItems = Inventory.GetReturnWorkshopItems(itemReceive.ref_id);
                        }
                        itemReceive.ref_number = ref_number;
                    }
                    

                }
                #endregion
                #region STOCK TRANSFER
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedST = db.tb_stock_transfer_voucher.Where(s => s.status == true && string.Compare(s.stock_transfer_status, "Approved") == 0).Count();
                    model.approvedSTListItems = (from st in db.tb_stock_transfer_voucher
                                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                 orderby st.approved_date descending
                                                 where st.status == true && string.Compare(st.stock_transfer_status, "Approved") == 0
                                                 select new Models.StockTransferViewModel() 
                                                 {
                                                     stock_transfer_id=st.stock_transfer_id,
                                                     stock_transfer_no=st.stock_transfer_no,
                                                     project_short_name=pro.project_no

                                                 }).Take(5).ToList();
                }
                #endregion
                #region ITEM RETURN
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedSR = db.tb_stock_issue_return.Where(s => s.status == true && string.Compare(s.issue_return_status, "Approved") == 0).Count();
                    model.approvedSRListItems = (from sr in db.tb_stock_issue_return
                                                 join transfer in db.tb_stock_transfer_voucher on sr.stock_issue_ref equals transfer.stock_transfer_id
                                                 join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                                 join project in db.tb_project on mr.ir_project_id equals project.project_id
                                                 orderby sr.approved_date descending
                                                 where sr.status == true && string.Compare(sr.issue_return_status, "Approved") == 0
                                                 select new Models.StockIssueReturnViewModel()
                                                 {
                                                     stock_issue_return_id = sr.stock_issue_return_id,
                                                     stock_issue_number = sr.issue_return_number,
                                                     project_short_name = project.project_no

                                                 }).Take(5).ToList();
                }
                #endregion
                #region Transfer Mainstock
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedTW = db.transferformmainstocks.Where(s => s.status == true && string.Compare(s.stock_transfer_status, "Approved") == 0).Count();
                    model.approvedTWListItems = (from tw in db.transferformmainstocks
                                                 join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                                 join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                 orderby tw.approved_date descending
                                                 where tw.status == true && string.Compare(tw.stock_transfer_status, "Approved") == 0
                                                 select new Models.TransferFromMainStockViewModel()
                                                 {
                                                     stock_transfer_id=tw.stock_transfer_id,
                                                     stock_transfer_no=tw.stock_transfer_no,
                                                     project_short_name = pro.project_no

                                                 }).Take(5).ToList();
                }
                #endregion
                #region Return MainStock
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                {
                    model.countApprovedRW = db.tb_return_main_stock.Where(s => s.status == true && string.Compare(s.return_main_stock_status, "Approved") == 0).Count();
                    model.approvedRWListItems = (from rw in db.tb_return_main_stock
                                                // join tr in db.transferformmainstocks on rw.return_ref_id equals tr.stock_transfer_id
                                               //  join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                                 join pro in db.tb_project on rw.return_ref_id equals pro.project_id
                                                 orderby rw.approved_date descending
                                                 where rw.status == true && string.Compare(rw.return_main_stock_status, "Approved") == 0
                                                 select new Models.ReturnMainStockViewModel()
                                                 {
                                                     return_main_stock_id=rw.return_main_stock_id,
                                                     return_main_stock_no=rw.return_main_stock_no,
                                                     project_short_name = pro.project_no

                                                 }).Take(5).ToList();
                }
                #endregion
            }
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ComingSoon()
        {
            return View();
        }
        public ActionResult EditPDF()
        {
            return View();
        }
        //public ActionResult ImageToPDF()
        //{
        //    //Create a PDF document 
        //    //PdfDocument pdfDocument = new PdfDocument();

        //    ////Add a page 
        //    //PdfPage page = pdfDocument.Pages.Add();
        //    //PdfGraphics graphics = page.Graphics;

        //    ////Getting page size to fit the image within the page
        //    //SizeF pageSize = page.GetClientSize();

        //    ////Load the image file 
        //    //string path = Server.MapPath(Url.Content("/Data/Ani.gif"));
        //    //Stream imageStream = new FileStream(path, FileMode.Open);

        //    ////Get the image stream and draw frame by frame
        //    //PdfTiffImage tiffImage = new PdfTiffImage(imageStream);

        //    //int frameCount = tiffImage.FrameCount;
        //    //for (int i = 0; i < frameCount; i++)
        //    //{
        //    //    //Selecting frame in TIFF
        //    //    tiffImage.ActiveFrame = i;
        //    //    //Draw TIFF frame
        //    //    page.Graphics.DrawImage(tiffImage, 0, 0, pageSize.Width, pageSize.Height);
        //    //}

        //    ////Saving the PDF to the MemoryStream
        //    //MemoryStream stream = new MemoryStream();
        //    //pdfDocument.Save(stream);

        //    //Set the position as '0'.
        //    //stream.Position = 0;

        //    ////Download the PDF document in the browser
        //    //FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

        //    //fileStreamResult.FileDownloadName = "Sample.pdf";

        //    //return fileStreamResult;
        //}

        public ActionResult WatermarkToPDF()
        {
            string path = Server.MapPath(Url.Content("/Data/test.pdf"));
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.LoadFromFile(path);
            PdfPageBase page = pdfDocument.Pages[0];

            PdfTilingBrush brush = new PdfTilingBrush(new SizeF(page.Canvas.ClientSize.Width / 2, page.Canvas.ClientSize.Height / 3));
            brush.Graphics.SetTransparency(0.3f);
            brush.Graphics.Save();
            brush.Graphics.TranslateTransform(brush.Size.Width / 2, brush.Size.Height / 2);
            brush.Graphics.RotateTransform(-45);
            brush.Graphics.DrawString("Draft Version", new PdfFont(PdfFontFamily.Helvetica, 24), PdfBrushes.Blue, 0, 0, new PdfStringFormat(PdfTextAlignment.Center));
            brush.Graphics.Restore();
            brush.Graphics.SetTransparency(1);
            page.Canvas.DrawRectangle(brush, new RectangleF(new PointF(0, 0), page.Canvas.ClientSize));

            pdfDocument.SaveToFile(Server.MapPath(Url.Content("/Data/TextWaterMark.pdf.pdf")));

            return View();
        }
    }
}