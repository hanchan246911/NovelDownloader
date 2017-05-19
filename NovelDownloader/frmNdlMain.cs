using System;
using System.Windows.Forms;
using NovelDownloader.Properties;
using NovelDownloader.Lib.Util;
using System.IO;
using System.Text;

namespace NovelDownloader
{
    public partial class frmNdlMain : Form
    {
        public frmNdlMain()
        {
            InitializeComponent();

            var novellists = DbMng.getNovelListAll();
            foreach (var novellist in novellists)
            {
                this.dgvNovelList.Rows.Add(novellist.Id,
                    novellist.Ncode,
                    novellist.Title,
                    novellist.Writername,
                    Resources.Msg_Detail);
            }
        }

        private void tsmiFileRun_Click(object sender, EventArgs e)
        {
            //                    var ncode = "n5084bv";
            var ncode = "n5655dt";

            NdlMng.getNovelInfo(ncode);

            MessageBox.Show(Resources.Msg_Finish,
                Resources.Msg_Information,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == this.dgvNovelList.Columns["detail"].Index)
            {
                using (var ndlDetail = new frmNdlDetail())
                {
                    var novelid = int.Parse(this.dgvNovelList.Rows[e.RowIndex].Cells["id"].Value.ToString());
                    ndlDetail.Novelid = novelid;
                    ndlDetail.ShowDialog();
                }
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //            var ncode = "n5084bv";
            var ncode = "n5655dt";

            NdlMng.outputNovelText(ncode);

            MessageBox.Show(Resources.Msg_Finish,
                Resources.Msg_Information,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            using (var ndlnew = new frmNdlNew())
            {
                ndlnew.ShowDialog();
                if (ndlnew.DialogResult == DialogResult.Cancel)
                    MessageBox.Show("Cancel", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void dummyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ncode = "n5084bv";
            var novellist = DbMng.getNovelList(ncode);

            var filepath = Path.Combine(NdlMng.createProjectDirPath(), "tmp", ncode, ncode + ".txt");
            using (var sw = new StreamWriter(filepath, false, Encoding.GetEncoding("utf-8")))
            {
                sw.Write(novellist.Html);
            }

            var subtitles = DbMng.getSubTitleByNovelId(ncode);
            foreach (var subtitle in subtitles)
            {
                var subpath = Path.Combine(NdlMng.createProjectDirPath(), "tmp", ncode, ncode + "-" + subtitle.Id + ".txt");
                using (var sw = new StreamWriter(subpath, false, Encoding.GetEncoding("utf-8")))
                {
                    sw.Write(subtitle.Html);
                }
            }

        }
    }
}
