using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Content.Models
{
    public class DeptModel
    {
        public string DeptNo { get; set; }
        public string DeptName { get; set; }
        public string DeptDescription { get; set; }
        public string CreateTime { get; set; }
        public string CreateUser { get; set; }
    }

    public class MgtDeptModel
    {
        public string CountryNo { get; set; }
        public string CountryName { get; set; }
        public List<DeptModel> Depts { get; set; }
    }
}
