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

            //if it has a subdirectory, checks those subdirectories
            if (hasDir)
            {
                List<FileInfo> musicFiles = new List<FileInfo>();
                musicFiles = Iterate(filePath, 0, musicFiles);
                List<Song> songList = SongProperties(musicFiles);

                //opens a connection to the SQL db
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=ADA\\INFO210;Database=DiscoFish;User=sa;password=changethislater";
                    conn.Open();

                    AddSongToDB(songList, conn);
                    AddArtistsToDB(musicFiles, conn);
                    AddAlbumsToDB(musicFiles, conn);
                    AlbumArtists(musicFiles, conn);
                    AlbumSongs(musicFiles, conn);
                    SongsArtists(musicFiles, conn);
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
            catch (DirectoryNotFoundException dnfe)
            {
                Console.WriteLine("Directory not found", dnfe.Message);
            }
            //otherwise returns false
            return false;
        }
        //method to take a filepath, and int representing which layer of subdir its in, and find, display (and eventually copy) information about all subdirecs in it
        static List<FileInfo> Iterate(string filePath, int layer, List<FileInfo> musicFiles)
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
                    Console.Write("\t");
                }
                //then writes the name, and if it contains any more subdirs
                Console.WriteLine(currentDir.Name + ": " + ContainsDir(filePath));

                //adds the found files to a temp holder, which is then dumped into the overall music list
                List<FileInfo> thisIteration = FindSongs(filePath);
                foreach (FileInfo thisSong in thisIteration)
                {
                    musicFiles.Add(thisSong);
                }

                //then checks for any other subdirs, to continue on the work
                if (ContainsDir(filePath)) Iterate(filePath, layer + 1, musicFiles);
            }

            return musicFiles;

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
                    try
                    {
                        //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                        TagLib.File tf = TagLib.File.Create(fileInfo.FullName);
                        //Creates a new Song object with this information

                        bool score = false;
                        string subgen = "";
                        if (tf.Tag.Album.Contains("Soundtrack") || tf.Tag.Album.Contains("OST") || tf.Tag.Album.Contains("soundtrack") || tf.Tag.Album.Contains("score") || tf.Tag.Album.Contains("Score") || tf.Tag.Album.Contains("movie"))
                        {
                            score = true;
                        }
                        if(tf.Tag.Genres.Length >= 2)
                        {
                            subgen = tf.Tag.Genres[1].ToString();
                        }
                        Song s = new Song(tf.Tag.Title, tf.Tag.Track, tf.Properties.Duration, tf.Tag.FirstGenre, subgen, false, false, score);
                        Console.WriteLine("\tAdded song " + tf.Tag.Title);
                        songList.Add(s);
                    }
                    catch (TagLib.CorruptFileException corrupt)
                    {
                        Console.WriteLine("\t" + corrupt.Message);
                    }
                    catch(NullReferenceException nre)
                    {
                        Console.WriteLine("\t" + nre.Message);
                    }
                }
            }
            return songList;
        }
        //method to add the new Song objects to the SQL database
        static void AddSongToDB(List<Song> songList, SqlConnection conn)
        {
            foreach (Song song in songList)
            {
                try
                {
                    String cleanTitle = (song.Title).Replace("'", "");

                    //checks to see if song already exists in db
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO Songs(Title,Length,TrackNumber,Genre,SubGenre,IsScore) SELECT '" + 
                        cleanTitle + "','" + song.TrackLength + "'," + song.TrackNumber + ", '"+song.Genre + "', '" + song.SubGenre + "', '" + song.Soundtrack +
                        "' WHERE NOT EXISTS (SELECT * FROM Songs WHERE [Title] = '" + cleanTitle + "');", conn);
                    using (SqlDataReader reader = insertCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("Added " + song.Title + " to the db");
                        }
                    }
                }
                catch(SqlException sql)
                {
                    Console.WriteLine(sql.Message);
                }
                catch(NullReferenceException nre)
                {
                    Console.WriteLine(nre.Message);
                }
            }
        }
        static void AddArtistsToDB(List<FileInfo> songFileList, SqlConnection conn)
        {
            if (songFileList.Count != 0)
            {
                foreach (FileInfo fileInfo in songFileList)
                {
                    try
                    {
                        //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                        TagLib.File tf = TagLib.File.Create(fileInfo.FullName);

                        Artists a = new Artists(tf.Tag.FirstAlbumArtist);
                        String cleanName = (a.Name).Replace("'", "");
                        SqlCommand updateCommand = new SqlCommand("INSERT INTO Artists(Name) SELECT('" + cleanName + 
                                                                    "')WHERE NOT EXISTS (SELECT * FROM Artists WHERE Name = '" + cleanName + "');",conn);
                        using (SqlDataReader reader = updateCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Added " + a.Name + " to the db");
                            }
                        }
                        Console.WriteLine("\tAdded artist " + tf.Tag.FirstAlbumArtist);
                    }
                    catch (TagLib.CorruptFileException corrupt)
                    {
                        Console.WriteLine("\t" + corrupt.Message);
                    }
                    catch (SqlException sql)
                    {
                        Console.WriteLine("\t" + sql.Message);
                    }
                    catch(NullReferenceException nre)
                    {
                        Console.WriteLine("\t" + nre.Message);
                    }
                }
            }
        }
        static void AddAlbumsToDB(List<FileInfo> songFileList, SqlConnection conn)
        {
            if (songFileList.Count != 0)
            {
                foreach (FileInfo fileInfo in songFileList)
                {
                    try
                    {
                        //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                        TagLib.File tf = TagLib.File.Create(fileInfo.FullName);

                        Album a = new Album(tf.Tag.Album,tf.Tag.Year);
                        String cleanTitle = (a.Title).Replace("'", "");
                        SqlCommand updateCommand = new SqlCommand("INSERT INTO Albums(Name, ReleaseYear) SELECT'" + cleanTitle + "', " + tf.Tag.Year +
                                                                    " WHERE NOT EXISTS (SELECT * FROM Albums WHERE Name = '" + cleanTitle + "');", conn);
                        using (SqlDataReader reader = updateCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Added " + a.Title + " to the db");
                            }
                        }
                        Console.WriteLine("\tAdded album " + tf.Tag.Album);
                    }
                    catch (TagLib.CorruptFileException corrupt)
                    {
                        Console.WriteLine("\t" + corrupt.Message);
                    }
                    catch (SqlException sql)
                    {
                        Console.WriteLine("\t" + sql.Message);
                    }
                    catch (NullReferenceException nre)
                    {
                        Console.WriteLine("\t" + nre.Message);
                    }
                }
            }
        }
        static void AlbumArtists(List<FileInfo> songFileList, SqlConnection conn)
        {
            if (songFileList.Count != 0)
            {
                foreach (FileInfo fileInfo in songFileList)
                {
                    try
                    {
                        //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                        TagLib.File tf = TagLib.File.Create(fileInfo.FullName);

                        Album al = new Album(tf.Tag.Album, tf.Tag.Year);
                        Artists ar = new Artists(tf.Tag.FirstAlbumArtist);
                        String cleanAlbum = tf.Tag.Album.Replace("'", "");
                        String cleanArtist = tf.Tag.FirstAlbumArtist.Replace("'", "");

                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = "INSERT INTO [Albums-Artists](Albums.AlbumID,Artists.ArtistID) " +
                            "VALUES((SELECT [Albums].[AlbumID] FROM Albums WHERE Name='" + cleanAlbum + "'), " +
                            "(SELECT Artists.[ArtistID] FROM Artists WHERE Name='" + cleanArtist + "'))";
                        
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Updated table for "+cleanArtist+"'s album "+cleanAlbum);
                            }
                        }
                        Console.WriteLine("\tAdded album " + tf.Tag.Album);
                    }
                    catch (TagLib.CorruptFileException corrupt)
                    {
                        Console.WriteLine("\t" + corrupt.Message);
                    }
                    catch (SqlException sql)
                    {
                        Console.WriteLine("\t" + sql.Message);
                    }
                    catch (NullReferenceException nre)
                    {
                        Console.WriteLine("\t" + nre.Message);
                    }
                }
            }
        }
        static void AlbumSongs(List<FileInfo> songFileList, SqlConnection conn)
        {
            if (songFileList.Count != 0)
            {
                foreach (FileInfo fileInfo in songFileList)
                {
                    try
                    {
                        //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                        TagLib.File tf = TagLib.File.Create(fileInfo.FullName);

                        Album al = new Album(tf.Tag.Album, tf.Tag.Year);
                        Song s = new Song(tf.Tag.Title);
                        String cleanAlbum = tf.Tag.Album.Replace("'", "");
                        String cleanSong = tf.Tag.Title.Replace("'", "");

                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = "INSERT INTO [Albums-Songs](Albums.[AlbumID],Songs.[SongID]) " +
                            "VALUES((SELECT [Albums].[AlbumID] FROM Albums WHERE Name='" + cleanAlbum + "'), " +
                            "(SELECT Songs.[SongID] FROM Songs WHERE Title='" + cleanSong + "'))";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Updated table for " + cleanAlbum + "'s song " + cleanSong);
                            }
                        }
                        Console.WriteLine("\tAdded album " + tf.Tag.Album);
                    }
                    catch (TagLib.CorruptFileException corrupt)
                    {
                        Console.WriteLine("\t" + corrupt.Message);
                    }
                    catch (SqlException sql)
                    {
                        Console.WriteLine("\t" + sql.Message);
                    }
                    catch (NullReferenceException nre)
                    {
                        Console.WriteLine("\t" + nre.Message);
                    }
                }
            }
        }
        static void SongsArtists(List<FileInfo> songFileList, SqlConnection conn)
        {
            if (songFileList.Count != 0)
            {
                foreach (FileInfo fileInfo in songFileList)
                {
                    try
                    {
                        //utilizes imported Taglib-sharp to know where relevant information is, based on filetype
                        TagLib.File tf = TagLib.File.Create(fileInfo.FullName);

                        Song s = new Song(tf.Tag.Title);
                        Artists ar = new Artists(tf.Tag.FirstAlbumArtist);
                        String cleanSong = tf.Tag.Title.Replace("'", "");
                        String cleanArtists = tf.Tag.FirstAlbumArtist.Replace("'", "");

                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = "INSERT INTO [Artists-Songs](Songs.[SongID],Artists.[ArtistID]) " +
                            "VALUES((SELECT [Songs].[SongID] FROM Songs WHERE Title='" + cleanSong + "'), " +
                            "(SELECT Artists.[ArtistID] FROM Artists WHERE Name='" + cleanArtists + "'))";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Updated table for " + cleanArtists + "'s song " + cleanSong);
                            }
                        }
                        Console.WriteLine("\tUpdated table for " + cleanArtists + "'s song " + cleanSong);
                    }
                    catch (TagLib.CorruptFileException corrupt)
                    {
                        Console.WriteLine("\t" + corrupt.Message);
                    }
                    catch (SqlException sql)
                    {
                        Console.WriteLine("\t" + sql.Message);
                    }
                    catch (NullReferenceException nre)
                    {
                        Console.WriteLine("\t" + nre.Message);
                    }
                }
            }
        }
    }
}
