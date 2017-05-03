using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace NovelDownloader
{
    public partial class NdlDetail : Form
    {
        private int novelid;
        public int Novelid
        {
            get { return novelid; }
            set { novelid = value; }
        }

        public NdlDetail()
        {
            InitializeComponent();
        }
 
        private void NdlDetail_Load(object sender, EventArgs e)
        {
            using (var cn = new SQLiteConnection(NdlDbMng.getConnectionString()))
            {
                cn.Open();
                var subtitles = NdlDbMng.getSubTitleByNovelId(this.novelid, cn);
                foreach (var subtitle in subtitles)
                {
                    this.dataGridView1.Rows.Add(
                        false,
                        subtitle.Id,
                        subtitle.Title,
                        subtitle.Upddate,
                        subtitle.Regdate);
                }
            }
        }
    }
}
