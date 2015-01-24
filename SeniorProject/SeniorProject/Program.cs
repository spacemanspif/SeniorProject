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
            Searcher("D:/Music");
        }

        //Method to take user inputted filepath and look for music files in it
        static void Searcher(string filepath)
        {
            //testing
            Console.WriteLine(filepath + "\n");

            List<DirectoryInfo> subDirecs = ContainsDir(filepath);
            foreach(DirectoryInfo d in subDirecs)
            {
                int layer = 0;
                List<DirectoryInfo> subSubs = ContainsDir(d.FullName);
                Console.WriteLine(d.ToString() + ": " + subSubs.Count);
                
            }

        }

        //Method to check for directories in a directory, and return subdirectories
        //allows for easier recursion, without needing to loop whole Searcher method
        static List<DirectoryInfo> ContainsDir(string dir)
        {
            //List of subdirectories
            List<DirectoryInfo> littleDir = new List<DirectoryInfo>();
            int layer = 0;
            
            DirectoryInfo di = new DirectoryInfo(dir);
            List<DirectoryInfo> dirlist = 
            try
            {
                if(di.GetDirectories().Length != 0)
                {
                    layer++;
                    foreach(DirectoryInfo d in di)
                    {

                    }
                    /*DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo d in diArr)
                    {
                        littleDir.Add(d);
                        
                    }**/
                }
                else
                {
                    Console.WriteLine(layer);
                }

                return littleDir;
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
               Debug.WriteLine(dirNotFound.Message);
            }
            return littleDir;
        }
    }
}
