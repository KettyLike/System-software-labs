<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page5.aspx.cs" Inherits="L5.page5" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function hideButton() {
            document.getElementById('Button1').style.visibility = 'hidden';
            document.getElementById('Button2').style.visibility = 'hidden';
            document.getElementById('Button3').style.visibility = 'hidden';
        }
    </script>
</head>
<body style="background: linear-gradient(to bottom, #62EEE4, #52F292);
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    margin: 0;">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label" Font-Size="XX-Large"></asp:Label>
            <br /><br />
            <asp:Label ID="Label2" runat="server" Visible="false" Text="Label" Font-Size="16pt"></asp:Label><br /><br />
            <asp:Image ID="Image1" runat="server" Style="max-width:150px; max-height:300px;" Visible="false" /><br /><br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="ReportID">
                <Columns>
                    <asp:BoundField DataField="ReportID" HeaderText="ID" />
                    <asp:BoundField DataField="UserName" HeaderText="Ім'я та Прізвище" />
                    <asp:BoundField DataField="CarModel" HeaderText="Модель машини" />
                    <asp:BoundField DataField="FormattedStartDate" HeaderText="Дата початку" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="FormattedEndDate" HeaderText="Дата завершення" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="Cost" HeaderText="Вартість" />                
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:RadioButton ID="RadioButtonSelect" runat="server" GroupName="OrderSelection"
                                OnCheckedChanged="RadioButtonSelect_CheckedChanged" AutoPostBack="true"
                                Text="Вибрати" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView><br /><br />
            <asp:Button ID="Button1" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button1_Click" Text="НАЗАД" Width="322px" />            
            <asp:Button ID="Button2" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button2_Click" Text="ПРИПИНИТИ" Width="322px" />
            <asp:Button ID="Button3" runat="server" OnClientClick = "hideButton(); c();" OnClick="Button3_Click" Text="ПРОДОВЖИТИ" Width="322px" />
        </div>
    </form>
</body>
</html>
