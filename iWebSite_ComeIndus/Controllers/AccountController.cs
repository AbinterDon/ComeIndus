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
        /// <summary>
        /// 登入頁面View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();//PartialView 暫時不套用Layout
        }

        /// <summary>
        /// 登入驗證
        /// 可以在這邊驗證完如果判斷是admin，則把ViewData["admin"] = visible;
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(AccountModels Model)
        {
            //SQL Select Member
            var sqlStr = string.Format("select Account, Username, Password, MailCheck, PwdChangeCheck, StatusNo from [dbo].[Member] where Account = {0}", SqlVal2(Model.Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);

            //資料庫內是否有此帳號
            if(data.Rows.Count > 0)
            {
                //帳號與密碼是否相符
                if (Model.Account == data.Rows[0].ItemArray.GetValue(0).ToString() &&
                SHA256_Compare(data.Rows[0].ItemArray.GetValue(2).ToString(),Model.Password))
                {
                    //登入成功，但遭到停權
                    if (data.Rows[0].ItemArray.GetValue(5).ToString() == "2")
                    {
                        //登入成功，但遭到停權
                        Model.ok = false;
                        Model.ResultMessage = "登入失敗，您的帳號已遭到『停權』。";
                        return View(Model);
                    }
                    else if(data.Rows[0].ItemArray.GetValue(3).ToString() != "1")//登入成功，但尚未驗證信箱
                    {
                        //前往驗證信箱畫面
                        return RedirectToAction("MailVerify", "Account", new Verify() { 
                            Account = Model.Account,
                            Username = data.Rows[0].ItemArray.GetValue(1).ToString()
                        });
                    }else if (data.Rows[0].ItemArray.GetValue(4).ToString() == "1")//帳號的密碼是否需要修改
                    {
                        //前往修改密碼畫面
                        return RedirectToAction("ChangePassword", "Account", new AccountModels()
                        {
                            Account = Model.Account
                        });
                    }
                    else
                    {
                        // 加入cookie，預設使用者關閉瀏覽器時清除
                        Response.Cookies.Append("userName", data.Rows[0].ItemArray.GetValue(1).ToString());
                        Response.Cookies.Append("account", data.Rows[0].ItemArray.GetValue(0).ToString());

                        //登入成功，已驗證信箱
                        return Redirect("/home/index");
                    }
                }
                else
                {
                    //登入失敗 帳號或密碼錯誤
                    Model.ok = false;
                    Model.ResultMessage = "登入失敗，帳號或密碼錯誤";
                    return View(Model);
                }
            }
            else
            {
                //登入失敗 找不到此帳號
                Model.ok = false;
                Model.ResultMessage = "登入失敗，找不到此帳號";
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
            Model.StatusNo = "0";
            Model.PwdChangeCheck = "0";

            //SQL Insert Member
            var sqlStr = string.Format(
                @"INSERT INTO [dbo].[Member] (" +
                    "[Account]," +
                    "[Password]," +
                    "[Username]," +
                    "[Actualname]," +
                    "[Gender]," +
                    "[Birthday]," +
                    "[MailCheck]," +
                    "[PwdChangeCheck]," +
                    "[CreateTime]," +
                    "[AccountStart]," +
                    "[StatusNo]" +
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
                    SqlVal2(Model.Actualname),
                    SqlVal2(Model.Gender),
                    SqlVal2(Model.Birthday),
                    SqlVal2(Model.MailCheck),
                    SqlVal2(Model.PwdChangeCheck),
                    DBC.ChangeTimeZone(),
                    DBC.ChangeTimeZone(),
                    SqlVal2(Model.StatusNo)+")"
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

        /// <summary>
        /// 忘記密碼 GET
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgetPassword(string Account)
        {
            return View(new Verify() { Account = Account});//PartialView
        }

        /// <summary>
        /// 忘記密碼信箱寄信 產生亂數密碼
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public bool PasswordMailVerify(Member Model)
        {
            //SQL Insert Member
            var sqlStr = string.Format("select Account,Username from [dbo].[Member] where Account = {0}", SqlVal2(Model.Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);

            //資料庫內是否有此帳號
            if (data.Rows.Count > 0)
            {
                //AutoMail實體化
                AutoMailClass mail = new AutoMailClass();

                #region 亂數密碼
                string ranNumber = "0123456789";
                string ranUpper = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
                string ranLower = "abcdefghijkmnopqrstuvwxyz";
                string ranSymbol = "!@#$%^&*";

                //密碼長度
                int passwordLength = 10;

                //密碼 char
                char[] chars = new char[passwordLength];

                //Random 亂數實體化
                Random rnd = new Random();

                //開始亂數
                for (int i = 0; i < passwordLength; i++)
                {
                    if(i % 5 == 0)
                    {
                        chars[i] = ranNumber[rnd.Next(0, ranNumber.Length)];
                    }else if(i % 3 == 0)
                    {
                        chars[i] = ranUpper[rnd.Next(0, ranUpper.Length)];
                    }else if (i % 2 == 0)
                    {
                        chars[i] = ranLower[rnd.Next(0, ranLower.Length)];
                    }
                    else
                    {
                        chars[i] = ranSymbol[rnd.Next(0, ranSymbol.Length)];
                    }
                }

                //New Password
                string pwd = new string(chars);
                #endregion

                //發送新密碼
                if (mail.ForgetPasswordSend(Model.Account, data.Rows[0].ItemArray.GetValue(1).ToString(), pwd))
                {
                    //把新密碼寫進資料庫
                    //sql where
                    var sqlWhere = string.Format("Account = {0}", SqlVal2(Model.Account));

                    //sql str
                    sqlStr = string.Format("UPDATE Member SET Password = {0}, PwdChangeCheck = {1}, ModifyTime = {2} where {3} and 1=1",
                        SqlVal2(SHA256_Encryption(pwd)), SqlVal2("1"), DBC.ChangeTimeZone(), sqlWhere);

                    //SQL Check Update成功(True)或失敗(False)
                    return _DB_Execute(sqlStr) == 1 ? true : false;
                }
                else
                {
                    //信件發送失敗
                    return false;
                }
            }
            else
            {
                //寄送失敗 找不到此帳號
                return false;
            }
        }

        /// <summary>
        /// 忘記密碼 GET
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword(string Account)
        {
            //若有沒有帳號 則取得Cookie的帳號
            if (string.IsNullOrEmpty(Account)) Account = Request.Cookies["account"];
            
            //檢測Account是否還是空的
            if (!string.IsNullOrEmpty(Account)) {
                return View(new AccountModels() { Account = Account });//PartialView
            }
            else
            {
                return Redirect("~/Home/Error");
            }
        }

        /// <summary>
        /// 忘記密碼 Post
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(AccountModels Model)
        {
            //把驗證碼寫進資料庫
            //sql where
            var sqlWhere = string.Format("Account = {0}", SqlVal2(Model.Account));

            //sql str
            var sqlStr = string.Format("UPDATE Member SET Password = {0}, PwdChangeCheck = {1}, ModifyTime = {2} where {3} and 1=1",
                SqlVal2(SHA256_Encryption(Model.Password)), SqlVal2("0"), DBC.ChangeTimeZone(), sqlWhere);

            //SQL Check Update成功(True)或失敗(False)
            if (_DB_Execute(sqlStr) == 1)
            {
                // 刪除cookie，預設使用者關閉瀏覽器時清除
                Response.Cookies.Delete("userName");
                Response.Cookies.Delete("account");

                //修改成功，重新登入
                return RedirectToAction("Login","Account",new AccountModels() { Account = Model.Account});
            }
            else
            {
                //修改失敗，回傳
                return View(new AccountModels() { ok = false, ResultMessage = "修改失敗" });
            }
        }

        /// <summary>
        /// 確認登入狀態
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool CheckLoginStatus()
        {
            if (string.IsNullOrEmpty(getUserStatusNo()))
            {
                return false;
            }
            {
                return true;
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            // 刪除cookie，預設使用者關閉瀏覽器時清除
            Response.Cookies.Delete("userName");
            Response.Cookies.Delete("account");

            //登出後返回首頁
            //return View("~/Views/Home/Index.cshtml");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 檢查是否有重複的帳號了
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpPost]
        public bool DuplicateAccountCheck(string Account)
        {
            //是否為空
            if (string.IsNullOrEmpty(Account)) return false;

            //SQL Select Member
            var sqlStr = string.Format("select Account from [dbo].[Member] where Account = {0}", SqlVal2(Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);

            //資料庫內是否有此帳號
            if (data.Rows.Count > 0)
            {
                //已經有該帳號
                return true;
            }

            //資料庫內目前沒此帳號
            return false;
        }
    }
}
