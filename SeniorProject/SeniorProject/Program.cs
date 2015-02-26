using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;


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

            //opens a connection to the SQL db
           using(SqlConnection conn = new SqlConnection())
           {
                conn.ConnectionString="Server=ADA\\INFO210;Database=DiscoFish;User=sa;Password=changethislater";
                conn.Open();
                //SqlCommand command = new SqlCommand("INSERT INTO Album VALUES(068,1,Rubber Soul,DATEFROMPARTS(1965,12,03) )",conn);
                
               //test SQL command
               SqlCommand command = new SqlCommand("SELECT * FROM Album;", conn);
                
               //new reader to pull data from connected SQL db
               using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // write the data on to the screen
                        Console.WriteLine("\n" + "ArtistID" + "\t" + "AlbumID" + "\t\t" + "Album" + "\t\t" + "Release Date");
                        Console.WriteLine(reader[0].ToString() + "\t\t" + reader[1].ToString() + "\t\t" + reader[2].ToString() + reader[3].ToString());
                    }
                }
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
                List<FileInfo> musicFiles = FindSongs(filePath);
                Console.WriteLine("Complete, found " + musicFiles.Count + " different files");

                //then checks for any other subdirs, to continue on the work
                if (ContainsDir(filePath)) Iterate(filePath, layer + 1);
                //SongProperties(musicFiles);
            }
            
        }
        //method to find all of the files in a dir that meet the file extension requirements, and return them in a list
        static List<FileInfo> FindSongs(string filePath)
        {
            DirectoryInfo dirIn = new DirectoryInfo(filePath);
            //first list that just takes all the files in the dir
            List<FileInfo> files = dirIn.GetFiles().ToList();
            //Second list to hold the files we want to return
            List<FileInfo> musicFiles = new List<FileInfo>();
            
            foreach (FileInfo thisFile in files)
            {
                //current list of accepted file types, might add more later
                if (thisFile.Extension == ".flac" || thisFile.Extension == ".mp3" || thisFile.Extension == ".m4a" || thisFile.Extension == ".wav" || thisFile.Extension == ".wma")
                {
                    musicFiles.Add(thisFile);
                }
            }
            //returns the list with only the files that have the right extensions
            return musicFiles;
        }
        //method to take fileInfo and glean relevant song information from it, then compile that information into a new Song object, and compile those songs in a list
        static List<Song> SongProperties(List<FileInfo> songFileInfo)
        {
            List<Song> songList = new List<Song>();
            if (songFileInfo.Count != 0)
            {
                foreach (FileInfo fileInfo in songFileInfo)
                {
                    //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                    TagLib.File tf = TagLib.File.Create(fileInfo.FullName);
                    //Creates a new Song object with this information
                    //NOTE: Currently having problems if Genres[] only has the one, removed Subgenre param for the time being
                    Song s = new Song(1, 1, tf.Tag.Title, tf.Tag.Track, tf.Length, tf.Tag.FirstGenre, false, false, false);
                    Console.WriteLine("\tAdded song " + tf.Tag.Title);
                    songList.Add(s);
                }
            }
            return songList;
        }
        static void AddSongToDB(List<Song> songList, SqlConnection conn)
        {
            foreach(Song song in songList)
            {
                SqlCommand command = new SqlCommand("INSERT INTO Songs;", conn);
            }
        }
    } 
}
