using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;


namespace SeniorProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //Filepath, eventually given by user, that will be checked for music
            string filePath = "D:/Music";
            
            //runs method to check for subdirecs
            Boolean hasDir = ContainsDir(filePath);
            Console.WriteLine(hasDir);

            //if it has a subdirectory, checks those subdirectories
            if(hasDir)
            {
                Iterate(filePath,0);
            }
        }

        //method to check directories for subdirectories, and returns a true or false
        static Boolean ContainsDir(string filePath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(filePath);
                //if GetDirectories returns anything other than 0 (no subdirecs), sends out true
                if (dir.GetDirectories().Length > 0) return true;
            }
            catch(DirectoryNotFoundException dnfe)
            {
                Console.WriteLine("Directory not found", dnfe.Message);    
            }
            //otherwise returns false
            return false;
        }
        //method to take a filepath, and int representing which layer of subdir its in, and find, display (and eventually copy) information about all subdirecs in it
        static void Iterate(string filePath, int layer)
        {

            DirectoryInfo dir = new DirectoryInfo(filePath);
            List<DirectoryInfo> dirList = dir.GetDirectories().ToList();

            //iterates through each iten in the list
            foreach (DirectoryInfo currentDir in dirList)
            {
                //changes the filepath to the newest directory
                filePath = currentDir.FullName;
                //if the directory is a subdir (or subsubdir), writes a tab for each "sub"
                for (int i = 0; i < layer; i++)
                {
                    Console.Write("    ");
                }
                //then writes the name, and if it contains any more subdirs
                Console.WriteLine(currentDir.Name + ": " + ContainsDir(filePath));

                //TESTING FindSongs
                FindSongs(filePath, layer);

                //then checks for any other subdirs, to continue on the work
                if (ContainsDir(filePath)) Iterate(filePath, layer + 1);
            }
        }
        static void FindSongs(string filePath, int layer)
        {
            DirectoryInfo dirIn = new DirectoryInfo(filePath);
            List<FileInfo> fileIn = dirIn.GetFiles().ToList();
            
            foreach (FileInfo fileInIt in fileIn)
            {
                for (int i = 0; i <= layer; i++)
                {
                    Console.Write("    ");
                    Console.WriteLine(fileInIt.Name);
                }
            }
        }
    } 
}
