using iWebSite_ComeIndus.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using iWebSite_ComeIndus.Areas.FeedBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Controllers;
using System.Data;
using System.Text.Json;

namespace iWebSite_ComeIndus.Areas.FeedBack.Controllers
{
    [Area(areaName: "FeedBack")]
    public class FeedBackController : _BaseController
    {
        public static List<FeedBackTypeModel> FDTypes = new List<FeedBackTypeModel> { };
        
        public List<FeedBackTypeModel> Index()
        {
            return NewFeedBack();
        }
        
        [HttpGet]
        public List<FeedBackTypeModel> NewFeedBack()
        {
            //SQL Select all type
            var sqlStr = string.Format("select FeedbackTypeNo, TypeName from [dbo].[FeedbackType]");
            var data = _DB_GetData(sqlStr);

            FDTypes = new List<FeedBackTypeModel>();
            foreach (DataRow row in data.Rows)
            {
                FeedBackTypeModel model = new FeedBackTypeModel();
            
                model.FeedbackTypeNo = row.ItemArray.GetValue(0).ToString();
                model.FeedbackTypeName = row.ItemArray.GetValue(1).ToString();
                FDTypes.Add(model);
            }
            
            return FDTypes;
        }

        [HttpPost]
        public string NewFeedBack(string TypeNo, string Title, string Content)
        {
            
            string resMsg = "";
            if(TypeNo == null)
            {
                resMsg = "回饋類型不可為空!!";
            }
            else if (Title == null || Title.Length > 50)
            {
                resMsg = "未輸入標題或長度超出限制!!";
            }
            // 長度限制
            else if (Content != null && Content.Length > 200)
            {
                resMsg = "回饋內容超出長度限制!!";
            }
            else
            { 
                //SQL Insert
                var sqlStr = string.Format(
                    @"INSERT INTO [dbo].[FeedBack](" +
                        "[FeedbackTypeNo]," +
                        "[FeedbackUser]," +
                        "[FeedbackTitle]," +
                        "[FeedbackContent]," +
                        "[CreateTime]" +
                    ")VALUES(" +
                        "{0}," +
                        "{1}," +
                        "{2}," +
                        "{3}," +
                        "{4}",
                        SqlVal2(TypeNo),
                        SqlVal2(Request.Cookies["account"]),
                        SqlVal2(Title),
                        SqlVal2(Content),
                        "getDate()" + ")"
                    );

                var check = _DB_Execute(sqlStr);
               
                //新增是否成功
                if (check == 1)
                {
                    resMsg = "success";
                }
                else
                {
                    resMsg = "fail";
                }
            }
            return resMsg;
        }

        public ActionResult ShowFeedBack()
        {
            if (getUserAuthority() == "1" || getUserAuthority() == "0" || getUserAuthority() == null)
            //if (getUserAuthority() == "1")
            {
                var sqlStr = string.Format(
               "select FeedbackNo, TypeName, FeedbackTitle, FeedbackContent, FeedbackUser, [dbo].[Feedback].CreateTime " +
               "from [dbo].[Feedback] " +
               "inner join [dbo].[FeedbackType] " +
               "on [dbo].[Feedback].FeedbackTypeNo = [dbo].[FeedbackType].FeedbackTypeNo");

                var data = _DB_GetData(sqlStr);
                return View(data);
            }
            else
            {
                //先暫時導到此頁面，之後改到其他頁面
                return View("~/Views/Home/Index.cshtml/#section_feedback"); 
                //return StatusCode(403);
            }
        }
        /*
        [HttpGet]
        public ActionResult NewFeedBack()
        {
            //SQL Select all type
            var sqlStr = string.Format("select FeedbackTypeNo, TypeName from [dbo].[FeedbackType]");
            var data = _DB_GetData(sqlStr);

            
            // prepare for dropdownlist
            List<SelectListItem> feedbackTypes = new List<SelectListItem>();
            foreach (DataRow row in data.Rows)
            {  
                feedbackTypes.Add(new SelectListItem { 
                    Text = row.ItemArray.GetValue(1).ToString(),
                });
            }
        
            FeedBackModel Model = new FeedBackModel();
            Model.TypeName = feedbackTypes;
                
            return View(Model);
        }

        [HttpPost]
        public string NewFeedBack(NewFeedBack Model)
        {
            //SQL Insert
            var sqlStr = string.Format(
                @"INSERT INTO [dbo].[FeedBack]
                 VALUES(" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}",
                    SqlVal2("006"),
                    SqlVal2("FD3"),
                    SqlVal2("sda@ff.ovm"),
                    SqlVal2(Model.Title),
                    SqlVal2(Model.Content),
                    "getDate()"+ ")"
                );

            var check = _DB_Execute(sqlStr);

            //新增是否成功
            if (check == 1) 
            {
                return "success";
            }
            return "fail";

        }

        public ActionResult ShowFeedBack()
        {
            var sqlStr = string.Format("select* from [dbo].[Feedback]");
            var data = _DB_GetData(sqlStr);

            return View(data);
        }
        */
    }
}
