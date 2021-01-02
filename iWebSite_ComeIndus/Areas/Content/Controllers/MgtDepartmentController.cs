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

        [HttpPost]
        public string InsertUnivDepartment(string CountryNo, string DeptName, string DeptDescription)
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return "fail";
            }

            string resMsg = "";
            if (DeptName == null || DeptName.Length > 50)
            {
                resMsg = "未輸入科系或長度超過限制!!";
            }
            // 長度限制
            else if (DeptDescription != null && DeptDescription.Length > 200)
            {
                resMsg = "敘述超出長度限制!!";
            }
            else
            {
                //SQL Insert
                var sqlStr = string.Format(
                    @"INSERT INTO [dbo].[Department](" +
                        "[DeptName]," +
                        "[DeptDescription]," +
                        "[CreateTime]," +
                        "[ModifyTime]" +
                    ") " +
                    "Output inserted.DeptNo " +
                    "VALUES(" +
                        "{0}," +
                        "{1}," +
                        "{2}," +
                        "{3}",
                        SqlVal2(DeptName),
                        SqlVal2(DeptDescription),
                        DBC.ChangeTimeZone(),
                        DBC.ChangeTimeZone() + ")"
                    );

                
                var output = _DB_GetData(sqlStr);
                if(output.Rows.Count == 1)
                {
                    // 取出自動生成的PK
                    var insertedID = output.Rows[0].ItemArray.GetValue(0).ToString();
                    
                    // 新增國家與科系的關聯
                    sqlStr = string.Format("INSERT INTO [dbo].[CountryDepartment] (" +
                        "[CountryNo], " +
                        "[DeptNo], " +
                        "[CreateTime], " +
                        "[ModifyTime]) " +
                        "VALUES " +
                        "({0}, " +
                        " {1}, " +
                        "{2}, " +
                        "{3})", 
                        CountryNo, insertedID, DBC.ChangeTimeZone(), DBC.ChangeTimeZone());

                    var check = _DB_Execute(sqlStr);

                    //新增是否成功
                    if (check == 1)
                    {
                        resMsg = "success";
                    }
                    else
                    {
                        resMsg = "fail";
                    }
                }
                else
                {
                    return "fail";
                }
                
            }
            return resMsg;
           
        }

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

                sqlStr = string.Format("SELECT dept.DeptNo, DeptName, DeptDescription, dept.CreateTime, dept.ModifyTime, CountryNo " +
                    "FROM[dbo].[Department] as dept " +
                    "inner join( " +
                    "select DeptNo, CountryNo from[dbo].[CountryDepartment]) as countryDept " +
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
                        ModifyTime = row.ItemArray.GetValue(4).ToString()
                    });
                }

                Models.Add(model);
            }

                return View("MgtDepartment", Models);
        }

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
                "[ModifyTime] = " + DBC.ChangeTimeZone() +
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

        public ActionResult DeleteUnivDepartment(string DeptNo)
        {
            // admin check
            if (getUserStatusNo() != "1")
            {
                return null;
            }

            string resMsg = "";
            string sqlStr = "DELETE [dbo].[Department] " +
                "WHERE [DeptNo] = '" + DeptNo + "'";

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
