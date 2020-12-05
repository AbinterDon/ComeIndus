using iWebSite_ComeIndus.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace iWebSite_ComeIndus.Controllers
{
    public class _BaseController : Controller
    {
        /// <summary>
        /// Config共用
        /// </summary>
        public IConfiguration Config;

        /// <summary>
        /// 資料庫共用類別
        /// </summary>
        public DBClass DBC { get; set; }

        /// <summary>
        /// BaseController建構元
        /// </summary>
        public _BaseController()
        {
            //DB共用類別 實體化
            DBClass DBC = new DBClass();

            //讀取AppSetting
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json");

            //Build
            Config = builder.Build();
        }

        /// <summary>
        /// 取得AppSetting參數
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string GetConfig(string parameter)
        {
            //Return
            return Config.GetValue<string>(parameter);
        }
    }
}
