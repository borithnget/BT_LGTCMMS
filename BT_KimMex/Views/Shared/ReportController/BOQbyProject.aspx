<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BOQ by Project</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string boqId = string.Empty;
                string reportPath = Server.MapPath("~/Reports/Table11BOQbyProject.rdlc");
                if (Request.QueryString["boqId"] != null)
                    boqId = Request.QueryString["boqId"].ToString();
                if (!string.IsNullOrEmpty(boqId))
                    BT_KimMex.Class.ReportHandler.GenerateBOQbyProject(this.ReportViewer1, reportPath, boqId);
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
