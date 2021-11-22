using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace shDB.MSSQL
{
    public class CSQL : IDisposable
    {
        readonly string m_sConnectionString;
        static SqlTransaction m_Trans;
        static SqlConnection m_SqlCon;

        public delegate void MessageEventHandler(string _sMsg);
        public event MessageEventHandler MessageEvent;

        private void MessageOut(string _sMsg)
        {
            MessageEvent?.Invoke(_sMsg);
        }

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
        //        MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return false;
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return null;
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
                    SqlCommand m_SqlCmd = new SqlCommand()
                    {
                        Connection = Conn,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = strSPName,
                    };

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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return null;
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return null;
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
            }
            //finally
            //{
            //}
        }

        #endregion
    }
}
