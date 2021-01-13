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
    public class MgtCountryDeptController : _BaseController
    {
        public IActionResult Index()
        {
            return ShowUnivDepartment();
        }

        /// <summary>
        /// 新增國家科系
        /// </summary>
        /// <param name="CountryNo"></param>
        /// <param name="DeptNo"></param>
        /// <returns></returns>
        private bool InsertCountryDept(string CountryNo, string DeptNo)
        {
            //檢查是否有重複的了
            if(CheckInsertCountryDept(CountryNo, DeptNo))
            {
                return false;
            }
            else
            {
                // 新增國家與科系的關聯
                var sqlStr = string.Format("INSERT INTO [dbo].[CountryDepartment] (" +
                    "[CountryNo], " +
                    "[DeptNo], " +
                    "[CreateTime], " +
                    "[ModifyTime], " +
                    "[CreateUser] " +
                    ") " +
                    "VALUES " +
                    "({0}, " +
                    " {1}, " +
                    " {2}, " +
                    " {3}, " +
                    " {4})",
                    CountryNo, DeptNo, DBC.ChangeTimeZone(), DBC.ChangeTimeZone(), SqlVal2(Request.Cookies["account"]));

                var check = _DB_Execute(sqlStr);

                //新增是否成功
                if (check == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 檢查是否有重複的
        /// </summary>
        /// <param name="CountryNo"></param>
        /// <param name="DeptNo"></param>
        /// <returns></returns>
        private bool CheckInsertCountryDept(string CountryNo, string DeptNo)
        {
            // 新增國家與科系的關聯
            var sqlStr = string.Format("Select 1 from [dbo].[CountryDepartment] where CountryNo = {0} and DeptNo ={1}",
                SqlVal2(CountryNo), SqlVal2(DeptNo));

            var check = _DB_GetData(sqlStr);

            //新增是否成功
            if (check.Rows.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
        public string InsertUnivDepartment(string CountryNo, string DeptNo, string DeptName, string DeptDescription)
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return "新增失敗，權限錯誤。";
            }

            string resMsg = "";

            //檢查是否有重複的了
            if (CheckInsertCountryDept(CountryNo, DeptNo))
            {
                resMsg = "新增失敗，該筆資料已經存在。";
            }
            else
            {
                var check = InsertCountryDept(CountryNo, DeptNo);

                if (check)
                {
                    resMsg = "新增成功!!";
                }
                else
                {
                    resMsg = "新增失敗，可能已經存在。";
                }
            }
            return resMsg;
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

            List<MgtDeptModel> Models = new List<MgtDeptModel>();
            
            // country
            var sqlStr = string.Format("Select CountryNo, CountryName From Countries");
            var countryData = _DB_GetData(sqlStr);

            foreach (DataRow countryRow in countryData.Rows)
            {
                MgtDeptModel model = new MgtDeptModel();
                model.CountryNo = countryRow.ItemArray.GetValue(0).ToString();
                model.CountryName = countryRow.ItemArray.GetValue(1).ToString();
                model.Depts = new List<DeptModel>();

                sqlStr = string.Format("SELECT dept.DeptNo, DeptName, DeptDescription, countryDept.CreateTime, countryDept.CreateUser, CountryNo " +
                    "FROM[dbo].[Department] as dept " +
                    "inner join( " +
                    "select DeptNo, CountryNo, CreateTime, CreateUser from[dbo].[CountryDepartment]) as countryDept " +
                    "on dept.DeptNo = countryDept.DeptNo " +
                    "where CountryNo = {0}", model.CountryNo);

                var data = _DB_GetData(sqlStr);
                foreach (DataRow row in data.Rows)
                {
                    model.Depts.Add(new DeptModel()
                    {
                        DeptNo = row.ItemArray.GetValue(0).ToString(),
                        DeptName = row.ItemArray.GetValue(1).ToString(),
                        DeptDescription = row.ItemArray.GetValue(2).ToString(),
                        CreateTime = row.ItemArray.GetValue(3).ToString(),
                        CreateUser = row.ItemArray.GetValue(4).ToString()
                    });
                }

                Models.Add(model);
            }

            return View("MgtCountryDept", Models);
        }

        /// <summary>
        /// 修改科系
        /// </summary>
        /// <param name="DeptNo"></param>
        /// <param name="DeptName"></param>
        /// <param name="DeptDescription"></param>
        /// <returns></returns>
        public ActionResult UpdateUnivDepartment(string DeptNo, string DeptName, string DeptDescription)
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return null;
            }

            string resMsg = "";
            string sqlStr = "UPDATE [dbo].[Department] " +
                "SET [DeptName] = N'" + DeptName + "', " +
                "[DeptDescription] = N'" + DeptDescription + "', " +
                "[ModifyTime] = " + DBC.ChangeTimeZone() + ", " +
                "[ModifyUser] = " + SqlVal2(Request.Cookies["account"]) +
                "WHERE [DeptNo] = '" + DeptNo + "'";

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
        /// 刪除國家科系
        /// </summary>
        /// <param name="CountryNo"></param>
        /// <param name="DeptNo"></param>
        /// <returns></returns>
        public string DeleteUnivDepartment(string CountryNo, string DeptNo)
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return "刪除失敗，權限錯誤。";
            }

            string resMsg = "";

            if (CheckDeleteCountryDept(CountryNo, DeptNo))
            {
                resMsg = "刪除失敗，此國家科系已綁定於畢業人數。";
            }
            else
            {
                string sqlStr = string.Format("DELETE FROM [dbo].[CountryDepartment] WHERE CountryNo={0} AND DeptNo={1}",
                SqlVal2(CountryNo), SqlVal2(DeptNo));

                var check = _DB_Execute(sqlStr);

                //刪除是否成功
                if (check == 1)
                {
                    resMsg = "刪除成功";
                }
                else
                {
                    resMsg = "刪除失敗，若持續發生此問題，請聯絡我們。";
                }
            }

            return resMsg;
        }

        /// <summary>
        /// 刪除前檢查是否有外來鍵到別人那
        /// </summary>
        /// <param name="CountryNo"></param>
        /// <param name="DeptNo"></param>
        /// <returns></returns>
        private bool CheckDeleteCountryDept(string CountryNo, string DeptNo)
        {
            // 新增國家與科系的關聯
            var sqlStr = string.Format(@"
                select 1 from dbo.Graduation where CountryDeptNo = 
                (Select TOP(1) [CountryDeptNo] from [dbo].[CountryDepartment] where CountryNo = {0} and DeptNo ={1})",
                SqlVal2(CountryNo), SqlVal2(DeptNo));

            var check = _DB_GetData(sqlStr);

            //新增是否成功
            if (check.Rows.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
