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
            var sqlStr = string.Format("SELECT NewsTypeNo, TypeName FROM [dbo].[NewsType]");
            var data = _DB_GetData(sqlStr);

            NewsTypes = new List<NewsTypeModel>();
            foreach (DataRow row in data.Rows)
            {
                NewsTypeModel model = new NewsTypeModel();
                model.NewsTypeNo = row.ItemArray.GetValue(0).ToString();
                model.TypeName = row.ItemArray.GetValue(1).ToString();
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
            if (string.IsNullOrEmpty(Model.NewsTitle) ||
                string.IsNullOrEmpty(Model.NewsContent) ||
                Model.NewsContent.Length > 200 ||
                Model.NewsTitle.Length > 50)
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
                    "[NewsEnd]," +
                    "[CreateUser]" +
                ")VALUES(" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}," +
                    "{8})",
                    SqlVal2(Model.NewsTypeNo),
                    SqlVal2(Model.NewsTitle),
                    SqlVal2(Model.NewsContent.Replace("\n", "<br>")),
                    0,
                    DBC.ChangeTimeZone(),
                    DBC.ChangeTimeZone(),
                    SqlVal2(Model.NewsStart),
                    SqlVal2(Model.NewsEnd),
                    SqlVal2(Request.Cookies["account"])
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

            if(checkMsg == "false" || resMsg == "Failed")
             {
                 return View(Model);
             }
             /*else
             {
                 //return RedirectToAction("ShowNews");
                 return View(Model);
             }
            */
            return View(Model);
        }

        /// <summary>
        /// 顯示最新消息
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <returns></returns>
        public ActionResult ShowNews(string NewsNo)
        {
            //SQL Select all type
            var sqlTypes = string.Format("SELECT NewsTypeNo, TypeName FROM [dbo].[NewsType]");
            var dataTypes = _DB_GetData(sqlTypes);

            NewsTypes = new List<NewsTypeModel>();
            foreach (DataRow row in dataTypes.Rows)
            {
                NewsTypeModel model = new NewsTypeModel();
                model.NewsTypeNo = row.ItemArray.GetValue(0).ToString();
                model.TypeName = row.ItemArray.GetValue(1).ToString();
                NewsTypes.Add(model);
            }

            ViewData["NewsTypes"] = NewsTypes;

            //目前登入權限
            var StatusNo = getUserStatusNo();

            //權限
            ViewData["StatusNo"] = StatusNo;

            //任何人都可進來查看 最新消息
            if (StatusNo == "1" || StatusNo == "0" || StatusNo == null)
            {
                return View(GetNews(NewsNo));
            }
            else
            {
                //導致Error頁面
                return Redirect("~/Home/Error");
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

            //若取得NewsNo不為空
            if (!string.IsNullOrEmpty(NewsNo))
            {
                var sqlStr = string.Format("" +
                        "SELECT NewsNo, [dbo].[News].NewsTypeNo, TypeName, NewsTitle, NewsContent, NewsHits, Convert(varchar(10), NewsStart,111) as NewsStart , Convert(varchar(10), NewsEnd,111) as NewsEnd " +
                        "FROM [dbo].[News] INNER JOIN [dbo].[NewsType] on [dbo].[News].NewsTypeNo = [dbo].[NewsType].NewsTypeNo " +
                        "where NewsNo = {0}", NewsNo
                    );

                string resMsg = "";

                var check = UpdateNewsHits(NewsNo);

                //修改是否成功
                if (check)
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
                var sqlStr = string.Format("SELECT {0} " +
                        "NewsNo," +
                        "NType.NewsTypeNo," +
                        "TypeName," +
                        "NewsTitle," +
                        "NewsContent," +
                        "NewsHits," +
                        "Convert(varchar(10), NewsStart, 111) as NewsStart," +
                        "Convert(varchar(10), NewsEnd, 111) as NewsEnd " +
                        "FROM [dbo].[News] as News INNER JOIN[dbo].[NewsType] as NType on NType.NewsTypeNo = News.NewsTypeNo " +
                        "where NewsEnd >= (SELECT convert(varchar, {1}, 111))" +
                        "ORDER BY NewsStart,News.CreateTime DESC", GetCount, DBC.ChangeTimeZone()
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
                            TypeName = row.ItemArray.GetValue(2).ToString(),
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
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool UpdateNews(NewsModel Model)
        {
            string sqlStr = string.Format(
                @"UPDATE [dbo].[News] " +
                    "SET [NewsTypeNo] = {0}, " +
                    "[NewsTitle] = {1}, " +
                    "[NewsContent] = {2}, " +
                    "[ModifyTime] = {3}, " +
                    "[ModifyUser] = {4} " +
                    "WHERE [NewsNo] = {5}",
                    SqlVal2(Model.NewsTypeNo), 
                    SqlVal2(Model.NewsTitle),
                    SqlVal2(Model.NewsContent.Replace("\n", "<br>")),
                    DBC.ChangeTimeZone(),
                    SqlVal2(Request.Cookies["account"]),
                    SqlVal2(Model.NewsNo));

            var check = _DB_Execute(sqlStr);

            //修改是否成功
            if (check == 1)
            {
                //成功
                return true;
            }
            else
            {
                //失敗
                return false;
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <returns></returns>
        public bool DeleteNews(string NewsNo)
        {
            string sqlStr = string.Format(
                @"DELETE [dbo].[News] " +
                    "WHERE [NewsNo] = {0}",
                 SqlVal2(NewsNo));

            var check = _DB_Execute(sqlStr);

            //刪除是否成功
            if (check == 1)
            {
                //成功
                return true;
            }
            else
            {
                //失敗
                return false;
            }
        }

        /// <summary>
        /// 更新點擊次數
        /// </summary>
        /// <param name="NewsNo"></param>
        /// <returns></returns>
        private bool UpdateNewsHits(string NewsNo)
        {
            //NewsTable
            string updateSqlStr = string.Format(
                @"UPDATE [dbo].[News] " +
                    "SET [NewsHits] = [NewsHits] + 1 " +
                    "WHERE [NewsNo] = " + SqlVal2(NewsNo));

            var check = _DB_Execute(updateSqlStr);

            if (check != 1) return false;

            //NewsHits
            string sql = string.Format(
                @"INSERT INTO [dbo].[NewsHits] (
                    [NewsNo],
                    [Account],
                    [CreateTime]) 
                    VALUES({0}, {1}, {2})", 
                    SqlVal2(NewsNo),
                    SqlVal2(Request.Cookies["account"]), 
                    DBC.ChangeTimeZone()
                );

            check = _DB_Execute(sql);

            if (check != 1) return false;

            return true;
        }
    }
}
