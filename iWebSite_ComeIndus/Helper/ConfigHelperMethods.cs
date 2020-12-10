using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static iWebSite_ComeIndus.Helper.ConfigHelper.AppSettings;

namespace iWebSite_ComeIndus.Helper
{
    public class ConfigHelperMethods 
    {
        public static IConfiguration Config;

        /// <summary>
        /// 建構元
        /// </summary>
        public ConfigHelperMethods()
        {
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
        public static string GetConfig(AppSettingsKey parameter)
        {
            //將參數轉為字串
            string key = parameter.ToString();

            //Return
            return Config.GetValue<string>(key);
        }
    }
}
