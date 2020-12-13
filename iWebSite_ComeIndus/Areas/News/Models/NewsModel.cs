using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.News.Models
{
    public class NewsTypeModel
    {
        public string NewsTypeNo { get; set; }
        public string NewsTypeName { get; set; }
    }
    public class NewsModel : NewsTypeModel
    {
        public string NewsNo { get; set; }
        
        public string NewsTitle { get; set; }
        public string NewsContent { get; set; }
        public int NewsHits { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public DateTime NewsStart { get; set; }
        public DateTime NewsEnd { get; set; }
    }
}
