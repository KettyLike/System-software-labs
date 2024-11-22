<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page1.aspx.cs" Inherits="L5.page1" %>

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
<body style="background: linear-gradient(to bottom, #F09797, #F097D8); 
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    margin: 0;">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label" Font-Size="20pt"></asp:Label><br />
        </div>
            <br /><br />
            <asp:Label ID="Label2" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br />
            <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="true" OnTextChanged="TextBox1_TextChanged"></asp:TextBox><br/>
            <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
            <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox><br/>
            <br /><br />
            <asp:Button ID="Button1" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button1_Click" Text="ІСНУЮЧІ ЗАМАВЛЕННЯ" Width="322px" />
            <asp:Button ID="Button2" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button2_Click" Text="НОВЕ ЗАМОВЛЕННЯ" Width="322px" />
    </form>
</body>
</html>
