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
            ////是否存在cookies
            //if (string.IsNullOrEmpty(Request.Cookies["account"]) && string.IsNullOrEmpty(Request.Cookies["userName"]))
            //{
            //    //SQL Insert Member
            //    var sqlStr = string.Format("select Account,Username from [dbo].[Member] where Account = {0}", SqlVal2(Request.Cookies["account"]));

            //    //SQL Check
            //    var data = _DB_GetData(sqlStr);

            //    //資料庫內是否有此帳號
            //    if (data.Rows.Count > 0) {
            //        return View(new SetResult() { ok = true});
            //    }
            //    else
            //    {
            //        return View(new SetResult() { ok = false });
            //    }
            //}
            return View();
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
