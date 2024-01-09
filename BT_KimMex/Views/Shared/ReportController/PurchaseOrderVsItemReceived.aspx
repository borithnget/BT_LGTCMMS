
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%--<%@ Page Language="C#"  Inherits="BT_KimMex.Views.Shared.ReportController.PurchaseOrderVsItemReceived" %>--%>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PurchaseOrderVsItemReceived Report</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {

            if (!IsPostBack)
            {
                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;
                //string product = string.Empty;
                string reportPath = Server.MapPath("~/Reports/PurchaseOrderVsItemReceived.rdlc");
                string status = string.Empty;
                if (Request.QueryString["dateFrom"] != null)
                    dateFrom = Convert.ToDateTime(Request.QueryString["dateFrom"]);
                if (Request.QueryString["dateTo"] != null)
                    dateTo = Convert.ToDateTime(Request.QueryString["dateTo"]);
                if (Request.QueryString["status"] != null)
                    status = Request.QueryString["status"].ToString();
                BT_KimMex.Class.ReportHandler.GeneratePurchaseOrderVsItemReceivedReport(this.ReportViewer1, reportPath, dateFrom, dateTo,status);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1105px" AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
    
    </div>
    </form>
</body>
</html>
