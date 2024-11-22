using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace L5
{
    public partial class page1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "ОРЕНДУЙТЕ І КАТАЙТЕСЬ З КОМПАНІЄЮ «РАУЛЬ БЕЗ РУЛЯ»!";
            Label2.Text = "Прізвище/Ім’я латиницею:";
            Label3.Text = "Емейл-адреса:";
            Session["UserEmail"] = TextBox2.Text.Trim();
            if (!IsPostBack)
            {
                Button1.Attributes.Add("onclick", "hideButton();");
                Button2.Attributes.Add("onclick", "hideButton();");
                Button1.Click += new EventHandler(Button1_Click);
                Button2.Click += new EventHandler(Button2_Click);
            }
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateAndProcessInput();
        }

        private void ValidateAndProcessInput()
        {
            if (string.IsNullOrWhiteSpace(TextBox1.Text))
            {
                return;
            }
            try
            {
                string input = TextBox1.Text.Trim();
                Regex regex = new Regex(@"^[A-Za-zÀ-ÿ\s'-]+$");
                if (regex.IsMatch(input))
                {
                    input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());

                    Session["UserName"] = input;
                }
                else
                {
                    Label2.Text = "Будь ласка, введіть Прізвище та Ім’я латиницею.";
                    TextBox1.Text = "";
                }
            }
            catch
            {
                Label2.Text = "Щось пішло не так.";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page5.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page2.aspx");
        }
    }
}