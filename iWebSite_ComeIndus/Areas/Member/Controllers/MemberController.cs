using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Controllers;
using System.Data;

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
            var Authority = getUserAuthority();

            if (string.IsNullOrEmpty(Account)) Account = Request.Cookies["account"];

            //權限
            ViewData["Authority"] = Authority;

            if (Authority == "1" || Authority == "0")
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
                    "SELECT Account, Actualname, Username, Convert(varchar(10), Birthday,120) as Birthday, Gender, Authority " +
                    "FROM [dbo].[Member] " +
                    "Where Account = {0}", SqlVal2(Account)
                );

            //Return
            return _DB_GetData(sqlStr);
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Actualname"></param>
        /// <param name="Username"></param>
        /// <param name="Birthday"></param>
        /// <param name="Gender"></param>
        /// <param name="Authority"></param>
        /// <returns></returns>
        public ActionResult UpdateMember(string Account, string Actualname, string Username, DateTime Birthday, string Gender, string Authority)
        {
            var sqlStr = "";

            if (!string.IsNullOrEmpty(Authority))
            {
                sqlStr = string.Format(
                    @"UPDATE [dbo].[Member] " +
                        "SET [Actualname] = {0}, " +
                        "[Username] = {1}, " +
                        "[Birthday] = {2}, " +
                        "[Gender] = {3}, " +
                        "[Authority] = {4}, " +
                        "[ModifyTime] = {5} " +
                        "WHERE [Account] = {6}",
                        SqlVal2(Actualname),
                        SqlVal2(Username),
                        SqlVal2(Birthday),
                        SqlVal2(Gender),
                        SqlVal2(Authority),
                        DBC.ChangeTimeZone(),
                        SqlVal2(Account)
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
                        SqlVal2(Actualname),
                        SqlVal2(Username),
                        SqlVal2(Birthday),
                        SqlVal2(Gender),
                        DBC.ChangeTimeZone(),
                        SqlVal2(Account)
                );
            }

            var check = _DB_Execute(sqlStr);

            //修改是否成功
            if (check == 1)
            {
                return View();
            }
            else
            {
                //導致Error頁面
                return Redirect("~/Home/Error");
                //return StatusCode(403);
            }
        }

        /// <summary>
        /// 顯示會員資料
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowMember()
        {
            //目前登入權限
            var Authority = getUserAuthority();

            //權限
            ViewData["Authority"] = Authority;

            if (Authority == "1" || Authority == "0")
            {
                return View(GetMember(Authority));
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
        /// <param name="Authority"></param>
        /// <returns></returns>
        private DataTable GetMember(string Authority)
        {
            var sqlStr = "";
            if (Authority == "1")
            {
                sqlStr = string.Format("" +
                    "SELECT Account, Actualname, Username, Convert(varchar(10), Birthday,111) as Birthday, Gender, Authority " +
                    "FROM [dbo].[Member]"
                );
            }
            else if (Authority == "0")
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
        public ActionResult DeleteMember(string Account)
        {
            string resMsg = "";
            string sqlStr = "DELETE [dbo].[Member] " +
                "WHERE [Account] = '" + Account + "'";

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
