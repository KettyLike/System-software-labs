using System;
using System.DirectoryServices;

namespace SEARCH
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string R = String.Empty;
            using (DirectoryEntry AD = new DirectoryEntry("LDAP://DC=MARCHENKO,DC=UA"))
            {
                using (DirectorySearcher S = new DirectorySearcher(AD))
                {
                    S.Filter = "(&(objectCategory=user)(objectClass=user)(samaccountname=*-23*))";
                    S.PropertiesToLoad.Add("sn");
                    S.PropertiesToLoad.Add("givenName");
                    S.PropertiesToLoad.Add("samaccountname");
                    SearchResultCollection searchResultCollection = S.FindAll();
                    Console.WriteLine("FOUND: " + searchResultCollection.Count.ToString() + "\n");
                    foreach (SearchResult user in searchResultCollection)
                    {
                        foreach (string property in user.Properties.PropertyNames)
                        {
                            if (property.ToLower() == "samaccountname")
                            {
                                foreach (Object value in user.Properties[property])
                                { R += value.ToString() + "   "; }
                            }
                        }
                        R += "\n";
                    }
                }
            }
            Console.WriteLine(R);
        }
    }
}
