using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace L5
{
    public partial class page2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/page1.aspx"))
                {
                    if (Session["UserName"].ToString() == string.Empty || Session["UserEmail"].ToString() == string.Empty)
                    {
                        Label1.Text = "ПОВЕРНІТЬСЯ НА 1-ШУ СТОРІНКУ!!!!";
                        Label2.Text = "ЗАПОВНІТЬ СПОЧАТКУ ВСІ ДАНІ";
                        Control[] controlsToHide = { Label3, Label4, Label5, Label6, U1, Submit1, DropDownList1, DropDownList2, Button1 };

                        foreach (Control control in controlsToHide)
                        {
                            control.Visible = false;
                        }
                    }
                    else
                    {
                        Label1.Text = "НОВЕ ЗАМОВЛЕННЯ — КРОК 1";
                        Label2.Text = "Прізвище/Ім’я латиницею:";
                        Label3.Text = Session["UserName"].ToString();
                        Label4.Text = "ПОСВІДЧЕННЯ ВОДІЯ (JPEG/PNG, min 100x150, max 200x300):";
                        Label5.Text = "КЛАС АВТО:";
                        Label6.Text = "МАРКА І МОДЕЛЬ:";
                        Button1.Attributes.Add("onclick", "hideButton();");
                        Button2.Attributes.Add("onclick", "hideButton();");
                        Button1.Click += new EventHandler(Button1_Click);
                        Button2.Click += new EventHandler(Button2_Click);
                    }                    
                }
            }
            catch 
            {
                Label1.Text = "PLEASE GO TO 1ST PAGE!!!!";
                Label2.Text = "REFERRER IS ABSENT";
                Control[] controlsToHide = { Label3, Label4, Label5, Label6, U1, Submit1, DropDownList1, DropDownList2, Button1 };

                foreach (Control control in controlsToHide)
                {
                    control.Visible = false;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page3.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            Response.Redirect("/page1.aspx");
        }

        protected void Submit1_ServerClick1(object sender, EventArgs e)
        {
            if (U1.HasFile)
            {
                string extension = Path.GetExtension(U1.FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    try
                    {
                        using (var image = System.Drawing.Image.FromStream(U1.PostedFile.InputStream))
                        {
                            int fileWidth = image.Width;
                            int fileHeight = image.Height;

                            if (fileWidth >= 100 && fileHeight >= 150 && fileWidth <= 200 && fileHeight <= 300)
                            {
                                string fileName = Path.GetFileNameWithoutExtension(U1.FileName);
                                string savePath = Server.MapPath("~/uploads/");
                                string fullSavePath = Path.Combine(savePath, fileName + extension);

                                int counter = 1;
                                while (File.Exists(fullSavePath))
                                {
                                    fullSavePath = Path.Combine(savePath, $"{fileName}_{counter}{extension}");
                                    counter++;
                                }

                                U1.PostedFile.SaveAs(fullSavePath);
                                Session["LicensePath"] = fullSavePath;
                                lblError.Text = "Файл успішно завантажено.";
                                lblError.Visible = true;
                            }
                            else
                            {
                                lblError.Text = "Помилка: розміри зображення мають бути від 100x150 до 200x300 пікселів.";
                                lblError.Visible = true;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = "Помилка при обробці файлу: " + ex.Message;
                        lblError.Visible = true;
                        return;
                    }
                }
                else
                {
                    lblError.Text = "Помилка: файл повинен бути у форматі JPG, JPEG або PNG.";
                    lblError.Visible = true;
                }
            }
            else
            {
                lblError.Text = "Помилка: ви не завантажили файл посвідчення водія.";
                lblError.Visible = true;
            }
        }

        protected void carClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList2.Items.Clear();
            string selectedClass = DropDownList1.SelectedValue;
            Session["CarClass"] = selectedClass;

            if (selectedClass == "Легковий")
            {
                foreach (var auto in CarModels.Small)
                {
                    DropDownList2.Items.Add(new ListItem($"{auto.Key} ({auto.Value} грн/доба)", auto.Key));
                }
            }
            else if (selectedClass == "Дрібновантажний")
            {
                foreach (var auto in CarModels.Large)
                {
                    DropDownList2.Items.Add(new ListItem($"{auto.Key} ({auto.Value} грн/доба)", auto.Key));
                }
            }
        }

        protected void carModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedModel = DropDownList2.SelectedValue;
            Session["CarModel"] = selectedModel;

            Session["CarOptions"] = new Dictionary<string, int>
            {
                {"Volkswagen Passat", 800},
                {"Skoda Fabia", 700},
                {"Toyota Corolla", 750},
                {"Honda Civic", 780},
                {"Ford Focus", 730},
                {"Mercedes Sprinter", 1200},
                {"Ford Transit", 1150},
                {"Volkswagen Crafter", 1250}
            };

            var carOptions = Session["CarOptions"] as Dictionary<string, int>;
            if (carOptions != null && carOptions.ContainsKey(selectedModel))
            {
                int carPrice = carOptions[selectedModel];
                Session["CarPrice"] = carPrice; 
            }
        }
    }

    public static class CarModels
    {
        public static readonly Dictionary<string, int> Small = new Dictionary<string, int>()
    {
        {"Оберіть модель", 0},
        {"Volkswagen Passat", 800},
        {"Skoda Fabia", 700},
        {"Toyota Corolla", 750},
        {"Honda Civic", 780},
        {"Ford Focus", 730}
    };

        public static readonly Dictionary<string, int> Large = new Dictionary<string, int>()
    {
        {"Оберіть модель", 0},
        {"Mercedes Sprinter", 1200},
        {"Ford Transit", 1150},
        {"Volkswagen Crafter", 1250}
    };
    }
}