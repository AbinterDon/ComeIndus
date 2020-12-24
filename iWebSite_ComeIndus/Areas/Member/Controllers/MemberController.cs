using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Controllers;

namespace iWebSite_ComeIndus.Areas.Member.Controllers
{
    [Area(areaName: "Member")]
    public class MemberController : Controller
    {
       

        

        [HttpGet]
        public ActionResult DeleteMember()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InsertMember()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ModifyMember()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ShowMember()
        {
            return View();
        }
    }
}
