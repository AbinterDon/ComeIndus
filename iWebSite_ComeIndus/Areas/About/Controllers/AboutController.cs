using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Controllers;

namespace iWebSite_ComeIndus.Areas.Aboout.Controllers
{
    [Area(areaName: "About")]
    public class AboutController : _BaseController
    {

        public IActionResult Index()
        {
            return View("About");
        }

    }
}
