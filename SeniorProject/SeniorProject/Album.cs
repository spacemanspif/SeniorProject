using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorProject
{
    class Album
    {
        public int AlbumID;
        public String Title;
        public uint ReleaseDate;

        public Album(string name, uint date)
        {
            // TODO: Complete member initialization
            this.Title=name;
            this.ReleaseDate = date;
        }
    }
}
