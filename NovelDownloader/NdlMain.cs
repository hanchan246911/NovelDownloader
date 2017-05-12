using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using NovelDownloader.Properties;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NovelDownloader
{
    public partial class NdlMain : Form
    {
        public NdlMain()
        {
            InitializeComponent();

            using (var cn = new SQLiteConnection(NdlDbMng.getConnectionString()))
            {
                cn.Open();
                var novellists = NdlDbMng.getNovelListAll(cn);
                foreach (var novellist in novellists)
                {
                    this.dataGridView1.Rows.Add(novellist.Id,
                        novellist.Ncode,
                        novellist.Title,
                        novellist.Writername,
                        Resources.Msg_Detail);
                }
            }
        }

        private void tsmiFileRun_Click(object sender, EventArgs e)
        {
            using (var cn = new SQLiteConnection(NdlDbMng.getConnectionString()))
            {
                cn.Open();
                cn.ChangePassword("");
                using (var trans = cn.BeginTransaction())
                {
                    var ncode = "n5084bv";
                    var novelid = NdlDbMng.getNovelId(ncode, cn);
                    var html = NdlHtmlMng.getNovel(ncode);

                    if (novelid < 0)
                        novelid = NdlDbMng.addNovelList(ncode, html, cn);
                    else
                        NdlDbMng.updateNovelList(novelid, html, cn);

                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    NdlDbMng.deleteSubtitleByNovelId(novelid, cn);

                    var articles = htmlDoc.DocumentNode
                            .SelectNodes(@"//div[@class=""index_box""]/dl[@class=""novel_sublist2""]");
                    var i = 0;
                    foreach (var a in articles)
                    {
                        i++;

                        var htmldl = new HtmlAgilityPack.HtmlDocument();
                        htmldl.LoadHtml(a.InnerHtml);

                        var dd = htmldl.DocumentNode.SelectNodes(@"//dd[@class=""subtitle""]/a")
                                .Select(x => new
                                {
                                    Url = x.Attributes["href"].Value.Trim(),
                                    Title = x.InnerText.Trim(),
                                }).ToList().FirstOrDefault();
                        var url = dd.Url;
                        var title = dd.Title;
                        var upddate = htmldl.DocumentNode.SelectNodes(@"//dt[@class=""long_update""]")
                                .Select(x => x.InnerText.Trim())
                                .ToList().FirstOrDefault().Substring(0, 16);
                        var pageSplit = url.Split('/');
                        var subhtml = NdlHtmlMng.getNovel(ncode, pageSplit[2]);
                        NdlDbMng.addSubtitle(i, novelid, dd.Title, dd.Url, subhtml, upddate, cn);
                    }
                    trans.Commit();
                }
                cn.Close();
            }

            MessageBox.Show(Resources.Msg_Finish,
                Resources.Msg_Information,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == this.dataGridView1.Columns["detail"].Index)
            {
                using (var ndlDetail = new NdlDetail())
                {
                    var novelid = int.Parse(this.dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString());
                    ndlDetail.Novelid = novelid;
                    ndlDetail.ShowDialog();
                }
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ncode = "n5084bv";

            var filename = ncode + ".txt";
            var dirpath = Path.Combine(createProjectDirPath(), "tmp");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            var filepath = Path.Combine(dirpath, filename);
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

//            using (var sw = new StreamWriter(filepath, true, Encoding.GetEncoding("shift_jis")))
            using (var sw = new StreamWriter(filepath, true, Encoding.GetEncoding("utf-8")))
            {
                using (var aConnection = NdlDbMng.getConnection())
                {
                    var novelists = NdlDbMng.getNovelList(ncode, aConnection);
                    var subtitles = NdlDbMng.getSubTitleByNovelId((int)novelists.Id, aConnection);
                    var novelsettings = NdlDbMng.getNovelSetting((int)novelists.Id, aConnection);
                    var replacesettings = NdlDbMng.getReplaceSetting((int)novelists.Id, aConnection);
                    var repflg = novelsettings.Replacestring;

                    var title = repflg ? repNStr(novelists.Title, replacesettings) : novelists.Title;
                    sw.WriteLine(title);
                    var writername = repflg ? repNStr(novelists.Writername, replacesettings) : novelists.Writername;
                    sw.WriteLine(writername);
                    sw.WriteLine("");

                    foreach (var subtitle in subtitles)
                    {
                        if (!subtitle.Output)
                            continue;

                        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(subtitle.Html);

                        var repsubtitle = repflg ? repNStr(subtitle.Title, replacesettings) : subtitle.Title;
                        if (novelsettings.Subtitleheading)
                            sw.WriteLine("［＃５字下げ］" + repsubtitle + "［＃「" + repsubtitle + "」は中見出し］");
                        else
                            sw.WriteLine(repsubtitle);
                        sw.WriteLine("");

                        var nodenovelp = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_p""]");
                        var novelp = nodenovelp == null ? null : nodenovelp.InnerText;

                        var nodenovelh = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_honbun""]");
                        var novelh = nodenovelh == null ? null : nodenovelh.InnerText;
                        if (novelh != null)
                        {
                            novelh = repflg ? repNStr(novelh, replacesettings) : novelh;
                            var novelsplit = novelh.Split('\n');

                            for (var i = 0; i < novelsplit.Length; i++)
                            {

                                var s = "";
                                var n = "";
                                var u = getNovelSplitAddLine(novelsplit, i, -1);
                                if (novelsettings.Multiblanklinedel == 0)
                                {
                                    s = getNovelSplitAddLine(novelsplit, i, 0);
                                    n = getNovelSplitAddLine(novelsplit, i, 1);
                                } else
                                {
                                    var mlflg = true;
                                    var recs = new string[novelsettings.Multiblanklinedel + 1];
                                    for (var j = 0; j < recs.Length; j++)
                                    {
                                        recs[j] = getNovelSplitAddLine(novelsplit, i, j);

                                        if (!String.IsNullOrWhiteSpace(recs[j]))
                                            mlflg = false;
                                    }
                                    if (mlflg) continue;
                                    s = recs[0];
                                    n = recs[1];
                                }

                                if (novelsettings.Oneblanklinedel
                                    && !String.IsNullOrWhiteSpace(u)
                                    && String.IsNullOrWhiteSpace(s)
                                    && !String.IsNullOrWhiteSpace(n))
                                    continue;

                                if (novelsettings.Indentation
                                    && !String.IsNullOrWhiteSpace(s)
                                    && !s.StartsWith("「")
                                    && !s.StartsWith("『")
                                    && !s.StartsWith("〔")
                                    && !s.StartsWith("（")
                                    && !s.StartsWith("【"))
                                    s = "　" + s;

                                sw.WriteLine(s);
                            }
                            sw.WriteLine("");
                        }

                        var nodenovela = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_a""]");
                        var novela = nodenovela == null ? null : nodenovela.InnerText;
                    }
                }
            }

            MessageBox.Show(Resources.Msg_Finish,
                Resources.Msg_Information,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string getNovelSplitAddLine(string[] novelsplit, int nowline, int addline)
        {
            var result = "";
            if ((addline > 0 && nowline < novelsplit.Length - addline)
               || (addline < 0 && nowline + addline >= 0)
               || (addline == 0))
            {
                result = novelsplit[nowline + addline];
                if (!String.IsNullOrEmpty(result))
                    result = result.Trim();
            }
            return result;
        }
 
        private string repNStr(string pre, List<Replacesetting> replasesetting)
        {
            if (String.IsNullOrEmpty(pre))
                return pre;

            var result = pre;
            foreach (var rep in replasesetting)
            {
                result = result.Replace(rep.Replacefrom, rep.Replaceto);
            }
            return result;
        }
        private string createProjectDirPath()
        {
            var projectDirPath = "";
            if (File.Exists("ProjectDirPath.txt"))
            {
                using (var sr = new StreamReader("ProjectDirPath.txt"))
                {
                    projectDirPath = sr.ReadToEnd();
                }
            }
            return projectDirPath.Replace("\r", "").Replace("\n", "");
        }
    }
}
