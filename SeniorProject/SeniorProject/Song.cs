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
        public String Artist;
        public int TrackNumber;
        public int TrackLength;
        public String Genre;
        public String SubGenre;
        public Boolean AudioBook;
        public Boolean Instrumental;
        public Boolean Soundtrack;
        private int p1;
        private int p2;
        private string p3;
        private uint p4;
        private long p5;
        private string p6;
        private string p7;
        private bool p8;
        private bool p9;
        private bool p10;

        public Song(int AlbumID, int ArtistID, string Title, uint TrackNum, long Length, string Genre, string SubGenre, bool Audiobook, bool Instrumental, bool Soundtrack)
        {
            // TODO: Complete member initialization
            this.p1 = AlbumID;
            this.p2 = ArtistID;
            this.p3 = Title;
            this.p4 = TrackNum;
            this.p5 = Length;
            this.p6 = Genre;
            this.p7 = SubGenre;
            this.p8 = AudioBook;
            this.p9 = Instrumental;
            this.p10 = Soundtrack;
        }
    }
}
