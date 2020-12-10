using iWebSite_ComeIndus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Controllers
{
    public class AccountController : _BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登入頁面View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(AccountModels Model)
        {
            //SQL Insert Member
            var sqlStr = string.Format("select Account,Password from [dbo].[Member] where Account = {0}",SqlVal2(Model.Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);

            //資料庫內是否有此帳號
            if(data.Rows.Count > 0)
            {
                //帳號與密碼是否相符
                if (Model.Account == data.Rows[0].ItemArray.GetValue(0).ToString() &&
                SHA256_Compare(data.Rows[0].ItemArray.GetValue(1).ToString(),Model.Password))
                {
                    //登入成功
                    return Redirect("/home/index");
                }
                else
                {
                    //登入失敗 帳號或密碼錯誤
                    return View(Model);
                }
            }
            else
            {
                //登入失敗 找不到此帳號
                return View(Model);
            }
        }

        /// <summary>
        /// 登入頁面View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(Member Model)
        {
            Model.MailCheck = "0";
            Model.Authority = "0";
            Model.PwdChangeCheck = "0";

            //SQL Insert Member
            var sqlStr = string.Format(
                @"INSERT INTO [dbo].[Member] (" +
                    "[Account]," +
                    "[Password]," +
                    "[Username]," +
                    "[Gender]," +
                    "[Birthday]," +
                    "[PhotoURL]," +
                    "[MailCheck]," +
                    "[PwdChangeCheck]," +
                    "[CreateTime]," +
                    "[AccountStart]," +
                    "[Authority]" +
                ")VALUES(" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}," +
                    "{8}," +
                    "{9}," +
                    "{10}",
                    SqlVal2(Model.Account),
                    SqlVal2(SHA256_Encryption(Model.Password)),
                    SqlVal2(Model.Username),
                    SqlVal2(Model.Gender),
                    SqlVal2(Model.Birthday),
                    SqlVal2(Model.PhotoURL),
                    SqlVal2(Model.MailCheck),
                    SqlVal2(Model.PwdChangeCheck),
                    "getDate()",
                    "getDate()",
                    SqlVal2(Model.Authority)+")"
                );

            //SQL Check
            var check = _DB_Execute(sqlStr);

            //新增是否成功
            if(check == 1)
            {
                //註冊成功
                return Redirect("/Home/index");
            }
            else
            {
                //註冊失敗
                return View(Model);
            }
        }
    }
}
