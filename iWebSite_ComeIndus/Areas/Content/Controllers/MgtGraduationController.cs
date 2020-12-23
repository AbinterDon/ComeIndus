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
    public class MgtGraduationController : _BaseController
    {
        public IActionResult Index()
        {
            // admin check
            if(getUserAuthority() != "1")
            {
                return Redirect("~/Home/Error");
            }

            MgtGradModel mgtModel = new MgtGradModel();

            mgtModel.Years = getTime();
            mgtModel.CountryDepts = new List<CountryDeptModel>();


            var sqlStr = string.Format("select [CountryNo], [CountryName] from Countries");
            var data = _DB_GetData(sqlStr);

            foreach (DataRow row in data.Rows)
            {
                CountryDeptModel country = new CountryDeptModel();
                country.CountryNo = row.ItemArray.GetValue(0).ToString();
                country.CountryName = row.ItemArray.GetValue(1).ToString();
                country.CountryDeptNo = new List<string>();
                country.DeptName = new List<string>();

                // get dept
                sqlStr = string.Format(
                    "SELECT [CountryDeptNo], [DeptName] " +
                    "FROM[dbo].[CountryDepartment] as countryDept " +
                    "inner join[dbo].[Department] as dept " +
                    "on countryDept.DeptNo = dept.DeptNo " +
                    "where countryNo = {0}; ", country.CountryNo);

                var deptData = _DB_GetData(sqlStr);
                foreach (DataRow deptRow in deptData.Rows)
                {
                    country.CountryDeptNo.Add(deptRow.ItemArray.GetValue(0).ToString());
                    country.DeptName.Add(deptRow.ItemArray.GetValue(1).ToString());
                }

                mgtModel.CountryDepts.Add(country);
            }
            return View("MgtGradData", mgtModel);
        }

        
        public List<UnivGraduationModel> ShowGrad(string yearStart, string yearEnd, string countryNo, string countryDeptNo)
        {
            // admin check
            if (getUserAuthority() != "1")
            {
                return null;
            }

            string condition =  "";

            if (countryDeptNo != "*")
            {
                condition = string.Format("where countryDept.CountryDeptNo = {0}", SqlVal2(countryDeptNo));
            }
            else
            {
                condition = string.Format("where CountryNo = {0} ", SqlVal2(countryNo));
            }

            if(yearStart != "*")
            {
                condition += string.Format("and GraduationYear >= {0} and GraduationYear <= {1}", SqlVal2(yearStart), SqlVal2(yearEnd));   
            }

            List<UnivGraduationModel> graduationData = new List<UnivGraduationModel>();

            var sqlStr = string.Format(
                    "SELECT [GraduationYear], [DeptName], [GraduationNumber], countryDept.[CountryDeptNo]" +
                    "FROM[dbo].[Graduation] as grad " +
                    "inner join[dbo].[CountryDepartment] as countryDept " +
                    "on grad.CountryDeptNo = countryDept.CountryDeptNo " +
                    "inner join[dbo].[Department] as dept " +
                    "on countryDept.DeptNo = dept.DeptNo " +
                    "{0} order by GraduationYear", condition);

            var data = _DB_GetData(sqlStr);

            foreach (DataRow row in data.Rows)
            {
                UnivGraduationModel model = new UnivGraduationModel();
                model.Year = row.ItemArray.GetValue(0).ToString();
                model.DeptName = row.ItemArray.GetValue(1).ToString();
                model.GraduationNumber = row.ItemArray.GetValue(2).ToString();
                model.CountryDeptNo = row.ItemArray.GetValue(3).ToString();

                graduationData.Add(model);
            }

            return graduationData;
        }

        public bool InsertGrad(string year, string countryDeptNo, string gradNum)
        {
            // admin check
            if (getUserAuthority() != "1")
            {
                return false;
            }

            int gradNumInt;

            if (!int.TryParse(gradNum, out gradNumInt) || gradNumInt < 0)
            {
                return false;
            }
                var sqlStr = string.Format("INSERT INTO [dbo].[Graduation] " +
                "([CountryDeptNo] " +
                ",[GraduationYear] " +
                ",[GraduationNumber] " +
                ",[CreateTime] " +
                ",[ModifyTime] " +
                ",[CreateUser]) " +
                "VALUES( " +
                " {0} " +
                ",{1} " +
                ",{2} " +
                ",getDate() " +
                ",getDate() " +
                ",{3}) ", SqlVal2(countryDeptNo), SqlVal2(year), SqlVal2(gradNum), SqlVal2(Request.Cookies["account"]));
            
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

        public bool DeleteGrad(string year, string countryDeptNo)
        {
            // admin check
            if (getUserAuthority() != "1")
            {
                return false;
            }

            var sqlStr = string.Format("DELETE FROM [dbo].[Graduation]" +
                "WHERE CountryDeptNo={0} AND GraduationYear={1} ", 
                SqlVal2(countryDeptNo), SqlVal2(year));

            var check = _DB_Execute(sqlStr);

            //是否成功
            if (check == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateGrad(string year, string countryDeptNo, string graduationNumber)
        {
            // admin check
            if (getUserAuthority() != "1")
            {
                return false;
            }

            int gradNumInt;

            if (!int.TryParse(graduationNumber, out gradNumInt) || gradNumInt < 0)
            {
                return false;
            }

            var sqlStr = string.Format("UPDATE [dbo].[Graduation] " +
                "SET [GraduationNumber] = {0} " +
                ",[ModifyTime] = getDate() " +
                ",[CreateUser] = {1} " +
                "WHERE " +
                "CountryDeptNo={2} AND GraduationYear={3}",
                SqlVal2(graduationNumber), 
                SqlVal2(Request.Cookies["account"]), 
                SqlVal2(countryDeptNo), 
                SqlVal2(year));

            var check = _DB_Execute(sqlStr);

            //是否成功
            if (check == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<TimeModel> getTime()
        {
            var sqlStr = string.Format("select [GraduationYear] from GraduationYear");
            var data = _DB_GetData(sqlStr);

            List<TimeModel> years = new List<TimeModel>();
            foreach (DataRow Row in data.Rows)
            {
                TimeModel model = new TimeModel();
                model.Year = Row.ItemArray.GetValue(0).ToString();

                years.Add(model);
            }

            return years;
        }
    }
}
