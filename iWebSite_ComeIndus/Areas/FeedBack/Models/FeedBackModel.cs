using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.FeedBack.Models
{
    public class FeedBackTypeModel
    {
        public string FeedbackTypeNo { get; set; }
        public string FeedbackTypeName { get; set; }
    }
    public class FeedBackModel : FeedBackTypeModel
    {
        public string FeedbackNo { get; set; }
        
        public string Username { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
    /*
     public class FeedBackModel
     {
         //public IEnumerable<SelectListItem> FeedbackTypeNo { get; set; }
         public IEnumerable<SelectListItem> TypeName { get; set; }
     }

     public class NewFeedBack
     {
         public string FeedbackNo { get; set; }
         public string FeedbackTypeNo { get; set; }
         public string FeedbackTypeName { get; set; }
         public string Username { get; set; }
         public string Title { get; set; }
         public string Content { get; set; }
         public DateTime CreateTime { get; set; }

     }
    */
}
