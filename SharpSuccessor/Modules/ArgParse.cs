using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpSuccessor.Modules
{
    internal class ArgParse
    {
        public static void Help()
        {
            string help = "";
            Console.WriteLine(help);
        }

        static List<string> sMods = new List<string>
        {
            "add",
            "clear",

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
