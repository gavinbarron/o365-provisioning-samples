<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SiteProvisioningWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Provision Custom Libraries</h1>
            Add custom columns, content types, listst and content to the host web
            <br />

            <asp:Button runat="server" ID="MakeLists" OnClick="AddListsToHostWeb" Text="Create Lists and Libraries" />
            <br />
<%--            <asp:Button runat="server" ID="DeleteColumns" OnClick="DeleteMediaAssetsColumns" Text="Delete site columns" />--%>
<%--            <br />--%>
<%--            <asp:Button runat="server" ID="DeleteContentTypes" OnClick="DeleteMediaAssetsContentTypes" Text="Delete content types" />--%>
        </div>
    </form>
</body>
</html>
