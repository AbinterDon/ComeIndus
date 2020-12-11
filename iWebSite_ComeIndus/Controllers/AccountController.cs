using iWebSite_ComeIndus.Extension;
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
        //public IActionResult Index()
        //{
        //    return View();
        //}

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
            var sqlStr = string.Format("select Account,Username,Password,MailCheck from [dbo].[Member] where Account = {0}",SqlVal2(Model.Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);

            //資料庫內是否有此帳號
            if(data.Rows.Count > 0)
            {
                //帳號與密碼是否相符
                if (Model.Account == data.Rows[0].ItemArray.GetValue(0).ToString() &&
                SHA256_Compare(data.Rows[0].ItemArray.GetValue(2).ToString(),Model.Password))
                {
                    //登入成功，但尚未驗證信箱
                    if(data.Rows[0].ItemArray.GetValue(3).ToString() != "1")
                    {
                        //前往驗證信箱畫面
                        return RedirectToAction("MailVerify", "Account", new Verify() { 
                            Account = Model.Account,
                            Username = data.Rows[0].ItemArray.GetValue(1).ToString()
                        });
                    }
                    else
                    {
                        //登入成功，已驗證信箱
                        return Redirect("/home/index");
                    }
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
            Model.ok = true;
            Model.MailCheck = "0";
            Model.Authority = "0";
            Model.PwdChangeCheck = "0";

            //SQL Insert Member
            var sqlStr = string.Format(
                @"INSERT INTO [dbo].[Member] (" +
                    "[Account]," +
                    "[Password]," +
                    "[Username]," +
                    "[Nickname]," +
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
                    "{10}," +
                    "{11}",
                    SqlVal2(Model.Account),
                    SqlVal2(SHA256_Encryption(Model.Password)),
                    SqlVal2(Model.Username),
                    SqlVal2(Model.Nickname),
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
                //信箱驗證
                if(RegisterMailVerify(Model))//Model.Account, Model.Username
                {
                    Model.ResultMessage = "註冊成功";

                    //註冊成功
                    return RedirectToAction("MailVerify", "Account", new Verify() { Account = Model.Account, Username = Model.Username });
                }
                else
                {
                    Model.ok = false;
                    Model.ResultMessage = "驗證信發送失敗";
                }
            }
            else
            {
                Model.ok = false;
                Model.ResultMessage = "註冊失敗";
                //註冊失敗
                //return View(Model);
            }
            return View(Model);
        }

        /// <summary>
        /// 註冊信箱驗證 產生隨機碼
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public bool RegisterMailVerify(Member Model)//string Account, string Username
        {
            //AutoMail實體化
            AutoMailClass mail = new AutoMailClass();

            //Random 亂數實體化
            Random rnd = new Random();

            //產生 100000~999999 之一的亂數
            string code = rnd.Next(100000, 999999 + 1).ToString();

            //發送註冊驗證信
            if (mail.RegisterVerify(Model.Account, Model.Username, code))
            {
                //把驗證碼寫進資料庫
                //sql where
                var sqlWhere = string.Format("Account = {0}", SqlVal2(Model.Account));

                //sql str
                var sqlStr = string.Format("UPDATE Member SET MailCheckCode = {0}, MailCheck = {1} where {2} and 1=1", SqlVal2(code), SqlVal2("0"), sqlWhere);

                //SQL Check Update成功(True)或失敗(False)
                return _DB_Execute(sqlStr) == 1 ? true : false;
            }
            else
            {
                //信件發送失敗
                return false;
            }
        }

        /// <summary>
        /// 信箱驗證畫面 GET
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MailVerify(string Account, string Username)
        {
            return View(new Verify() { Account = Account, Username = Username });
        }

        /// <summary>
        /// 信箱驗證畫面 POST
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MailVerify(Verify Model)
        {
            //SQL Insert Member
            var sqlStr = string.Format("select MailCheckCode from [dbo].[Member] where Account = {0}", SqlVal2(Model.Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);

            //資料庫內是否有此帳號
            if (data.Rows.Count > 0)
            {
                //驗證碼是否相符
                if (Model.VerificationCode == data.Rows[0].ItemArray.GetValue(0).ToString())
                {
                    //SQL where
                    var sqlWhere = string.Format("Account = {0}", SqlVal2(Model.Account));

                    //SQL str
                    sqlStr = string.Format("UPDATE Member SET MailCheck = {0} where {1} and 1=1", SqlVal2("1"), sqlWhere);

                    //SQL MailCheck更新結果
                    if (_DB_Execute(sqlStr) == 1)
                    {
                        //驗證成功
                        Model.Verified = true;
                        Model.ResultMessage = "驗證成功";
                    }
                    else
                    {
                        //驗證失敗 
                        Model.ok = false;
                        Model.ResultMessage = "驗證失敗[0]";
                    }
                }
                else
                {
                    //驗證失敗 
                    Model.ok = false;
                    Model.ResultMessage = "驗證失敗[1]";
                }
            }
            else
            {
                //驗證失敗 找不到此帳號
                Model.ok = false;
                Model.ResultMessage = "驗證失敗，找不到該帳號";
            }

            //Return
            return View(Model);
        }
    }
}
