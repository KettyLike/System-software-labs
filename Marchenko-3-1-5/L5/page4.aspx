<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page4.aspx.cs" Inherits="L5.page4" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function hideButton() {
            document.getElementById('Button1').style.visibility = 'hidden';
        }
    </script>
</head>
<body style="background: linear-gradient(to bottom, #7571EC, #62EEE4);
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    margin: 0;">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label" Font-Size="XX-Large"></asp:Label>
            <br /><br /><br />
            <asp:Label ID="Label2" runat="server" Text="Label" Font-Size="16pt"></asp:Label>
            <br /><br />
            <asp:Image ID="Image1" runat="server" />
            <br /><br />
            <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="16pt"></asp:Label>
            <br /><br />
            <asp:Label ID="Label4" runat="server" Text="Label" Font-Size="16pt"></asp:Label>
            <br /><br />
            <asp:Label ID="Label5" runat="server" Text="Label" Font-Size="16pt"></asp:Label>
            <br /><br />
            <asp:Button ID="Button1" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button1_Click" Text="ДАЛІ" Width="322px" />
        </div>
    </form>
</body>
</html>
