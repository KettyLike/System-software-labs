using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace L5
{
    public partial class page3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/page2.aspx"))
                    {
                        if (Session["LicensePath"].ToString() == string.Empty || Session["CarClass"].ToString() == string.Empty || Session["CarModel"].ToString() == string.Empty)
                        {
                            Label1.Text = "ПОВЕРНІТЬСЯ НА 2-ГУ СТОРІНКУ!!!!";
                            Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                            Control[] controlsToHide = { Label3, Label4, Label5, Label6, Label7, Label8, Label9, Label10, Label11, TextBox1, TextBox2, TextBox3, TextBox4, TextBox5, TextBox6, DropDownList1, Button1 };

                            foreach (Control control in controlsToHide)
                            {
                                control.Visible = false;
                            }
                        }
                        else
                        {
                            Label1.Text = "НОВЕ ЗАМОВЛЕННЯ — КРОК 2";
                            Label2.Text = "Дата початку оренди:";
                            Label3.Text = "Число:";
                            Label4.Text = "Місяць:";
                            Label5.Text = "Рік:";
                            Label6.Text = "Тривалість:";
                            Label7.Text = "Дата закінчення оренди:";
                            Label8.Text = "Число:";
                            Label9.Text = "Місяць:";
                            Label10.Text = "Рік:";
                            Button1.Attributes.Add("onclick", "hideButton();");
                            Button2.Attributes.Add("onclick", "hideButton();");
                            Button1.Click += new EventHandler(Button1_Click);
                            Button2.Click += new EventHandler(Button2_Click);
                        }
                    }
                    else
                    {
                        Label1.Text = "PLEASE GO TO 2ND PAGE!!!!";
                        Label2.Text = "REFERRER IS ABSENT";
                        Control[] controlsToHide = { Label3, Label4, Label5, Label6, Label7, Label8, Label9, Label10, Label11, TextBox1, TextBox2, TextBox3, TextBox4, TextBox5, TextBox6, DropDownList1, Button1 };

                        foreach (Control control in controlsToHide)
                        {
                            control.Visible = false;
                        }
                    }
                }
                
            }
            catch
            {
                Label1.Text = "ERROR";
                Label2.Text = "REFERRER IS ABSENT";
                Control[] controlsToHide = { Label3, Label4, Label5, Label6, Label7, Label8, Label9, Label10, Label11, TextBox1, TextBox2, TextBox3, TextBox4, TextBox5, TextBox6, DropDownList1, Button1 };

                foreach (Control control in controlsToHide)
                {
                    control.Visible = false;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page4.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page2.aspx");
        }

        protected void CalculateEndDate()
        {
            if (string.IsNullOrWhiteSpace(TextBox1.Text) ||
        string.IsNullOrWhiteSpace(TextBox2.Text) ||
        string.IsNullOrWhiteSpace(TextBox3.Text) ||
        string.IsNullOrWhiteSpace(DropDownList1.SelectedValue))
            {
                return;
            }

            try
            {
                int startDay = int.Parse(TextBox1.Text);
                int startYear = int.Parse(TextBox3.Text);
                string startMonthStr = TextBox2.Text.Trim().ToLower();

                if (!IsValidMonth(startMonthStr))
                {
                    throw new FormatException("Некоректний місяць");
                }

                int startMonth = ConvertMonthToNumber(startMonthStr);
                DateTime startDate = new DateTime(startYear, startMonth, startDay);

                DateTime endDate = startDate;
                string duration = DropDownList1.SelectedValue;

                switch (duration)
                {
                    case "1 день":
                        endDate = startDate.AddDays(1);
                        break;
                    case "3 дні":
                        endDate = startDate.AddDays(3);
                        break;
                    case "1 тиждень":
                        endDate = startDate.AddDays(7);
                        break;
                    case "3 тижні":
                        endDate = startDate.AddDays(21);
                        break;
                }

                TextBox4.Text = endDate.Day.ToString();
                TextBox5.Text = GetMonthName(endDate.Month);
                TextBox6.Text = endDate.Year.ToString();

                double cost = CalculateCost(startDate, duration);

                Session["StartDate"] = startDate.ToString("dd.MM.yyyy");
                Session["EndDate"] = endDate.ToString("dd.MM.yyyy");
                Session["Duration"] = duration;
                Session["Cost"] = cost.ToString("F2") + " грн";

                Label11.Text = "Вартість замовлення = " + Session["Cost"];
            }
            catch (Exception ex)
            {
                Label2.Text = "Будь ласка, введіть коректні дані для початкової дати! " + ex.Message;
            }
        }

        protected bool IsValidMonth(string month)
        {
            string[] validMonths = new string[]
            {
                "січень", "лютий", "березень", "квітень", "травень", "червень",
                "липень", "серпень", "вересень", "жовтень", "листопад", "грудень"
            };
            return validMonths.Contains(month);
        }

        protected int ConvertMonthToNumber(string month)
        {
            Dictionary<string, int> monthMapping = new Dictionary<string, int>()
            {
                { "січень", 1 }, { "лютий", 2 }, { "березень", 3 }, { "квітень", 4 },
                { "травень", 5 }, { "червень", 6 }, { "липень", 7 }, { "серпень", 8 },
                { "вересень", 9 }, { "жовтень", 10 }, { "листопад", 11 }, { "грудень", 12 }
            };

            return monthMapping.ContainsKey(month) ? monthMapping[month] : 0;
        }

        protected string GetMonthName(int month)
        {
            string[] monthNames = new string[]
            {
                "січень", "лютий", "березень", "квітень", "травень", "червень",
                "липень", "серпень", "вересень", "жовтень", "листопад", "грудень"
            };

            return monthNames[month - 1];
        }

        protected double CalculateCost(DateTime startDate, string duration)
        {
            double dailyPrice = Convert.ToDouble(Session["CarPrice"]);
            double totalCost = 0;

            switch (duration)
            {
                case "1 день":
                    totalCost = dailyPrice;
                    break;
                case "3 дні":
                    totalCost = dailyPrice * 0.7 * 3;
                    break;
                case "1 тиждень":
                    totalCost = dailyPrice * 0.6 * 7;
                    break;
                case "3 тижні":
                    totalCost = dailyPrice * 0.5 * 21;
                    break;
            }

            return totalCost;
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            CalculateEndDate();
        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {
            CalculateEndDate();
        }

        protected void TextBox3_TextChanged(object sender, EventArgs e)
        {
            CalculateEndDate();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateEndDate();
        }
    }
}