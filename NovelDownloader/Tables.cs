using System;
using System.Data.Linq.Mapping;


namespace NovelDownloader
{
    [Table(Name = "Novellists")]
    public class Novellist
    {
        // ID
        [Column(Name = "id", DbType = "INT", IsPrimaryKey = true)]
        public int? Id { get; set; }

        // Novel Key
        [Column(Name = "ncode", DbType = "NVARCHAR", CanBeNull = false)]
        public String Ncode { get; set; }

        // Html
        [Column(Name = "html", DbType = "NVARCHAR", CanBeNull = false)]
        public String Html { get; set; }

        // Title
        [Column(Name = "title", DbType = "NVARCHAR", CanBeNull = false)]
        public String Title { get; set; }

        // Writername
        [Column(Name = "writername", DbType = "NVARCHAR", CanBeNull = false)]
        public String Writername { get; set; }

        // Reg Date
        [Column(Name = "regdate", DbType = "NVARCHAR", CanBeNull = true)]
        public String Regdate { get; set; }
    }

    [Table(Name = "Subtitles")]
    public class Subtitle
    {
        // ID
        [Column(Name = "id", DbType = "INT", IsPrimaryKey = true)]
        public int Id { get; set; }

        // Novel ID
        [Column(Name = "novelid", DbType = "INT", IsPrimaryKey = true)]
        public int Novelid { get; set; }

        // Capter
        [Column(Name = "capter", DbType = "NVARCHAR", CanBeNull = true)]
        public String Capter { get; set; }

        // SubTitle
        [Column(Name = "title", DbType = "NVARCHAR", CanBeNull = true)]
        public String Title { get; set; }

        // Url
        [Column(Name = "url", DbType = "NVARCHAR", CanBeNull = true)]
        public String Url { get; set; }

        // Html
        [Column(Name = "html", DbType = "NVARCHAR", CanBeNull = false)]
        public String Html { get; set; }

        // Output
        [Column(Name = "output", DbType = "INT", CanBeNull = true)]
        public Boolean Output { get; set; }

        // Update Date
        [Column(Name = "upddate", DbType = "NVARCHAR", CanBeNull = true)]
        public String Upddate { get; set; }

        // Reg Date
        [Column(Name = "regdate", DbType = "NVARCHAR", CanBeNull = true)]
        public String Regdate { get; set; }
    }

    [Table(Name = "NovelSettings")]
    public class Novelsetting
    {
        // novel id
        [Column(Name = "novelid", DbType = "INT", IsPrimaryKey = true)]
        public int Novelid { get; set; }

        // subtitleheading
        [Column(Name = "subtitleheading", DbType = "INT")]
        public Boolean Subtitleheading { get; set; }

        // indentation
        [Column(Name = "indentation", DbType = "INT")]
        public Boolean Indentation { get; set; }

        // oneblanklinedel
        [Column(Name = "oneblanklinedel", DbType = "INT")]
        public Boolean Oneblanklinedel { get; set; }

        // multiblanklinedel
        [Column(Name = "multiblanklinedel", DbType = "INT")]
        public int Multiblanklinedel { get; set; }

        // preface
        [Column(Name = "preface", DbType = "INT")]
        public Boolean Preface { get; set; }

        // trailer
        [Column(Name = "trailer", DbType = "INT")]
        public Boolean Trailer { get; set; }

        // replacestring
        [Column(Name = "replacestring", DbType = "INT")]
        public Boolean Replacestring { get; set; }

        // Reg Date
        [Column(Name = "regdate", DbType = "NVARCHAR", CanBeNull = true)]
        public String Regdate { get; set; }
    }

    [Table(Name = "ReplaceSettings")]
    public class Replacesetting
    {
        // id
        [Column(Name = "id", DbType = "INT", IsPrimaryKey = true)]
        public int Id { get; set; }

        // novel id
        [Column(Name = "novelid", DbType = "INT", IsPrimaryKey = true)]
        public int Novelid { get; set; }

        // Replace From
        [Column(Name = "replacefrom", DbType = "NVARCHAR", CanBeNull = true)]
        public String Replacefrom { get; set; }

        // Replace To
        [Column(Name = "replaceto", DbType = "NVARCHAR", CanBeNull = true)]
        public String Replaceto { get; set; }

        // Reg Date
        [Column(Name = "regdate", DbType = "NVARCHAR", CanBeNull = true)]
        public String Regdate { get; set; }
    }
}