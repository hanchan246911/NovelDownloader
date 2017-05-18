using NovelDownloader.Lib.Util;
using NovelDownloader.Properties;
using System;
using System.Windows.Forms;

namespace NovelDownloader
{
    public partial class frmNdlNew : Form
    {
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
            var html = HtmlMng.getNovel(ncode);

            if (String.IsNullOrWhiteSpace(html))
            {
                MessageBox.Show(String.Format(Resources.Msg_NcodeNotFound, ncode),
                    Resources.Msg_Information,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.txtTitle.Text = HtmlMng.getNovelTitle(html);
            this.txtWritername.Text = HtmlMng.getNovelWriterName(html);
            this.txtSummary.Text = HtmlMng.getNovelSummary(html);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtNcode.Text = "";
            this.txtTitle.Text = "";
            this.txtWritername.Text = "";
            this.txtSummary.Text = "";
        }
    }
}
