using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

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
            }

        }
    }
}
