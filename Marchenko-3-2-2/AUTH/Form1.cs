using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace AUTH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(IsADAuthenticated(textBox1.Text, textBox2.Text).ToString());
        }

        public bool IsADAuthenticated(string L, string P)    //  L - Login, P - Password
        {
            try
            {
                using (DirectoryEntry AD = new DirectoryEntry("LDAP://DC=MARCHENKO,DC=UA", L, P))
                {
                    using (DirectorySearcher S = new DirectorySearcher(AD))
                    {
                        S.Filter = "(SAMAccountName=" + L + ")"; S.PropertiesToLoad.Add("cn");
                        SearchResult R = S.FindOne();
                        if (R == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool Validate(string L, string P)
        {
            using (PrincipalContext C = new PrincipalContext(ContextType.Domain, "MARIA.MARCHENKO.UA"))
            {
                bool R = C.ValidateCredentials(L, P);
                return R;
            }
        }

        public bool Validate2(string L, string P)
        {
            bool R = false;
            try
            {
                DirectoryEntry E = new DirectoryEntry("LDAP://DC=MARCHENKO,DC=UA", L, P);
                object N = E.NativeObject;
                R = true;
            }
            catch
            {
            }
            return (R);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Validate(textBox1.Text, textBox2.Text).ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Validate2(textBox1.Text, textBox2.Text).ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=Logistics,DC=MARCHENKO,DC=UA"))
            {
                using (DirectoryEntry u = AD.Children.Add("CN=Mariia", "user"))
                {
                    u.Properties["displayName"].Add("Mariia");
                    u.Properties["userPrincipalName"].Add("Mariia@Marchenko.ua");
                    u.Properties["sAMAccountName"].Add("Mariia");
                    u.CommitChanges();

                    SetPassword(u, "P@ssw0rd");
                    u.Properties["userAccountControl"].Value = "544";
                    u.CommitChanges();
                }
            }

            MessageBox.Show("OK");
        }

        private static void SetPassword(DirectoryEntry UE, string password)
        {
            object[] oPassword = new object[] { password };
            object ret = UE.Invoke("SetPassword", oPassword);
            UE.CommitChanges();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://DC=MARCHENKO,DC=UA"))
            {
                using (DirectoryEntry u = AD.Children.Add("OU=SupportTeam", "organizationalUnit"))
                {
                    u.CommitChanges();
                }
            }
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                using (DirectoryEntry u = AD.Children.Add("OU=Engineers", "organizationalUnit"))
                {
                    u.CommitChanges();
                }
            }
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                using (DirectoryEntry u = AD.Children.Add("OU=ComputerEngineers", "organizationalUnit"))
                {
                    u.CommitChanges();
                }
            }

            MessageBox.Show("OUs Created!");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                for (int i = 1; i < 31; i++)
                {
                    using (DirectoryEntry u = AD.Children.Add("CN=Maria" + i, "user"))
                    {
                        u.Properties["displayName"].Add("Maria" + i);
                        u.Properties["userPrincipalName"].Add("Maria" + i + "@Marchenko.ua");
                        u.Properties["sAMAccountName"].Add("Maria" + i);
                        u.CommitChanges();

                        SetPassword(u, "P@ssw0rd");
                        u.Properties["userAccountControl"].Value = "544";
                        u.Properties["department"].Add("Branch Office");
                        u.Properties["description"].Add("Very Good User");
                        u.Properties["businessCategory"].Add("Secretary");
                        u.Properties["businessCategory"].Add("Manager");
                        u.Properties["businessCategory"].Add("HelpDesk");
                        u.CommitChanges();
                    }
                }
            }

            MessageBox.Show("OK");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                foreach (DirectoryEntry u in AD.Children)
                {
                    if (u.Properties["objectClass"].Contains("user"))
                    {
                        u.Properties["Company"].Add("Kiev National University");
                        u.Properties["telephoneNumber"].Add("044-1234567");
                        u.Properties["MarchenkoDescription"].Add("Прогресивна працівниця");
                        u.Properties["MarchenkoID"].Add("98765");
                        u.Properties["MarchenkoTaxID"].Add("11223344");
                        u.Properties["MarchenkoCovidVaccinated"].Add("FALSE");
                        u.Properties["otherMobile"].Add("044-2345678");
                        u.Properties["otherMobile"].Add("044-3456789");
                        u.Properties["otherMobile"].Add("044-4567890");
                        u.CommitChanges();
                    }
                }

                MessageBox.Show("User Modified");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                using (DirectoryEntry g = AD.Children.Add("CN=SuperUsers", "group"))
                {
                    g.Properties["sAMAccountName"].Add("SuperUsers");
                    g.CommitChanges();
                }
                using (DirectoryEntry g = AD.Children.Add("CN=GoodUsers", "group"))
                {
                    g.Properties["sAMAccountName"].Add("GoodUsers");
                    g.CommitChanges();
                }
                using (DirectoryEntry g = new DirectoryEntry("LDAP://CN=GoodUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
                {
                    g.Properties["member"].Add("CN=SuperUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA");
                    g.CommitChanges();
                }
            }

            MessageBox.Show("Groups Created");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                using (DirectoryEntry G = new DirectoryEntry("LDAP://CN=SuperUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
                {
                    foreach (DirectoryEntry u in AD.Children)
                    {
                        if (u.Properties["objectClass"].Contains("user"))
                        {
                            G.Properties["member"].Add(u.Properties["distinguishedName"].Value);
                        }
                    }
                    G.CommitChanges();
                }

                foreach (DirectoryEntry u in AD.Children)
                {
                    if (u.Properties["objectClass"].Contains("user"))
                    {
                        u.MoveTo(new DirectoryEntry("LDAP://OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"));
                        u.CommitChanges();
                    }
                }
            }

            MessageBox.Show("Users Moved");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                richTextBox1.Clear();
                foreach (DirectoryEntry u in AD.Children)
                {
                    if (IsUserInSecurityGroup(u.Properties["distinguishedName"].Value.ToString(), "CN=SuperUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
                    richTextBox1.AppendText(u.Properties["cn"].Value.ToString() + "\n");
                }
            }            
        }

        protected bool IsUserInSecurityGroup(string user_dn, string sgroup_dn)
        {
            bool vv = false;
            using (DirectoryEntry AdList = new DirectoryEntry("LDAP://" + sgroup_dn))
            {
                PropertyCollection RPCol2 = AdList.Properties;
                foreach (string pr2 in RPCol2.PropertyNames)
                {
                    if (pr2.ToLower() == "member")
                    {
                        foreach (Object vc2 in RPCol2[pr2])
                        {
                            if (vc2.ToString() == user_dn)
                            {
                                vv = true;
                            }
                        }
                    }
                }
            }
            return vv;
        }

        protected bool IsUserInSecurityGroup2(string user_dn, string sgroup_dn)
        {
            using (DirectoryEntry G = new DirectoryEntry("LDAP://" + sgroup_dn))
            {
                DirectorySearcher S = new DirectorySearcher(G);
                S.Filter = "(member=" + user_dn + ")";
                SearchResultCollection SRC = S.FindAll();
                if (SRC.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsUserInSecurityGroup3(string user_dn, string sgroup_dn)
        {
            bool vv = false;
            using (DirectoryEntry G = new DirectoryEntry("LDAP://" + sgroup_dn))
            {
                if (G.Properties["member"].Contains(user_dn)) vv = true;
            }
            return vv;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                richTextBox1.Clear();
                foreach (DirectoryEntry u in AD.Children)
                {
                    if (IsUserInSecurityGroup2(u.Properties["distinguishedName"].Value.ToString(), "CN=SuperUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
                        richTextBox1.AppendText(u.Properties["cn"].Value.ToString() + "\n");
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                richTextBox1.Clear();
                foreach (DirectoryEntry u in AD.Children)
                {
                    if (IsUserInSecurityGroup3(u.Properties["distinguishedName"].Value.ToString(), "CN=SuperUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
                        richTextBox1.AppendText(u.Properties["cn"].Value.ToString() + "\n");
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://CN=SuperUsers,OU=ComputerEngineers,OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA"))
            {
                for (int i = 1; i < 3; i++)
                {
                    AD.Properties["member"].Remove("CN=Maria" + i + ",OU=Engineers,OU=SupportTeam,DC=MARCHENKO,DC=UA");
                }
                AD.CommitChanges();
            }

            MessageBox.Show("Users Removed");
        }
    }
}
