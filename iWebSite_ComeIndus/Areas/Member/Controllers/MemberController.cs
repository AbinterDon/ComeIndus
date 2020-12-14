using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Member.Controllers
{
    public class MemberController : Controller
    {
        [Area(areaName: "Member")]

        public ActionResult Index()
        {
            return View("ShowMember");
        }

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
