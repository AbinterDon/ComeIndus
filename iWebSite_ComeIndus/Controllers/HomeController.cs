using iWebSite_ComeIndus.Areas.News.Controllers;
using iWebSite_ComeIndus.Areas.News.Models;
using iWebSite_ComeIndus.Extension;
using iWebSite_ComeIndus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Controllers
{
    public class HomeController : _BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //最新消息
            NewsController News = new NewsController();

            //最新消息取得 
            List<NewsModel> NewsData = News.ReturnNews(Helper.ConfigHelper.AppSettings.IndexNewsCount);

            //是否存在cookies
            if (!string.IsNullOrEmpty(Request.Cookies["account"]) && !string.IsNullOrEmpty(Request.Cookies["userName"]))
            {
                //SQL Insert Member
                var sqlStr = string.Format("select Authority from [dbo].[Member] where Account = {0}", SqlVal2(Request.Cookies["account"]));

                //SQL Check
                var data = _DB_GetData(sqlStr);

                //資料庫內是否有此帳號
                if (data.Rows.Count > 0)
                {
                    //取得權限
                    ViewData["Authority"] = data.Rows[0].ItemArray.GetValue(0).ToString();
                    //return View(new SetResult() { ok = true });
                }
            }

            //Return
            return View(NewsData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
