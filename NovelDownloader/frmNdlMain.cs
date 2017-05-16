using System;
using System.Windows.Forms;
using NovelDownloader.Properties;

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
                this.dataGridView1.Rows.Add(novellist.Id,
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
            if (e.ColumnIndex == this.dataGridView1.Columns["detail"].Index)
            {
                using (var ndlDetail = new frmNdlDetail())
                {
                    var novelid = int.Parse(this.dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString());
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

    }
}
