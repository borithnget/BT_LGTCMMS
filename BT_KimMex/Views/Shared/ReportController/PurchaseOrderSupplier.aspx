<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <title>Purchase Order by Supplier with balance</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;
                string supplier = string.Empty;
                string project = string.Empty;
                string reportPath = Server.MapPath("~/Reports/3PurchaseOrderSupplier.rdlc");
                if (Request.QueryString["dateFrom"] != null)
                    dateFrom = Convert.ToDateTime(Request.QueryString["dateFrom"]);
                if (Request.QueryString["dateTo"] != null)
                    dateTo = Convert.ToDateTime(Request.QueryString["dateTo"]);
                if (Request.QueryString["supplier"] != null)
                    supplier = Request.QueryString["supplier"].ToString();
                if (Request.QueryString["project"] != null)
                    project = Request.QueryString["project"].ToString();
                //if (!string.IsNullOrEmpty(supplier))
                BT_KimMex.Class.ReportHandler.GeneratePurchaseOrderSupplierReport(this.ReportViewer1, reportPath, dateFrom, dateTo,supplier,project);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
        <br />
    
    </div>
    </form>
</body>
</html>
