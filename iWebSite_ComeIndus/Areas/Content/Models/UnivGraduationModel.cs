using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Content.Models
{
    public class UnivGraduationModel
    {
        public string Country { get; set; }
        public string Year { get; set; }
        public string DeptName { get; set; }
        public string CountryDeptNo { get; set; }
        public string GraduationNumber { get; set; }
    }

    public class CountryDeptModel
    {
        public string CountryNo { get; set; }
        public string CountryName { get; set; }
        public List<string> CountryDeptNo { get; set; }
        public List<string> DeptName { get; set; }
        public List<int> GraduationNumber { get; set; }
        // default value
        public CountryDeptModel()
        {
            DeptName = new List<string>();
            GraduationNumber = new List<int>();
        }
    }

    public class TimeModel
    {
        public string Year { get; set; }
    }

    public class MgtGradModel
    {
        public List<CountryDeptModel> CountryDepts { get; set; }
        public List<TimeModel> Years { get; set; }
    }

   
   
    
} 

