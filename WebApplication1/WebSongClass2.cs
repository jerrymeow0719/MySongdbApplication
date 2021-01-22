using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1
{
    class WebSongClass2A
    {
        public IList<WebSongClass2A_1> DataList { get; set; }
        public IList<WebSongClass2_2> Page { get; set; }
        public string Ok { get; set; }
        public string Msg { get; set; }
    }

    class WebSongClass2B
    {
        public IList<WebSongClass2B_1> DataList { get; set; }
        public IList<WebSongClass2_2> Page { get; set; }
        public string Ok { get; set; }
        public string Msg { get; set; }
        public string sdate { get; set; }
        public string edate { get; set; }
    }

    class WebSongClass2A_1
    {
        public string lanype { get; set; }
        public string id { get; set; }
        public string songname { get; set; }
        public string singer { get; set; }
        public string iconurl { get; set; }
        public string songurl { get; set; }
    }

    class WebSongClass2B_1
    {
        public string thisweek { get; set; }
        public string lastweek { get; set; }
        public string weeks { get; set; }
        public string id { get; set; }
        public string songname { get; set; }
        public string singer { get; set; }
        public string iconurl { get; set; }
        public string songurl { get; set; }
    }

    class WebSongClass2_2
    {
        public int MaxPage { get; set; }
        public int ThisPage { get; set; }
        public int PreviousPage { get; set; }
        public int NextPage { get; set; }
    }
}
