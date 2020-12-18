using iWebSite_ComeIndus.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using iWebSite_ComeIndus.Areas.News.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Controllers;
using System.Data;

namespace iWebSite_ComeIndus.Areas.News.Controllers
{
    [Area(areaName: "News")]
    public class NewsController : _BaseController
    {
        public static List<NewsTypeModel> NewsTypes = new List<NewsTypeModel> { };
        public static List<NewsModel> newsModel = new List<NewsModel> { };
        public IActionResult Index()
        {
            return View("NewNews");
        }

        [HttpGet]
        public ActionResult NewNews()
        {
            //SQL Select all type
            var sqlStr = string.Format("SELECT NewsTypeNo, TypeDescription FROM [dbo].[NewsType]");
            var data = _DB_GetData(sqlStr);

            NewsTypes = new List<NewsTypeModel>();
            foreach (DataRow row in data.Rows)
            {
                NewsTypeModel model = new NewsTypeModel();
                model.NewsTypeNo = row.ItemArray.GetValue(0).ToString();
                model.TypeDescription = row.ItemArray.GetValue(1).ToString();
                NewsTypes.Add(model);
            }

            ViewData["NewsTypes"] = NewsTypes;
            return View();
        }

        [HttpPost]
        public ActionResult NewNews(NewsModel Model)
        {
            string resMsg = "";
            // 長度限制
            if (Model.NewsContent.Length > 200 || Model.NewsTitle.Length > 50)
            {
                resMsg = "標題或內容超出長度限制!!";
            }
            else
            {
                //SQL Insert
                var tDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                var sqlStr = string.Format(
                @"INSERT INTO [dbo].[News](" +
                    "[NewsTypeNo]," +
                    "[NewsTitle]," +
                    "[NewsContent]," +
                    "[NewsHits]," +
                    "[CreateTime]," +
                    "[ModifyTime]," +
                    "[NewsStart]," +
                    "[NewsEnd]" +
                ")VALUES(" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}",
                    SqlVal2(Model.NewsTypeNo),
                    SqlVal2(Model.NewsTitle),
                    SqlVal2(Model.NewsContent),
                    0,
                    SqlVal2(tDate),
                    SqlVal2(tDate),
                    SqlVal2(Model.NewsStart),
                    SqlVal2(Model.NewsEnd) + ")"
                );

                var check = _DB_Execute(sqlStr);

                //新增是否成功
                if (check == 1)
                {
                    resMsg = "success";
                    //return View("NewNews", "Success!!");
                }
                else
                {
                    resMsg = "fail";
                }
            }

            //return View("NewNews", "Fail :(");
            ViewData["result"] = resMsg;
            ViewData["NewsTypes"] = NewsTypes;
            return View();
        }

        public ActionResult ShowNews()
        {
            //SQL Select all type
            var sqlTypes = string.Format("SELECT NewsTypeNo, TypeDescription FROM [dbo].[NewsType]");
            var dataTypes = _DB_GetData(sqlTypes);

            NewsTypes = new List<NewsTypeModel>();
            foreach (DataRow row in dataTypes.Rows)
            {
                NewsTypeModel model = new NewsTypeModel();
                model.NewsTypeNo = row.ItemArray.GetValue(0).ToString();
                model.TypeDescription = row.ItemArray.GetValue(1).ToString();
                NewsTypes.Add(model);
            }

            ViewData["NewsTypes"] = NewsTypes;
            if (getUserAuthority() == "1" || getUserAuthority() == "0" || getUserAuthority() == null)
            //if (getUserAuthority() == "1")
            {
                var tDate = DateTime.Now.Date;

                var sqlStr = string.Format(
               "SELECT NewsNo, [dbo].[News].NewsTypeNo, TypeDescription, NewsTitle, NewsContent, NewsHits, Convert(varchar(10), NewsStart,111) as NewsStart , Convert(varchar(10), NewsEnd,111) as NewsEnd " +
               "FROM [dbo].[News] " +
               "INNER JOIN [dbo].[NewsType] " +
               "on [dbo].[News].NewsTypeNo = [dbo].[NewsType].NewsTypeNo " +
               "ORDER BY NewsStart DESC");

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

        public ActionResult UpdateNews(string NewsNo, string NewsTypeNo, string NewsTitle, string NewsContent)
        {
            string resMsg = "";
            var modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlStr = "UPDATE [dbo].[News] " +
                "SET [NewsTypeNo] = '" + NewsTypeNo + "', " +
                "[NewsTitle] = '" + NewsTitle + "', " +
                "[NewsContent] = '" + NewsContent + "', " +
                "[ModifyTime] = '" + modifyTime + "' " +
                "WHERE [NewsNo] = '" + NewsNo + "'";

            var check = _DB_Execute(sqlStr);

            //修改是否成功
            if (check == 1)
            {
                resMsg = "success";
            }
            else
            {
                resMsg = "fail";
            }

            ViewData["result"] = resMsg;
            return View();
        }

        public ActionResult DeleteNews(string NewsNo)
        {
            string resMsg = "";
            string sqlStr = "DELETE [dbo].[News] " +
                "WHERE [NewsNo] = '" + NewsNo + "'";

            var check = _DB_Execute(sqlStr);

            //刪除是否成功
            if (check == 1)
            {
                resMsg = "success";
            }
            else
            {
                resMsg = "fail";
            }

            ViewData["result"] = resMsg;
            return View();
        }
    }
}
