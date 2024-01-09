<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectList.aspx.cs" Inherits="BT_KimMex.Views.Shared.ReportController.ProjectList" %>
<%--<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>--%>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string status = string.Empty;
                string reportPath = Server.MapPath("~/Reports/PurchaseOrderToSupplier.rdlc");
                if (Request.QueryString["status"] != null)
                    status = Request.QueryString["status"].ToString();

                if (!string.IsNullOrEmpty(status))
                    BT_KimMex.Class.ReportHandler.GenerateProjectbyStatus(this.ReportViewer1, reportPath,status);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1></h1>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server">
        </rsweb:ReportViewer>
        <br />
    
    </div>
    </form>
</body>
</html>
