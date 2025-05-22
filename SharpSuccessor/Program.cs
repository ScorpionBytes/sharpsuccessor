using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Cryptography;

namespace SharpSuccessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ASCII\n");
            try
            {
                Modules.ArgParse.Execute(args);

            }
            catch (Exception e)
            {
                Console.WriteLine("[!] Exception: " + e.Message);
            }
        }
    }
}
