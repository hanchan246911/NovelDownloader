using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using NovelDownloader.Properties;
using System.IO;
using System.Text;

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

            using (var sw = new StreamWriter(filepath, true, Encoding.GetEncoding("shift_jis")))
            {
                using (var aConnection = NdlDbMng.getConnection())
                {
                    var novelid = NdlDbMng.getNovelId(ncode, aConnection);
                    var subtitles = NdlDbMng.getSubTitleByNovelId(novelid, aConnection);

                    foreach (var subtitle in subtitles)
                    {
                        if (!subtitle.Output)
                            continue;

                        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(subtitle.Html);

                        sw.Write(subtitle.Title);

                        var nodenovelp = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_p""]");
                        var novelp = nodenovelp == null ? null : nodenovelp.InnerText;

                        var nodenovelh = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_honbun""]");
                        var novelh = nodenovelh == null ? null : nodenovelh.InnerText;
                        if (novelh != null)
                        {
                            var novelsplit = novelh.Split('\n');

                            for (var i = 0; i < novelsplit.Length; i++)
                            {
                                sw.Write(novelsplit[i]);
                            }
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
