using BT_KimMex.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Razor.Parser.SyntaxTree;

namespace BT_KimMex.Class
{
    public class EnumConstants
    {
        public static string WORKSHOP = "1";
        #region MessageEnum
        public enum MessageParameter
        {
            Successfull,
            Error,
            Warning,

            SaveSuccessfull,
            SaveError,
            SaveWarning,

            UpdateSuccessfull,
            UpdateError,
            UpdateWarning,

            DeleteSuccessfull,
            DeleteError,
            DeleteWarning
        }
        #endregion
        #region ErrorLogEnum
        public enum ErrorType
        {
            Warning,
            Error,
            Message
        }
        #endregion
        public static string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
    }
    public class Role
    {
        public static string SystemAdmin = "Admin";
        public static string ManagingDirector = "Managing Director";
        public static string TechnicalDirector = "Technical Director";
        public static string OperationDirector = "Operation Director";
        public static string FinanceManager = "Finance Manager";
        public static string AccountingManager = "Accounting Manager";
        public static string ProcurementManager="Procurement Manager";
        public static string AstOperationDirector = "Ast. Operation Director";
        public static string ProjectManager = "Project Manager";
        public static string SiteManager = "Site Manager";
        public static string SiteSupervisor = "Site Supervisor";
        public static string SiteAdmin = "Site Admin";
        //public static string SiteStockKeeper = "Site Stock Keeper";
        public static string SiteStockKeeper = "Stock Controller";
        public static string QAQCOfficer = "QA/QC Officer";
        public static string Purchaser = "Purchaser";
        public static string WorkshopController = "Workshop Controller";
        public static string InvoiceClerk = "Invoice Clerk";

    }
    public class Status
    {
        public static string Pending = "pending";
        public static string Approved = "approved";
        public static string Rejected = "rejected";
        public static string Completed = "completed";
        public static string CheckRequest = "requested check";
        public static string Checked = "checked";
        public static string CheckRejected = "check rejected";
        public static string PendingFeedback = "pending feedback";
        public static string Feedbacked = "feedbacked";
        public static string cancelled = "cancelled";
        public static string RequestCancelled = "request cancelled";
        public static string Active = "active";
        public static string DeActive = "de-active";
        public static string Edit = "edit";
        public static string WaitingApproval = "waiting approval";
        public static string CancelledMR = "mr cancelled";
        public static string Void = "void";
        public static string MREditted = "mr edited";
    }
    public class ProjectStatus
    {
        public static string Active = "Active";
        public static string Inactive = "Inactive";
        public static string Completed = "Completed";
        public static string PeningComplete = "Pending Complete";
    }
    public class ShowStatus
    {
        public static string MRCreated = "MR Created";
        public static string MRApproved = "MR Approved";
        public static string MRCompleted = "MR Completed";
        public static string MRRejected = "MR Rejected";
        public static string MRCancelled = "MR Cancelled";
        public static string MRPendingFeedback = "MR Pending Feedback";
        public static string MRFeedbacked = "MR Feedbacked";
        public static string MRRequestCancelled = "MR Request Cancelled";
        public static string MREditted = "MR Edited";
        public static string MRPartialRequestCancelled = "MR Partial Request Cancelled";

        public static string PRCreated = "PR Created";
        public static string PRApproved = "PR Approved";
        public static string PRPending = "PR Pending";
        public static string PRRejected = "PR Rejected";
        public static string PRCancelled = "PR Cancelled";
        public static string PRRequestCancelled = "PR Request Cancelled";

        public static string SupplierQuotePending = "Supplier Quote Pending";
        public static string SupplierQuoteCreated = "Supplier Quote Created";
        public static string SupplierQuoteEdited = "Supplier Quote Edited";
        public static string SupplierQuoteCancelled = "Supplier Quote Cancelled";

        public static string QuoteCreated = "Quote Created";
        public static string QuoteApproved = "Quote Approved";
        public static string QuoteCompleted = "Quote Completed";
        public static string QuoteRejected = "Quote Rejected";
        public static string QuoteCancelled = "Quote Cancelled";
        public static string QuoteRequestCancelled = "Quote Request Cancelled";
        public static string QuoteChecked = "Quote Checked";
        public static string QuoteCheckedReject = "Quote Checked Rejected";

        public static string POPending = "PO Pending";
        public static string POCreated = "PO Created";
        public static string POCompleted = "PO Completed";
        public static string POApproved = "PO Approved";
        public static string PORejected = "PO Rejected";
        public static string POCancelled = "PO Cancelled";
        public static string PORequestCancelled = "PO Request Cancelled";
        public static string POPartialApproved = "PO Partial Approved";

        public static string GRNPending = "GRN Pending";
        public static string GRNCreated = "GRN Created";
        public static string GRNApproved = "GRN Approved";
        public static string GRNCompleted = "GRN Completed";
        public static string GRNCancelled = "GRN Cancelled";
        public static string GRNRejected = "GRN Rejected";
        public static string GRNPartialCreated = "GRN Partial Created";
        public static string GRNPartialApproved = "GRN Partial Approved";
        public static string GRNPartialCompleted = "GRN Partial Completed";
        public static string GRNPartialCancelled = "GRN Partial Cancelled";
        public static string RemainGRNPending = "Remain GRN Pending";
        public static string GRNPartialRejected = "GRN Partial Rejected";
        public static string TWCreated = "TW Created";
        public static string TWCompleted = "TW Completed";
        public static string STCreated = "ST Created";
        public static string STApproved = "ST Approved";
        public static string STChecked = "ST Checked";
        public static string STCompleted = "ST Completed";
        public static string SRPending = "SR Pending";
        public static string SRCreated = "SR Created";
        public static string SRApproved = "SR Approved";
        public static string SRChecked = "SR Checked";
        public static string SRCompleted = "SR Completed";
        public static string WOICreated = "WOI Created";
        public static string WOIApproved = "WOI Approved";
        public static string WOIChecked = "WOI Checked";
        public static string WORCreated = "WOR Created";
        public static string WORApproved = "WOR Approved";
        public static string WORCompleted = "WOR Completed";
        public static string WORChecked = "WOR Checked";
        public static string Created = "Created";
        public static string Approved = "Approved";
        public static string Completed = "Completed";
        public static string Checked = "Checked";
        public static string Active = "Active";
        public static string DeActive = "De-active";
        public static string RequestCancelled = "Request Cancelled";
        public static string Rejected = "Rejected";
        public static string CancelledMR = "MR Cancelled";
        public static string PendingFeedback = "Pending Feedback";

        public static string GetQuoteShowStatus(string status)
        {
            string showStatus = status;
            switch (status)
            {
                case "Pending":
                    showStatus = QuoteCreated;
                    break;
                case "Approved":
                    showStatus = QuoteApproved;
                    break;
                case "Completed":
                    showStatus = QuoteCompleted;
                    break;
                case "mr edited":
                    showStatus = ShowStatus.MREditted;
                    break;
                default:
                    showStatus = status;
                    break;

            }
            return showStatus;
        }

        public static string GetPRShowStatus(string status)
        {
            if (string.Compare(status, Status.Pending) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "warning", ShowStatus.PRCreated);
            }
            else if (string.Compare(status, Status.Approved) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "primary", ShowStatus.PRApproved);
    }
                        else if (string.Compare(status, Status.Rejected) == 0)
                        {
                return string.Format("<span class='label label-{0}'>{1}</span>", "danger", ShowStatus.PRRejected);                   
}
                        else if (string.Compare(status, Status.RequestCancelled) == 0)
                        {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-red", ShowStatus.PRRequestCancelled);
                        }
                        else if (string.Compare(status, Status.cancelled) == 0)
                        {
            return string.Format("<span class='label {0}'>{1}</span>", "w3-red", ShowStatus.MRCancelled);
    }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
            {
                return "";
            }
        }

        public static string GetSupplierQuoteShowStatus(string status)
        {
            if (string.Compare(status, BT_KimMex.Class.Status.Approved) == 0)
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>","green",ShowStatus.SupplierQuoteCreated); 
            }
            else if (string.Compare(status, BT_KimMex.Class.Status.Edit) == 0)
            {
                            return string.Format("<span class='label w3-{0}'>{1}</span>", "orange", ShowStatus.SupplierQuoteEdited);
    
            }
                                    else if (string.Compare(status, BT_KimMex.Class.Status.cancelled) == 0)
                                    {
                            return string.Format("<span class='label w3-{0}'>{1}</span>", "red", ShowStatus.SupplierQuoteCancelled);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
                        {
                           return string.Format("<span class='label w3-{0}'>{1}</span>","red", status); 
                        }
        }

        public static string GetQuoteShowStatusFull(string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                if (string.Compare(status, "Pending") == 0)
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "red", ShowStatus.QuoteCreated);
                }
                else if (string.Compare(status, "Approved") == 0)
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "blue", ShowStatus.QuoteApproved);
                }
                else if (string.Compare(status, "Completed") == 0)
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "blue", ShowStatus.QuoteCompleted);
                }
                else if (string.Compare(status, Status.RequestCancelled) == 0)
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "grey", ShowStatus.QuoteRequestCancelled);
                }
                else if (string.Compare(status.ToLower(), Status.Rejected) == 0)
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "gray", ShowStatus.QuoteRejected);
                }
                else if (string.Compare(status, Status.cancelled) == 0)
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "gray", ShowStatus.MRCancelled);
                }
                else if (string.Compare(status, Status.MREditted) == 0)
                {
                    return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
                }
                else
                {
                    return string.Format("<span class='label w3-{0}'>{1}</span>", "gray", status);
                }
            }else
            {
                return string.Empty;
            }
            
        }
        public static string GetPurchaseOrderShowStatus(string status)
        {
            if (string.Compare(status, Status.Pending) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "warning", ShowStatus.POCreated);
            }
            else if (string.Compare(status, Status.Approved) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "primary", ShowStatus.POApproved);
            }
            else if (string.Compare(status, Status.Rejected) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "danger", ShowStatus.PORejected);
            }
            else if (string.Compare(status, Status.Completed) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "success", ShowStatus.POCompleted);
            }
            else if (string.Compare(status, Status.cancelled) == 0)
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>", "red", ShowStatus.POCancelled);
            }
            else if (string.Compare(status, Status.RequestCancelled) == 0)
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>", "red", ShowStatus.PORequestCancelled);
            }
            else if (string.Compare(status, Status.CancelledMR) == 0)
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>", "red", ShowStatus.CancelledMR);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>", "red", status);
            }
        }
        public static string GetTransferFromWorkshopShowStatus(string status)
        {
            if(string.Compare(status,Status.Pending)==0 || string.Compare(status, Status.Feedbacked) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "red", ShowStatus.Created);
            }else if (string.Compare(status, Status.PendingFeedback) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "green", ShowStatus.PendingFeedback);
            }
            else if (string.Compare(status, Status.Completed) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "teal", ShowStatus.Approved);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "grey", status);
            }
        }
        public static string GetStockTransfershowStatus(string status)
        {
            if (string.Compare(status, Status.Pending) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "red", ShowStatus.STCreated);
            }else if (string.Compare(status, Status.PendingFeedback) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "red", status);
            }
            else if (string.Compare(status, Status.Feedbacked) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "green", status);
            }
            else if (string.Compare(status, Status.Completed) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "blue", STApproved);
            }
            else if (string.Compare(status, Status.Approved) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "yellow", STChecked);
            }
            else if (string.Compare(status, Status.Rejected) == 0 || string.Compare(status,Status.cancelled)==0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "grey", status);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "gray", status);
            }
        }

        public static string GetStockReturnShowStatus(string status)
        {
            if(string.Compare(status,Status.Pending)==0 || string.Compare(status, Status.Feedbacked) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "danger", SRCreated);
            }
            else if (string.Compare(status, Status.PendingFeedback) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "info", PendingFeedback);
            }
            else if (string.Compare(status, Status.Approved) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "primary", SRChecked);
            }
            else if (string.Compare(status, Status.Completed) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "success",SRApproved);
            }
            else if (string.Compare(status, Status.CheckRejected) == 0)
            {
                return string.Format("<span class='label label-{0}'>{1}</span>", "danger", "Check Rejected");
            }
            else if (string.Compare(status, Status.RequestCancelled) == 0)
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>", "red", ShowStatus.RequestCancelled);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
            {
                return string.Format("<span class='label w3-{0}'>{1}</span>","grey",status);
            }
        }

        public static string GetMRTWShowStatusHTML(string status)
        {
            if (string.Compare(status, ShowStatus.TWCreated) == 0)
            {
                return string.Format("<label class='label w3-red'>{0}</label>",status);
            }
                                else if (string.Compare(status, ShowStatus.GRNPending) == 0)
                                {
                return string.Format("<label class='label w3-yellow'>{0}</label>", status);
}
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else if (string.Compare(status, ShowStatus.TWCompleted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-blue", ShowStatus.TWCompleted);
            }
            else
                                {
                return string.Format("<label class='label w3-grey'>{0}</label>", status);
                                }
        }

        public static string GetMRSTShowStatusHTML(string status)
        {
            if (string.Compare(status, ShowStatus.PRCreated) == 0 || string.Compare(status, ShowStatus.SupplierQuoteCreated) == 0 || string.Compare(status, ShowStatus.QuoteCreated) == 0)
            {
                return string.Format("<label class='label w3-red'>{0}</label>",status); 
            }
                            else if (string.Compare(status, ShowStatus.SupplierQuotePending) == 0)
                            {
                return string.Format("<label class='label w3-yellow'>{0}</label>", status);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
                            {
                return string.Format("<label class='label w3-grey'>{0}</label>", status);
                            }
        }

        public static string GetMRPOShowStatus(string status)
        {
            if (string.Compare(status, ShowStatus.PRCreated) == 0 || string.Compare(status, ShowStatus.SupplierQuoteCreated) == 0 || string.Compare(status, ShowStatus.QuoteCreated) == 0)
            {
                return string.Format("<label class='label w3-red'>{0}</label>",status); 
            }
                                else if (string.Compare(status, ShowStatus.SupplierQuotePending) == 0)
                                {
            return string.Format("<label class='label w3-yellow'>{0}</label>", status);
            }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else
                                {
            return string.Format("<label class='label w3-grey'>{0}</label>", status); 
                                }
        }

        public static string GetMRShowStatusHTML(string status,bool isPartial=false)
        {
            if (string.Compare(status, "Pending") == 0)
            {
                return string.Format("<label class='label w3-red'>{0}</label>", ShowStatus.MRCreated);
                                
    }
            else if (string.Compare(status, "edit") == 0)
            {
                return string.Format("<label class='label w3-red'>{0}</label>", ShowStatus.MREditted);

            }
            else if (string.Compare(status, "Approved") == 0)
                            {
                return string.Format("<label class='label w3-blue'>{0}</label>", ShowStatus.MRApproved);
}
                            else if (string.Compare(status, "Completed") == 0)
                            {
                return string.Format("<label class='label w3-green'>{0}</label>", status);
                            }
                            else if (string.Compare(status, "Rejected") == 0)
                            {
                return string.Format("<label class='label w3-gray'>{0}</label>", ShowStatus.MRRejected);
                            }
                            else if (string.Compare(status, Status.RequestCancelled) == 0)
                            {
                if (isPartial)
                {
                    return string.Format("<label class='label w3-gray'>{0}</label>", ShowStatus.MRPartialRequestCancelled);
                }
                else
                {
                    return string.Format("<label class='label w3-gray'>{0}</label>", ShowStatus.MRRequestCancelled);
                }
                
                            }
                            else if (string.Compare(status, Status.PendingFeedback)==0) {
                return string.Format("<label class='label w3-orange'>{0}</label>", ShowStatus.MRPendingFeedback);
                            }
                            else if (string.Compare(status, Status.Feedbacked)==0) {
                return string.Format("<label class='label w3-orange'>{0}</label>", ShowStatus.MRFeedbacked);
                    }
            else if (string.Compare(status, Status.MREditted) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MREditted);
            }
            else if (string.Compare(status, Status.CancelledMR) == 0)
            {
                return string.Format("<span class='label {0}'>{1}</span>", "w3-grey", ShowStatus.MRCancelled);
            }
            else
                            {
return string.Format("<label class='label w3-gray'>{0}</label>", status);
                            }
        }

        public static string GetGRNShowStatusHTML(string received_status,bool is_received_partial)
        {
            if(string.Compare(received_status,Status.Pending)==0 || string.Compare(received_status, Status.Feedbacked) == 0)
            {
                if (is_received_partial)
                {
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "red", ShowStatus.GRNPartialCreated);          
                } else
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "red", ShowStatus.GRNCreated);
            }
            else if (string.Compare(received_status, Status.PendingFeedback) == 0)
            {
                return string.Format("<label class='label w3-{0}'>{1}</label>", "blue", Status.PendingFeedback);
            }else if (string.Compare(received_status, Status.Completed) == 0)
            {
                if(is_received_partial)
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "green", ShowStatus.RemainGRNPending);
                else
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "green", ShowStatus.GRNCompleted);
            }else if(string.Compare(received_status,Status.Rejected)==0 || string.Compare(received_status, Status.cancelled) == 0)
            {
                if (is_received_partial)
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "gray", ShowStatus.GRNPartialCancelled);
                else
                    
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "gray", ShowStatus.GRNCancelled);
            }
            else if (string.Compare(received_status, Status.Approved) == 0)
            {
                if(is_received_partial)
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "blue", ShowStatus.GRNPartialApproved);
                else
                    return string.Format("<label class='label w3-{0}'>{1}</label>", "blue", ShowStatus.GRNApproved);
            }
            else
                return string.Format("<label class='label w3-{0}'>{1}</label>", "grey",received_status );
        }
        public static string GetWOIShowStatusHTML(string status)
        {
            if (string.Compare(status, Status.Pending) == 0)
            {
                return string.Format("<span class='label label-danger'>{0}</span>", @ShowStatus.WOICreated);
    }
                                        else if (string.Compare(status, Status.Approved) == 0)
                                        {
                return string.Format("<span class='label label-warning'>{0}</span>", @ShowStatus.WOIChecked);
                                           
}
                                        else if (string.Compare(status, Status.Completed) == 0)
{
                return string.Format("<span class='label label-primary'>{0}</span>", @ShowStatus.WOIApproved);
                                            
                                        }
                                        else
{
                return string.Format("<span class='label w3-gray'>{0}</span>", status);
                                            
                                        }
        }

    }

    public class AJAXResultModel
    {
        //public bool isSuccess
        //{
        //    get { return true;}
        //    set { isSuccess = value; }
        //}
        //public string message
        //{
        //    get { return string.Empty; }
        //    set { message = value; }
        //}
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public AJAXResultModel()
        {
            isSuccess = true;
            message = string.Empty;
        }
        public AJAXResultModel(bool isSuccess, string message)
        {
            this.isSuccess = isSuccess;
            this.message = message;
        }
    }
}