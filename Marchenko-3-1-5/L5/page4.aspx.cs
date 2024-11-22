using System;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Data.SqlClient;

namespace L5
{
    public partial class page4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/page3.aspx"))
                    {
                        if (Session["EndDate"].ToString() == string.Empty || Session["Cost"].ToString() == string.Empty || Session["Duration"].ToString() == string.Empty)
                        {
                            Label1.Text = "ПОВЕРНІТЬСЯ НА 3-ТЮ СТОРІНКУ!!!!";
                            Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                            Control[] controlsToHide = { Label3, Label4, Label5, Image1, Button1 };
                            foreach (Control control in controlsToHide)
                            {
                                control.Visible = false;
                            }
                        }
                        else
                        {
                            DisplayReport();
                            SendEmailReport();
                            SaveReportToDatabase();
                        }
                    }
                    else
                    {
                        Label1.Text = "PLEASE GO TO 3RD PAGE!!!!";
                        Label2.Text = "REFERRER IS ABSENT";
                        Control[] controlsToHide = { Label3, Label4, Label5, Image1, Button1 };

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
            Label1.Text = "ЗВІТ ПРО ЗАМОВЛЕННЯ";
            Label2.Text = "Ім'я/Прізвище: " + Session["UserName"];
            string fullPath = Session["LicensePath"]?.ToString();
            string relativePath = fullPath?.Substring(fullPath.IndexOf("uploads"));
            if (Session["LicensePath"] != null)
            {
                Image1.ImageUrl = "~/" + relativePath.Replace("\\", "/");
            }
            Label3.Text = "Тип машини: " + Session["CarClass"] + "<br />Модель машини: " + Session["CarModel"];
            Label4.Text = $"Дата початку оренди: {Session["StartDate"]}<br />" +
            $"Дата закінчення оренди: {Session["EndDate"]}<br />" +
            $"Тривалість оренди: {Session["Duration"]}<br />" +
                                      $"Вартість: {Session["Cost"]}";
            Label5.Text = "Звіт надіслано на адресу: " + Session["UserEmail"];
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
                string duration = Session["Duration"]?.ToString();
                string cost = Session["Cost"]?.ToString();

                string emailBody = $@"
                    <h2>Звіт про замовлення</h2>
                    <p><strong>Ім'я/Прізвище:</strong> {userName}</p>
                    <p><strong>Тип машини:</strong> {carClass}</p>
                    <p><strong>Модель машини:</strong> {carModel}</p>
                    <p><strong>Дата початку оренди:</strong> {startDate}</p>
                    <p><strong>Дата закінчення оренди:</strong> {endDate}</p>
                    <p><strong>Тривалість оренди:</strong> {duration}</p>
                    <p><strong>Вартість:</strong> {cost}</p>";

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

        protected void SaveReportToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RentalDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO RentalReports (UserName, UserEmail, CarClass, CarModel, StartDate, EndDate, Cost, LicensePath) " +
                               "VALUES (@UserName, @UserEmail, @CarClass, @CarModel, @StartDate, @EndDate, @Cost, @LicensePath)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", Session["UserName"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UserEmail", Session["UserEmail"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CarClass", Session["CarClass"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CarModel", Session["CarModel"] ?? DBNull.Value); 
                    command.Parameters.AddWithValue("@StartDate", DateTime.ParseExact(Session["StartDate"].ToString(), "dd.MM.yyyy", null));
                    command.Parameters.AddWithValue("@EndDate", DateTime.ParseExact(Session["EndDate"].ToString(), "dd.MM.yyyy", null));
                    command.Parameters.AddWithValue("@Cost", Session["Cost"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LicensePath", Session["LicensePath"] ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}