using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;

//TODO switch writelining to ContainsDirec, that way the recursive bits are done in the method call

namespace SeniorProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string fp = "D:/Music";
            
            Boolean hasDir = ContainsDir(fp);
            Console.WriteLine(hasDir);

            if(hasDir == true)
            {
                Iterate(fp);
            }

            
        }

        static Boolean ContainsDir(string fp)
        {
            Boolean hasDir = false;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(fp);
                int layers = dir.GetDirectories().Length;
                if(layers != 0)
                {
                    hasDir = true;
                }

            }
            catch(DirectoryNotFoundException dnfe)
            {
                Console.WriteLine("Directory not found", dnfe.Message);    
            }
            return hasDir;
        }

        static void Iterate(string fp)
        {
            DirectoryInfo di = new DirectoryInfo(fp);
            List<DirectoryInfo> dil = di.GetDirectories().ToList();

            foreach (DirectoryInfo dilIt in dil)
            {
                fp = dilIt.FullName;
                Console.WriteLine(dilIt.Name + ": " + ContainsDir(fp));

                if(ContainsDir(fp))
                {
                    Iterate(fp);
                }
            }


        }
    } 
}
