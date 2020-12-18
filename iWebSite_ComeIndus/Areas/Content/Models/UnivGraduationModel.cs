using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Content.Models
{
    /*
    public class UnivGraduationModel
    {
        public string Country { get; set; }
        public string GraduationNumber { get; set; }
        public string Department { get; set; }
        public string Year { get; set; }
    }
    */

    public class CountryGradModel
    {
        public List<string> DeptName { get; set; }
        public List<int> GraduationNumber { get; set; }
    }
} 

