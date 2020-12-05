using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Helper;

namespace iWebSite_ComeIndus.Extension
{
    /// <summary>
    /// CommonClass建構元
    /// </summary>
    public class CommonClass{ }

    /// <summary>
    /// DB Management
    /// </summary>
    public class DBClass {
        /// <summary>
        /// 建構元
        /// </summary>
        public DBClass()
        {
            DbConn = new System.Data.SqlClient.SqlConnection(ConnectionString);
        }

        /// <summary>
        /// DB Connection
        /// </summary>
        public SqlConnection DbConn { get; set; }

        /// <summary>
        /// DB Adpt
        /// </summary>
        protected SqlDataAdapter DbAdpt { get; set; }

        /// <summary>
        /// 連線字串
        /// </summary>
        private string F_ConnStr
        {
            get
            {
                //DB連線字串
                return ConfigHelper.AppSettings.ConnectionStrings;
            }
        }

        /// <summary>
        /// 給定 ConnectionString
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return F_ConnStr;
            }
        }
    }
}
