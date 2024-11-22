using System;
using System.Net.Mail;
using System.Net;
using System.Web.UI;

namespace L5
{
    public partial class page7 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/page6.aspx"))
                    {
                        if (Session["SelectedButton"].ToString() == string.Empty)
                        {
                            Label1.Text = "ПОВЕРНІТЬСЯ НА 5-ТУ СТОРІНКУ!!!!";
                            Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                            Control[] controlsToHide = { Label3, Label4, Label5, Label6, Image1, Button1 };
                            foreach (Control control in controlsToHide)
                            {
                                control.Visible = false;
                            }
                        }
                        else if (Session["SelectedButton"].ToString() == "Extend" && Session["selDur"].ToString() == string.Empty)
                        {
                            Label1.Text = "ПОВЕРНІТЬСЯ НА 6-ТУ СТОРІНКУ!!!!";
                            Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                            Control[] controlsToHide = { Label3, Label4, Label5, Label6, Image1, Button1 };
                            foreach (Control control in controlsToHide)
                            {
                                control.Visible = false;
                            }
                        }
                        else
                        {
                            DisplayReport();
                            SendEmailReport();
                        }
                    }
                    else
                    {
                        Label1.Text = "PLEASE GO TO 6TH PAGE!!!!";
                        Label2.Text = "REFERRER IS ABSENT";
                        Control[] controlsToHide = { Label3, Label4, Label5, Label6, Image1, Button1 };

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
                Control[] controlsToHide = { Label3, Label4, Label5, Image1, Button1 };
                foreach (Control control in controlsToHide)
                {
                    control.Visible = false;
                }
            }
        }
        protected void DisplayReport()
        {
            Label1.Text = "ЗВІТ ПРО ПРИПИНЕННЯ/ПОДОВЖЕННЯ ЗАМОВЛЕННЯ";
            Label2.Text = "Ім'я/Прізвище: " + Session["UserName"];
            string fullPath = Session["LicensePath"]?.ToString();
            string relativePath = fullPath?.Substring(fullPath.IndexOf("uploads"));
            if (Session["LicensePath"] != null)
            {
                Image1.ImageUrl = "~/" + relativePath.Replace("\\", "/");
            }
            Label3.Text = "Тип машини: " + Session["CarClass"] + "<br />Модель машини: " + Session["CarModel"];
            Label4.Text = $"Дата початку оренди: {Session["StartDate"]}<br />" +
            $"Дата закінчення оренди: {Session["EndDate"]}<br />" + $"Вартість: {Session["Cost"]}";
            if (Session["SelectedButton"].ToString() == "Extend")
            {
                Label5.Text = "Ваше замовлення було подовжено.";
            }
            else
            {
                Label5.Text = "Ваше замовлення було припинено.";
            }
            Label6.Text = "Звіт надіслано на адресу: " + Session["UserEmail"];
        }

        protected void SendEmailReport()
        {
            try
            {
                string userEmail = Session["UserEmail"]?.ToString();
                string userName = Session["UserName"]?.ToString();
                string carClass = Session["CarClass"]?.ToString();
                string carModel = Session["CarModel"]?.ToString();
                string startDate = Session["StartDate"]?.ToString();
                string endDate = Session["EndDate"]?.ToString();
                string cost = Session["Cost"]?.ToString();

                string emailBody = $@"
                    <h2>Звіт про припинення/подовження замовлення</h2>
                    <p><strong>Ім'я/Прізвище:</strong> {userName}</p>
                    <p><strong>Тип машини:</strong> {carClass}</p>
                    <p><strong>Модель машини:</strong> {carModel}</p>
                    <p><strong>Дата початку оренди:</strong> {startDate}</p>
                    <p><strong>Дата закінчення оренди:</strong> {endDate}</p>
                    <p><strong>Вартість:</strong> {cost}</p>";
                if  (Session["SelectedButton"].ToString() == "Extend")
                {
                    emailBody += "Ваше замовлення було подовжено.";
                } else
                {
                    emailBody += "Ваше замовлення було припинено.";
                }

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("test5220480@gmail.com"),
                    Subject = "Звіт про оренду машини",
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(userEmail);

                SmtpClient C = new SmtpClient("smtp.gmail.com");
                C.Port = 587;
                C.Credentials = new NetworkCredential("test5220480", "evzh ldsu xsys bnne");
                C.EnableSsl = true;
                C.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Label5.Text = "Помилка при надсиланні звіту: " + ex.Message;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page1.aspx");
        }
    }
}