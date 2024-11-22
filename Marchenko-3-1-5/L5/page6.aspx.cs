using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace L5
{
    public partial class page6 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/page5.aspx"))
                    {
                        if (Session["SelectedReportID"].ToString() == string.Empty || Session["SelectedButton"].ToString() == string.Empty)
                        {
                            Label1.Text = "ПОВЕРНІТЬСЯ НА 5-ТУ СТОРІНКУ!!!!";
                            Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                            Control[] controlsToHide = { DropDownList1, Button2 };
                            foreach (Control control in controlsToHide)
                            {
                                control.Visible = false;
                            }
                        }
                        else
                        {
                            Label1.Text = "ПІДТВЕРДЖЕННЯ ДІЇ";
                            string action = Request.QueryString["action"];
                            if (Session["SelectedButton"].ToString() == "Stop")
                            {
                                Label2.Text = "Ви впевнені, що хочете припинити це замовлення?";
                                DropDownList1.Visible = false;
                            }                                
                            else if (Session["SelectedButton"].ToString() == "Extend")
                            {
                                Label2.Text = "Ви впевнені, що хочете продовжити це замовлення?";
                                DropDownList1.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        Label1.Text = "PLEASE GO TO 5TH PAGE!!!!";
                        Label2.Text = "REFERRER IS ABSENT";
                        Control[] controlsToHide = { DropDownList1, Button2 };
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
                Control[] controlsToHide = { DropDownList1, Button2 };
                foreach (Control control in controlsToHide)
                {
                    control.Visible = false;
                }
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page5.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (Session["SelectedButton"].ToString() == "Extend")
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RentalDBConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand fetchCommand = new SqlCommand("SELECT UserName, UserEmail, CarClass, CarModel, FORMAT(StartDate, 'dd.MM.yyyy') AS FormattedStartDate, EndDate, Cost, LicensePath FROM RentalReports WHERE ReportID = @ReportID", connection);
                    fetchCommand.Parameters.AddWithValue("@ReportID", Session["SelectedReportID"]);

                    SqlDataReader reader = fetchCommand.ExecuteReader();
                    DateTime endDate;
                    decimal currentCost;
                    string carClass;
                    string carModel;

                    if (reader.Read())
                    {
                        endDate = Convert.ToDateTime(reader["EndDate"]);

                        string costString = reader["Cost"].ToString();
                        costString = System.Text.RegularExpressions.Regex.Replace(costString, @"[^\d.,]", ""); // видаляє все, крім цифр і розділювача
                        currentCost = Decimal.Parse(costString);
                        carClass = reader["CarClass"].ToString();
                        carModel = reader["CarModel"].ToString();
                        Session["CarClass"] = carClass;
                        Session["CarModel"] = carModel;
                        Session["UserName"] = reader["UserName"].ToString();
                        Session["UserEmail"] = reader["UserEmail"].ToString();
                        Session["StartDate"] = reader["FormattedStartDate"].ToString();
                        Session["LicensePath"] = reader["LicensePath"].ToString();
                    }
                    else
                    {
                        Label2.Text = "Report not found.";
                        return;
                    }
                    reader.Close();

                    string selectedDuration = DropDownList1.SelectedValue;
                    Session["selDur"] = selectedDuration;

                    switch (selectedDuration)
                    {
                        case "1 день":
                            endDate = endDate.AddDays(1);
                            break;
                        case "3 дні":
                            endDate = endDate.AddDays(3);
                            break;
                        case "1 тиждень":
                            endDate = endDate.AddDays(7);
                            break;
                        case "3 тижні":
                            endDate = endDate.AddDays(21);
                            break;
                    }

                    decimal additionalCost = (decimal)CalculateCost(carClass, carModel, selectedDuration);
                    decimal updatedCost = currentCost + additionalCost;

                    SqlCommand updateCommand = new SqlCommand("UPDATE RentalReports SET EndDate = @NewEndDate, Cost = @NewCost WHERE ReportID = @ReportID", connection);
                    updateCommand.Parameters.AddWithValue("@NewEndDate", endDate);
                    updateCommand.Parameters.AddWithValue("@NewCost", updatedCost);
                    updateCommand.Parameters.AddWithValue("@ReportID", Session["SelectedReportID"]);

                    updateCommand.ExecuteNonQuery();

                    Session["endDate"] = endDate.ToString("dd.MM.yyyy");
                    Session["Cost"] = updatedCost;

                    System.Threading.Thread.Sleep(2000);
                    Response.Redirect("/page7.aspx");
                }
            }
            else if (Session["SelectedButton"].ToString() == "Stop")
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RentalDBConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand deleteCommand = new SqlCommand("DELETE FROM RentalReports WHERE ReportID = @ReportID", connection);
                    deleteCommand.Parameters.AddWithValue("@ReportID", Session["SelectedReportID"]);

                    deleteCommand.ExecuteNonQuery();
                }

                System.Threading.Thread.Sleep(2000);
                Response.Redirect("/page7.aspx");
            }
        }

        protected double CalculateCost(string carClass, string carModel, string duration)
        {
            double dailyPrice = 0;
            if (carClass == "Легковий" && CarModels.Small.ContainsKey(carModel))
            {
                dailyPrice = CarModels.Small[carModel];
            }
            else if (carClass == "Дрібновантажний" && CarModels.Large.ContainsKey(carModel))
            {
                dailyPrice = CarModels.Large[carModel];
            }
            else
            {
                throw new ArgumentException("Модель не знайдена у вибраному класі авто.");
            }

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
    }
}