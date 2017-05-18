using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NovelDownloader.Lib.Util
{
    class HtmlMng
    {
        private static string NOVEL_URL = "http://ncode.syosetu.com/";

        private static string DIV = "/";

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
                var pageSplit = page.Split('/');
                filePath = Path.Combine(projectDirPath, "tmp", ncode, ncode + "-" + pageSplit[2] + ".txt");
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
