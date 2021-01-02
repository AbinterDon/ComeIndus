using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Controllers;
using System.Data;
using iWebSite_ComeIndus.Areas.Member.Models;

namespace iWebSite_ComeIndus.Areas.Member.Controllers
{
    [Area(areaName: "Member")]
    public class MemberController : _BaseController
    {
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 顯示要修改的會員資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public IActionResult ModifyMember(string Account)
        {
            //目前登入權限
            var StatusNo = getUserStatusNo();

            //若有沒有帳號 則取得Cookie的帳號
            if (string.IsNullOrEmpty(Account)) Account = Request.Cookies["account"];

            //權限
            ViewData["StatusNo"] = StatusNo;

            if (StatusNo != null && (StatusNo == "1" || StatusNo == "0" || StatusNo == "2"))
            {
                return View(GetAccount(Account));
            }
            else
            {
                //導致Error頁面
                return Redirect("~/Home/Error");
                //return StatusCode(403);
            }
        }

        /// <summary>
        /// 撈DB會員資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        private DataTable GetAccount(string Account)
        {
            var sqlStr = string.Format("" +
                    "SELECT Account, Actualname, Username, Convert(varchar(10), Birthday,120) as Birthday, Gender, StatusNo " +
                    "FROM [dbo].[Member] " +
                    "Where Account = {0}", SqlVal2(Account)
                );

            //Return
            return _DB_GetData(sqlStr);
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool UpdateMember(MemberModels Model)
        {
            var sqlStr = "";

            DateTime Temp = new DateTime();

            //檢查年分
            if(Model.Birthday != null)
            {
                Temp = (DateTime)Model.Birthday;
                if(Temp.Year <= 1970)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(Model.StatusNo))
            {
                sqlStr = string.Format(
                    @"UPDATE [dbo].[Member] " +
                        "SET [Actualname] = {0}, " +
                        "[Username] = {1}, " +
                        "[Birthday] = {2}, " +
                        "[Gender] = {3}, " +
                        "[StatusNo] = {4}, " +
                        "[ModifyTime] = {5} " +
                        "WHERE [Account] = {6}",
                        SqlVal2(Model.Actualname),
                        SqlVal2(Model.Username),
                        SqlVal2(Model.Birthday),
                        SqlVal2(Model.Gender),
                        SqlVal2(Model.StatusNo),
                        DBC.ChangeTimeZone(),
                        SqlVal2(Model.Account)
                );
            }
            else
            {
                sqlStr = string.Format(
                    @"UPDATE [dbo].[Member] " +
                        "SET [Actualname] = {0}, " +
                        "[Username] = {1}, " +
                        "[Birthday] = {2}, " +
                        "[Gender] = {3}, " +
                        "[ModifyTime] = {4} " +
                        "WHERE [Account] = {5}",
                        SqlVal2(Model.Actualname),
                        SqlVal2(Model.Username),
                        SqlVal2(Model.Birthday),
                        SqlVal2(Model.Gender),
                        DBC.ChangeTimeZone(),
                        SqlVal2(Model.Account)
                );
            }

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
        /// 顯示會員資料
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowMember()
        {
            //目前登入權限
            var StatusNo = getUserStatusNo();

            //權限
            ViewData["StatusNo"] = StatusNo;

            //是管理員的話
            if (StatusNo == "1")
            {
                return View(GetMember(StatusNo));

            }else if(StatusNo == "0")//是一般會員
            {
                return RedirectToAction("ModifyMember");
            }
            else
            {
                //導致Error頁面
                return Redirect("~/Home/Error");
                //return StatusCode(403);
            }
        }

        /// <summary>
        /// 撈DB會員資料
        /// </summary>
        /// <param name="StatusNo"></param>
        /// <returns></returns>
        private DataTable GetMember(string StatusNo)
        {
            var sqlStr = "";
            if (StatusNo == "1")
            {
                sqlStr = string.Format("" +
                    "SELECT Account, Actualname, Username, Convert(varchar(10), Birthday,111) as Birthday, Gender, StatusNo " +
                    "FROM [dbo].[Member]"
                );
            }
            else if (StatusNo == "0")
            {
                sqlStr = string.Format("" +
                    "SELECT Account, Actualname, Username, Convert(varchar(10), Birthday,111) as Birthday, Gender " +
                    "FROM [dbo].[Member] " +
                    "WHERE Account = {0}"
                , SqlVal2(Request.Cookies["account"]));
            }

            //Return
            return _DB_GetData(sqlStr);
        }

        /// <summary>
        /// 刪除會員資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public bool DeleteMember(string Account)
        {
            bool check = true;

            if(!string.IsNullOrEmpty(Account) && getUserStatusNo(Account) != "1")
            {
                string resMsg = "";
                string sqlStr = "DELETE [dbo].[Member] " +
                    "WHERE [Account] = '" + Account + "'";

                var ExecuteCheck = _DB_Execute(sqlStr);

                //刪除是否成功
                if (ExecuteCheck == 1)
                {
                    resMsg = "success";
                }
                else
                {
                    resMsg = "fail";
                    check = false;
                }

                ViewData["result"] = resMsg;
            }
            else
            {
                check = false;
            }

            return check;
        }
    }
}
