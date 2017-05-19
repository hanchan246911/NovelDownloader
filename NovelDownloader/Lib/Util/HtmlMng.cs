using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NovelDownloader.Lib.Util
{
    class HtmlMng
    {
#if !DEBUG
        private static string NOVEL_URL = "http://ncode.syosetu.com/";

        private static string DIV = "/";
#endif

        public static string getNovel(string ncode)
        {
            return getNovel(ncode, null);
        }

        public static string getNovel(string ncode, string page)
        {
#if DEBUG
            return testFileLoad(ncode, page);
#else
            var url = NOVEL_URL + (page == null ? ncode : ncode + DIV + page) + DIV;
            return getHTML(url);
#endif
        }

        static string getHTML(string url)
        {
            WebRequest wr = WebRequest.Create(url);
            wr.Timeout = 10000;
            HttpWebRequest pcHttpRec = (HttpWebRequest)wr;
            pcHttpRec.Proxy = null;

            Encoding enc = Encoding.GetEncoding("UTF-8");
            string result = "";
            using (Stream st = wr.GetResponse().GetResponseStream())
            using (StreamReader sr = new StreamReader(st, enc))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }

        private static string testFileLoad(string ncode, string page)
        {
            var result = "";
            var projectDirPath = createProjectDirPath();

            var filePath = "";
            if (page == null)
                filePath = Path.Combine(projectDirPath, "tmp", ncode, ncode + ".txt");
            else
            {
                filePath = Path.Combine(projectDirPath, "tmp", ncode, ncode + "-" + page + ".txt");
            }

            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return result;
        }
 
        public static string getNovelTitle(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode
                        .SelectNodes(@"//p[@class=""novel_title""]")
                        .Select(a => a.InnerText.Trim())
                        .SingleOrDefault();
        }

        public static string getNovelSummary(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            var ret = htmlDoc.DocumentNode
                        .SelectNodes(@"//div[@id=""novel_ex""]")
                        .Select(a => a.InnerText.Trim())
                        .SingleOrDefault();
            if (!System.String.IsNullOrWhiteSpace(ret))
            {
                ret = ret.Replace("\n", "\r\n");
            }
            return ret;
        }

        public static string getNovelWriterName(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode
                        .SelectNodes(@"//div[@class=""novel_writername""]")
                        .Select(a => a.InnerText.Trim())
                        .SingleOrDefault().Substring(3);
        }

        public static HtmlAgilityPack.HtmlNodeCollection getNovelSubtitleList(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc.DocumentNode
                    .SelectNodes(@"//div[@class=""index_box""]/dl[@class=""novel_sublist2""]");
        }

        public static Subtitle getNovelSubtitle(string ncode, string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            var dd = htmlDoc.DocumentNode.SelectNodes(@"//dd[@class=""subtitle""]/a")
                    .Select(x => new
                    {
                        Url = x.Attributes["href"].Value.Trim(),
                        Title = x.InnerText.Trim(),
                    }).ToList().FirstOrDefault();
            var ret = new Subtitle();
            ret.Url = dd.Url;
            ret.Title = dd.Title;
            ret.Upddate = htmlDoc.DocumentNode.SelectNodes(@"//dt[@class=""long_update""]")
                    .Select(x => x.InnerText.Trim())
                    .ToList().FirstOrDefault().Substring(0, 16);
            var pageSplit = ret.Url.Split('/');
            ret.Id = int.Parse(pageSplit[2]);
            ret.Html = HtmlMng.getNovel(ncode, pageSplit[2]);
            return ret;
        }
 
        private static string createProjectDirPath()
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
