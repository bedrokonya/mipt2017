<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebController._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>Azure Scientific Computation Sample</h2>
        <asp:TextBox ID="TextBox1" runat="server" Width="305px"></asp:TextBox>
    
        <asp:Button ID="Button1" runat="server" Text="Submit Task" 
            onclick="Button1_Click" />
        <hr />
        <asp:Button ID="Button2" runat="server" Text="Load Result" 
            onclick="Button2_Click" />
    
    </div>
    <asp:Label ID="Label1" runat="server" Text="(no result)"></asp:Label>
    </form>
</body>
</html>
