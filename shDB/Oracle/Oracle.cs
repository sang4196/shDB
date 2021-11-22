using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace shDB.Oracle
{
    //public class CSQL : IDisposable
    //{
    //    readonly string m_sConnectionString;
    //    static OracleConnection m_SqlCon;
    //    static OracleTransaction m_Trans;

    //    public delegate void MessageEventHandler(string _sMsg);
    //    public event MessageEventHandler MessageEvent;

    //    private void MessageOut(string _sMsg)
    //    {
    //        MessageEvent?.Invoke(_sMsg);
    //    }

    //    #region Transaction

    //    public void BeginTransaction()
    //    {
    //        try
    //        {
    //            m_Trans = m_SqlCon.BeginTransaction();
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    public void RollBack()
    //    {
    //        m_Trans.Rollback();
    //    }

    //    public void Commit()
    //    {
    //        m_Trans.Commit();
    //    }
    //    #endregion

    //    #region Constructor

    //    public string ConnString
    //    {
    //        get { return m_sConnectionString; }
    //    }

    //    public OracleConnection SqlCon
    //    {
    //        get { return m_SqlCon; }
    //    }

    //    public OracleTransaction Trans
    //    {
    //        get { return m_Trans; }
    //    }

    //    //public CSQL()
    //    //{
    //    //    try
    //    //    {
    //    //        m_sConnectionString = "Server=localhost;database=DF_DB;Password=1;User ID=sa;";
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //    //    }
    //    //}

    //    public CSQL(string _sConnection)
    //    {
    //        try
    //        {
    //            m_sConnectionString = _sConnection;
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    #endregion

    //    #region Oracle Connect, Disconnect
    //    public bool Sql_Connect()
    //    {
    //        try
    //        {
    //            if (m_sConnectionString != "")
    //            {
    //                m_SqlCon = new OracleConnection(m_sConnectionString);

    //                if (m_SqlCon.State != ConnectionState.Open)
    //                    m_SqlCon.Open();

    //                if (m_SqlCon.State == ConnectionState.Open)
    //                    return true;
    //            }

    //            return false;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    public void Sql_Disconnect()
    //    {
    //        try
    //        {
    //            if (m_SqlCon != null)
    //            {
    //                if (m_SqlCon.State == ConnectionState.Open)
    //                    m_SqlCon.Close();

    //                m_SqlCon = null;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }
    //    #endregion

    //    #region Execute

    //    /// <summary>
    //    /// 읽기 저장프로시저 실행 : SP함수명만 있는 경우
    //    /// </summary>
    //    /// <param name="Conn">DB연결객체</param>
    //    /// <param name="strSPName">SP명</param>
    //    /// <returns>Select결과</returns>
    //    public DataTable ExecReadStoredProc2(object Conn, string strSPName, string run_IP, string run_ID)
    //    {
    //        DataTable dt = new DataTable();
    //        string run_dt = string.Empty;
    //        int o_seq = 0;
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {

    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;
    //                m_SqlCmd.BindByName = true;

    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();
    //                ProcRunStart(m_SqlCmd, strSPName, ref run_dt, ref o_seq, run_IP, run_ID);

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(dt);

    //                ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);
    //            }
    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            dt.Dispose();
    //        }
    //    }
    //    public DataSet ExecReadStoredProc_DS(object Conn, string strSPName, DataTable dtSPParameter, string run_IP, string run_ID)
    //    {
    //        DataSet ds = new DataSet();
    //        string run_dt = string.Empty;
    //        int o_seq = 0;
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {
    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;

    //                m_SqlCmd.BindByName = true;

    //                ProcRunStart(m_SqlCmd, strSPName, dtSPParameter, ref run_dt, ref o_seq, run_IP, run_ID);

    //                #region run
    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();
    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", string.Empty).ToUpper().Trim(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        Value = dr[(int)PARAM_TYPE.VALUE],
    //                    };
    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }
    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(ds);
    //                #endregion

    //                ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);
    //            }
    //            return ds;
    //        }
    //        catch (Exception ex)
    //        {
    //            ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq, ex.Message);
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            ds.Dispose();
    //        }
    //    }
    //    public DataTable ExecReadStoredProc(object Conn, string strSPName)
    //    {
    //        DataTable dt = new DataTable();
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {

    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;
    //                m_SqlCmd.BindByName = true;

    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(dt);
    //            }
    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            dt.Dispose();
    //        }
    //    }

    //    /// <summary>
    //    /// 읽기 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
    //    /// </summary>
    //    /// <param name="Conn">DB연결객체</param>
    //    /// <param name="strSPName">SP명</param>
    //    /// <param name="dtSPParameter">인자 배열</param>
    //    /// <returns>Select결과</returns>
    //    public DataTable ExecReadStoredProc2(object Conn, string strSPName, DataTable dtSPParameter, string run_IP, string run_ID)
    //    {
    //        DataTable dt = new DataTable();
    //        string run_dt = string.Empty;
    //        int o_seq = 0;
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {
    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;

    //                m_SqlCmd.BindByName = true;

    //                ProcRunStart(m_SqlCmd, strSPName, dtSPParameter, ref run_dt, ref o_seq, run_IP, run_ID);

    //                #region run
    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();
    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", string.Empty).ToUpper().Trim(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        Value = dr[(int)PARAM_TYPE.VALUE],
    //                    };
    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }
    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(dt);
    //                #endregion

    //                ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);
    //            }
    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq, ex.Message);
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            dt.Dispose();
    //        }
    //    }
    //    public DataTable ExecReadStoredProc(object Conn, string strSPName, DataTable dtSPParameter)
    //    {
    //        DataTable dt = new DataTable();
    //        string run_dt = string.Empty;
    //        int o_seq = 0;
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {
    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;

    //                m_SqlCmd.BindByName = true;

    //                //ProcRunStart(m_SqlCmd, strSPName, dtSPParameter, ref run_dt, ref o_seq, run_IP, run_ID);

    //                #region run
    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();
    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", string.Empty).ToUpper().Trim(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        Value = dr[(int)PARAM_TYPE.VALUE],
    //                    };
    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }
    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(dt);
    //                #endregion

    //                //ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);
    //            }
    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq, ex.Message);
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            dt.Dispose();
    //        }
    //    }

    //    /// <summary>
    //    /// 읽기 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우(SELECT SP 사용시 트랜잭션 필요할 경우 사용) lmj
    //    /// </summary>
    //    /// <param name="Conn">DB연결객체</param>
    //    /// /// <param name="Trans">트랜잭션객체</param>
    //    /// <param name="strSPName">SP명</param>
    //    /// <param name="dtSPParameter">인자 배열</param>
    //    /// <returns>Select결과</returns>
    //    public DataTable ExecTransStoredProc_Select(OracleConnection Conn, OracleTransaction Trans, string strSPName, DataTable dtSPParameter)
    //    {
    //        DataTable dt = new DataTable();
    //        try
    //        {
    //            //if (mfConnect() == true)
    //            if (Conn.State == ConnectionState.Open)
    //            {
    //                OracleCommand m_SqlCmd = new OracleCommand()
    //                {
    //                    Connection = Conn,
    //                    Transaction = Trans,
    //                    CommandType = CommandType.StoredProcedure,
    //                    CommandText = strSPName,
    //                };

    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        IsNullable = true,

    //                        Value = dr[(int)PARAM_TYPE.VALUE].ToString(),
    //                    };

    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }

    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(dt);
    //            }
    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            dt.Dispose();
    //        }
    //    }
    //    public DataTable ExecTransStoredProc_Select2(OracleConnection Conn, OracleTransaction Trans, string strSPName, DataTable dtSPParameter, string run_IP, string run_ID)
    //    {
    //        DataTable dt = new DataTable();
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        string run_dt = "";
    //        int o_seq = 0;
    //        try
    //        {
    //            //if (mfConnect() == true)
    //            if (Conn.State == ConnectionState.Open)
    //            {

    //                m_SqlCmd.Connection = Conn;
    //                m_SqlCmd.Transaction = Trans;
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;

    //                ProcRunStart(m_SqlCmd, strSPName, dtSPParameter, ref run_dt, ref o_seq, run_IP, run_ID);

    //                #region Run
    //                m_SqlCmd.CommandText = strSPName;
    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        IsNullable = true,

    //                        Value = dr[(int)PARAM_TYPE.VALUE].ToString(),
    //                    };

    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }

    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                da.Fill(dt);
    //                #endregion

    //                ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);
    //            }
    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq, ex.Message);
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //        finally
    //        {
    //            dt.Dispose();
    //        }
    //    }

    //    /// <summary>
    //    /// 트랜잭션(DML) 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
    //    /// </summary>
    //    /// <param name="Conn">DB연결객체</param>
    //    /// <param name="Trans">트랜잭션객체</param>
    //    /// <param name="strSPName">SP명</param>
    //    /// <param name="dtSPParameter">인자배열</param>
    //    /// <returns>DML실행결과</returns>
    //    public string ExecTransStoredProc2(object Conn, object Trans, string strSPName, DataTable dtSPParameter, string run_IP, string run_ID)
    //    {
    //        CTransactionErr Result = new CTransactionErr();
    //        string sErrRtn;
    //        string run_dt = string.Empty;
    //        int o_seq = 0;
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {
    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.Transaction = (OracleTransaction)Trans;
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;
    //                m_SqlCmd.BindByName = true;

    //                ProcRunStart(m_SqlCmd, strSPName, dtSPParameter, ref run_dt, ref o_seq, run_IP, run_ID);

    //                #region run
    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();

    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", string.Empty).ToUpper().Trim(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        IsNullable = true,
    //                        Value = dr[(int)PARAM_TYPE.VALUE],
    //                    };

    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }

    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                //string strReturn = Convert.ToString(m_SqlCmd.ExecuteScalar());
    //                m_SqlCmd.ExecuteNonQuery();

    //                //처리 결과를 구조체 변수에 저장시킴
    //                Result.ErrNum = int.Parse(m_SqlCmd.Parameters["ERRORCODE"].Value.ToString());

    //                if (string.IsNullOrEmpty(m_SqlCmd.Parameters["ERRMSG"].Value.ToString().Replace("null", "")))
    //                    Result.ErrMsg = string.Empty;
    //                else
    //                    Result.ErrMsg = m_SqlCmd.Parameters["ERRMSG"].Value.ToString();

    //                //Output Param이 있는 경우 ArrayList에 저장시킴.

    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    if (dr[(int)PARAM_TYPE.NAME].ToString() != "ERRORCODE" &&
    //                        dr[(int)PARAM_TYPE.NAME].ToString() != "ERRMSG" &&
    //                        (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION] == ParameterDirection.Output)
    //                    {
    //                        Result.AddOutput(m_SqlCmd.Parameters[dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", "")].Value == null ? string.Empty : m_SqlCmd.Parameters[dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", "")].Value.ToString());
    //                    }
    //                }
    //                sErrRtn = Result.EncodingErrTransaction();
    //                #endregion

    //                ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq);

    //                return sErrRtn;
    //            }
    //            else
    //            {
    //                Result.ErrNum = 99;
    //                Result.ErrMsg = "DB Disconnect";
    //                sErrRtn = Result.EncodingErrTransaction();
    //                return sErrRtn;
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            ProcRunEnd(m_SqlCmd, strSPName, run_dt, o_seq, ex.Message);
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //    }
    //    public string ExecTransStoredProc(object Conn, object Trans, string strSPName, DataTable dtSPParameter)
    //    {
    //        CTransactionErr Result = new CTransactionErr();
    //        OracleCommand m_SqlCmd = new OracleCommand();
    //        string sErrRtn;
    //        try
    //        {
    //            if (((OracleConnection)Conn).State == ConnectionState.Open)
    //            {
    //                m_SqlCmd.Connection = ((OracleConnection)Conn);
    //                m_SqlCmd.Transaction = (OracleTransaction)Trans;
    //                m_SqlCmd.CommandType = CommandType.StoredProcedure;
    //                m_SqlCmd.BindByName = true;

    //                #region run
    //                m_SqlCmd.CommandText = strSPName.ToUpper().Trim();

    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    OracleParameter param = new OracleParameter()
    //                    {
    //                        ParameterName = dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", string.Empty).ToUpper().Trim(),
    //                        Direction = (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION],
    //                        OracleDbType = (OracleDbType)dr[(int)PARAM_TYPE.TYPE],
    //                        IsNullable = true,
    //                        Value = dr[(int)PARAM_TYPE.VALUE],
    //                    };

    //                    if (dr[(int)PARAM_TYPE.LENGTH].ToString() != "")
    //                    {
    //                        if (int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString()) > 0)
    //                            param.Size = int.Parse(dr[(int)PARAM_TYPE.LENGTH].ToString());
    //                    }

    //                    m_SqlCmd.Parameters.Add(param);
    //                }

    //                m_SqlCmd.ExecuteNonQuery();

    //                Result.ErrNum = int.Parse(m_SqlCmd.Parameters["ERRORCODE"].Value.ToString());

    //                if (string.IsNullOrEmpty(m_SqlCmd.Parameters["ERRMSG"].Value.ToString().Replace("null", "")))
    //                    Result.ErrMsg = string.Empty;
    //                else
    //                    Result.ErrMsg = m_SqlCmd.Parameters["ERRMSG"].Value.ToString();


    //                foreach (DataRow dr in dtSPParameter.Rows)
    //                {
    //                    if (dr[(int)PARAM_TYPE.NAME].ToString() != "ERRORCODE" &&
    //                        dr[(int)PARAM_TYPE.NAME].ToString() != "ERRMSG" &&
    //                        (ParameterDirection)dr[(int)PARAM_TYPE.DIRECTION] == ParameterDirection.Output)
    //                    {
    //                        Result.AddOutput(m_SqlCmd.Parameters[dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", "")].Value == null ? string.Empty : m_SqlCmd.Parameters[dr[(int)PARAM_TYPE.NAME].ToString().Replace("@", "")].Value.ToString());
    //                    }
    //                }
    //                sErrRtn = Result.EncodingErrTransaction();
    //                #endregion

    //                return sErrRtn;
    //            }
    //            else
    //            {
    //                Result.ErrNum = 99;
    //                Result.ErrMsg = "DB Disconnect";
    //                sErrRtn = Result.EncodingErrTransaction();
    //                return sErrRtn;
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //            return null;
    //        }
    //    }

    //    public DataTable ExecReadCommendText(string strConnectionString, string strCommandText)
    //    {
    //        DataTable dt = new DataTable();
    //        using (OracleConnection Conn = new OracleConnection(strConnectionString))
    //        {
    //            try
    //            {
    //                //Open상태가 아니면 Open한다.
    //                if (Conn.State != ConnectionState.Open)
    //                    Conn.Open();

    //                if (Conn.State == ConnectionState.Open)
    //                {
    //                    OracleCommand m_SqlCmd = new OracleCommand()
    //                    {
    //                        Connection = ((OracleConnection)Conn),
    //                        CommandType = CommandType.Text,
    //                        CommandText = strCommandText,
    //                    };

    //                    OracleDataAdapter da = new OracleDataAdapter(m_SqlCmd);
    //                    da.Fill(dt);
    //                }
    //                return dt;
    //            }

    //            catch (Exception ex)
    //            {
    //                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //                return null;
    //            }
    //            finally
    //            {
    //                Conn.Close();
    //                dt.Dispose();
    //            }
    //        }
    //    }

    //    #endregion

    //    #region Parameter
    //    /// <summary>
    //    /// 저장프로시져에 넘겨줄 Parameter 데이터테이블 설정
    //    /// </summary>
    //    /// <returns>Parameter테이블</returns>
    //    public DataTable SetParamDataTable()
    //    {
    //        DataTable dt = null;
    //        try
    //        {
    //            dt = new DataTable();

    //            DataColumn dc = new DataColumn("Name", typeof(string));
    //            dt.Columns.Add(dc);

    //            dc = new DataColumn("Direction", typeof(ParameterDirection));
    //            dt.Columns.Add(dc);

    //            dc = new DataColumn("Type", typeof(OracleDbType));
    //            dt.Columns.Add(dc);

    //            dc = new DataColumn("Value", typeof(object));
    //            dt.Columns.Add(dc);

    //            dc = new DataColumn("Length", typeof(int));
    //            dt.Columns.Add(dc);

    //            return dt;
    //        }
    //        catch (Exception ex)
    //        {
    //            return dt;
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    /// <summary>
    //    /// 저장프로시져에 넘겨줄 Parameter 추가
    //    /// </summary>
    //    /// <param name="dt">Parameter 테이블</param>
    //    /// <param name="strName">Paramter 명</param>
    //    /// <param name="Direction">Parameter 방향(in/out)</param>
    //    /// <param name="DBType">Parameter DB유형</param>
    //    /// <param name="strValue">Paramter 인자값</param>
    //    /// <param name="intSize">Parameter 크기</param>
    //    public void AddParamDataRow(DataTable dt, string strName, ParameterDirection Direction
    //        , OracleDbType DBType, object objValue, int intSize)
    //    {
    //        try
    //        {
    //            DataRow dr = dt.NewRow();
    //            dr[(int)PARAM_TYPE.NAME] = strName;
    //            dr[(int)PARAM_TYPE.DIRECTION] = Direction;
    //            dr[(int)PARAM_TYPE.TYPE] = DBType;
    //            dr[(int)PARAM_TYPE.VALUE] = objValue;
    //            dr[(int)PARAM_TYPE.LENGTH] = intSize;
    //            dt.Rows.Add(dr);
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    /// <summary>
    //    /// 저장프로시져에 넘겨줄 Parameter 추가
    //    /// </summary>
    //    /// <param name="dt">Parameter 테이블</param>
    //    /// <param name="Direction">Parameter 방향(in/out)</param>
    //    /// <param name="DBType">Parameter DB유형</param>
    //    /// <param name="strValue">Paramter 인자값</param>
    //    public void AddParamDataRow(DataTable dt, string strName, ParameterDirection Direction
    //        , OracleDbType DBType, object objValue)
    //    {
    //        try
    //        {
    //            DataRow dr = dt.NewRow();
    //            dr[(int)PARAM_TYPE.NAME] = strName;
    //            dr[(int)PARAM_TYPE.DIRECTION] = Direction;
    //            dr[(int)PARAM_TYPE.TYPE] = DBType;
    //            dr[(int)PARAM_TYPE.VALUE] = objValue;
    //            dt.Rows.Add(dr);
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //        finally
    //        {
    //        }
    //    }

    //    /// <summary>
    //    /// 저장프로시져에 넘겨줄 Parameter 추가
    //    /// </summary>
    //    /// <param name="dt">Parameter 테이블</param>
    //    /// <param name="strName">Paramter 명</param>
    //    /// <param name="Direction">Parameter 방향(in/out)</param>
    //    /// <param name="DBType">Parameter DB유형</param>
    //    public void AddParamDataRow(DataTable dt, string strName, ParameterDirection Direction
    //        , OracleDbType DBType)
    //    {
    //        try
    //        {
    //            DataRow dr = dt.NewRow();
    //            dr[(int)PARAM_TYPE.NAME] = strName;
    //            dr[(int)PARAM_TYPE.DIRECTION] = Direction;
    //            dr[(int)PARAM_TYPE.TYPE] = DBType;
    //            dt.Rows.Add(dr);
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    /// <summary>
    //    /// 저장프로시져에 넘겨줄 Parameter 추가
    //    /// </summary>
    //    /// <param name="dt">Parameter 테이블</param>
    //    /// <param name="strName">Paramter 명</param>
    //    /// <param name="Direction">Parameter 방향(in/out)</param>
    //    /// <param name="DBType">Parameter DB유형</param>
    //    /// <param name="intSize">Parameter 크기</param>
    //    public void AddParamDataRow(DataTable dt, string strName, ParameterDirection Direction
    //        , OracleDbType DBType, int intSize)
    //    {
    //        try
    //        {
    //            DataRow dr = dt.NewRow();
    //            dr[(int)PARAM_TYPE.NAME] = strName;
    //            dr[(int)PARAM_TYPE.DIRECTION] = Direction;
    //            dr[(int)PARAM_TYPE.TYPE] = DBType;
    //            dr[(int)PARAM_TYPE.LENGTH] = intSize;
    //            dt.Rows.Add(dr);
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
    //        }
    //    }

    //    #endregion

    //    #region IDisposable 멤버

    //    ~CSQL()
    //    {
    //        Dispose(false);
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //    }

    //    protected virtual void Dispose(bool bolDisposing)
    //    {
    //        GC.SuppressFinalize(this);
    //    }

    //    #endregion

    //    #region ProcRun
    //    void ProcRunStart(OracleCommand m_SqlCmd, string strSPName, DataTable dtSPParameter, ref string run_dt, ref int o_seq, string run_IP, string run_ID)
    //    {
    //        string VAL_NM = "";
    //        string VAL_VAL = "";

    //        foreach (DataRow item in dtSPParameter.Rows)
    //        {
    //            VAL_NM += item[(int)PARAM_TYPE.NAME].ToString() + ",";
    //            VAL_VAL += item[(int)PARAM_TYPE.VALUE].ToString() + ",";
    //        }
    //        if (VAL_NM.Length > 2000) VAL_NM = VAL_NM.Substring(0, 2000);
    //        if (VAL_VAL.Length > 2000) VAL_VAL = VAL_VAL.Substring(0, 2000);

    //        m_SqlCmd.CommandText = "UP_U_SYSPROCRUN";
    //        m_SqlCmd.Parameters.Add("I_STRPROC_NM", strSPName.ToUpper().Trim());
    //        m_SqlCmd.Parameters.Add("I_STRVAL_NM", VAL_NM);
    //        m_SqlCmd.Parameters.Add("I_STRVAL_VAL", VAL_VAL);
    //        m_SqlCmd.Parameters.Add("I_RUN_IP", run_IP);
    //        m_SqlCmd.Parameters.Add("I_RUN_ID", run_ID);
    //        OracleParameter p = new OracleParameter()
    //        {
    //            ParameterName = "O_RUN_DT",
    //            OracleDbType = OracleDbType.Varchar2,
    //            Direction = ParameterDirection.Output,
    //            Size = 10,
    //        };
    //        m_SqlCmd.Parameters.Add(p);
    //        p = new OracleParameter()
    //        {
    //            ParameterName = "O_SEQ",
    //            OracleDbType = OracleDbType.Int32,
    //            Direction = ParameterDirection.Output,
    //        };
    //        m_SqlCmd.Parameters.Add(p);

    //        m_SqlCmd.ExecuteNonQuery();

    //        run_dt = m_SqlCmd.Parameters["O_RUN_DT"].Value.ToString();
    //        o_seq = int.Parse(m_SqlCmd.Parameters["O_SEQ"].Value.ToString());
    //        m_SqlCmd.Parameters.Clear();
    //    }
    //    void ProcRunStart(OracleCommand m_SqlCmd, string strSPName, ref string run_dt, ref int o_seq, string run_IP, string run_ID)
    //    {
    //        string VAL_NM = "";
    //        string VAL_VAL = "";

    //        m_SqlCmd.CommandText = "UP_U_SYSPROCRUN";
    //        m_SqlCmd.Parameters.Add("I_STRPROC_NM", strSPName.ToUpper().Trim());
    //        m_SqlCmd.Parameters.Add("I_STRVAL_NM", VAL_NM);
    //        m_SqlCmd.Parameters.Add("I_STRVAL_VAL", VAL_VAL);
    //        m_SqlCmd.Parameters.Add("I_RUN_IP", run_IP);
    //        m_SqlCmd.Parameters.Add("I_RUN_ID", run_ID);
    //        OracleParameter p = new OracleParameter()
    //        {
    //            ParameterName = "O_RUN_DT",
    //            OracleDbType = OracleDbType.Varchar2,
    //            Direction = ParameterDirection.Output,
    //            Size = 10,
    //        };
    //        m_SqlCmd.Parameters.Add(p);
    //        p = new OracleParameter()
    //        {
    //            ParameterName = "O_SEQ",
    //            OracleDbType = OracleDbType.Int32,
    //            Direction = ParameterDirection.Output,
    //        };
    //        m_SqlCmd.Parameters.Add(p);

    //        m_SqlCmd.ExecuteNonQuery();

    //        run_dt = m_SqlCmd.Parameters["O_RUN_DT"].Value.ToString();
    //        o_seq = int.Parse(m_SqlCmd.Parameters["O_SEQ"].Value.ToString());
    //        m_SqlCmd.Parameters.Clear();
    //    }
    //    void ProcRunEnd(OracleCommand m_SqlCmd, string strSPName, string run_dt, int o_seq, string ex_msg = "")
    //    {
    //        m_SqlCmd.Parameters.Clear();
    //        m_SqlCmd.CommandText = "UP_U_SYSPROCRUN_END";

    //        OracleParameter p = new OracleParameter()
    //        {
    //            ParameterName = "I_STRPROC_NM",
    //            Value = strSPName.ToUpper().Trim(),
    //        };
    //        m_SqlCmd.Parameters.Add(p);
    //        p = new OracleParameter()
    //        {
    //            ParameterName = "I_STRRUN_DT",
    //            Value = run_dt,
    //        };
    //        m_SqlCmd.Parameters.Add(p);
    //        p = new OracleParameter()
    //        {
    //            ParameterName = "I_NUMSEQ",
    //            Value = o_seq,
    //        };
    //        m_SqlCmd.Parameters.Add(p);
    //        p = new OracleParameter()
    //        {
    //            ParameterName = "I_EX_MSG",
    //            Value = ex_msg,
    //        };
    //        m_SqlCmd.Parameters.Add(p);
    //        m_SqlCmd.ExecuteNonQuery();
    //    }
    //    #endregion
    //}
}
