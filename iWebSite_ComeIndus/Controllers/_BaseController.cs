using iWebSite_ComeIndus.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public static List<CacheData> cacheDatas { get; set; } = new List<CacheData>();

        /// <summary>
        /// BaseController建構元
        /// </summary>
        public _BaseController()
        {
            //DB共用類別 實體化
            DBC = new DBClass();
        }

        /// <summary>
        /// 
        /// </summary>
        public class CacheData
        {
            public string strSql { get; set; }
            /// <summary>
            /// FOR _DB_GetData USE
            /// </summary>
            public DataTable dt { get; set; }

            /// <summary>
            /// FOR _DB_GetStr USE
            /// </summary>
            public string resultStr { get; set; }

            public DateTime createTime { get; set; }
        }

        /// <summary>
        /// SQL值功能，空值回傳""，有單引號「'」改成2個單引號「''」
        /// </summary>
        /// <param name="strSqlVal"></param>
        /// <returns></returns>
        public static string SqlVal(object strSqlVal)
        {
            if (strSqlVal != null)
                return string.IsNullOrEmpty(strSqlVal.ToString()) ? "" : strSqlVal.ToString().Replace("'", "''");
            else
                return "";
        }

        /// <summary>
        /// SQL值功能，空值回傳"NULL"，有單引號「'」改成2個單引號「''」，並以單引號包括「'{0}'」
        /// </summary>
        /// <param name="strSqlVal"></param>
        /// <returns></returns>
        public static string SqlVal2(object strSqlVal)
        {
            string retVal = SqlVal(strSqlVal);
            if (retVal == "")
            {
                retVal = "NULL";
            }
            else
            {
                if (strSqlVal is DateTime)
                {
                    retVal = string.Format("'{0}'", DateTime.Parse(retVal).ToString("yyyy/MM/dd"));
                }
                else if (strSqlVal is String)
                {
                    retVal = string.Format("N'{0}'", retVal);
                }
                else
                {
                    retVal = string.Format("'{0}'", retVal);
                }
            }
            return retVal;
        }

        /// <summary>
        /// (DB)依SQL指令取得資料結果(回傳DataTable)
        /// </summary>
        /// <param name="strSql">SQL</param>
        /// <returns></returns>
        public DataTable _DB_GetData(string strSql, bool cache = false, int cacheSeconds = 60)
        {
            if (cache && cacheSeconds > 0)
            {
                cacheDatas.RemoveAll(x => x.createTime < DateTime.Now.AddSeconds(cacheSeconds * -1));
                var result = cacheDatas.Find(x => x.strSql == strSql);
                if (result == null)
                {
                    var dt = DBC.GetSQLCmd_DT(strSql);
                    cacheDatas.Add(new CacheData()
                    {
                        strSql = strSql,
                        dt = dt,
                        createTime = DateTime.Now
                    });
                    return dt;
                }
                else
                {
                    return result.dt;
                }
            }
            else
            {
                return DBC.GetSQLCmd_DT(strSql);
            }
        }

        /// <summary>
        /// (DB)依SQL指令取得資料結果(一個字串)
        /// </summary>
        public string _DB_GetStr(string strSql, bool cache = false, int cacheSeconds = 60)
        {
            if (cache && cacheSeconds > 0)
            {
                cacheDatas.RemoveAll(x => x.createTime < DateTime.Now.AddSeconds(cacheSeconds * -1));
                var result = cacheDatas.Find(x => x.strSql == strSql);
                if (result == null)
                {
                    var str = DBC.GetSQLCmd_Str(strSql);
                    cacheDatas.Add(new CacheData()
                    {
                        strSql = strSql,
                        resultStr = str,
                        createTime = DateTime.Now
                    });
                    return str;
                }
                else
                {
                    return result.resultStr;
                }
            }
            else
            {
                return DBC.GetSQLCmd_Str(strSql);
            }
        }

        /// <summary>
        /// (DB)依SQL指令取得資料結果(字串陣列[第一欄])
        /// </summary>
        public List<string> _DB_GetStrArr(string strSql)
        {
            return _DB_GetStrArr(strSql, 0);
        }

        /// <summary>
        /// (DB)依SQL指令取得資料結果(字串陣列[第n欄])
        /// </summary>
        /// <param name="strSql">SQL</param>
        /// <param name="n">第n欄(從0開始)</param>
        public List<string> _DB_GetStrArr(string strSql, int n)
        {
            return DBC.GetSQLCmd_DT(strSql)
                .AsEnumerable()
                .Select(row => row.Field<object>(n).ToString()).ToList();
        }

        /// <summary>
        /// 依SQL指令執行
        /// </summary>
        public int _DB_Execute(string strSql)
        {
            try
            {
                return DBC.ExecCmd(strSql);
            }
            catch (Exception e)
            {
                //LogInfo("_DB_Execute", e + ":" + strSql);
                throw e;
                //return 0; (再做參數控制出庫/DEV環境)
            }
        }

        /// <summary>
        /// 依SQL指令執行
        /// </summary>
        public int _DB_Execute_Async(string strSql)
        {
            return DBC.ExecCmdAsync(strSql);
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string SHA256_Encryption(string str)//SHA256
        {
            if(!string.IsNullOrEmpty(str))
            {
                //實體化SHA256類別
                SHA256CryptoServiceProvider SHA256 = new SHA256CryptoServiceProvider();

                //將字串編碼成 UTF8 位元組陣列
                var bytes = System.Text.Encoding.UTF8.GetBytes(str);

                //加密(SHA256) 並且將結果放到ans變數內
                string Ans = BitConverter.ToString(SHA256.ComputeHash(bytes));

                //Return
                return Ans;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// SHA256比對
        /// </summary>
        /// <param name="OriginalKey"></param>
        /// <param name="CheckKey"></param>
        /// <returns></returns>
        public bool SHA256_Compare(string OriginalKey, string CheckKey)
        {
            //比較兩組KEY是否一樣
            if (OriginalKey == SHA256_Encryption(CheckKey))
            {
                //一樣的話True
                return true;
            }
            else 
            {
                //不一樣的話False
                return false;
            }
        }
    }
}
