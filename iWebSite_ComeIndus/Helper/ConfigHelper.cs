using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Helper;

namespace iWebSite_ComeIndus.Helper
{
    public class ConfigHelper
    {
        public static class AppSettings
        {
            /// <summary>
            /// 目前用不到此Class
            /// </summary>
            public enum AppSettingsClass {
                /// <summary>
                /// 系統別
                /// </summary>
                System,
                /// <summary>
                /// 通用
                /// </summary>
                Common,
            }

            /// <summary>
            /// 參數對比
            /// </summary>
            public enum AppSettingsKey {
                #region 系統別
                /// <summary>
                /// 系統版本
                /// </summary>
                Core_SystemVersion,
                /// <summary>
                /// 首頁最新消息數量
                /// </summary>
                IndexNewsCount,
                #endregion 

                #region 核心 Core
                /// <summary>
                /// DB 連線字串
                /// </summary>
                Core_ConnectionStrings,
                #endregion 
            }

            /// <summary>
            /// 系統版本號
            /// </summary>
            public static string SystemVersion { get { return ConfigHelperMethods.GetConfig(AppSettingsKey.Core_SystemVersion); } }

            /// <summary>
            /// 首頁最新消息數量
            /// </summary>
            public static string IndexNewsCount { get { return ConfigHelperMethods.GetConfig(AppSettingsKey.IndexNewsCount); } }

            /// <summary>
            /// DB 連線字串
            /// </summary>
            public static string ConnectionStrings { get { return ConfigHelperMethods.GetConfig(AppSettingsKey.Core_ConnectionStrings); } }


        }
    }
}
