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
                Console.WriteLine(d.ToString());

                List<DirectoryInfo> subSubs = ContainsDir(d.FullName);


                if(subSubs.Count != 0)
                {
                    foreach(DirectoryInfo sd in subSubs)
                    {
                        Console.WriteLine("   -" + sd.ToString());
                    }
                }
                
            }
            /*
            //Takes the filepath, gets relevant information about the directory
            DirectoryInfo di = new DirectoryInfo(filepath);
            //Makes an array of the files in the array
            DirectoryInfo[] da = di.GetDirectories();

            //Checks the directories to see if they contain directories
            foreach (DirectoryInfo d in da)
            {
                Console.WriteLine(d.ToString());
                DirectoryInfo newDi = new DirectoryInfo(d.FullName);
                DirectoryInfo[] newDa = newDi.GetDirectories();

                if (newDa.Length != 0)
                {
                    foreach (DirectoryInfo newD in newDa)
                    {
                        Console.WriteLine("   - " + newD.ToString());
                        //Searcher(newD.ToString());
                    }


                }
            } **/

        }

        //Method to check for directories in a directory, and return subdirectories
        //allows for easier recursion, without needing to loop whole Searcher method
        static List<DirectoryInfo> ContainsDir(string dir)
        {
            //List of subdirectories
            List<DirectoryInfo> littleDir = new List<DirectoryInfo>();
            
            DirectoryInfo di = new DirectoryInfo(dir);
            try
            {
                if(di.GetDirectories().Length != 0)
                {
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo d in diArr)
                    {
                        littleDir.Add(d);
                    }
                }
                /*DirectoryInfo[] diArr = di.GetDirectories();
                //checks to see if the direc contains any direcs, otherwise returns a simple message
                if (diArr.Length == 0)
                {
                    Console.WriteLine("This directory contains no subdirectories");
                }
                else
                {
                    //if it does contain subdirecs, it returns them in a list
                    
                }**/

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
