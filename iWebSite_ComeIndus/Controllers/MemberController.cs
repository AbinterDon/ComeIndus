using Microsoft.AspNetCore.Mvc;
using iWebSite_ComeIndus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Controllers
{
    public class MemberController : _BaseController
    {
        

        public ActionResult ModifyInfor()
        {
            return View();
        }

        /*public ActionResult Save(Member Model)
        {
            var sqlStr = string.Format("select Account,Username,Password,MailCheck,PwdChangeCheck from [dbo].[Member] where Account = {0}", SqlVal2(Model.Account));

            //SQL Check
            var data = _DB_GetData(sqlStr);




            return View(Model);

        }*/
    }
}
