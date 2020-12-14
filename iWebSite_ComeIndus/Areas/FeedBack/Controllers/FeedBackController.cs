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

namespace iWebSite_ComeIndus.Areas.FeedBack.Controllers
{
    [Area(areaName: "FeedBack")]
    public class FeedBackController : _BaseController
    {
        public static List<FeedBackTypeModel> FDTypes = new List<FeedBackTypeModel> { };
        
        public IActionResult Index()
        {
            return NewFeedBack();
        }
        
        [HttpGet]
        public ActionResult NewFeedBack()
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

            ViewData["FDTypes"] = FDTypes;
            return View("NewFeedBack");
        }

        [HttpPost]
        public ActionResult NewFeedBack(FeedBackModel Model)
        {
            string resMsg = "";
            // 長度限制
            if (Model.Content.Length > 200 || Model.Title.Length > 50)
            {
                resMsg = "標題或內容超出長度限制!!";
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
                        SqlVal2(Model.FeedbackTypeNo),
                        SqlVal2(Request.Cookies["account"]),
                        SqlVal2(Model.Title),
                        SqlVal2(Model.Content),
                        "getDate()" + ")"
                    );

                var check = _DB_Execute(sqlStr);

                //新增是否成功
                if (check == 1)
                {
                    resMsg = "success";
                    //return View("NewFeedBack", "Success!!");
                }
                else
                {
                    resMsg = "fail";
                }
            }


            //return View("NewFeedBack", "Fail :(");
            ViewData["result"] = resMsg;
            ViewData["FDTypes"] = FDTypes;
            return View();
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
                return View("~/Views/Home/Index.cshtml");
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
