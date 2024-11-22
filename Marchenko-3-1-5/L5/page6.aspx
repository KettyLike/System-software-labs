<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page6.aspx.cs" Inherits="L5.page6" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function hideButton() {
            document.getElementById('Button1').style.visibility = 'hidden';
            document.getElementById('Button2').style.visibility = 'hidden';
        }
    </script>
</head>
<body style="background: linear-gradient(to bottom, #52F292, #92F252);
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    margin: 0;">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label" Font-Size="20pt"></asp:Label>
            <br />
        </div>
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="Label" Font-Size="16pt"></asp:Label>
            <br />
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" Visible="false">
                <asp:ListItem Text="Зазначте тривалість" Value="" />
                <asp:ListItem Text="1 день" Value="1 день" />
                <asp:ListItem Text="3 дні" Value="3 дні" />
                <asp:ListItem Text="1 тиждень" Value="1 тиждень" />
                <asp:ListItem Text="3 тижні" Value="3 тижні" />
            </asp:DropDownList>
            <br/><br/>
            <asp:Button ID="Button1" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button1_Click" Text="НАЗАД" Width="322px" />
            <asp:Button ID="Button2" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button2_Click" Text="ТАК" Width="322px" />
    </form>
</body>
</html>
