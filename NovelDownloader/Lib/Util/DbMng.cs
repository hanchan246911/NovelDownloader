using System;
using System.Linq;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace NovelDownloader.Lib.Util
{
    class DbMng
    {
        static private string getDBPath()
        {
            var projectDirPath = createProjectDirPath();
            return Path.Combine(projectDirPath, "db", "ndl.db");
        }

        static public string getConnectionString()
        {
            return getConnectionString(DbMng.getDBPath());
        }

        static public string getConnectionString(string path)
        {
            var cns = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                // Password = "0uJpcSjUxY3OHSeztGri4RUP1OIqw15D",
            };
            return cns.ToString();
        }
 
        static public SQLiteConnection getConnection()
        {
            return new SQLiteConnection(getConnectionString());
        }
 
        static public DataContext getContext()
        {
            return getContext(getConnection());
        }

        static public DataContext getContext(SQLiteConnection aConnection)
        {
            return new DataContext(aConnection);
        }

        static public List<Novellist> getNovelListAll()
        {
            using (var aConnection = getConnection())
            {
                using (var aConText = new DataContext(aConnection))
                {
                    var novellists = aConText.GetTable<Novellist>();
                    var aQueryResult =
                            (from a in novellists
                             orderby a.Ncode
                             select a).ToList();
                    return aQueryResult;
                }
            }
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

        static public List<Subtitle> getSubTitleByNovelId(string ncode)
        {
            var novelid = getNovelId(ncode);
            using (var aConnection = getConnection())
            {
                return getSubTitleByNovelId(novelid, aConnection);
            }
        }

        static public List<Subtitle> getSubTitleByNovelId(int novelid)
        {
            using (var aConnection = getConnection())
            {
                return getSubTitleByNovelId(novelid, aConnection);
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
                         orderby a.Id
                         select a).ToList();
                return aQueryResult;
            }
        }

        static public Subtitle getSubTitleById(int novelid, int id)
        {
            using (var aConnection = getConnection())
            {
                return getSubTitleById(novelid, id, aConnection);
            }
        }

        static public Subtitle getSubTitleById(int novelid, int id, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var subtitles = aConText.GetTable<Subtitle>();
                var aQueryResult =
                        (from a in subtitles
                         where a.Novelid == novelid
                            && a.Id == id
                         select a).ToList().FirstOrDefault();
                return aQueryResult;
            }
        }

        static public bool IsNcodeExists(string ncode)
        {
            return getNovelId(ncode) > 0 ? true : false;
        }

        static public int addNovelList(Novellist novellst, DataContext aConText)
        {
            var novellists = aConText.GetTable<Novellist>();
            novellst.Regdate = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            novellists.InsertOnSubmit(novellst);
            aConText.SubmitChanges();

            return getNovelId(novellst.Ncode, aConText);
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
 
        static public Novellist getNovelList(string ncode)
        {
            return getNovelList(ncode, getConnection());
        }

        static public Novellist getNovelList(string ncode, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var novellists = aConText.GetTable<Novellist>();
                var rec = (from a in novellists
                           where a.Ncode == ncode
                           select a).SingleOrDefault();
                return rec;
            }
        }

        static public Novellist getNovelList(int novelid)
        {
            return getNovelList(novelid, getConnection());
        }

        static public Novellist getNovelList(int novelid, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var novellists = aConText.GetTable<Novellist>();
                var rec = (from a in novellists
                           where a.Id == novelid
                           select a).SingleOrDefault();
                return rec;
            }
        }

        static public Novelsetting getNovelSetting(int novelid)
        {
            return getNovelSetting(novelid, getConnection());
        }

        static public Novelsetting getNovelSetting(int novelid, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var novelsettings = aConText.GetTable<Novelsetting>();
                var rec = (from a in novelsettings
                           where a.Novelid == novelid
                           select a).SingleOrDefault();
                return rec;
            }
        }

        static public List<Replacesetting> getReplaceSetting(int novelid, SQLiteConnection aConnection)
        {
            using (var aConText = new DataContext(aConnection))
            {
                var replacesettings = aConText.GetTable<Replacesetting>();
                var rec = (from a in replacesettings
                           where a.Novelid == novelid
                           select a).ToList();
                return rec;
            }
        }

        static public int getNovelId(string ncode)
        {
            using (var aConnection = getConnection())
            {
                return getNovelId(ncode, aConnection);
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
                addSubtitle(id, novelid, subtitle, url, html, upddate, aConText);
            }
        }

        static public void addSubtitle(int id, int novelid, string subtitle, string url, string html, string upddate, DataContext aConText)
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

        static public void addSubtitle(Subtitle subtitle, DataContext aConText)
        {
            Table<Subtitle> subtitles = aConText.GetTable<Subtitle>();
            subtitle.Regdate = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            subtitles.InsertOnSubmit(subtitle);
            aConText.SubmitChanges();
        }

        static public void addNewNovelSetting(int novelid, DataContext aConText) {
            var novelsetting = new Novelsetting();
            novelsetting.Novelid = novelid;
            novelsetting.Subtitleheading = true;
            novelsetting.Indentation = true;
            novelsetting.Oneblanklinedel = true;
            novelsetting.Multiblanklinedel = 3;
            novelsetting.Preface = true;
            novelsetting.Trailer = true;
            novelsetting.Replacestring = false;
            addNewNovelSetting(novelsetting, aConText);
        }

        static public void addNewNovelSetting(Novelsetting novelsetting, DataContext aConText)
        {
            var novelsettings = aConText.GetTable<Novelsetting>();
            novelsetting.Regdate = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            novelsettings.InsertOnSubmit(novelsetting);
            aConText.SubmitChanges();
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
