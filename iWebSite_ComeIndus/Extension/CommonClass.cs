using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using iWebSite_ComeIndus.Helper;
using System.Data;

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
        /// Trans
        /// </summary>
        public SqlTransaction _trans = null;

        /// <summary>
        /// 有transaction=true
        /// </summary>
        bool _bHasTrans { get { return (_trans != null); } }

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

        /// <summary>
        /// DB連線如果為空值則從新取得一次
        /// </summary>
        public void CheckDBConnection()
        {
            if (DbConn.ConnectionString == "")
            {
                DbConn = new System.Data.SqlClient.SqlConnection(ConnectionString);
            }
        }

        /// <summary>
        /// 依SQL指令取得資料結果(回傳Object)
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public object GetSQLCmd_Obj(string sSQL, string FiledName)
        {
            object sRetV = new object();

            DataTable DT_1 = new DataTable("DT1");

            CheckDBConnection();

            DbAdpt = new System.Data.SqlClient.SqlDataAdapter(sSQL, DbConn); //不用置換Connection
            if (_bHasTrans) DbAdpt.SelectCommand.Transaction = _trans;

            DbAdpt.Fill(DT_1);
            if (DT_1.Rows.Count > 0)
            {
                sRetV = DT_1.Rows[0][FiledName];
            }
            return sRetV;
        }

        /// <summary>
        /// 依SQL指令取得資料結果(回傳DataTable)
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public DataTable GetSQLCmd_DT(string sSQL)
        {

            DataTable DT_1 = new DataTable("DT1");

            CheckDBConnection();

            DbAdpt = new System.Data.SqlClient.SqlDataAdapter(sSQL, DbConn); //不用置換Connection
            if (_bHasTrans) DbAdpt.SelectCommand.Transaction = _trans;
            DbAdpt.SelectCommand.CommandTimeout = 600;
            try
            {
                DbAdpt.Fill(DT_1);
            }
            catch (Exception ex)
            {
                throw new Exception("SQL指令：" + sSQL + "\r\n" + ex.Message);
            }

            return DT_1;
        }

        /// <summary>
        /// 依SQL指令取得資料結果(一個字串)
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public String GetSQLCmd_Str(string sSQL)
        {

            string sRetV = "";

            CheckDBConnection();

            if (_bHasTrans)
            {

                System.Data.SqlClient.SqlCommand Cmd1 = new System.Data.SqlClient.SqlCommand();
                Cmd1.CommandTimeout = 30000;
                Cmd1.Connection = DbConn;
                Cmd1.Transaction = _trans;
                Cmd1.CommandText = sSQL;

                try
                {
                    sRetV = Cmd1.ExecuteScalar() + "";
                }
                catch (Exception ex)
                {
                    _trans.Rollback();
                    throw new Exception("SQL指令執行失敗:" + sSQL + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }
            }
            else
            {

                System.Data.SqlClient.SqlCommand Cmd1 = new System.Data.SqlClient.SqlCommand();
                Cmd1.CommandTimeout = 30000;
                Cmd1.Connection = DbConn;
                Cmd1.CommandText = sSQL;

                DbConn.Open();
                try
                {
                    sRetV = Cmd1.ExecuteScalar() + "";
                }
                catch (Exception ex)
                {
                    DbConn.Close();
                    throw new Exception("SQL指令執行失敗:" + sSQL + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }
                DbConn.Close();
            }

            return sRetV;
        }

        /// <summary>
        /// 依SQL指令取得資料結果(一個字串),依指定欄位回傳值
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="FiledName"></param>
        /// <returns></returns>
        public String GetSQLCmd_Str(string sSQL, string FiledName)
        {
            string sRetV = "";

            DataTable DT_1 = new DataTable("DT1");

            CheckDBConnection();

            DbAdpt = new System.Data.SqlClient.SqlDataAdapter(sSQL, DbConn); //不用置換Connection
            if (_bHasTrans) DbAdpt.SelectCommand.Transaction = _trans;

            DbAdpt.Fill(DT_1);
            if (DT_1.Rows.Count > 0)
            {
                sRetV = DT_1.Rows[0][FiledName].ToString();
            }

            return sRetV;
        }

        /// <summary>
        /// 依SQL指令執行
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public int ExecCmd(string sSQL)
        {
            int RetV = 0;
            try
            {
                CheckDBConnection();

                if (_bHasTrans)
                {

                    System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, DbConn, _trans);
                    Cmd.CommandTimeout = 30000;
                    RetV = Cmd.ExecuteNonQuery();
                }
                else
                {

                    DbConn.Open();
                    System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, DbConn);
                    Cmd.CommandTimeout = 30000;
                    RetV = Cmd.ExecuteNonQuery();
                    DbConn.Close();
                }
            }
            catch (Exception ex)
            {
                string strMsg = "SQL指令執行失敗:" + sSQL + "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                if (_bHasTrans)
                {
                    try
                    {
                        _trans.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        strMsg += string.Format(" Rollback Exception Type: {0} ", ex2.GetType());
                        strMsg += string.Format(" StackTrace : {0} ", ex2.StackTrace);
                    }
                }
                DbConn.Close();

                throw new Exception(strMsg);
            }

            return RetV;
        }

        /// <summary>
        /// 依SQL指令執行
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public int ExecCmdAsync(string sSQL)
        {
            int RetV = 0;
            try
            {
                CheckDBConnection();

                if (_bHasTrans)
                {

                    System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, DbConn, _trans);
                    IAsyncResult cres = Cmd.BeginExecuteNonQuery(null, null);
                    RetV = Cmd.EndExecuteNonQuery(cres);
                }
                else
                {

                    DbConn.Open();
                    System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, DbConn);
                    //Cmd.CommandTimeout = 1500;
                    //Cmd.EndExecuteNonQuery();
                    IAsyncResult cres = Cmd.BeginExecuteNonQuery(null, null);
                    RetV = Cmd.EndExecuteNonQuery(cres);
                    DbConn.Close();
                }
            }
            catch (Exception ex)
            {
                string strMsg = "SQL指令執行失敗:" + sSQL + "\r\n"
                                + ex.Message;
                throw new Exception(strMsg);
            }

            return RetV;
        }

        /// <summary>
        /// 依SQL指令執行(回傳自動編碼)
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="sNO"></param>
        /// <returns></returns>
        public void ExecCmd(string sSQL, out int sNO)
        {
            sNO = 0;
            try
            {
                CheckDBConnection();

                if (_bHasTrans)
                {


                    System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, DbConn, _trans);
                    Cmd.CommandTimeout = 30000;
                    if (Cmd.ExecuteNonQuery() == 1)
                    {
                        Cmd = new System.Data.SqlClient.SqlCommand();
                        Cmd.CommandText = "Select SCOPE_IDENTITY()";
                        Cmd.Connection = DbConn;
                        Cmd.Transaction = _trans;
                        System.Data.SqlClient.SqlDataReader dr = Cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            sNO = Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();
                    }
                }
                else
                {

                    DbConn.Open();
                    System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, DbConn);
                    Cmd.CommandTimeout = 30000;
                    if (Cmd.ExecuteNonQuery() == 1)
                    {
                        DbConn.Close();

                        DbConn.Open();
                        Cmd = new System.Data.SqlClient.SqlCommand();
                        Cmd.CommandText = "Select SCOPE_IDENTITY()";
                        Cmd.Connection = DbConn;
                        System.Data.SqlClient.SqlDataReader dr = Cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            sNO = Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (_bHasTrans)
                {
                }
                else
                {
                    DbConn.Close();
                }
            }
        }

        /// <summary>
        /// 依SQL指令執行
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public int ExecCmd(SqlCommand Cmd)
        {
            int RetV = 0;

            CheckDBConnection();

            if (_bHasTrans)
            {

                Cmd.Connection = DbConn;
                Cmd.CommandTimeout = 30000;
                Cmd.Transaction = _trans;
                RetV = Cmd.ExecuteNonQuery();
            }
            else
            {

                DbConn.Open();
                Cmd.Connection = DbConn;
                Cmd.CommandTimeout = 30000;
                RetV = Cmd.ExecuteNonQuery();

                DbConn.Close();
            }

            return RetV;
        }

        /// <summary>
        /// 依SQL指令執行(回傳自動編碼)
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public int ExecCmd(SqlCommand Cmd, out int sNO)
        {
            sNO = 0;
            int RetV = 0;
            try
            {
                CheckDBConnection();

                if (_bHasTrans)
                {


                    Cmd.Connection = DbConn;
                    Cmd.CommandTimeout = 30000;
                    Cmd.Transaction = _trans;
                    Cmd.CommandText += ";SELECT CAST(scope_identity() AS int)";
                    sNO = (Int32)Cmd.ExecuteScalar();
                }
                else
                {

                    DbConn.Open();
                    Cmd.Connection = DbConn;
                    Cmd.CommandTimeout = 30000;
                    Cmd.CommandText += ";SELECT CAST(scope_identity() AS int)";
                    sNO = (Int32)Cmd.ExecuteScalar();
                }
            }
            catch
            {
            }
            finally
            {
                if (_bHasTrans)
                {
                }
                else
                {
                    DbConn.Close();
                }
            }

            return RetV;
        }

        /// <summary>
        /// 切換時區
        /// </summary>
        /// <returns></returns>
        public string ChangeTimeZone()
        {
            //取得本系統時區
            return "SWITCHOFFSET(getdate(), '" + ConfigHelper.AppSettings.SystemTimeZone + "')";
        }
    }
}
