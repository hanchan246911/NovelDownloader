using System;
using System.Linq;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace NovelDownloader
{
    class NdlDbMng
    {
        static private string getDBPath()
        {
            var projectDirPath = createProjectDirPath();
            return Path.Combine(projectDirPath, "db", "ndl.db");
        }
 
        static public string getConnectionString()
        {
            var pathToDbFile = NdlDbMng.getDBPath();

            var cns = new SQLiteConnectionStringBuilder
            {
                DataSource = pathToDbFile,
                //                Password = "0uJpcSjUxY3OHSeztGri4RUP1OIqw15D",
            };
            return cns.ToString();
        }

        static public List<Novellist> getNovelListAll(SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var novellists = aConText.GetTable<Novellist>();
                var aQueryResult =
                        (from a in novellists
                         select a).ToList();
                return aQueryResult;
            }
        }
 
        static public List<Subtitle> getSubTitleByNovelId(int novelid, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var subtitles = aConText.GetTable<Subtitle>();
                var aQueryResult =
                        (from a in subtitles
                         where a.Novelid == novelid
                         select a).ToList();
                return aQueryResult;
            }
        }
        static public int addNovelList(string ncode, string html, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                var title = htmlDoc.DocumentNode
                        .SelectNodes(@"//p[@class=""novel_title""]")
                        .Select(a => a.InnerText.Trim())
                        .ToList().FirstOrDefault();

                var writername = htmlDoc.DocumentNode
                        .SelectNodes(@"//div[@class=""novel_writername""]")
                        .Select(a => a.InnerText.Trim())
                        .ToList().FirstOrDefault().Substring(3);

                var novellists = aConText.GetTable<Novellist>();
                var rec = new Novellist
                {
                    Ncode = ncode,
                    Html = html,
                    Title = title,
                    Writername = writername,
                    Regdate = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                };
                novellists.InsertOnSubmit(rec);
                aConText.SubmitChanges();

                return getNovelId(ncode, aConText);
            }
        }

        static public void updateNovelList(int id, string html, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                var title = htmlDoc.DocumentNode
                        .SelectNodes(@"//p[@class=""novel_title""]")
                        .Select(a => a.InnerText.Trim())
                        .ToList().FirstOrDefault();

                var writername = htmlDoc.DocumentNode
                        .SelectNodes(@"//div[@class=""novel_writername""]")
                        .Select(a => a.InnerText.Trim())
                        .ToList().FirstOrDefault().Substring(3);

                aConText.ExecuteCommand(
                    @"UPDATE Novellists SET html = {0}, title = {1}, writername = {2}, regdate = {3} WHERE id = {4}",
                    html,
                    title,
                    writername,
                    DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                    id);
            }
        }

        static public int getNovelId(string ncode, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                return getNovelId(ncode, aConText);
            }
        }

        static public int getNovelId(string ncode, DataContext aConText)
        {
            var novellists = aConText.GetTable<Novellist>();
            var aQueryResult =
                    (from a in novellists
                     where a.Ncode == ncode
                     select a).ToList().FirstOrDefault();
            return aQueryResult == null ? -1 : (int)aQueryResult.Id;
        }

        static public void deleteSubtitleByNovelId(int novelid, SQLiteConnection aConnection)
        {
            using (DataContext aConText = new DataContext(aConnection))
            {
                Table<Subtitle> subtitles = aConText.GetTable<Subtitle>();
                IQueryable<Subtitle> aDelTargets =
                        from a in subtitles
                        where a.Novelid == novelid
                        select a;
                subtitles.DeleteAllOnSubmit(aDelTargets);
                aConText.SubmitChanges();
            }
        }

        static public void addSubtitle(int id, int novelid, string subtitle, string url, string html, string upddate, SQLiteConnection aConnection)
        {
            using (DataContext aConText = new DataContext(aConnection))
            {
                Table<Subtitle> subtitles = aConText.GetTable<Subtitle>();
                var rec = new Subtitle
                {
                    Id = id,
                    Novelid = novelid,
                    Title = subtitle,
                    Url = url,
                    Html = html,
                    Upddate = upddate,
                    Regdate = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                };
                subtitles.InsertOnSubmit(rec);
                aConText.SubmitChanges();

            }
        }

        static private string createProjectDirPath()
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
