using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace L2_1
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        public Form1()
        {
            InitializeComponent();
            comboBoxFill();
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            string DB = @"Data Source = (localDB)\MSSQLLocalDB;Initial Catalog=CRIMES-LAB2;Integrated Security=True";
            try
            {
                conn = new SqlConnection(DB);
                conn.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }

            string queryCheck1 = "select * from CCU where Section = N'Загальна';";
            string insertQuery1 = "insert into CCU (Section) values (N'Загальна');";
            CheckAndInsert(queryCheck1, insertQuery1);
            string queryCheck2 = "select * from CCU where Section = N'Особлива';";
            string insertQuery2 = "insert into CCU (Section) values (N'Особлива');";
            CheckAndInsert(queryCheck2, insertQuery2);
        }

        public void CheckAndInsert(string queryCheck, string insertQuery)
        {
            SqlCommand command = new SqlCommand(queryCheck, conn);
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Close();
            }
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (checkBox1.Checked == false && checkBox2.Checked == false)
                {
                    DGV1.Refresh();

                    string Query = "select * from CRIMES as x inner join CCU as y on x.Section_ID = y.ID";
                    DGV1.DataSource = GetSQLTable(Query);
                    for (int i = 0; i < DGV1.Columns.Count; i++)
                        DGV1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                else if (checkBox1.Checked && checkBox2.Checked == false)
                {
                    if (IsValidNumber(textBox1.Text))
                    {
                        DGV1.Refresh();

                        string Query = $"select * from CRIMES as x inner join CCU as y on x.Section_ID = y.ID where x.ID = '{textBox1.Text}';";
                        DGV1.DataSource = GetSQLTable(Query);
                        for (int i = 0; i < DGV1.Columns.Count; i++)
                            DGV1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                    else MessageBox.Show("Для виконання даного запиту поле \"ID\" не може бути пустим!");
                }
                else if (checkBox1.Checked == false && checkBox2.Checked)
                {
                    if (IsValidNumber(comboBox1.Text))
                    {
                        DGV1.Refresh();

                        string Query = $"select * from CRIMES as x inner join CCU as y on x.Section_ID = y.ID where x.Section_ID = '{comboBox1.SelectedItem}';";
                        DGV1.DataSource = GetSQLTable(Query);
                        for (int i = 0; i < DGV1.Columns.Count; i++)
                            DGV1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                    else MessageBox.Show("Для виконання даного запиту поле \"Section_ID\" не може бути пустим!");
                }
                else if (checkBox2.Checked && checkBox1.Checked)
                {
                    if (IsValidNumber(comboBox1.Text) && IsValidNumber(textBox1.Text))
                    {
                        DGV1.Refresh();

                        string Query = $"select * from CRIMES as x inner join CCU as y on x.Section_ID = y.ID where x.Section_ID = '{comboBox1.SelectedItem}' and x.ID = '{textBox1.Text}';";
                        DGV1.DataSource = GetSQLTable(Query);
                        for (int i = 0; i < DGV1.Columns.Count; i++)
                            DGV1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                    else MessageBox.Show("Для виконання даного запиту поля \"ID\" та \"Section_ID\" не можуть бути пустим!");
                }
            }
            else if (radioButton2.Checked)
            {
                if (IsValidString(textBox2.Text) && IsValidNumber(textBox3.Text) && IsValidNumber(textBox4.Text) && IsValidNumber(comboBox1.Text) && IsValidString(textBox5.Text))
                {
                    string Query = $"insert into CRIMES (Crime_Name, Article, Part, Section_ID, Qualification) values (N'{textBox2.Text}', {textBox3.Text}, {textBox4.Text}, {comboBox1.SelectedItem}, N'{textBox5.Text}')";
                    ExecuteQuery(Query);
                }
                else MessageBox.Show("Для виконання даного запиту всі поля, крім \"ID\" повинні бути заповненими правильним форматом даних!");
            }
            else if (radioButton3.Checked)
            {
                if (IsValidNumber(textBox1.Text))
                {
                    string Query = $"update CRIMES set Crime_Name = N'{textBox2.Text}', Article = {textBox3.Text}, Part = {textBox4.Text}, Section_ID = {comboBox1.SelectedItem}, Qualification = N'{textBox5.Text}' where ID = {textBox1.Text}";
                    ExecuteQuery(Query);
                }
                else MessageBox.Show("Для виконання даного запиту всі поля повинні бути заповненими правильним форматом даних!");
            }
            else if (radioButton4.Checked)
            {
                if (IsValidNumber(textBox1.Text))
                {
                    string Query = $"delete from CRIMES where ID = {textBox1.Text};";
                    ExecuteQuery(Query);
                }
                else MessageBox.Show("Для виконання даного запиту поле \"ID\" повинне бути заповненим правильним форматом даних!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 childForm = new Form2();
            childForm.ShowDialog();
        }

        public static DataTable GetSQLTable(string Query)
        {
            string DB = $"Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=CRIMES-LAB2;Integrated Security=True";
            using (SqlConnection Conn = new SqlConnection(DB))
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                Conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(Query, DB);
                da.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
        }

        public void ExecuteQuery(string Query)
        {
            string DB = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=CRIMES-LAB2;Integrated Security=True";
            using (SqlConnection сonn = new SqlConnection(DB))
            {
                сonn.Open();
                SqlCommand Comm = new SqlCommand(Query, сonn);
                MessageBox.Show(Comm.ExecuteNonQuery().ToString());
            }
        }

        private void comboBoxFill()
        {
            string Query = "select * from CCU;";
            DataTable T = GetSQLTable(Query);
            foreach (DataRow row in T.Rows)
                comboBox1.Items.Add(row[0]);
        }

        public bool IsValidNumber(string input)
        {
            Regex regex = new Regex("^[0-9]+$");
            Match match = regex.Match(input);
            return match.Success;
        }

        public bool IsValidString(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
