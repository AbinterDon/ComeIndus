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

        [HttpGet]
        public ActionResult InsertUnivDepartment()
        {
            return View("InsertUnivDepartment");
        }

        [HttpPost]
        public ActionResult InsertUnivDepartment(UnivDepartmentModel Model)
        {
            //尚未加入管理員身分檢查

            string resMsg = "";
            if (Model.DeptName == null || Model.DeptName.Length > 50)
            {
                resMsg = "未輸入內容!!";
            }
            // 長度限制
            else if (Model.DeptDescription != null && Model.DeptDescription.Length > 200)
            {
                resMsg = "內容超出長度限制!!";
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
                        SqlVal2(Model.DeptName),
                        SqlVal2(Model.DeptDescription),
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

            ViewData["UniDept-result"] = resMsg;

            return View();
        }

        public ActionResult ShowUnivDepartment() 
        {
            var sqlStr = string.Format("select * from [dbo].[Department]");

            var data = _DB_GetData(sqlStr);
            return View("ShowUnivDepartment", data);
        }
    }
}
