using iWebSite_ComeIndus.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Areas.Content.Models;
using System.Data;

namespace iWebSite_ComeIndus.Areas.Content.Controllers
{
    [Area(areaName: "Content")]
    public class UnivGraduationController : _BaseController
    {
        public IActionResult Index()
        {
            return View("ShowUnivGraduation");
        }

        [HttpGet()]
        //public List<UnivGraduationModel> ShowUnivGraduation(UnivGraduationModel Model)
        public List<UnivGraduationModel> ShowUnivGraduation(string country)
        {
            // 假DB
            Dictionary<string, string> countrys = new Dictionary<string, string>()
            {
                {"台灣", "TW"},
                {"中國", "CN"}
            };

            //假年份
            string year = "2017";

            //------------------------------------------------------------

            var countryID = countrys[country];

            var sqlStr = string.Format(
                "select GraduationNumber, DeptName " +
                "from [dbo].[Graduation] " +
                "inner join [dbo].[Department] " +
                "on [dbo].[Graduation].DeptNo = [dbo].[Department].DeptNo " +
                "where GraduationNo like '{0}%'", countryID);
            
            var data = _DB_GetData(sqlStr);

            List<UnivGraduationModel> graduationData = new List<UnivGraduationModel>();

            foreach (DataRow row in data.Rows)
            {
                UnivGraduationModel model = new UnivGraduationModel();

                model.GraduationNumber = row.ItemArray.GetValue(0).ToString();
                model.Department = row.ItemArray.GetValue(1).ToString();
                model.color = "#abc";
                graduationData.Add(model);
            }

            //string jsondata = JsonConvert.SerializeObject(graduationData);
            //return View("ShowUnivGraduation", jsondata);
            return graduationData;
        }

       
    }
}
