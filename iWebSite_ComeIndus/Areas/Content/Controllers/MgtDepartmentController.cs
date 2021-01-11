using iWebSite_ComeIndus.Areas.Content.Models;
using iWebSite_ComeIndus.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Content.Controllers
{
    [Area(areaName: "Content")]
    public class MgtDepartmentController : _BaseController
    {
        public IActionResult Index()
        {
            return ShowUnivDepartment();
        }

        /// <summary>
        /// 顯示國家科系
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowUnivDepartment()
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return Redirect("~/Home/Error");
            }

            List<DeptModel> Models = new List<DeptModel>();

            var sqlStr = string.Format("Select DeptNo, DeptName, DeptDescription, CreateTime, CreateUser From Department");
            var DeptData = _DB_GetData(sqlStr);
            foreach (DataRow DeptRow in DeptData.Rows)
            {
                Models.Add(new DeptModel()
                {
                    DeptNo = DeptRow.ItemArray.GetValue(0).ToString(),
                    DeptName = DeptRow.ItemArray.GetValue(1).ToString(),
                    DeptDescription = DeptRow.ItemArray.GetValue(2).ToString(),
                    CreateTime = DeptRow.ItemArray.GetValue(3).ToString(),
                    CreateUser = DeptRow.ItemArray.GetValue(4).ToString()
                });
            }

            return View("MgtDepartment", Models);
        }

        /// <summary>
        /// 新增科系
        /// </summary>
        /// <param name="CountryNo"></param>
        /// <param name="DeptNo"></param>
        /// <param name="DeptName"></param>
        /// <param name="DeptDescription"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertUnivDepartment(string DeptName, string DeptDescription)
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return "權限錯誤！";
            }

            //Return Msg
            string resMsg = "新增成功！";

            if (DeptName == null || DeptName.Length > 50)
            {
                resMsg = "未輸入科系或長度超過限制!!";
            }
            else if (DeptDescription != null && DeptDescription.Length > 200)//長度限制
            {
                resMsg = "敘述超出長度限制!!";
            }
            else
            {
                //檢查科系名稱是否已經存在
                var sqlStr = string.Format("SELECT DeptNo From [dbo].[Department] WHERE DeptName={0}", SqlVal2(DeptName));
                var data = _DB_GetData(sqlStr);

                if (data.Rows.Count > 0)
                {
                    //科系名稱已存在
                    resMsg = "新增失敗，該科系已存在！";
                }
                else
                {
                    //SQL Insert
                    sqlStr = string.Format(
                        @"INSERT INTO [dbo].[Department](" +
                        "[DeptName]," +
                        "[DeptDescription]," +
                        "[CreateTime]," +
                        "[ModifyTime]," +
                        "[CreateUser] " +
                        ") " +
                        "VALUES(" +
                        "{0}," +
                        "{1}," +
                        "{2}," +
                        "{3}," +
                        "{4}",
                        SqlVal2(DeptName),
                        SqlVal2(DeptDescription),
                        DBC.ChangeTimeZone(),
                        DBC.ChangeTimeZone(),
                        SqlVal2(Request.Cookies["account"]) + ")"
                        );

                    //執行是否成功
                    if (_DB_Execute(sqlStr) != 1)
                    {
                        resMsg = "新增失敗，若持續發生此問題，請聯絡我們。";
                    }
                }
            }

            //Return
            return resMsg;
        }

        /// <summary>
        /// 修改科系
        /// </summary>
        /// <param name="DeptNo"></param>
        /// <param name="DeptName"></param>
        /// <param name="DeptDescription"></param>
        /// <returns></returns>
        [HttpPost]
        public string UpdateUnivDepartment(string DeptNo, string DeptName, string DeptDescription)
        {
            //Return Msg
            string resMsg = "修改成功！";

            // admin check
            if (getUserStatusNo() != "1")
            {
                resMsg = "修改失敗，權限錯誤！";
            }
            else
            {
                string sqlStr = "UPDATE [dbo].[Department] " +
                "SET [DeptName] = N'" + DeptName + "', " +
                "[DeptDescription] = N'" + DeptDescription + "', " +
                "[ModifyTime] = " + DBC.ChangeTimeZone() + ", " +
                "[ModifyUser] = " + SqlVal2(Request.Cookies["account"]) +
                "WHERE [DeptNo] = '" + DeptNo + "'";

                var check = _DB_Execute(sqlStr);

                //修改是否成功
                if (check != 1)
                {
                    resMsg = "修改失敗，若持續發生此問題，請與我們聯繫。";
                }
            }

            //Return Msg
            return resMsg;
        }

        /// <summary>
        /// 刪除國家科系
        /// </summary>
        /// <param name="CountryNo"></param>
        /// <param name="DeptNo"></param>
        /// <returns></returns>
        [HttpPost]
        public string DeleteUnivDepartment(string DeptNo)
        {
            //Return Msg
            string resMsg = "刪除成功！";

            // admin check
            if (getUserStatusNo() != "1")
            {
                resMsg = "刪除失敗，權限錯誤！";
            }
            else
            {
                string sqlStr = string.Format("DELETE [dbo].[Department] " +
                "WHERE [DeptNo] = {0}", SqlVal2(DeptNo));

                var check = _DB_Execute(sqlStr);

                //刪除是否成功
                if (check != 1)
                {
                    resMsg = "刪除失敗，若持續發生此問題，請與我們聯繫。";
                }
            }

            //Return Msg
            return resMsg;
        }
    }
}
