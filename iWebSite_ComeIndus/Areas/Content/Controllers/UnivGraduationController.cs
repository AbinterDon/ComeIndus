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
            //return View("ShowUnivGraduation");
            return View("GraduationFromDiffCountry");
        }

        [HttpGet()]
        //public List<UnivGraduationModel> ShowUnivGraduation(UnivGraduationModel Model)
        public Dictionary<string, CountryGradModel> GraduationFromDiffCountry(string year)
        {
            var sqlStr = string.Format("select [CountryNo], [CountryName] from Countries");
            var data = _DB_GetData(sqlStr);

            Dictionary<string, CountryGradModel> graduationData = new Dictionary<string, CountryGradModel>();

            foreach (DataRow row in data.Rows)
            {
                string countryNo = row.ItemArray.GetValue(0).ToString();
                string countryName = row.ItemArray.GetValue(1).ToString();

                sqlStr = string.Format(
                "select [DeptName], [GraduationNumber], [GraduationYear] " +
                "from[dbo].[Department] as a " +
                "inner join( " +
                "select[CountryDeptNo],[CountryNo], [DeptNo] " +
                "from[dbo].[CountryDepartment] " +
                "where CountryNo = {0} " +
                ") as b " +
                "on b.DeptNo = a.DeptNo " +
                "inner join( " +
                "select * " +
                "from[dbo].[Graduation] " +
                "where GraduationYear = {1} " +
                ") as c " +
                "on b.CountryDeptNo = c.CountryDeptNo", countryNo, year);

                var countryGradData = _DB_GetData(sqlStr);
                CountryGradModel model = new CountryGradModel();
                model.DeptName = new List<string>();
                model.GraduationNumber = new List<int>();

                foreach (DataRow gradRow in countryGradData.Rows)
                {
                    model.DeptName.Add(gradRow.ItemArray.GetValue(0).ToString());
                    model.GraduationNumber.Add((int)gradRow.ItemArray.GetValue(1));
                }

                graduationData[countryName] = model;
            }

            return graduationData; 
        }

        /// <summary>
        /// 下載
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Download(string year = "2011")
        {
            //~/Content/UnivGraduation/Download?year=2011
            return RedirectToAction("UnivGraduation", "excel", new UnivGraduationModel() { Year = year });
        }
    }
}
