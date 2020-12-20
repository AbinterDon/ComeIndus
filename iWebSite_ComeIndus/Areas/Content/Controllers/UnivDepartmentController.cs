using iWebSite_ComeIndus.Areas.Content.Models;
using iWebSite_ComeIndus.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Content.Controllers
{
    [Area(areaName: "Content")]
    public class UnivDepartmentController : _BaseController
    {
        public IActionResult Index()
        {
            return ShowUnivDepartment();
        }

        [HttpPost]
        public string InsertUnivDepartment(string DeptName, string DeptDescription)
        {
            //尚未加入管理員身分檢查

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
                    ")VALUES(" +
                        "{0}," +
                        "{1}," +
                        "{2}," +
                        "{3}",
                        SqlVal2(DeptName),
                        SqlVal2(DeptDescription),
                        "getDate()",
                        "getDate()" + ")"
                    );

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

     
            return resMsg;
           
            //return ShowUnivDepartment();
        }

        public ActionResult ShowUnivDepartment() 
        {
            var sqlStr = string.Format("select * from [dbo].[Department]");

            var data = _DB_GetData(sqlStr);
            return View("UnivDepartment", data);
        }

        public ActionResult UpdateUnivDepartment(string DeptNo, string DeptName, string DeptDescription)
        {
            string resMsg = "";
            string sqlStr = "UPDATE [dbo].[Department] " +
                "SET [DeptName] = '" + DeptName + "', " +
                "[DeptDescription] = '" + DeptDescription + "', " +
                "[ModifyTime] = getDate() " +
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
