using NovelDownloader.Lib.Util;
using NovelDownloader.Properties;
using System;
using System.Windows.Forms;

namespace NovelDownloader
{
    public partial class frmNdlNew : Form
    {

        public string Html { get; set; }

        public frmNdlNew()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.txtNcode.Text))
            {
                MessageBox.Show(String.Format(Resources.Msg_FieldInput, "ncode"),
                    Resources.Msg_Information,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ncode = this.txtNcode.Text.Trim();
            this.Html = HtmlMng.getNovel(ncode);

            if (String.IsNullOrWhiteSpace(Html))
            {
                MessageBox.Show(String.Format(Resources.Msg_NcodeNotFound, ncode),
                    Resources.Msg_Information,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.txtTitle.Text = HtmlMng.getNovelTitle(Html);
            this.txtWritername.Text = HtmlMng.getNovelWriterName(Html);
            this.txtSummary.Text = HtmlMng.getNovelSummary(Html);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var ncode = this.txtNcode.Text.Trim();
            if (String.IsNullOrWhiteSpace(Html))
            {
                MessageBox.Show(String.Format(Resources.Msg_NcodeNotFound, ncode),
                    Resources.Msg_Information,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DbMng.IsNcodeExists(ncode))
            {
                MessageBox.Show(String.Format(Resources.Msg_AlreadyBeenRegistered, ncode),
                    Resources.Msg_Information,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var cnn = DbMng.getConnection())
            {
                cnn.Open();
                cnn.ChangePassword("");
                using (var trans = cnn.BeginTransaction())
                {
                    using (var context = DbMng.getConttext(cnn))
                    {
                        var novellist = new Novellist();
                        novellist.Ncode = ncode;
                        novellist.Title = this.txtTitle.Text;
                        novellist.Writername = this.txtWritername.Text;
                        novellist.Html = this.Html;
                        var novelid = DbMng.addNovelList(novellist, context);
                        var subtitles = HtmlMng.getNovelSubtitleList(this.Html);
                        foreach (var a in subtitles)
                        {
                            var subtitle = HtmlMng.getNovelSubtitle(ncode, a.InnerHtml);
                            DbMng.addSubtitle(subtitle, context);
                        }
                    }
                    trans.Commit();
                }
                cnn.Close();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtNcode.Text = "";
            this.txtTitle.Text = "";
            this.txtWritername.Text = "";
            this.txtSummary.Text = "";
            this.Html = "";
        }
    }
}
