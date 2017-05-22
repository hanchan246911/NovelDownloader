using System;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using System.Data.Linq;
using System.Data;
using NovelDownloader.Properties;
using NovelDownloader.Lib.Util;

namespace NovelDownloader
{
    public partial class frmNdlDetail : Form
    {
        private int novelid;
        public int Novelid
        {
            get { return novelid; }
            set { novelid = value; }
        }

        public frmNdlDetail()
        {
            InitializeComponent();
        }
 
        private void NdlDetail_Load(object sender, EventArgs e)
        {
            using (var cn = new SQLiteConnection(DbMng.getConnectionString()))
            {
                cn.Open();
                var subtitles = DbMng.getSubTitleByNovelId(this.novelid, cn);
                foreach (var subtitle in subtitles)
                {
                    this.dataGridView1.Rows.Add(
                        subtitle.Output,
                        subtitle.Id,
                        subtitle.Title,
                        subtitle.Upddate,
                        subtitle.Regdate);
                }
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            var dg = dataGridView1.Rows.Cast<DataGridViewRow>();
            dg.AsEnumerable().Select(r => r.Cells["chkselect"].Value = true).ToList();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            var dg = dataGridView1.Rows.Cast<DataGridViewRow>();
            dg.AsEnumerable().Select(r => r.Cells["chkselect"].Value = false).ToList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var cn = DbMng.getConnection())
            {
                using (var cnt = DbMng.getContext(cn))
                {
                    try
                    {
                        cn.Open();
                        cnt.Transaction = cn.BeginTransaction();

                        var dgl = dataGridView1.Rows.Cast<DataGridViewRow>();
                        foreach (var dgr in dgl)
                        {
                            Table<Subtitle> subtitles = cnt.GetTable<Subtitle>();
                            var rec = (from a in subtitles
                                       where a.Novelid == this.novelid && a.Id == int.Parse(dgr.Cells["id"].Value.ToString())
                                       select a).SingleOrDefault();

                            rec.Output = (bool)dgr.Cells["chkselect"].Value;
                        }
                        cnt.SubmitChanges();
                        cnt.Transaction.Commit();

                        MessageBox.Show(Resources.Msg_Finish,
                            Resources.Msg_Information,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        cnt.Transaction.Rollback();
                    } finally
                    {
                        if (cn.State == ConnectionState.Open)
                            cn.Close();
                    }
                }
            }
        }
    }
}
