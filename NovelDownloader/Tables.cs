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
}