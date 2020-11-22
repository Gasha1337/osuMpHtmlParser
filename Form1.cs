using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osuHtmlMpParser
{
    public partial class Form1 : Form
    {
        static string htmlString;

        static List<string> playerStrings = new List<string>();
        static List<string> addedNicknames = new List<string>();
        //static string stringAfterNickname = @"https://osu.ppy.sh/rankings/osu/performance";
        static string stringBeforeNickname1 = @"https://osu.ppy.sh/users";

        static List<string> playerStringsTest;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();

            if (ofd.FileName != "") using (StreamReader sr = new StreamReader(ofd.FileName)) htmlString = sr.ReadToEnd();
            else return;

            playerStrings.Clear();


            string mapSplitPattern = "<div class=\"mp-history-content__item\"><div class=\"mp-history-game\">";
            List<string> mapStrings = Regex.Split(htmlString, mapSplitPattern).ToList<string>();

            if (mapStrings.Count == 1)
            {
                mapSplitPattern = "mp-history-events__game";
                mapStrings = Regex.Split(htmlString, mapSplitPattern).ToList<string>();
            }

            string lastMapEndPattern = "</span></div></div></div></div></div></div></div></div>";
            mapStrings[mapStrings.Count - 1] = Regex.Split(mapStrings[mapStrings.Count - 1], lastMapEndPattern)[0];
            mapStrings.RemoveAt(0);


            string playerSplitPattern = "mp-history-player-score__main";
            foreach (var map in mapStrings)
            {
                playerStrings.AddRange(Regex.Split(map, playerSplitPattern));
            }

            //string dogshit = "\"><div class=\"mp-history-game\"><a class=\"mp-history-game__header";
            string dogshit = "mp-history-game__header";
            playerStringsTest = playerStrings.ToList();
            for (int i = 0; i < playerStrings.Count; i++)
            {
                if (playerStrings[i].Contains(dogshit)) playerStrings[i] = "0";
            }

            
            var playerStrings1 = playerStrings.Where(s => s != "0").ToList<string>();
            addedNicknames.Clear();

            foreach (var player in playerStrings1)
            {
                int indexNickname = player.IndexOf(stringBeforeNickname1);
                string nickname = player.Substring(indexNickname + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];

                if (addedNicknames.Contains(nickname)) continue;
                else
                {
                    richTextBox2.Text += nickname + " ";
                    foreach (var player1 in playerStrings1)
                    {
                        //int indexNickname1 = player1.IndexOf(stringAfterNickname);
                        //string s_nickname = player1.Substring(indexNickname1 - stringAfterNickname.Length - 20, 50).Split('<')[0].Split('>')[1];
                        int indexNickname1 = player1.IndexOf(stringBeforeNickname1);
                        string s_nickname = player1.Substring(indexNickname1 + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];

                        if (s_nickname == nickname)
                        {
                            string stringBeforeScore = "mp-history-player-score__stat-number mp-history-player-score__stat-number--large";
                            int indexScore = player1.IndexOf(stringBeforeScore);
                            string score = player1.Substring(indexScore + stringBeforeScore.Length, 100).Split('>')[1].Split('<')[0];
                            if (score.Contains("&nbsp;"))
                            {
                                var index = score.IndexOf("&nbsp;");
                                score = score.Remove(index, 6);
                            }
                            score = score.Replace(",", string.Empty);

                            richTextBox2.Text += score + " ";
                        }
                    }
                    richTextBox2.Text += "\n";
                    addedNicknames.Add(nickname);
                }
                
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> mapsPlayed = new List<string>();
            richTextBox2.Clear();
            int indexMap = 0;
            
            foreach (var player in playerStringsTest)
            {
                if (player.Contains("class=\"mp-history-game__header\" href=\""))
                {
                    string stringBeforeMap = "class=\"mp-history-game__header\" href=\"";
                    int indexMapString = player.IndexOf(stringBeforeMap);
                    string mapLink = player.Substring(indexMapString + stringBeforeMap.Length, 40).Split('"')[0];

                    mapsPlayed.Add(mapLink);
                    int countDuplicates = 0;
                    foreach (string item in mapsPlayed)
                    {
                        if (item == mapLink) countDuplicates++;
                    }


                    if (indexMap != 0) richTextBox2.Text += "\n";
                    indexMap++;
                    if (countDuplicates > 1) richTextBox2.Text += "MAP № " + indexMap +"  "+mapLink+" attempt №"+countDuplicates+"\n";
                    else richTextBox2.Text += "MAP № " + indexMap + "  " + mapLink + "\n";

                }
                else
                {
                    string stringBeforeScore = "mp-history-player-score__stat-number mp-history-player-score__stat-number--large";
                    int indexScore = player.IndexOf(stringBeforeScore);
                    int indexNickname = player.IndexOf(stringBeforeNickname1);
                    string nickname = player.Substring(indexNickname + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];
                    string score = player.Substring(indexScore + stringBeforeScore.Length, 100).Split('>')[1].Split('<')[0];
                    score = score.Replace(",", string.Empty);
                    if (score.Contains("&nbsp;"))
                    {
                        var index = score.IndexOf("&nbsp;");
                        score = score.Remove(index, 6);
                    }
                    
                    richTextBox2.Text += nickname + ": " + score + "\n";
                }
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            addedNicknames.Clear();
            var playerStrings1 = playerStrings.Where(s => s != "0").ToList<string>();

            foreach (var player in playerStrings1)
            {
                int indexNickname = player.IndexOf(stringBeforeNickname1);
                string nickname = player.Substring(indexNickname + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];

                if (addedNicknames.Contains(nickname)) continue;
                else
                {
                    richTextBox2.Text += nickname + "\n";
                    foreach (var player1 in playerStrings1)
                    {
                        int indexNickname1 = player1.IndexOf(stringBeforeNickname1);
                        string s_nickname = player1.Substring(indexNickname1 + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];

                        if (s_nickname == nickname)
                        {
                            string stringBeforeScore = "mp-history-player-score__stat-number mp-history-player-score__stat-number--large";
                            int indexScore = player1.IndexOf(stringBeforeScore);
                            string score = player1.Substring(indexScore + stringBeforeScore.Length, 100).Split('>')[1].Split('<')[0];
                            score = score.Replace(",", string.Empty);
                            if (score.Contains("&nbsp;"))
                            {
                                var index = score.IndexOf("&nbsp;");
                                score = score.Remove(index, 6);
                            }
                            

                            richTextBox2.Text += score +"\n";
                        }
                    }
                    richTextBox2.Text += "\n";
                    addedNicknames.Add(nickname);
                }

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            addedNicknames.Clear();
            var playerStrings1 = playerStrings.Where(s => s != "0").ToList<string>();

            foreach (var player in playerStrings1)
            {
                int indexNickname = player.IndexOf(stringBeforeNickname1);
                string nickname = player.Substring(indexNickname + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];

                if (addedNicknames.Contains(nickname)) continue;
                else
                {
                    richTextBox2.Text += nickname + " ";
                    foreach (var player1 in playerStrings1)
                    {
                        int indexNickname1 = player1.IndexOf(stringBeforeNickname1);
                        string s_nickname = player1.Substring(indexNickname1 + stringBeforeNickname1.Length, 50).Split('<')[0].Split('>')[1];

                        if (s_nickname == nickname)
                        {
                            string stringBeforeScore = "mp-history-player-score__stat-number mp-history-player-score__stat-number--large";
                            int indexScore = player1.IndexOf(stringBeforeScore);
                            string score = player1.Substring(indexScore + stringBeforeScore.Length, 100).Split('>')[1].Split('<')[0];
                            score = score.Replace(",", string.Empty);
                            if (score.Contains("&nbsp;"))
                            {
                                var index = score.IndexOf("&nbsp;");
                                score = score.Remove(index, 6);
                            }

                            richTextBox2.Text += score + " ";
                        }
                    }
                    richTextBox2.Text += "\n";
                    addedNicknames.Add(nickname);
                }

            }

        }

        private void richTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        // class="mp-history-player-score__username" href
        // </a></div><a href


        // "mp-history-game" начало карты
        //  </div></div></div></div></div></div></div></div> конец каждой карты


        // "mp-history-player-score__main" начало плеера
        // background-image: url(&quot;/images/layout/mp-history/shapes-team-none.svg&quot;);"></div> конец плеера 
    }
}
