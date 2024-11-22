<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page2.aspx.cs" Inherits="L5.page2" %>

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
<body style="background: linear-gradient(to bottom, #F097D8, #D371EC);
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
            <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/><br/>
            <asp:Label ID="Label4" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
            <asp:FileUpload ID="U1" runat="server" Width="250px" />
            <input type="submit" id="Submit1" value="UPLOAD " runat="server" 
                name="Submit1" onserverclick="Submit1_ServerClick1"
                size="450px" style="color: #0000FF; font-weight: bold; width: 250px;" /><br />
            <asp:Label ID="lblError" style="color: red;" runat="server" CssClass="error" Visible="false" /><br/><br/>
            <div style="display: inline-block; margin-right: 50px;">
                <asp:Label ID="Label5" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="carClass_SelectedIndexChanged">
                    <asp:ListItem Text="Виберіть клас" Value="" />
                    <asp:ListItem Text="Легковий" Value="Легковий" />
                    <asp:ListItem Text="Дрібновантажний" Value="Дрібновантажний" />
                </asp:DropDownList>
            </div>
            <div style="display: inline-block;">
                <asp:Label ID="Label6" runat="server" Text="Label" Font-Size="16pt"></asp:Label><br/>
                <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="carModel_SelectedIndexChanged">
                    <asp:ListItem Text="Оберіть модель" Value="" />
                </asp:DropDownList>
            </div><br/><br/><br/><br/>
            <asp:Button ID="Button1" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button1_Click" Text="ДАЛІ" Width="322px" />
            <asp:Button ID="Button2" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button2_Click" Text="НАЗАД" Width="322px" />
    </form>
</body>
</html>
