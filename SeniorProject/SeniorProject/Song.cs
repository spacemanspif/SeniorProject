using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorProject
{
    class Song
    {
        public int AlbumID;
        public int ArtistID;
        public String Title;
        public uint TrackNumber;
        public TimeSpan TrackLength;
        public String Genre;
        public String SubGenre;
        public Boolean AudioBook;
        public Boolean Instrumental;
        public Boolean Soundtrack;

        public Song(int alid, int arid, string t, uint tnum, TimeSpan len, string gen, bool ab, bool ins, bool st)
        {
            // TODO: Complete member initialization
            this.AlbumID = alid;
            this.ArtistID = arid;
            this.Title = t;
            this.TrackNumber = tnum;
            this.TrackLength = len;
            this.Genre = gen;
            //this.SubGenre = sgen;
            this.AudioBook = ab;
            this.Instrumental = ins;
            this.Soundtrack = st;
        }

        public Song(string t)
        {
            this.Title = t;
        }
    }
}
