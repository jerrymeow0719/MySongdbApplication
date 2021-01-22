using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;
using WebApplication1;
using funcs;

namespace WebApplication1
{
    class MainClass
    {
        static void Main(string[] args)
        {
            object[] Newsong = { 1, null };
            funcs.funcs.WebSong1(Newsong);
            System.Threading.Thread.Sleep(5000);
            funcs.funcs.WebSong2(Newsong);

            System.Threading.Thread.Sleep(5000);

            object[] Songrank = { 2, null };
            funcs.funcs.WebSong1(Songrank);
            System.Threading.Thread.Sleep(5000);
            funcs.funcs.WebSong2(Songrank);


            funcs.funcs.CheckSongDateActive(1);
            System.Threading.Thread.Sleep(5000);
            funcs.funcs.CheckSongDateActive(2);

            funcs.funcs.WebSongUrl();
            System.Threading.Thread.Sleep(5000);
            funcs.funcs.updateWebSongUrl();
        }
    }
}
