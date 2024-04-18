
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockBalanceBywarehouse.aspx.cs" Inherits="BT_KimMex.Views.Shared.ReportController.StockBalanceBywarehouse" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>StockBalanceBywarehouse Report</title>
    <script runat="server">
        void Page_Init(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string WarehouseId = string.Empty;
                string dateFrom = string.Empty;
                string dateTo = string.Empty;
                string reportPath = Server.MapPath("~/Reports/StockBalanceByDateandWarehouse.rdlc");
                if (Request.QueryString["WarehouseId"] != null)
                    WarehouseId = Request.QueryString["WarehouseId"].ToString();
                if (Request.QueryString["dateFrom"] != null)
                    dateFrom = Request.QueryString["dateFrom"].ToString();
                if (Request.QueryString["dateTo"] != null)
                    dateTo = Request.QueryString["dateTo"].ToString();

                //if (!string.IsNullOrEmpty(WarehouseId))
                BT_KimMex.Class.ReportHandler.GenerateStockBalanceMonthlyByWarehouse(this.ReportViewer1, reportPath ,WarehouseId,Convert.ToDateTime(dateFrom),Convert.ToDateTime(dateTo));
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1105px"  AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
        <br />
    </div>
    </form>
</body>
</html>

