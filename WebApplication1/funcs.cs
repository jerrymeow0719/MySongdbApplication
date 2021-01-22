using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WebApplication1;

namespace funcs
{
    public class funcs
    {
        private static string connectionString = "server=127.0.0.1;port=3306;database=songdb;user=root;password=hir19920719";
        // First parsing in 2021/01/08
        public static async void WebSong1(params object[] paramList)
        {
            int songtype = 0;
            int dateTimeType = 0;
            DateTime dateTime = DateTime.Now;
            if (paramList[0] != null)
            {
                songtype = (int)paramList[0];
            }
            if (paramList[1] != null)
            {
                dateTime = (DateTime)paramList[1];
                dateTimeType = 1;
            }

            var client = new RestClient("https://www.cashboxparty.com/ashx/ah_Song.ashx");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Referer", "https://www.cashboxparty.com/Music/KTVMusic.aspx");
            request.AddHeader("Content-Type", "text/plain");

            if (songtype == 1)
            {
                if (dateTimeType == 0)
                {
                    request.AddParameter("text/plain", "[{\"m\":\"sn\",\"lang\":\"01,02\",\"date\":\"\"}]", ParameterType.RequestBody);
                }
                else if (dateTimeType == 1)
                {
                    string dateTimeRequest = "[{\"m\":\"sn\",\"lang\":\"01,02\",\"date\":\"" + dateTime.ToString("d") + "\"}]";
                    request.AddParameter("text/plain", dateTimeRequest, ParameterType.RequestBody);
                }
            }
            else 
            {
                request.AddParameter("text/plain", "[{\"m\":\"bl\",\"t\":\"1\",\"lang\":\"01,02\"}]", ParameterType.RequestBody);
            }
            IRestResponse response = client.Execute(request);

            if (songtype == 1)
            {
                List<WebSongClass1A> webSongClass1 = JsonConvert.DeserializeObject<List<WebSongClass1A>>(response.Content);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    foreach (WebSongClass1A data in webSongClass1)
                    {
                        if (string.Compare(data.LangCode, "01") != 0)
                            continue;

                        int Getid = -1;
                        using (MySqlCommand cmdselect = conn.CreateCommand())
                        {
                            cmdselect.CommandText = @"SELECT id FROM NewSong WHERE songname = @songname AND singer = @singer;";
                            cmdselect.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songname",
                                DbType = System.Data.DbType.String,
                                Value = data.SongName,
                            });
                            cmdselect.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@singer",
                                DbType = System.Data.DbType.String,
                                Value = data.Singer,
                            });

                            object result = cmdselect.ExecuteScalar();
                            if (result != null)
                            {
                                Getid = int.Parse(result.ToString());
                            }
                        }

                        if (Getid == -1) //Insert
                        {

                            using (MySqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"INSERT INTO NewSong (nowactive, indate, songname, singer, songurl, webclass1, webclass2, webclass3) VALUES (@nowactive, @indate, @songname, @singer, @songurl, @webclass1, @webclass2, @webclass3);";

                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@nowactive",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 1,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@indate",
                                    DbType = System.Data.DbType.String,
                                    Value = DateTime.Now.ToString("yyyy/MM/dd"),
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@songname",
                                    DbType = System.Data.DbType.String,
                                    Value = data.SongName,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@singer",
                                    DbType = System.Data.DbType.String,
                                    Value = data.Singer,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@songurl",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 0,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass1",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 1,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass2",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 0,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass3",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 0,
                                });
                                await cmd.ExecuteNonQueryAsync();
                                System.Threading.Thread.Sleep(200);
                            }
                        }
                        else //Update
                        {
                            using (MySqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"UPDATE NewSong SET webclass1 = @webclass1 WHERE id = @id;";
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass1",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 1,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@id",
                                    DbType = System.Data.DbType.Int32,
                                    Value = Getid,
                                });
                                await cmd.ExecuteNonQueryAsync();
                                System.Threading.Thread.Sleep(200);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            else
            {
                List<WebSongClass1B> webSongClass1 = JsonConvert.DeserializeObject<List<WebSongClass1B>>(response.Content);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    foreach (WebSongClass1B data in webSongClass1)
                    {
                        if (string.Compare(data.LangCode, "01") != 0)
                            continue;

                        if (Convert.ToInt32(data.ThisWeekend) > 20)
                            break;

                        using (MySqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO WeekRank (nowactive, indate, songname, singer, songurl, store, songrank) VALUES (@nowactive, @indate, @songname, @singer, @songurl, @store, @songrank);";

                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@nowactive",
                                DbType = System.Data.DbType.Int32,
                                Value = 1,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@indate",
                                DbType = System.Data.DbType.String,
                                Value = DateTime.Now.ToString("yyyy/MM/dd"),
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songname",
                                DbType = System.Data.DbType.String,
                                Value = data.SongName,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@singer",
                                DbType = System.Data.DbType.String,
                                Value = data.Singer,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songurl",
                                DbType = System.Data.DbType.Int32,
                                Value = 0,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@store",
                                DbType = System.Data.DbType.Int32,
                                Value = 1,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songrank",
                                DbType = System.Data.DbType.Int32,
                                Value = data.ThisWeekend,
                            });
                            await cmd.ExecuteNonQueryAsync();
                            System.Threading.Thread.Sleep(200);
                        }
                    }
                    conn.Close();
                }
            }
        }

        public static async void WebSong2(params object[] paramList)
        {
            int songtype = 0;
            if (paramList[0] != null)
            {
                songtype = (int)paramList[0];
            }

            var client = new RestClient("https://www.holiday.com.tw/Ashx/SongInfo.ashx");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "text/plain");
            if (songtype == 1)
            {
                request.AddParameter("text/plain", "[{\"m\":\"new\",\"ltype\":\"c\",\"page\":1}]", ParameterType.RequestBody);
            }
            else 
            {
                request.AddParameter("text/plain", "[{\"m\":\"top\",\"ltype\":\"nc\",\"page\":1}]", ParameterType.RequestBody);
            }
            IRestResponse response = client.Execute(request);
            
            string JsonText = response.Content.ToString().Remove(0,1);
            JsonText = JsonText.Remove(JsonText.Length - 1);

            if (songtype == 1)
            {
                WebSongClass2A webSongClass2 = JsonConvert.DeserializeObject<WebSongClass2A>(JsonText);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    foreach (WebSongClass2A_1 data in webSongClass2.DataList)
                    {
                        int Getid = -1;
                        using (MySqlCommand cmdselect = conn.CreateCommand())
                        {
                            cmdselect.CommandText = @"SELECT id FROM NewSong WHERE songname = @songname AND singer = @singer;";
                            cmdselect.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songname",
                                DbType = System.Data.DbType.String,
                                Value = toChinese(data.songname),
                            });
                            cmdselect.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@singer",
                                DbType = System.Data.DbType.String,
                                Value = toChinese(data.singer),
                            });

                            object result = cmdselect.ExecuteScalar();
                            if (result != null)
                            {
                                Getid = int.Parse(result.ToString());
                            }
                        }

                        if (Getid == -1) //Insert
                        {
                            using (MySqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"INSERT INTO NewSong (nowactive, indate, songname, singer, songurl, webclass1, webclass2, webclass3) VALUES (@nowactive, @indate, @songname, @singer, @songurl, @webclass1, @webclass2, @webclass3);";

                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@nowactive",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 1,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@indate",
                                    DbType = System.Data.DbType.String,
                                    Value = DateTime.Now.ToString("yyyy/MM/dd"),
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@songname",
                                    DbType = System.Data.DbType.String,
                                    Value = toChinese(data.songname),
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@singer",
                                    DbType = System.Data.DbType.String,
                                    Value = toChinese(data.singer),
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@songurl",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 0,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass1",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 0,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass2",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 1,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass3",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 0,
                                });
                                await cmd.ExecuteNonQueryAsync();
                                System.Threading.Thread.Sleep(200);
                            }
                        }
                        else //Update
                        {
                            using (MySqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"UPDATE NewSong SET webclass2 = @webclass2 WHERE id = @id;";
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@webclass2",
                                    DbType = System.Data.DbType.Int32,
                                    Value = 1,
                                });
                                cmd.Parameters.Add(new MySqlParameter
                                {
                                    ParameterName = "@id",
                                    DbType = System.Data.DbType.Int32,
                                    Value = Getid,
                                });
                                await cmd.ExecuteNonQueryAsync();
                                System.Threading.Thread.Sleep(200);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            else 
            {
                WebSongClass2B webSongClass2 = JsonConvert.DeserializeObject<WebSongClass2B>(JsonText);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    foreach (WebSongClass2B_1 data in webSongClass2.DataList)
                    {
                        if (Convert.ToInt32(data.thisweek) > 20)
                            break;

                        using (MySqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO WeekRank (nowactive, indate, songname, singer, songurl, store, songrank) VALUES (@nowactive, @indate, @songname, @singer, @songurl, @store, @songrank);";

                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@nowactive",
                                DbType = System.Data.DbType.Int32,
                                Value = 1,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@indate",
                                DbType = System.Data.DbType.String,
                                Value = DateTime.Now.ToString("yyyy/MM/dd"),
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songname",
                                DbType = System.Data.DbType.String,
                                Value = toChinese(data.songname),
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@singer",
                                DbType = System.Data.DbType.String,
                                Value = toChinese(data.singer),
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songurl",
                                DbType = System.Data.DbType.Int32,
                                Value = 0,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@store",
                                DbType = System.Data.DbType.Int32,
                                Value = 2,
                            });
                            cmd.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@songrank",
                                DbType = System.Data.DbType.Int32,
                                Value = data.thisweek,
                            });
                            await cmd.ExecuteNonQueryAsync();
                            System.Threading.Thread.Sleep(200);
                        }
                    }
                    conn.Close();
                }
            }
        }

        public static async void CheckSongDateActive(int songtype)
        {
            if (songtype == 1) //NewSong
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    string[] GetDate = new string[2];
                    using (MySqlCommand cmddate = conn.CreateCommand())
                    {
                        cmddate.CommandText = @"SELECT * FROM UpdateDate ORDER BY id DESC LIMIT 2;";
                        using (MySqlDataReader resultDate = cmddate.ExecuteReader())
                        { 
                            for (int index = 0; index < 2; index++)
                            {
                                resultDate.Read();
                                GetDate[index] = resultDate["updatedate"].ToString();
                            }
                        }
                        //conn.Close();
                    }

                    List<int> idList = new List<int>();
                    using (MySqlCommand cmdlist = conn.CreateCommand())
                    {
                        cmdlist.CommandText = @"SELECT * FROM NewSong ORDER BY id DESC LIMIT 30;";
                        using (MySqlDataReader resultList = cmdlist.ExecuteReader())
                        {
                            while(resultList.Read())
                            {
                                if (resultList["indate"].ToString() == GetDate[1] && ((int)resultList["nowactive"] == 1))
                                {
                                    idList.Add((int)resultList["id"]);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        //conn.Close();
                    }
                    foreach (int updateid in idList)
                    {
                        using (MySqlCommand cmdUpdate = conn.CreateCommand())
                        {
                            cmdUpdate.CommandText = @"UPDATE NewSong SET indate = @indate WHERE id = @id;";
                            cmdUpdate.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@indate",
                                DbType = System.Data.DbType.String,
                                Value = GetDate[0],
                            });
                            cmdUpdate.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@id",
                                DbType = System.Data.DbType.Int32,
                                Value = updateid,
                            });
                            await cmdUpdate.ExecuteNonQueryAsync();
                            System.Threading.Thread.Sleep(200);
                            //conn.Close();
                        }
                    }

                    using (MySqlCommand cmdUpdate = conn.CreateCommand())
                    {
                        cmdUpdate.CommandText = @"UPDATE NewSong SET nowactive = @nowactive WHERE indate = @indate;";
                        cmdUpdate.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@nowactive",
                            DbType = System.Data.DbType.Int32,
                            Value = 0,
                        });
                        cmdUpdate.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@indate",
                            DbType = System.Data.DbType.String,
                            Value = GetDate[1],
                        });
                        await cmdUpdate.ExecuteNonQueryAsync();
                        System.Threading.Thread.Sleep(200);
                        //conn.Close();
                    }
                    conn.Close();
                }
            }
            else //WeekRank
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    string[] GetDate = new string[2];
                    using (MySqlCommand cmddate = conn.CreateCommand())
                    {
                        cmddate.CommandText = @"SELECT * FROM UpdateDate ORDER BY id DESC LIMIT 2;";
                        using (MySqlDataReader resultDate = cmddate.ExecuteReader())
                        {
                            for (int index = 0; index < 2; index++)
                            {
                                resultDate.Read();
                                GetDate[index] = resultDate["updatedate"].ToString();
                            }
                        }
                        //conn.Close();
                    }

                    List<int> idList = new List<int>();
                    using (MySqlCommand cmdlist = conn.CreateCommand())
                    {
                        cmdlist.CommandText = @"SELECT id FROM WeekRank WHERE indate = @indate AND nowactive = @nowactive";
                        cmdlist.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@indate",
                            DbType = System.Data.DbType.String,
                            Value = GetDate[1],
                        });
                        cmdlist.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@nowactive",
                            DbType = System.Data.DbType.Int32,
                            Value = 1,
                        });
                        using (MySqlDataReader resultList = cmdlist.ExecuteReader())
                        {
                            while (resultList.Read())
                            {
                                idList.Add((int)resultList["id"]);
                            }
                        }
                        //conn.Close();
                    }
                    foreach (int updateid in idList)
                    {
                        using (MySqlCommand cmdUpdate = conn.CreateCommand())
                        {
                            cmdUpdate.CommandText = @"UPDATE WeekRank SET nowactive = @nowactive WHERE id = @id;";
                            cmdUpdate.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@nowactive",
                                DbType = System.Data.DbType.Int32,
                                Value = 0,
                            });
                            cmdUpdate.Parameters.Add(new MySqlParameter
                            {
                                ParameterName = "@id",
                                DbType = System.Data.DbType.Int32,
                                Value = updateid,
                            });
                            await cmdUpdate.ExecuteNonQueryAsync();
                            System.Threading.Thread.Sleep(200);
                            //conn.Close();
                        }
                    }
                    conn.Close();
                }
            }
        }

        public static async void WebSongUrl()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string GetDate = "";
                using (MySqlCommand cmddate = conn.CreateCommand())
                {
                    cmddate.CommandText = @"SELECT * FROM UpdateDate ORDER BY id DESC LIMIT 1;";
                    using (MySqlDataReader resultDate = cmddate.ExecuteReader())
                    { 
                        if (resultDate.Read())
                        {
                            GetDate = resultDate["updatedate"].ToString();
                        }
                    }
                    //conn.Close();
                }

                List<string[]> GetSong = new List<string[]>();

                // Parse new song from NewSong
                //conn.Open();
                using (MySqlCommand cmdNewSong = conn.CreateCommand())
                {
                    cmdNewSong.CommandText = @"SELECT songname,singer FROM NewSong WHERE indate = @indate AND nowactive = @nowactive";
                    cmdNewSong.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@indate",
                        DbType = System.Data.DbType.String,
                        Value = GetDate,
                    });
                    cmdNewSong.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@nowactive",
                        DbType = System.Data.DbType.Int32,
                        Value = 1,
                    });
                    using (MySqlDataReader resultNewSong = cmdNewSong.ExecuteReader())
                    {
                        while (resultNewSong.Read())
                        {
                            GetSong.Add(new string[2] { resultNewSong["songname"].ToString(), resultNewSong["singer"].ToString() });
                        }
                        //conn.Close();
                    }
                }

                // Parse new song from WeekRank
                //conn.Open();
                using (MySqlCommand cmdRankSong = conn.CreateCommand())
                {
                    cmdRankSong.CommandText = @"SELECT songname,singer FROM WeekRank WHERE indate = @indate AND nowactive = @nowactive";
                    cmdRankSong.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@indate",
                        DbType = System.Data.DbType.String,
                        Value = GetDate,
                    });
                    cmdRankSong.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@nowactive",
                        DbType = System.Data.DbType.Int32,
                        Value = 1,
                    });
                    using (MySqlDataReader resultRankSong = cmdRankSong.ExecuteReader())
                    {

                        while (resultRankSong.Read())
                        {
                            GetSong.Add(new string[2] { resultRankSong["songname"].ToString(), resultRankSong["singer"].ToString() });
                        }
                        //conn.Close();
                    }
                }

                //conn.Open();
                foreach (string[] data in GetSong)
                {
                    using (MySqlCommand cmdInsert = conn.CreateCommand())
                    {
                        cmdInsert.CommandText = @"INSERT IGNORE INTO SongUrl (songname, singer, songurl) VALUES (@songname, @singer, @songurl);";
                        cmdInsert.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@songname",
                            DbType = System.Data.DbType.String,
                            Value = data[0],
                        });
                        cmdInsert.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@singer",
                            DbType = System.Data.DbType.String,
                            Value = data[1],
                        });
                        cmdInsert.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@songurl",
                            DbType = System.Data.DbType.String,
                            Value = null,
                        });
                        await cmdInsert.ExecuteNonQueryAsync();
                        System.Threading.Thread.Sleep(200);
                    }
                }
                conn.Close();
            }
        }

        public static async void updateWebSongUrl()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                // Parse NewSong
                //conn.Open();
                using (MySqlCommand cmdNewSong = conn.CreateCommand())
                {
                    cmdNewSong.CommandText = @"UPDATE NewSong INNER JOIN SongUrl ON NewSong.songname = SongUrl.songname AND NewSong.singer = SongUrl.singer
                                            SET NewSong.songurl = SongUrl.id
                                            WHERE nowactive = @nowactive;";
                    cmdNewSong.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@nowactive",
                        DbType = System.Data.DbType.Int32,
                        Value = 1,
                    });
                    await cmdNewSong.ExecuteNonQueryAsync();
                }

                // Parse WeekRank
                //conn.Open();
                using (MySqlCommand cmdWeekRank = conn.CreateCommand())
                {
                    cmdWeekRank.CommandText = @"UPDATE WeekRank INNER JOIN SongUrl ON WeekRank.songname = SongUrl.songname AND WeekRank.singer = SongUrl.singer
                                            SET WeekRank.songurl = SongUrl.id
                                            WHERE nowactive = @nowactive;";
                    cmdWeekRank.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = "@nowactive",
                        DbType = System.Data.DbType.Int32,
                        Value = 1,
                    });
                    await cmdWeekRank.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
        }

        private static string toChinese(string intxt)
        {
            return (System.Web.HttpUtility.UrlDecode(intxt));      
        }
    }
}
