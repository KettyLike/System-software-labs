using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace L5
{
    public partial class page5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/page1.aspx"))
                    {
                        if (Session["UserName"].ToString() == string.Empty || Session["UserEmail"].ToString() == string.Empty)
                        {
                            Label1.Text = "ПОВЕРНІТЬСЯ НА 1-ШУ СТОРІНКУ!!!!";
                            Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                            Label2.Visible = true;
                            Control[] controlsToHide = { Image1, GridView1, Button2, Button3 };
                            foreach (Control control in controlsToHide)
                            {
                                control.Visible = false;
                            }
                        }
                        else
                        {
                            Display();
                        }
                    }
                    else
                    {
                        Label1.Text = "PLEASE GO TO 1ST PAGE!!!!";
                        Label2.Text = "REFERRER IS ABSENT";
                        Label2.Visible = true;
                        Control[] controlsToHide = { Image1, GridView1, Button2, Button3 };
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
                Label2.Visible = true;
                Control[] controlsToHide = { Image1, GridView1, Button2, Button3 };
                foreach (Control control in controlsToHide)
                {
                    control.Visible = false;
                }
            }
        }
        protected void Display()
        {
            Label1.Text = "ІСНУЮЧІ ЗАМОВЛЕННЯ";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RentalDBConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string pathQuery = "SELECT LicensePath FROM RentalReports WHERE UserName = @UserName";
                    SqlCommand pathCmd = new SqlCommand(pathQuery, conn);
                    pathCmd.Parameters.AddWithValue("@UserName", Session["UserName"]);
                    object result = pathCmd.ExecuteScalar();

                    if (result != null)
                    {
                        string fullPath = result.ToString();
                        string relativePath = fullPath?.Substring(fullPath.IndexOf("uploads"));
                        Image1.ImageUrl = "~/" + relativePath.Replace("\\", "/");
                        Image1.Visible = true;
                    }
                    else
                    {
                        Image1.Visible = false;
                    }

                    string query = "SELECT ReportID, UserName, CarModel, FORMAT(StartDate, 'dd.MM.yyyy') AS FormattedStartDate, FORMAT(EndDate, 'dd.MM.yyyy') AS FormattedEndDate, Cost FROM RentalReports WHERE UserName = @UserName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserName", Session["UserName"]);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
                catch (Exception ex)
                {
                    Label2.Text = "Помилка завантаження замовлень: " + ex.Message;
                    Label2.Visible = true;
                }
            }
        }

        protected void RadioButtonSelect_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            GridViewRow row = (GridViewRow)rb.NamingContainer;
            int reportId = Convert.ToInt32(GridView1.DataKeys[row.RowIndex].Value); 

            Session["SelectedReportID"] = reportId;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page1.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            if (Session["SelectedReportID"] != null)
            {
                Session["SelectedButton"] = "Stop";
                Response.Redirect("/page6.aspx");
            }
            else
            {
                Label2.Text = "Будь ласка, виберіть замовлення для припинення.";
                Label2.Visible = true;
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            if (Session["SelectedReportID"] != null)
            {
                Session["SelectedButton"] = "Extend";
                Response.Redirect("/page6.aspx");
            }
            else
            {
                Label2.Text = "Будь ласка, виберіть замовлення для продовження.";
                Label2.Visible = true;
            }
        }
    }
}