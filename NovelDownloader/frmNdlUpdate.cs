using NovelDownloader.Lib.Util;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NovelDownloader
{
    public partial class frmNdlUpdate : Form
    {
        public string Ncode { set; get; }

        public frmNdlUpdate()
        {
            InitializeComponent();
        }

        private void frmNdlUpdate_Load(object sender, System.EventArgs e)
        {
            var lsubtitles = DbMng.getSubTitleByNovelId(this.Ncode);
            var osubtitles = new List<Subtitle>();
            var html = HtmlMng.getNovel(this.Ncode);
            var subtitles = HtmlMng.getNovelSubtitleList(html);
            foreach (var a in subtitles)
            {
                var subtitle = HtmlMng.getNovelSubtitle(this.Ncode, a.InnerHtml);
                osubtitles.Add(subtitle);
            }

            foreach (var lsubtitle in lsubtitles)
            {
                var upddate = osubtitles.Find(x => x.Url == lsubtitle.Url);

                var updflg = true;
                if (lsubtitle.Upddate.Equals(upddate.Upddate))
                    updflg = false;

                this.dgvSubtitles.Rows.Add(updflg,
                    lsubtitle.Id,
                    lsubtitle.Capter,
                    lsubtitle.Title,
                    lsubtitle.Upddate,
                    upddate.Upddate);
            }

            foreach (var osubtitle in osubtitles)
            {
                var notlocal = lsubtitles.Find(x => x.Url.Contains(osubtitle.Url));
                if (notlocal != null)
                    continue;

                this.dgvSubtitles.Rows.Add(true,
                    0,
                    osubtitle.Capter,
                    osubtitle.Title,
                    "",
                    osubtitle.Upddate);
            }
        }
    }
}
