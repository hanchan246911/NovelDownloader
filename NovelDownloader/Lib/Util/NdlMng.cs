using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace NovelDownloader.Lib.Util
{
    class NdlMng
    {

        // 中央寄せ
        public static string TEXT_CENTER = "［＃センター］";
        // 改丁
        public static string LITERATURE = "［＃改丁］";

        // 地から１字上げ
        public static string BRINGEND_CHAR_1 = "［＃地から１字上げ］";
        // 地から２字上げ
        public static string BRINGEND_CHAR_2 = "［＃地から２字上げ］";
        // 地から３字上げ
        public static string BRINGEND_CHAR_3 = "［＃地から３字上げ］";

        // 見出し
        public static class HEADLINE
        {
            public static class LINE
            {
                public static class L
                {
                    // 大見出し
                    public static string START = "［＃大見出し］";
                    // 大見出し終わり
                    public static string END = "［＃大見出し終わり］";
                }

                public static class M
                {
                    // 中見出し
                    public static string START = "［＃中見出し］";
                    // 中見出し終わり
                    public static string END = "［＃中見出し終わり］";
                }

                public static class S
                {
                    // 小見出し
                    public static string START = "［＃小見出し］";
                    // 小見出し終わり
                    public static string END = "［＃小見出し終わり］";
                }
            }
        }

        // 字下げ
        public static class INDENTATION
        {
            public static class BLOCK
            {
                public static string START_F = "［＃ここから{0}字下げ］";
                public static string START_1 = "［＃ここから１字下げ］";
                public static string START_2 = "［＃ここから２字下げ］";
                public static string START_3 = "［＃ここから３字下げ］";
                public static string START_4 = "［＃ここから４字下げ］";
                public static string START_5 = "［＃ここから５字下げ］";
                public static string END = "［＃ここで字下げ終わり］";
            }

            public static class LINE
            {
                public static string START_F = "［＃{0}字下げ］";
                public static string START_1 = "［＃１字下げ］";
                public static string START_2 = "［＃２字下げ］";
                public static string START_3 = "［＃３字下げ］";
                public static string START_4 = "［＃４字下げ］";
                public static string START_5 = "［＃５字下げ］";
            }
        }

        // 罫囲み
        public static class RULED_LINE_ENCLOSURE
        {
            // ここから罫囲み
            public static string START = "［＃ここから罫囲み］";
            // ここで罫囲み終わり
            public static string END = "［＃ここで罫囲み終わり］";
        }

        // 小さな文字
        public static class CHARSIZE_LEVEL_DOWN
        {
            public static class BLOCK
            {
                public static string START_F = "［＃ここから{0}段階小さな文字］";
                public static string START_1 = "［＃ここから１段階小さな文字］";
                /// <summary>ここから２段階小さな文字</summary>
                public static string START_2 = "［＃ここから２段階小さな文字］";
                public static string START_3 = "［＃ここから３段階小さな文字］";
                public static string START_4 = "［＃ここから４段階小さな文字］";
                public static string START_5 = "［＃ここから５段階小さな文字］";
                public static string END     = "［＃ここで小さな文字終わり］";
            }
        }

        // 二重山括弧
        public static class DOUBLE_BRACKETS
        {
            // 始め二重山括弧
//            public static string START = "※［＃始め二重山括弧、1-1-52］";
            public static string START = "≪";
            // 終わり二重山括弧
//            public static string END = "※［＃終わり二重山括弧、1-1-53］";
            public static string END = "≫";
        }
        // ※　→　※［＃米印、1-2-8］
        public static string ASTERISK = "※［＃米印、1-2-8］";

        // ［　→　※［＃始め角括弧、1-1-46］
        // ］　→　※［＃終わり角括弧、1-1-47］
        // 〔　→　※［＃始めきっこう（亀甲）括弧、1-1-44］
        // 〕　→　※［＃終わりきっこう（亀甲）括弧、1-1-45］
        // ｜　→　※［＃縦線、1-1-35］
        // ＃　→　※［＃井げた、1-1-84］

        public static void getNovelInfo(string ncode)
        {
            using (var cn = new SQLiteConnection(DbMng.getConnectionString()))
            {
                cn.Open();
                cn.ChangePassword("");
                using (var trans = cn.BeginTransaction())
                {
                    var novelid = DbMng.getNovelId(ncode, cn);
                    var html = HtmlMng.getNovel(ncode);

                    if (novelid < 0)
                        novelid = DbMng.addNovelList(ncode, html, cn);
                    else
                        DbMng.updateNovelList(novelid, html, cn);

                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    DbMng.deleteSubtitleByNovelId(novelid, cn);

                    var articles = htmlDoc.DocumentNode
                            .SelectNodes(@"//div[@class=""index_box""]/dl[@class=""novel_sublist2""]");
                    var i = 0;
                    foreach (var a in articles)
                    {
                        i++;

                        var htmldl = new HtmlAgilityPack.HtmlDocument();
                        htmldl.LoadHtml(a.InnerHtml);

                        var dd = htmldl.DocumentNode.SelectNodes(@"//dd[@class=""subtitle""]/a")
                                .Select(x => new
                                {
                                    Url = x.Attributes["href"].Value.Trim(),
                                    Title = x.InnerText.Trim(),
                                }).ToList().FirstOrDefault();
                        var url = dd.Url;
                        var title = dd.Title;
                        var upddate = htmldl.DocumentNode.SelectNodes(@"//dt[@class=""long_update""]")
                                .Select(x => x.InnerText.Trim())
                                .ToList().FirstOrDefault().Substring(0, 16);
                        var pageSplit = url.Split('/');
                        var subhtml = HtmlMng.getNovel(ncode, pageSplit[2]);
                        DbMng.addSubtitle(i, novelid, dd.Title, dd.Url, subhtml, upddate, cn);
                    }
                    trans.Commit();
                }
                cn.Close();
            }
        }

        public static void outputNovelText(string ncode)
        {
            var filename = ncode + ".txt";
            var dirpath = Path.Combine(createProjectDirPath(), "tmp");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            var filepath = Path.Combine(dirpath, filename);
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            using (var sw = new StreamWriter(filepath, true, Encoding.GetEncoding("utf-8")))
            {
                using (var aConnection = DbMng.getConnection())
                {
                    var novelists = DbMng.getNovelList(ncode, aConnection);
                    var subtitles = DbMng.getSubTitleByNovelId((int)novelists.Id, aConnection);
                    var novelsettings = DbMng.getNovelSetting((int)novelists.Id, aConnection);
                    var replacesettings = DbMng.getReplaceSetting((int)novelists.Id, aConnection);
                    var repflg = novelsettings.Replacestring;

                    var title = repflg ? repNStr(novelists.Title, replacesettings) : novelists.Title;
                    sw.WriteLine(title);
                    var writername = repflg ? repNStr(novelists.Writername, replacesettings) : novelists.Writername;
                    sw.WriteLine(BRINGEND_CHAR_3 + writername);

                    var captertitle = "";
                    foreach (var subtitle in subtitles)
                    {
                        if (!subtitle.Output)
                            continue;

                        if (captertitle != subtitle.Capter)
                        {
                            sw.WriteLine(LITERATURE);
                            sw.WriteLine(TEXT_CENTER + HEADLINE.LINE.L.START + subtitle.Capter + HEADLINE.LINE.L.END);
                            captertitle = subtitle.Capter;
                        }


                        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(subtitle.Html);

                        sw.WriteLine(LITERATURE);

                        var repsubtitle = repflg ? repNStr(subtitle.Title, replacesettings) : subtitle.Title;
                        if (novelsettings.Subtitleheading)
                            sw.WriteLine(TEXT_CENTER + HEADLINE.LINE.M.START + repsubtitle + HEADLINE.LINE.M.END);
                        else
                            sw.WriteLine(repsubtitle);
                        sw.WriteLine("");

                        if (novelsettings.Preface) {
                            var nodenovelp = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_p""]");
                            var novelp = nodenovelp == null ? null : nodenovelp.InnerText;
                            if (!String.IsNullOrEmpty(novelp))
                            {
                                sw.WriteLine(INDENTATION.BLOCK.START_2 + CHARSIZE_LEVEL_DOWN.BLOCK.START_2 + RULED_LINE_ENCLOSURE.START);
                                outputText(novelp, repflg, replacesettings, novelsettings, sw);
                                sw.WriteLine(RULED_LINE_ENCLOSURE.END);
                                sw.WriteLine(CHARSIZE_LEVEL_DOWN.BLOCK.END + INDENTATION.BLOCK.END);
                            }
                        }

                        var nodenovelh = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_honbun""]");
                        var novelh = nodenovelh == null ? null : nodenovelh.InnerText;
                        if (novelh != null)
                            outputText(novelh, repflg, replacesettings, novelsettings, sw);

                        if (novelsettings.Preface)
                        {
                            var nodenovela = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id=""novel_a""]");
                            var novela = nodenovela == null ? null : nodenovela.InnerText;
                            if (!String.IsNullOrEmpty(novela))
                            {
                                sw.WriteLine(INDENTATION.BLOCK.START_2 + CHARSIZE_LEVEL_DOWN.BLOCK.START_2 + RULED_LINE_ENCLOSURE.START);
                                outputText(novela, repflg, replacesettings, novelsettings, sw);
                                sw.WriteLine(RULED_LINE_ENCLOSURE.END);
                                sw.WriteLine(CHARSIZE_LEVEL_DOWN.BLOCK.END + INDENTATION.BLOCK.END);
                            }
                        }
                    }
                }
            }
        }

        private static void outputText(string text, bool repflg
            , List<Replacesetting> replacesettings
            , Novelsetting novelsettings
            , StreamWriter sw)
        {

            var novelh = repflg ? repNStr(text, replacesettings) : text;
            var novelsplit = novelh.Split('\n');

            for (var i = 0; i < novelsplit.Length; i++)
            {

                var s = "";
                var n = "";
                var u = getNovelSplitAddLine(novelsplit, i, -1);
                if (novelsettings.Multiblanklinedel == 0)
                {
                    s = getNovelSplitAddLine(novelsplit, i, 0);
                    n = getNovelSplitAddLine(novelsplit, i, 1);
                }
                else
                {
                    var mlflg = true;
                    var recs = new string[novelsettings.Multiblanklinedel + 1];
                    for (var j = 0; j < recs.Length; j++)
                    {
                        recs[j] = getNovelSplitAddLine(novelsplit, i, j);

                        if (!String.IsNullOrWhiteSpace(recs[j]))
                            mlflg = false;
                    }
                    if (mlflg) continue;
                    s = recs[0];
                    n = recs[1];
                }

                if (novelsettings.Oneblanklinedel
                    && !String.IsNullOrWhiteSpace(u)
                    && String.IsNullOrWhiteSpace(s)
                    && !String.IsNullOrWhiteSpace(n))
                    continue;

                if (s.Trim().StartsWith("※"))
                    s = s.Replace("※", ASTERISK);

                if (s.Trim().StartsWith("《"))
                    s = s.Replace("《", DOUBLE_BRACKETS.START)
                        .Replace("》", DOUBLE_BRACKETS.END);

                if (novelsettings.Indentation
                    && !String.IsNullOrWhiteSpace(s)
                    && !s.StartsWith("「")
                    && !s.StartsWith("『")
                    && !s.StartsWith("〔")
                    && !s.StartsWith("（")
                    && !s.StartsWith("【")
                    && !s.StartsWith("≪")
                    && !s.StartsWith("※"))
                    s = "　" + s;

                s = s.Replace("&quot;", @"""")
                        .Replace("&lt;", "<")
                        .Replace("&gt;", ">")
                        .Replace("&amp;", "&");

                sw.WriteLine(s);
            }
            sw.WriteLine("");
        }


        public static string createProjectDirPath()
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

        private static string repNStr(string pre, List<Replacesetting> replasesetting)
        {
            if (String.IsNullOrEmpty(pre))
                return pre;

            var result = pre;
            foreach (var rep in replasesetting)
            {
                result = result.Replace(rep.Replacefrom, rep.Replaceto);
            }
            return result;
        }

        private static string getNovelSplitAddLine(string[] novelsplit, int nowline, int addline)
        {
            var result = "";
            if ((addline > 0 && nowline < novelsplit.Length - addline)
               || (addline < 0 && nowline + addline >= 0)
               || (addline == 0))
            {
                result = novelsplit[nowline + addline];
                if (!String.IsNullOrEmpty(result))
                    result = result.Trim();
            }
            return result;
        }
    }
}
