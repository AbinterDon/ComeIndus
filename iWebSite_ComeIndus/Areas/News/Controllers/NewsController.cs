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

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View("NewNews");
        }

        /// <summary>
        /// 消息種類
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 最新消息新增
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewNews(NewsModel Model)
        {
            string resMsg = "";
            string checkMsg = "";
            
            // 長度限制
            if (Model.NewsContent.Length > 200 || Model.NewsTitle.Length > 50 || Model.NewsContent == null || Model.NewsTitle == null)
            {

                resMsg = "標題或內容不符合長度限制!! 標題與內容不可為空，且標題要在50字內，內容不可超過200字";
               
                checkMsg = "false";
                
            }
            else
            {
                checkMsg = "true";
                //SQL Insert
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
                    "getDate()",
                    "getDate()",
                    SqlVal2(Model.NewsStart),
                    SqlVal2(Model.NewsEnd) + ")"
                );

                var check = _DB_Execute(sqlStr);

                //新增是否成功
                if (check == 1)
                {
                    resMsg = "新增成功";
                    //return View("NewNews", "Success!!");
                }
                else
                {
                    resMsg = "Failed";
                }
            }

            //return View("NewNews", "Fail :(");
            ViewData["result"] = resMsg;
            ViewData["NewsTypes"] = NewsTypes;
            ViewData["checkMsg"] = checkMsg;
            TempData["Message"] = resMsg;
                
            return View();
        }

        /// <summary>
        /// 顯示最新消息
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <returns></returns>
        public ActionResult ShowNews(string NewsNo)
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
                return View(GetNews(NewsNo));
            }
            else
            {
                //先暫時導到此頁面，之後改到其他頁面
                return View("~/Views/Home/Index.cshtml");
                //return StatusCode(403);
            }
        }

        /// <summary>
        /// 撈DB最新消息
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <param name="GetCount"></param>
        /// <returns></returns>
        private DataTable GetNews(string NewsNo, string GetCount = "")
        {
            //若取得數量不為空
            if (!string.IsNullOrEmpty(GetCount))
            {
                GetCount = string.Format("TOP({0})", GetCount) ;
            }

            //若取得數量不為空
            if (!string.IsNullOrEmpty(NewsNo))
            {
                var sqlStr = string.Format("" +
                        "SELECT NewsNo, [dbo].[News].NewsTypeNo, TypeDescription, NewsTitle, NewsContent, NewsHits, Convert(varchar(10), NewsStart,111) as NewsStart , Convert(varchar(10), NewsEnd,111) as NewsEnd " +
                        "FROM [dbo].[News] INNER JOIN [dbo].[NewsType] on [dbo].[News].NewsTypeNo = [dbo].[NewsType].NewsTypeNo " +
                        "where NewsNo = {0}", NewsNo
                    );

                string resMsg = "";
                string updateSqlStr = "UPDATE [dbo].[News] " +
                "SET [NewsHits] = [NewsHits] + 1 " +
                "WHERE [NewsNo] = '" + NewsNo + "'";

                var check = _DB_Execute(updateSqlStr);

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

                //Return
                return _DB_GetData(sqlStr);
            }
            else
            {
                //SQL 順便做有效時間塞選
                var sqlStr = string.Format("" +
                        "SELECT {0} NewsNo, [dbo].[News].NewsTypeNo, TypeDescription, NewsTitle, NewsContent, NewsHits, Convert(varchar(10), NewsStart,111) as NewsStart , Convert(varchar(10), NewsEnd,111) as NewsEnd " +
                        "FROM [dbo].[News] INNER JOIN [dbo].[NewsType] on [dbo].[News].NewsTypeNo = [dbo].[NewsType].NewsTypeNo " +
                        "where NewsEnd >= (SELECT convert(varchar, getdate(), 111)) " +
                        "ORDER BY NewsStart DESC", GetCount
                    );

                //Return
                return _DB_GetData(sqlStr);
            }
        }

        /// <summary>
        /// 取得最新消息
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <param name="GetCount"></param>
        /// <returns></returns>
        public List<NewsModel> ReturnNews(string NewsNo, string GetCount = "")
        {
            //Model
            List<NewsModel> Model = new List<NewsModel>();

            //News Data
            var data = GetNews(NewsNo, GetCount);

            //抓取最新消息
            foreach (DataRow row in data.Rows)
            {
                //Add model
                Model.Add(
                        new NewsModel()
                        {
                            NewsNo = row.ItemArray.GetValue(0).ToString(),
                            NewsTypeNo = row.ItemArray.GetValue(1).ToString(),
                            TypeDescription = row.ItemArray.GetValue(2).ToString(),
                            NewsTitle = row.ItemArray.GetValue(3).ToString(),
                            NewsContent = row.ItemArray.GetValue(4).ToString(),
                            NewsHits = Convert.ToInt32(row.ItemArray.GetValue(5)),
                            NewsStart = Convert.ToDateTime(row.ItemArray.GetValue(6)),
                            NewsEnd = Convert.ToDateTime(row.ItemArray.GetValue(7))
                            //NewsStart = Convert.ToDateTime(row.ItemArray.GetValue(6).ToString()),
                            //NewsEnd = Convert.ToDateTime(row.ItemArray.GetValue(6).ToString()),
                            //NewsStart = Convert.ToDateTime(DateTime.ParseExact(row.ItemArray.GetValue(6).ToString(), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces)),
                            //NewsEnd = Convert.ToDateTime(DateTime.ParseExact(row.ItemArray.GetValue(7).ToString(), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces)),
                        }
                    );
            }

            //Return
            return Model;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <param name="NewsTypeNo"></param>
        /// <param name="NewsTitle"></param>
        /// <param name="NewsContent"></param>
        /// <returns></returns>
        public ActionResult UpdateNews(string NewsNo, string NewsTypeNo, string NewsTitle, string NewsContent)
        {
            string resMsg = "";
            string sqlStr = "UPDATE [dbo].[News] " +
                "SET [NewsTypeNo] = '" + NewsTypeNo + "', " +
                "[NewsTitle] = '" + NewsTitle + "', " +
                "[NewsContent] = '" + NewsContent + "', " +
                "[ModifyTime] = getDate() " +
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

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <returns></returns>
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
