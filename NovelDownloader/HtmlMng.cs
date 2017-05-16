using System.IO;
using System.Net;
using System.Text;

namespace NovelDownloader
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
            // return testFileLoad(ncode, page);
            var url = NOVEL_URL + (page == null ? ncode : ncode + DIV + page) + DIV;
            return getHTML(url);
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
