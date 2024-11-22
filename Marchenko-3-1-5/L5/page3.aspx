<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page3.aspx.cs" Inherits="L5.page3" %>

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
<body style="background: linear-gradient(to bottom, #D371EC, #7571EC);
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    margin: 0;">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label" Font-Size="XX-Large"></asp:Label>
            <br />
        </div>
            <br/><br/>
            <asp:Label ID="Label2" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
            <div style="display: inline-block; margin-right: 20px;">
                <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="true" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            </div>
            <div style="display: inline-block; margin-right: 20px;">
                <asp:Label ID="Label4" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:TextBox ID="TextBox2" runat="server" AutoPostBack="true" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
            </div>
            <div style="display: inline-block;">
                <asp:Label ID="Label5" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:TextBox ID="TextBox3" runat="server" AutoPostBack="true" OnTextChanged="TextBox3_TextChanged"></asp:TextBox>
            </div><br/><br/>
            <asp:Label ID="Label6" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
            
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                <asp:ListItem Text="Зазначте тривалість" Value="" />
                <asp:ListItem Text="1 день" Value="1 день" />
                <asp:ListItem Text="3 дні" Value="3 дні" />
                <asp:ListItem Text="1 тиждень" Value="1 тиждень" />
                <asp:ListItem Text="3 тижні" Value="3 тижні" />
            </asp:DropDownList>
            <br/><br/>
            <asp:Label ID="Label7" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
            <div style="display: inline-block; margin-right: 20px;">
                <asp:Label ID="Label8" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:TextBox ID="TextBox4" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div style="display: inline-block; margin-right: 20px;">
                <asp:Label ID="Label9" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:TextBox ID="TextBox5" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div style="display: inline-block;">
                <asp:Label ID="Label10" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:TextBox ID="TextBox6" runat="server" ReadOnly="true"></asp:TextBox>
            </div><br/><br/>
            <asp:Label ID="Label11" runat="server" Text="Label" Font-Size="XX-Large"></asp:Label><br/><br/>
            <asp:Button ID="Button1" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button1_Click" Text="ДАЛІ" Width="322px" />
            <asp:Button ID="Button2" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button2_Click" Text="НАЗАД" Width="322px" />
    </form>
</body>
</html>
