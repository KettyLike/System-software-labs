using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace L2_1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                DGV1.Refresh();

                string Query = "select * from CCU";
                DGV1.DataSource = GetSQLTable(Query);
                for (int i = 0; i < DGV1.Columns.Count; i++)
                    DGV1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
            else if (radioButton2.Checked)
            {
                if (IsValidString(textBox2.Text))
                {
                    string Query = $"insert into CCU (Section) values (N'{textBox2.Text}');";
                    ExecuteQuery(Query);
                }
                else MessageBox.Show("Для виконання даного запиту поле \"Section\" повинне бути заповненими правильним форматом даних!");
            }
            else if (radioButton3.Checked)
            {
                if (IsValidNumber(textBox1.Text))
                {
                    string Query = $"update CCU set Section = N'{textBox2.Text}' where ID = {textBox1.Text}";
                    ExecuteQuery(Query);
                }
                else MessageBox.Show("Для виконання даного запиту всі поля повинні бути заповненими правильним форматом даних!");
            }
            else if (radioButton4.Checked)
            {
                if (IsValidNumber(textBox1.Text))
                {
                    string Query = $"delete from CCU where ID = {textBox1.Text};";
                    ExecuteQuery(Query);
                }
                else MessageBox.Show("Для виконання даного запиту поле \"ID\" повинне бути заповненим правильним форматом даних!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 Form1 = new Form1();
            Form1.Show();
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

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}