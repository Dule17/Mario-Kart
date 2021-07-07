using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace GameServer
{
    class SQL_database
    {
        public SQLiteConnection conn;


        public SQL_database()
        {
            string[] s = Directory.GetCurrentDirectory().ToString().Split('\\');
            string path = "";
            for(int i=0;i<s.Length-3;i++)path+=s[i]+"\\";
            string connectionString = "Data Source="+path+ "Users.db;Version=3;";
            conn = new SQLiteConnection(connectionString);
        }

        public void ShowALL()
        {
            conn.Open();


            using (SQLiteCommand cmd = new SQLiteCommand("select * from Players", conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["username"].ToString());
                        }
                    }
                } 

            }
            conn.Close();
        }

        public int Login(string username,string password)
        {
            conn.Open();

            int database_Index = -1;
            using (SQLiteCommand cmd = new SQLiteCommand("select Id,username,password from Players", conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                           if(reader["username"].ToString()==username&&reader["password"].ToString()==password)
                            {
                                database_Index = int.Parse(reader["Id"].ToString());
                                break;
                            }
                        }
                    }
                }

            }
            conn.Close();
            return database_Index;
        }


        public void UPDATE(int id,int win,int loses,float fastestLap,float fastestGame)
        {
            conn.Open();

            float newLapTime, newGameTime;
            int newWins, newLoses;
            newGameTime = 0;
            newLapTime = 0;
            newWins = 0;
            newLoses = 0;
            using (SQLiteCommand cmd = new SQLiteCommand("select * from Players", conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            if (int.Parse(reader["Id"].ToString()) == id)
                            {

                                if (win == 0) newWins = int.Parse(reader["wins"].ToString());
                                else newWins = int.Parse(reader["wins"].ToString())+1;

                                if (loses == 0) newLoses = int.Parse(reader["loses"].ToString());
                                else newLoses = int.Parse(reader["loses"].ToString()) + 1;
                                if (reader["fastest_lap"].ToString()!=""&& float.Parse(reader["fastest_lap"].ToString())<fastestLap) newLapTime = float.Parse(reader["fastest_lap"].ToString());
                                else newLapTime = fastestLap;

                                if (reader["fastest_Game"].ToString() != ""&&float.Parse(reader["fastest_Game"].ToString()) < fastestGame) newGameTime = float.Parse(reader["fastest_Game"].ToString());
                                else newGameTime = fastestGame;


                                break;
                            }
                        }
                    }
                }
                SQLiteCommand cmd2 = new SQLiteCommand();
                cmd2.Connection = conn;
                cmd2.CommandText = "update Players set wins=@win, loses=@loses, fastest_lap=@lap, fastest_Game=@game where Id=@id";
                cmd2.Parameters.AddWithValue("@win", newWins);
                cmd2.Parameters.AddWithValue("@loses", newLoses);
                cmd2.Parameters.AddWithValue("@lap", newLapTime);
                cmd2.Parameters.AddWithValue("@game", newGameTime);
                cmd2.Parameters.AddWithValue("@id",id);
                cmd2.ExecuteNonQuery();
            }
            conn.Close();
        }
        string Get_fastest_Lap()
        {
            string s = "Fastest Lap:";
            using (SQLiteCommand cmd = new SQLiteCommand("select username,min(fastest_lap)as fastest_lap from Players", conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            // Console.WriteLine("fastest_lap:" + reader["username"].ToString() + ":" + reader["fastest_lap"].ToString());
                            int min =(int) float.Parse(reader["fastest_lap"].ToString()) / 60;
                            float sec= float.Parse(reader["fastest_lap"].ToString()) % 60;
                            s += reader["username"].ToString() + " " + min+":"+sec.ToString("00.00");
                        }
                    }
                }

            }
            return s;
        }

        string Get_fastet_Game()
        {
            string s = "Fastest Game:";
            using (SQLiteCommand cmd = new SQLiteCommand("select username,min(fastest_Game)as fastest_Game from Players", conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            //  Console.WriteLine("Fastest game:" + reader["username"].ToString() + ":" + reader["fastest_Game"].ToString());
                            int min = (int)float.Parse(reader["fastest_Game"].ToString()) / 60;
                            float sec = float.Parse(reader["fastest_Game"].ToString()) % 60;
                            s += reader["username"].ToString() + " " + min + ":" + sec.ToString("00.00");
                        }
                    }
                }

            }
            return s;
        }

        string Get_Wins_Loses()
        {
            string s = "Win/Lose ratio:";
            using (SQLiteCommand cmd = new SQLiteCommand("select username,wins,loses from Players where wins=(select max(wins/(loses+1)) from Players)", conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            // Console.WriteLine("Max wins:" + reader["username"].ToString() + ":" + reader["wins"].ToString());
                            float ratio = float.Parse(reader["wins"].ToString()) / (1 + float.Parse(reader["loses"].ToString()));
                           s+= reader["username"].ToString() + " " + reader["wins"].ToString();
                        }
                    }
                }

            }
            return s;
        }
        public string RANKING()
        {
            conn.Open();
            string s="";
            s+=Get_Wins_Loses()+"\n";
            s+=Get_fastet_Game() + "\n";
            s+=Get_fastest_Lap() + "\n";
            conn.Close();

            return s;
        }

        public void INSERT(string userame,string password)
        {
            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into Players(username,password,wins,loses) values(@name,@pass,@wins,@loses)";
                cmd.Parameters.AddWithValue("@name", userame);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@wins", 0);
                cmd.Parameters.AddWithValue("@loses", 0);
                cmd.ExecuteNonQuery();
                conn.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
