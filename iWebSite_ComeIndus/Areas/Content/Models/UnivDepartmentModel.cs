using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Areas.Content.Models
{
    public class UnivDepartmentModel
    {
        public string DeptName { get; set; }
        public string DeptDescription { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
