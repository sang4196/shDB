using System;
using System.Data;
using System.Collections;

using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace shDB
{
    namespace MSSQL
    {
        public class CSQL : IDisposable
        {
            readonly string m_sConnectionString;
            static SqlTransaction m_Trans;
            static SqlConnection m_SqlCon;

            #region con
            public string ConnString
            {
                get { return m_sConnectionString; }
            }

            public SqlConnection SqlConnection
            {
                get { return m_SqlCon; }
            }

            //public CSQL()
            //{
            //    try
            //    {
            //        m_sConnectionString = "Server=localhost;database=DF_DB;Password=1;User ID=sa;";
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            //    }
            //}

            public CSQL(string _sConnection)
            {
                try
                {
                    m_sConnectionString = _sConnection;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            ~CSQL()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool _bDisposing)
            {
                GC.SuppressFinalize(this);
            }
            #endregion

            #region Transaction

            public void BeginTransaction()
            {
                try
                {
                    m_Trans = m_SqlCon.BeginTransaction();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }

            public void RollBack()
            {
                m_Trans.Rollback();
            }

            public void Commit()
            {
                m_Trans.Commit();
            }
            public SqlTransaction Trans
            {
                get { return m_Trans; }
            }
            #endregion

            #region connect disconnect
            public bool Sql_Connect()
            {
                try
                {
                    if (m_sConnectionString != "")
                    {
                        m_SqlCon = new SqlConnection(m_sConnectionString);

                        if (m_SqlCon.State != ConnectionState.Open)
                            m_SqlCon.Open();

                        if (m_SqlCon.State == ConnectionState.Open)
                            return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }

            public void Sql_Disconnect()
            {
                try
                {
                    if (m_SqlCon != null)
                    {
                        if (m_SqlCon.State == ConnectionState.Open)
                            m_SqlCon.Close();

                        m_SqlCon = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            #endregion

            #region Execute

            /// <summary>
            /// exec query
            /// </summary>
            /// <param name="_Connection">Connection</param>
            /// <param name="_sQuery">Text query</param>
            /// <returns></returns>
            public DataTable ExecuteQuery_Text(SqlConnection _Connection, string _sQuery)
            {
                DataTable dt = new DataTable();
                try
                {
                    if (_Connection.State == ConnectionState.Open)
                    {
                        SqlCommand m_SqlCmd = new SqlCommand()
                        {
                            Connection = _Connection,
                            CommandType = CommandType.Text,
                            CommandText = _sQuery
                        };

                        SqlDataAdapter da = new SqlDataAdapter(m_SqlCmd);
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            public DataTable ExecuteQuery_SP(SqlConnection Conn, string strSPName, DataTable dtSPParameter)
            {
                DataTable dt = new DataTable();
                try
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        SqlCommand m_SqlCmd = new SqlCommand();
                        m_SqlCmd.Connection = Conn;
                        m_SqlCmd.CommandType = CommandType.StoredProcedure;
                        m_SqlCmd.CommandText = strSPName;
                        foreach (DataRow dr in dtSPParameter.Rows)
                        {
                            SqlParameter param = new SqlParameter()
                            {
                                ParameterName = dr[(int)PARAM_TYPE.NAME].ToString(),
                                Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
                                SqlDbType = (SqlDbType)dr[(int)PARAM_TYPE.TYPE],
                                Value = dr[(int)PARAM_TYPE.VALUE].ToString(),
                            };

                            if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
                            {
                                if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
                                    param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
                            }

                            m_SqlCmd.Parameters.Add(param);
                        }

                        SqlDataAdapter da = new SqlDataAdapter(m_SqlCmd);
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            public int ExecuteTransQuery_Text(SqlConnection _Connection, string _sQuery)
            {
                string sErrRtn;
                try
                {
                    if (_Connection.State == ConnectionState.Open)
                    {
                        SqlCommand m_SqlCmd = new SqlCommand()
                        {
                            Connection = _Connection,
                            CommandType = CommandType.Text,
                            CommandText = _sQuery,
                        };

                        return m_SqlCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }

            /// <summary>
            /// exec sp with transction, exist param
            /// </summary>
            /// <param name="_Connection">ConnectionB</param>
            /// <param name="_Trans">Transaction</param>
            /// <param name="_sSPName">SP name</param>
            /// <param name="_dtParam">param</param>
            /// <returns></returns>
            public string ExecuteTransQuery_SP(SqlConnection _Connection, SqlTransaction _Trans, string _sSPName, DataTable _dtParam)
            {
                CTransactionErr TransErr = new CTransactionErr();
                string sErrRtn;
                try
                {
                    if (_Connection.State == ConnectionState.Open)
                    {
                        SqlCommand m_SqlCmd = new SqlCommand()
                        {
                            Connection = _Connection,
                            Transaction = _Trans,
                            CommandType = CommandType.StoredProcedure,
                            CommandText = _sSPName,
                        };

                        foreach (DataRow dr in _dtParam.Rows)
                        {
                            SqlParameter param = new SqlParameter()
                            {
                                ParameterName = dr[(int)PARAM_TYPE.NAME].ToString(),
                                Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
                                SqlDbType = (SqlDbType)dr[(int)PARAM_TYPE.TYPE],
                                IsNullable = true,
                            };

                            if ((SqlDbType)dr[(int)PARAM_TYPE.TYPE] != SqlDbType.Image)
                                param.Value = dr[(int)PARAM_TYPE.VALUE].ToString();
                            else
                                param.Value = (byte[])dr[(int)PARAM_TYPE.VALUE];

                            if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
                            {
                                if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
                                    param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
                            }

                            m_SqlCmd.Parameters.Add(param);
                        }

                        m_SqlCmd.ExecuteScalar();

                        TransErr.ErrNum = int.Parse(m_SqlCmd.Parameters["@Rtn"].Value.ToString());
                        TransErr.ErrMsg = m_SqlCmd.Parameters["@ErrMsg"].Value.ToString();

                        foreach (DataRow dr in _dtParam.Rows)
                        {
                            if (dr[(int)PARAM_TYPE.NAME].ToString() != "@ErrMsg" && (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION] == ParameterDirection.Output)
                            {
                                TransErr.AddOutput(m_SqlCmd.Parameters[dr[(int)PARAM_TYPE.NAME].ToString()].Value.ToString());
                            }
                        }
                        sErrRtn = TransErr.EncodingErrTransaction();
                        return sErrRtn;
                    }
                    else
                    {
                        TransErr.ErrNum = -9999;
                        TransErr.ErrMsg = "DB Disconnect";
                        sErrRtn = TransErr.EncodingErrTransaction();
                        return sErrRtn;
                    }
                }
                catch (Exception ex)
                {
                    TransErr.Exception = $"{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}";
                    TransErr.ErrNum = -9999;
                    sErrRtn = TransErr.EncodingErrTransaction();
                    return sErrRtn;
                }
                //finally
                //{
                //}
            }

            #endregion

            #region Parameter
            /// <summary>
            /// set parameter
            /// </summary>
            /// <returns>Parameter table</returns>
            public DataTable SetParam()
            {
                DataTable dt;
                try
                {
                    dt = new DataTable();

                    DataColumn dc = new DataColumn("Name", typeof(string));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Direction", typeof(ParameterDirection));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Type", typeof(SqlDbType));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Value", typeof(object));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Length", typeof(int));
                    dt.Columns.Add(dc);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }

            /// <summary>
            /// add parameter
            /// </summary>
            /// <param name="_dt">Parameter table</param>
            /// <param name="_sName">Paramter name</param>
            /// <param name="_Direction">in/out</param>
            /// <param name="_Type">Value type</param>
            /// <param name="_nSize">Value size</param>
            public void AddParam(DataTable _dt, string _sName, ParameterDirection _Direction, SqlDbType _Type, int _nSize = 0)
            {
                try
                {
                    DataRow dr = _dt.NewRow();
                    dr[(int)PARAM_TYPE.NAME] = _sName;
                    dr[(int)PARAM_TYPE.DIRECTION] = _Direction;
                    dr[(int)PARAM_TYPE.TYPE] = _Type;
                    dr[(int)PARAM_TYPE.LENGTH] = _nSize;
                    _dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            /// <summary>
            /// add parameter
            /// </summary>
            /// <param name="_dt">Parameter table</param>
            /// <param name="_sName">Paramter name</param>
            /// <param name="_Direction">in/out</param>
            /// <param name="_Type">Value type</param>
            /// <param name="_sValue">Value</param>
            public void AddParam(DataTable _dt, string _sName, ParameterDirection _Direction, SqlDbType _Type, string _sValue)
            {
                try
                {
                    DataRow dr = _dt.NewRow();
                    dr[(int)PARAM_TYPE.NAME] = _sName;
                    dr[(int)PARAM_TYPE.DIRECTION] = _Direction;
                    dr[(int)PARAM_TYPE.TYPE] = _Type;
                    dr[(int)PARAM_TYPE.VALUE] = _sValue;
                    _dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            /// <summary>
            /// add parameter
            /// </summary>
            /// <param name="_dt">Parameter table</param>
            /// <param name="_sName">Paramter name</param>
            /// <param name="_Direction">in/out</param>
            /// <param name="_Type">Value type</param>
            /// <param name="_sValue">Value</param>
            /// <param name="_nSize">Value size</param>
            public void AddParam(DataTable _dt, string _sName, ParameterDirection _Direction, SqlDbType _Type, string _sValue, int _nSize = 0)
            {
                try
                {
                    DataRow dr = _dt.NewRow();
                    dr[(int)PARAM_TYPE.NAME] = _sName;
                    dr[(int)PARAM_TYPE.DIRECTION] = _Direction;
                    dr[(int)PARAM_TYPE.TYPE] = _Type;
                    dr[(int)PARAM_TYPE.VALUE] = _sValue;
                    dr[(int)PARAM_TYPE.LENGTH] = _nSize;
                    _dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            #endregion
        }

    }

    

    namespace ACCESS
    {
        public class CSQL : IDisposable
        {
            private readonly string m_sConnectionString;
            private static OleDbConnection m_SqlCon;
            private static OleDbTransaction m_Trans;

            #region con
            public string ConnString
            {
                get { return m_sConnectionString; }
            }

            public OleDbConnection SqlCon
            {
                get { return m_SqlCon; }
            }

            //public CSQL()
            //{
            //    try
            //    {
            //        m_sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\user5\\Documents\\Database1.accdb;";
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            //    }
            //    //finally
            //    //{
            //    //}
            //}

            public CSQL(string _sConnection)
            {
                try
                {
                    m_sConnectionString = _sConnection;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            ~CSQL()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool _bDisposing)
            {
                GC.SuppressFinalize(this);
            }
            #endregion

            #region Transaction

            public void BeginTransaction()
            {
                try
                {
                    m_Trans = m_SqlCon.BeginTransaction();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }

            public void RollBack()
            {
                m_Trans.Rollback();
            }

            public void Commit()
            {
                m_Trans.Commit();
            }
            public OleDbTransaction Trans
            {
                get { return m_Trans; }
            }
            #endregion

            #region MS-ACCESS Connect, Disconnect
            public bool Sql_Connect()
            {
                try
                {
                    if (m_sConnectionString != null)
                    {
                        m_SqlCon = new OleDbConnection(m_sConnectionString);

                        if (m_SqlCon.State != ConnectionState.Open)
                            m_SqlCon.Open();

                        if (m_SqlCon.State == ConnectionState.Open)
                            return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            public void Sql_Disconnect()
            {
                try
                {
                    if (m_SqlCon != null)
                    {
                        if (m_SqlCon.State == ConnectionState.Open)
                            m_SqlCon.Close();
                        m_SqlCon = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            #endregion

            #region Execute

            /// <summary>
            /// exec query
            /// </summary>
            /// <param name="_Connection">Connection</param>
            /// <param name="_sQuery">Text query</param>
            /// <returns></returns>
            public DataTable ExecuteQuery_Text(OleDbConnection _Connection, string _sQuery)
            {
                DataTable dt = new DataTable();
                try
                {
                    if (_Connection.State == ConnectionState.Open)
                    {
                        OleDbCommand m_SqlCmd = new OleDbCommand()
                        {
                            Connection = _Connection,
                            CommandType = CommandType.Text,
                            CommandText = _sQuery,
                        };

                        OleDbDataAdapter da = new OleDbDataAdapter(m_SqlCmd);
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            /// <summary>
            /// exec SP with transation, exist param
            /// </summary>
            /// <param name="_Connection">ConnectionB</param>
            /// <param name="_Trans">transation</param>
            /// <param name="_sSPName">SP name</param>
            /// <param name="_dtParam">param</param>
            /// <returns></returns>
            public string ExecuteTransQuery_SP(OleDbConnection _Connection, OleDbTransaction _Trans, string _sSPName, DataTable _dtParam)
            {
                CTransactionErr TransErr = new CTransactionErr();
                string sErrRtn;
                try
                {
                    if (_Connection.State == ConnectionState.Open)
                    {
                        OleDbCommand m_SqlCmd = new OleDbCommand()
                        {
                            Connection = _Connection,
                            Transaction = _Trans,
                            CommandType = CommandType.StoredProcedure,
                            CommandText = _sSPName,
                        };

                        foreach (DataRow dr in _dtParam.Rows)
                        {
                            OleDbParameter param = new OleDbParameter()
                            {
                                ParameterName = dr[(int)PARAM_TYPE.NAME].ToString(),
                                Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
                                OleDbType = (OleDbType)dr[(int)PARAM_TYPE.TYPE],
                                IsNullable = true
                            };

                            //if ((OleDbType)dr[(int)PARAM_TYPE.TYPE] != OleDbType.Image)
                            //    param.Value = dr[(int)PARAM_TYPE.VALUE].ToString();
                            //else
                            //    param.Value = (byte[])dr[(int)PARAM_TYPE.VALUE];
                            param.Value = dr[(int)PARAM_TYPE.VALUE].ToString();

                            if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
                            {
                                if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
                                    param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
                            }

                            m_SqlCmd.Parameters.Add(param);
                        }

                        m_SqlCmd.ExecuteScalar();

                        //처리 결과를 구조체 변수에 저장시킴
                        TransErr.ErrNum = int.Parse(m_SqlCmd.Parameters["@Rtn"].Value.ToString());
                        TransErr.ErrMsg = m_SqlCmd.Parameters["@ErrMsg"].Value.ToString();

                        foreach (DataRow dr in _dtParam.Rows)
                        {
                            if (dr[(int)PARAM_TYPE.NAME].ToString() != "@ErrMsg" && (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION] == ParameterDirection.Output)
                            {
                                TransErr.AddOutput(m_SqlCmd.Parameters[dr[(int)PARAM_TYPE.NAME].ToString()].Value.ToString());
                            }
                        }
                        sErrRtn = TransErr.EncodingErrTransaction();
                        return sErrRtn;
                    }
                    else
                    {
                        TransErr.ErrNum = 99;
                        TransErr.ErrMsg = "DB Disconnect";
                        sErrRtn = TransErr.EncodingErrTransaction();
                        return sErrRtn;
                    }

                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            #endregion

            #region Parameter
            /// <summary>
            /// set param table
            /// </summary>
            /// <returns>Parameter table</returns>
            public DataTable SetParam()
            {
                DataTable dt;
                try
                {
                    dt = new DataTable();

                    DataColumn dc = new DataColumn("Name", typeof(string));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Direction", typeof(ParameterDirection));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Type", typeof(OleDbType));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Value", typeof(object));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Length", typeof(int));
                    dt.Columns.Add(dc);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }

            /// <summary>
            /// add parameter
            /// </summary>
            /// <param name="_dt">Parameter table</param>
            /// <param name="_sName">Paramter name</param>
            /// <param name="_Direction">in/out</param>
            /// <param name="_Type">Value type</param>
            /// <param name="_nSize">Value size</param>
            public void AddParam(DataTable _dt, string _sName, ParameterDirection _Direction, OleDbType _Type, int _nSize = 0)
            {
                try
                {
                    DataRow dr = _dt.NewRow();
                    dr[(int)PARAM_TYPE.NAME] = _sName;
                    dr[(int)PARAM_TYPE.DIRECTION] = _Direction;
                    dr[(int)PARAM_TYPE.TYPE] = _Type;
                    dr[(int)PARAM_TYPE.LENGTH] = _nSize;
                    _dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            /// <summary>
            /// add parameter
            /// </summary>
            /// <param name="_dt">Parameter table</param>
            /// <param name="_sName">Paramter name</param>
            /// <param name="_Direction">in/out</param>
            /// <param name="_Type">Value type</param>
            /// <param name="_sValue">Value</param>
            public void AddParam(DataTable _dt, string _sName, ParameterDirection _Direction, OleDbType _Type, string _sValue)
            {
                try
                {
                    DataRow dr = _dt.NewRow();
                    dr[(int)PARAM_TYPE.NAME] = _sName;
                    dr[(int)PARAM_TYPE.DIRECTION] = _Direction;
                    dr[(int)PARAM_TYPE.TYPE] = _Type;
                    dr[(int)PARAM_TYPE.VALUE] = _sValue;
                    _dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            public void AddParam(DataTable _dt, string _sName, ParameterDirection _Direction, OleDbType _Type, string _sValue, int _nSize = 0)
            {
                try
                {
                    DataRow dr = _dt.NewRow();
                    dr[(int)PARAM_TYPE.NAME] = _sName;
                    dr[(int)PARAM_TYPE.DIRECTION] = _Direction;
                    dr[(int)PARAM_TYPE.TYPE] = _Type;
                    dr[(int)PARAM_TYPE.VALUE] = _sValue;
                    dr[(int)PARAM_TYPE.LENGTH] = _nSize;
                    _dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            #endregion
        }
    }

    #region Transaction class
    public class CTransactionErr
    {
        int nErrNum;
        string sErrMsg;
        string sException;
        List<string> arrOutput;

        string[] arrErrSep = { "<Err>" };
        string[] arrOutSep = { "<OUT>" };

        public int ErrNum
        {
            get { return nErrNum; }
            set { nErrNum = value; }
        }

        public string ErrMsg
        {
            get { return sErrMsg; }
            set { sErrMsg = value; }
        }

        public string Exception
        {
            get { return sException; }
            set { sException = value; }
        }

        public CTransactionErr()
        {
            nErrNum = 0;
            sErrMsg = "";
            sException = "";
            arrOutput = new List<string>();
        }

        /// <summary>
        /// add return array
        /// </summary>
        /// <param name="_sValue"></param>
        public void AddOutput(string _sValue)
        {
            arrOutput.Add(_sValue);
        }

        /// <summary>
        /// delete return array by index
        /// </summary>
        /// <param name="_nIndex"></param>
        public void DeleteOutput(int _nIndex)
        {
            arrOutput.RemoveAt(_nIndex);
        }

        /// <summary>
        /// get return array
        /// </summary>
        /// <returns></returns>
        public List<string> GetOutput()
        {
            return arrOutput;
        }

        /// <summary>
        /// get return array by index
        /// </summary>
        /// <param name="_nIndex"></param>
        /// <returns></returns>
        public string GetOutput(int _nIndex)
        {
            if (arrOutput.Count >= _nIndex + 1)
            {
                return arrOutput[_nIndex];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// convert class to err msg
        /// </summary>
        /// <param name="_Err"></param>
        /// <returns>Encoding value</returns>
        public string EncodingErrTransaction()
        {
            string sErr = "";

            try
            {
                sErr = nErrNum.ToString() + arrErrSep[0] +
                       sErrMsg + arrErrSep[0] +
                       sException + arrErrSep[0];

                foreach (var item in arrOutput)
                {
                    sErr += item + arrOutSep[0];
                }
                return sErr;
            }
            catch
            {
                return sErr;
            }
            finally
            {
            }
        }

        /// <summary>
        /// convert err msg to class
        /// </summary>
        /// <param name="_sErr"></param>
        /// <returns>Decoding class</returns>
        public CTransactionErr DecodingErrTransaction(string _sErr)
        {
            CTransactionErr ErrTrans = new CTransactionErr();

            try
            {
                string[] arrErrMsg = _sErr.Split(arrErrSep, StringSplitOptions.None);

                ErrTrans.nErrNum = int.Parse(arrErrMsg[(int)TRANS_MEMBER.ERR_NUM]);
                ErrTrans.sErrMsg = arrErrMsg[(int)TRANS_MEMBER.ERR_MSG];
                ErrTrans.sException = arrErrMsg[(int)TRANS_MEMBER.EXCEPTION];

                string[] arrTmp = arrErrMsg[(int)TRANS_MEMBER.OUTPUT].Split(arrOutSep, StringSplitOptions.None);
                for (int i = 0; i < arrTmp.Length - 1; i++)
                    ErrTrans.AddOutput(arrTmp[i]);

                return ErrTrans;
            }
            catch
            {
                return ErrTrans;
            }
            finally
            {
            }
        }
    }
    #endregion

    enum TRANS_MEMBER
    {
        ERR_NUM = 0,
        ERR_MSG,
        EXCEPTION,
        OUTPUT,
    }

    enum PARAM_TYPE
    {
        NAME = 0,
        DIRECTION,
        TYPE,
        VALUE,
        LENGTH,
    }

}