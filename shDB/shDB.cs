using System;
using System.Data;
using System.Collections;

using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace shDB
{
    namespace MSSQL
    {
        public class SQLS : IDisposable
        {
            private System.ComponentModel.Component m_component;
            private string m_strConnString;
            public static SqlConnection m_SqlCon;
            private static SqlTransaction m_Trans;

            #region con
            public string ConnString
            {
                get { return m_strConnString; }
            }

            public SqlConnection SqlCon
            {
                get { return m_SqlCon; }
            }

            /// <summary>
            /// 생성자 : Web.Config에서 DB연결정보 얻기
            /// </summary>
            public SQLS()
            {
                m_component = new System.ComponentModel.Component();
                try
                {
                    m_strConnString = "Server=localhost;database=DF_DB;Password=1;User ID=sa;";
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
            /// 생성자 : DB연결정보 얻기
            /// </summary>
            /// <param name="strConnection">연결문자</param>
            public SQLS(string strConnection)
            {
                m_component = new System.ComponentModel.Component();
                try
                {
                    m_strConnString = strConnection;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            ~SQLS()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool bolDisposing)
            {
                if (bolDisposing)
                {
                    if (m_component != null)
                        m_component.Dispose();
                }

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

            public void TransactionHandling(int ErrNum)
            {
                try
                {
                    if (ErrNum.Equals(0))
                        m_Trans.Commit();
                    else
                        m_Trans.Rollback();
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

            #region MS-SQL 서버 연결 및 해제
            /// <summary>
            /// MS-SQL 서버 연결.
            /// </summary>
            /// <returns>연결여부</returns>
            public bool sql_Connect()
            {
                try
                {
                    //연결정보가 없는 경우
                    if (m_strConnString == "")
                        return false;
                    else
                    {
                        m_SqlCon = new SqlConnection(m_strConnString);
                        //Open상태가 아니면 Open한다.
                        if (m_SqlCon.State != ConnectionState.Open)
                            m_SqlCon.Open();

                        if (m_SqlCon.State != ConnectionState.Open)
                            return false;
                        else
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }

            /// <summary>
            /// MS-SQL 서버 연결 끊기.
            /// </summary>
            public void sql_DisConnect()
            {
                try
                {
                    if (m_SqlCon == null) return;

                    if (m_SqlCon.State == ConnectionState.Open)
                    {
                        m_SqlCon.Close();
                        m_SqlCon = null;
                    }
                    else
                        m_SqlCon = null;
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

            #region 저장프로시저 실행

            /// <summary>
            /// 읽기 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
            /// </summary>
            /// <param name="Conn">DB연결객체</param>
            /// <param name="strSPName">SP명</param>
            /// <param name="dtSPParameter">인자 배열</param>
            /// <returns>Select결과</returns>
            public DataTable executeQuery_Proc(SqlConnection Conn, string strSPName, DataTable dtSPParameter)
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
                            SqlParameter param = new SqlParameter();
                            param.ParameterName = dr["ParamName"].ToString();
                            param.Direction = (ParameterDirection)dr["ParamDirect"];
                            param.SqlDbType = (SqlDbType)dr["DBType"];

                            param.Value = dr["Value"].ToString();
                            if (dr["Length"].ToString() != "")
                            {
                                if (Convert.ToInt32(dr["Length"].ToString()) > 0)
                                    param.Size = Convert.ToInt32(dr["Length"].ToString());
                            }
                            m_SqlCmd.Parameters.Add(param);
                        }

                        m_SqlCmd.CommandTimeout = 300;

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

            /// <summary>
            /// 읽기 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
            /// </summary>
            /// <param name="Conn">DB연결객체</param>
            /// <param name="strQuery">Text쿼리</param>
            /// <returns>Select결과</returns>
            public DataTable executeQuery_Text(SqlConnection Conn, string strQuery)
            {
                DataTable dt = new DataTable();
                try
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        SqlCommand m_SqlCmd = new SqlCommand();
                        m_SqlCmd.Connection = Conn;
                        m_SqlCmd.CommandType = CommandType.Text;
                        m_SqlCmd.CommandText = strQuery;

                        m_SqlCmd.CommandTimeout = 300;

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


            /// <summary>
            /// 트랜잭션(DML) 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
            /// </summary>
            /// <param name="Conn">DB연결객체</param>
            /// <param name="Trans">트랜잭션객체</param>
            /// <param name="strSPName">SP명</param>
            /// <param name="dtSPParameter">인자배열</param>
            /// <returns>DML실행결과</returns>
            public string executeTransQuery_Proc(SqlConnection Conn, SqlTransaction Trans, string strSPName, DataTable dtSPParameter)
            {
                TransErrRtn Result = new TransErrRtn();
                string strErrRtn = "";
                try
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        SqlCommand m_SqlCmd = new SqlCommand();
                        m_SqlCmd.Connection = Conn;
                        m_SqlCmd.Transaction = Trans;
                        m_SqlCmd.CommandType = CommandType.StoredProcedure;
                        m_SqlCmd.CommandText = strSPName;

                        m_SqlCmd.CommandTimeout = 300;

                        foreach (DataRow dr in dtSPParameter.Rows)
                        {
                            SqlParameter param = new SqlParameter();
                            param.ParameterName = dr["ParamName"].ToString();
                            param.Direction = (ParameterDirection)dr["ParamDirect"];
                            param.SqlDbType = (SqlDbType)dr["DBType"];
                            param.IsNullable = true;

                            if ((SqlDbType)dr["DBType"] != SqlDbType.Image)
                                param.Value = dr["Value"].ToString();
                            else
                                param.Value = (byte[])dr["Value"];

                            if (dr["Length"].ToString() != "")
                            {
                                if (Convert.ToInt32(dr["Length"].ToString()) > 0)
                                    param.Size = Convert.ToInt32(dr["Length"].ToString());
                            }

                            m_SqlCmd.Parameters.Add(param);
                        }

                        string strReturn = Convert.ToString(m_SqlCmd.ExecuteScalar());
                        //m_SqlCmd.ExecuteNonQuery();

                        //처리 결과를 구조체 변수에 저장시킴
                        Result.ErrNum = Convert.ToInt32(m_SqlCmd.Parameters["@Rtn"].Value);
                        Result.ErrMessage = m_SqlCmd.Parameters["@ErrorMessage"].Value.ToString();
                        //Output Param이 있는 경우 ArrayList에 저장시킴.
                        Result.mfInitReturnValue();
                        foreach (DataRow dr in dtSPParameter.Rows)
                        {
                            if (dr["ParamName"].ToString() != "@ErrorMessage" && (ParameterDirection)dr["ParamDirect"] == ParameterDirection.Output)
                            {
                                Result.mfAddReturnValue(m_SqlCmd.Parameters[dr["ParamName"].ToString()].Value.ToString());
                            }
                        }
                        strErrRtn = Result.mfEncodingErrMessage(Result);
                        return strErrRtn;
                    }
                    else
                    {
                        Result.ErrNum = 99;
                        Result.ErrMessage = "DataBase 연결되지 않았습니다.";
                        strErrRtn = Result.mfEncodingErrMessage(Result);
                        return strErrRtn;
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



            #region 저장프로시져에 넘겨줄 Parameter 정의
            /// <summary>
            /// 저장프로시져에 넘겨줄 Parameter 데이터테이블 설정
            /// </summary>
            /// <returns>Parameter테이블</returns>
            public DataTable setParam()
            {
                DataTable dt = null;
                try
                {
                    dt = new DataTable();

                    DataColumn dc = new DataColumn("ParamName", typeof(string));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("ParamDirect", typeof(ParameterDirection));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("DBType", typeof(SqlDbType));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Value", typeof(object));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Length", typeof(int));
                    dt.Columns.Add(dc);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 저장프로시져에 넘겨줄 Parameter 추가
            /// </summary>
            /// <param name="dt">Parameter 테이블</param>
            /// <param name="strName">Paramter 명</param>
            /// <param name="Direction">Parameter 방향(in/out)</param>
            /// <param name="DBType">Parameter DB유형</param>
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, SqlDbType DBType)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    //dr["Length"] = intSize;
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, SqlDbType DBType, int intSize)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    dr["Length"] = intSize;
                    dt.Rows.Add(dr);
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
            /// 저장프로시져에 넘겨줄 Parameter 추가
            /// </summary>
            /// <param name="dt">Parameter 테이블</param>
            /// <param name="Direction">Parameter 방향(in/out)</param>
            /// <param name="DBType">Parameter DB유형</param>
            /// <param name="strValue">Paramter 인자값</param>
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, SqlDbType DBType, string strValue)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    dr["Value"] = strValue;
                    //dr["Length"] = intSize;
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, SqlDbType DBType, string strValue, int intSize)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    dr["Value"] = strValue;
                    dr["Length"] = intSize;
                    dt.Rows.Add(dr);
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

        #region 트랜잭션 처리시 Return 처리 Class
        [Serializable]
        public class TransErrRtn
        {
            private int intErrNum;              //에러번호
            private string strErrMessage;       //에러메세지    
                                                //private ArrayList arrReturnValue;   //반환할 결과값
            private ArrayList arrReturnValue = new ArrayList();
            private string strSystemMessage;
            private string strSystemStackTrace;
            private string strSystemInnerException;
            private string strInterfaceResultCode;
            private string strInterfaceResultMessage;

            public int ErrNum
            {
                get { return intErrNum; }
                set { intErrNum = value; }
            }

            public string ErrMessage
            {
                get { return strErrMessage; }
                set { strErrMessage = value; }
            }

            public string SystemMessage
            {
                get { return strSystemMessage; }
                set { strSystemMessage = value; }
            }

            public string SystemStackTrace
            {
                get { return strSystemStackTrace; }
                set { strSystemStackTrace = value; }
            }

            public string SystemInnerException
            {
                get { return strSystemInnerException; }
                set { strSystemInnerException = value; }
            }

            public string InterfaceResultCode
            {
                get { return strInterfaceResultCode; }
                set { strInterfaceResultCode = value; }
            }

            public string InterfaceResultMessage
            {
                get { return strInterfaceResultMessage; }
                set { strInterfaceResultMessage = value; }
            }

            public TransErrRtn()
            {
                intErrNum = 0;
                strErrMessage = "";
                //arrReturnValue = null;
                //arrReturnValue.Clear();
                strSystemMessage = "";
                strSystemStackTrace = "";
                strSystemInnerException = "";
                strInterfaceResultCode = "";
                strInterfaceResultMessage = "";
            }

            /// <summary>
            /// 리턴값 배열 초기화
            /// </summary>
            public void mfInitReturnValue()
            {
                arrReturnValue.Clear();
            }

            /// <summary>
            /// 리턴값 배열에 값을 추가
            /// </summary>
            /// <param name="strValue"></param>
            public void mfAddReturnValue(string strValue)
            {
                arrReturnValue.Add(strValue);
            }

            /// <summary>
            /// 리턴갑 배열에 값을 삭제
            /// </summary>
            /// <param name="intIndex"></param>
            public void mfDeleteReturnValue(int intIndex)
            {
                arrReturnValue.Remove(intIndex);
            }

            /// <summary>
            /// 리턴값 배열 얻기
            /// </summary>
            /// <returns></returns>
            public ArrayList mfGetReturnValue()
            {
                return arrReturnValue;
            }

            /// <summary>
            /// 리턴갑 배열중 특정값 얻기
            /// </summary>
            /// <param name="intIndex"></param>
            /// <returns></returns>
            public string mfGetReturnValue(int intIndex)
            {
                if (arrReturnValue.Count >= intIndex + 1)
                {
                    return arrReturnValue[intIndex].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// 에러메세지 구조체 정보를 문자열로 변환
            /// </summary>
            /// <param name="Err">트랜잭션처리정보 구조체</param>
            /// <returns>Encoding값</returns>
            public string mfEncodingErrMessage(TransErrRtn Err)
            {
                string strErr = "";
                string strErrSep = "<Err>";
                string strOutSep = "<OUT>";
                try
                {
                    strErr = Err.intErrNum.ToString() + strErrSep +
                             Err.strErrMessage + strErrSep +
                             Err.strSystemMessage + strErrSep +
                             Err.strSystemStackTrace + strErrSep +
                             Err.strSystemInnerException + strErrSep +
                             Err.strInterfaceResultCode + strErrSep +       //추가
                             Err.strInterfaceResultMessage + strErrSep;     //추가

                    if (Err.arrReturnValue.Count > 0)
                    {
                        //strErr = strErr + "OUTPUT";
                        for (int i = 0; i < Err.arrReturnValue.Count; i++)
                        {
                            strErr = strErr + Err.arrReturnValue[i].ToString() + strOutSep;
                        }
                    }
                    return strErr;

                }
                catch //(Exception ex)
                {
                    return strErr;
                }
                finally
                {
                }
            }

            /// <summary>
            /// 에러메시지 문자를 구조체로 변환
            /// </summary>
            /// <param name="strErr">트랜잭션처리정보 문자열</param>
            /// <returns>Decoding값</returns>
            public TransErrRtn mfDecodingErrMessage(string strErr)
            {
                TransErrRtn errMsg = new TransErrRtn();
                try
                {
                    string[] arrErrSep = { "<Err>" };
                    string[] arrOutSep = { "<OUT>" };

                    string[] arrErrMsg = strErr.Split(arrErrSep, StringSplitOptions.None);

                    errMsg.intErrNum = Convert.ToInt32(arrErrMsg[0]);
                    if (arrErrMsg.Length > 1)
                        errMsg.strErrMessage = arrErrMsg[1];

                    if (arrErrMsg.Length > 2)
                        errMsg.strSystemMessage = arrErrMsg[2];

                    if (arrErrMsg.Length > 3)
                        errMsg.strSystemStackTrace = arrErrMsg[3];

                    if (arrErrMsg.Length > 4)
                        errMsg.strSystemInnerException = arrErrMsg[4];

                    if (arrErrMsg.Length > 5)
                        errMsg.strInterfaceResultCode = arrErrMsg[5];       //추가

                    if (arrErrMsg.Length > 6)
                        errMsg.strInterfaceResultMessage = arrErrMsg[6];    //추가


                    if (strErr.Split(arrOutSep, StringSplitOptions.None).Length > 0)
                    {
                        string strtemp = strErr.Substring(strErr.LastIndexOf("<Err>") + arrErrSep[0].Length, strErr.Length - strErr.LastIndexOf("<Err>") - arrErrSep[0].Length);
                        string[] arrOutput = strtemp.Split(arrOutSep, StringSplitOptions.None);

                        for (int i = 0; i < arrOutput.Length - 1; i++)
                            errMsg.mfAddReturnValue(arrOutput[i]);
                    }
                    return errMsg;
                }
                catch //(Exception ex)
                {
                    return errMsg;
                }
                finally
                {
                }
            }
        }
        #endregion
    }

    namespace ORACLE
    {

    }

    namespace ACCESS
    {
        public class SQLS : IDisposable
        {
            private System.ComponentModel.Component m_component;
            private string m_strConnString;
            private static OleDbConnection m_SqlCon;
            private static OleDbTransaction m_Trans;

            #region con
            public string ConnString
            {
                get { return m_strConnString; }
            }

            public OleDbConnection SqlCon
            {
                get { return m_SqlCon; }
            }

            /// <summary>
            /// 생성자 : Web.Config에서 DB연결정보 얻기
            /// </summary>
            public SQLS()
            {
                m_component = new System.ComponentModel.Component();
                try
                {
                    m_strConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\user5\\Documents\\Database1.accdb;";
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
            /// 생성자 : DB연결정보 얻기
            /// </summary>
            /// <param name="strConnection">연결문자</param>
            public SQLS(string strConnection)
            {
                m_component = new System.ComponentModel.Component();
                try
                {
                    m_strConnString = strConnection;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            ~SQLS()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool bolDisposing)
            {
                if (bolDisposing)
                {
                    if (m_component != null)
                        m_component.Dispose();
                }

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

            public void TransactionHandling(int ErrNum)
            {
                try
                {
                    if (ErrNum.Equals(0))
                        m_Trans.Commit();
                    else
                        m_Trans.Rollback();
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

            #region MS-SQL 서버 연결 및 해제
            /// <summary>
            /// MS-ACCESS 서버 연결.
            /// </summary>
            /// <returns>연결여부</returns>
            public bool sql_Connect()
            {
                try
                {
                    //연결정보가 없는 경우
                    if (m_strConnString == "")
                        return false;
                    else
                    {
                        m_SqlCon = new OleDbConnection(m_strConnString);
                        //Open상태가 아니면 Open한다.
                        if (m_SqlCon.State != ConnectionState.Open)
                            m_SqlCon.Open();

                        if (m_SqlCon.State != ConnectionState.Open)
                            return false;
                        else
                            return true;
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

            /// <summary>
            /// MS-ACCESS 서버 연결 끊기.
            /// </summary>
            public void sql_DisConnect()
            {
                try
                {
                    if (m_SqlCon == null) return;

                    if (m_SqlCon.State == ConnectionState.Open)
                    {
                        m_SqlCon.Close();
                        m_SqlCon = null;
                    }
                    else
                        m_SqlCon = null;
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

            #region 저장프로시저 실행

            /// <summary>
            /// 읽기 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
            /// </summary>
            /// <param name="Conn">DB연결객체</param>
            /// <param name="strSPName">SP명</param>
            /// <param name="dtSPParameter">인자 배열</param>
            /// <returns>Select결과</returns>
            public DataTable executeQuery_Proc(OleDbConnection Conn, string strSPName, DataTable dtSPParameter)
            {
                DataTable dt = new DataTable();
                try
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        OleDbCommand m_SqlCmd = new OleDbCommand();
                        m_SqlCmd.Connection = Conn;
                        m_SqlCmd.CommandType = CommandType.StoredProcedure;
                        m_SqlCmd.CommandText = strSPName;
                        foreach (DataRow dr in dtSPParameter.Rows)
                        {
                            OleDbParameter param = new OleDbParameter();
                            param.ParameterName = dr["ParamName"].ToString();
                            param.Direction = (ParameterDirection)dr["ParamDirect"];
                            param.OleDbType = (OleDbType)dr["DBType"];

                            param.Value = dr["Value"].ToString();
                            if (dr["Length"].ToString() != "")
                            {
                                if (Convert.ToInt32(dr["Length"].ToString()) > 0)
                                    param.Size = Convert.ToInt32(dr["Length"].ToString());
                            }
                            m_SqlCmd.Parameters.Add(param);
                        }

                        m_SqlCmd.CommandTimeout = 300;

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
            /// 읽기 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
            /// </summary>
            /// <param name="Conn">DB연결객체</param>
            /// <param name="strQuery">Text쿼리</param>
            /// <returns>Select결과</returns>
            public DataTable executeQuery_Text(OleDbConnection Conn, string strQuery)
            {
                DataTable dt = new DataTable();
                try
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        OleDbCommand m_SqlCmd = new OleDbCommand();
                        m_SqlCmd.Connection = Conn;
                        m_SqlCmd.CommandType = CommandType.Text;
                        m_SqlCmd.CommandText = strQuery;

                        m_SqlCmd.CommandTimeout = 300;

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
            /// 트랜잭션(DML) 저장프로시저 실행 : 인자(데이터테이블)가 있는 경우
            /// </summary>
            /// <param name="Conn">DB연결객체</param>
            /// <param name="Trans">트랜잭션객체</param>
            /// <param name="strSPName">SP명</param>
            /// <param name="dtSPParameter">인자배열</param>
            /// <returns>DML실행결과</returns>
            public string executeTransQuery_Proc(OleDbConnection Conn, OleDbTransaction Trans, string strSPName, DataTable dtSPParameter)
            {
                TransErrRtn Result = new TransErrRtn();
                string strErrRtn = "";
                try
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        OleDbCommand m_SqlCmd = new OleDbCommand();
                        m_SqlCmd.Connection = Conn;
                        m_SqlCmd.Transaction = Trans;
                        m_SqlCmd.CommandType = CommandType.StoredProcedure;
                        m_SqlCmd.CommandText = strSPName;

                        m_SqlCmd.CommandTimeout = 300;

                        foreach (DataRow dr in dtSPParameter.Rows)
                        {
                            OleDbParameter param = new OleDbParameter();
                            param.ParameterName = dr["ParamName"].ToString();
                            param.Direction = (ParameterDirection)dr["ParamDirect"];
                            param.OleDbType = (OleDbType)dr["DBType"];
                            param.IsNullable = true;

                            //if ((OleDbType)dr["DBType"] != OleDbType.Image)
                            //    param.Value = dr["Value"].ToString();
                            //else
                            //    param.Value = (byte[])dr["Value"];

                            if (dr["Length"].ToString() != "")
                            {
                                if (Convert.ToInt32(dr["Length"].ToString()) > 0)
                                    param.Size = Convert.ToInt32(dr["Length"].ToString());
                            }

                            m_SqlCmd.Parameters.Add(param);
                        }

                        string strReturn = Convert.ToString(m_SqlCmd.ExecuteScalar());
                        //m_SqlCmd.ExecuteNonQuery();

                        //처리 결과를 구조체 변수에 저장시킴
                        Result.ErrNum = Convert.ToInt32(m_SqlCmd.Parameters["@Rtn"].Value);
                        Result.ErrMessage = m_SqlCmd.Parameters["@ErrorMessage"].Value.ToString();
                        //Output Param이 있는 경우 ArrayList에 저장시킴.
                        Result.mfInitReturnValue();
                        foreach (DataRow dr in dtSPParameter.Rows)
                        {
                            if (dr["ParamName"].ToString() != "@ErrorMessage" && (ParameterDirection)dr["ParamDirect"] == ParameterDirection.Output)
                            {
                                Result.mfAddReturnValue(m_SqlCmd.Parameters[dr["ParamName"].ToString()].Value.ToString());
                            }
                        }
                        strErrRtn = Result.mfEncodingErrMessage(Result);
                        return strErrRtn;
                    }
                    else
                    {
                        Result.ErrNum = 99;
                        Result.ErrMessage = "DataBase 연결되지 않았습니다.";
                        strErrRtn = Result.mfEncodingErrMessage(Result);
                        return strErrRtn;
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



            #region 저장프로시져에 넘겨줄 Parameter 정의
            /// <summary>
            /// 저장프로시져에 넘겨줄 Parameter 데이터테이블 설정
            /// </summary>
            /// <returns>Parameter테이블</returns>
            public DataTable setParam()
            {
                DataTable dt = null;
                try
                {
                    dt = new DataTable();

                    DataColumn dc = new DataColumn("ParamName", typeof(string));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("ParamDirect", typeof(ParameterDirection));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("DBType", typeof(OleDbType));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Value", typeof(object));
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Length", typeof(int));
                    dt.Columns.Add(dc);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 저장프로시져에 넘겨줄 Parameter 추가
            /// </summary>
            /// <param name="dt">Parameter 테이블</param>
            /// <param name="strName">Paramter 명</param>
            /// <param name="Direction">Parameter 방향(in/out)</param>
            /// <param name="DBType">Parameter DB유형</param>
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, OleDbType DBType)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    //dr["Length"] = intSize;
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, OleDbType DBType, int intSize)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    dr["Length"] = intSize;
                    dt.Rows.Add(dr);
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
            /// 저장프로시져에 넘겨줄 Parameter 추가
            /// </summary>
            /// <param name="dt">Parameter 테이블</param>
            /// <param name="Direction">Parameter 방향(in/out)</param>
            /// <param name="DBType">Parameter DB유형</param>
            /// <param name="strValue">Paramter 인자값</param>
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, OleDbType DBType, string strValue)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    dr["Value"] = strValue;
                    //dr["Length"] = intSize;
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                //finally
                //{
                //}
            }
            public void addParam(DataTable dt, string strName, ParameterDirection Direction, OleDbType DBType, string strValue, int intSize)
            {
                try
                {
                    DataRow dr = dt.NewRow();
                    dr["ParamName"] = strName;
                    dr["ParamDirect"] = Direction;
                    dr["DBType"] = DBType;
                    dr["Value"] = strValue;
                    dr["Length"] = intSize;
                    dt.Rows.Add(dr);
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

        #region 트랜잭션 처리시 Return 처리 Class
        [Serializable]
        public class TransErrRtn
        {
            private int intErrNum;              //에러번호
            private string strErrMessage;       //에러메세지    
                                                //private ArrayList arrReturnValue;   //반환할 결과값
            private ArrayList arrReturnValue = new ArrayList();
            private string strSystemMessage;
            private string strSystemStackTrace;
            private string strSystemInnerException;
            private string strInterfaceResultCode;
            private string strInterfaceResultMessage;

            public int ErrNum
            {
                get { return intErrNum; }
                set { intErrNum = value; }
            }

            public string ErrMessage
            {
                get { return strErrMessage; }
                set { strErrMessage = value; }
            }

            public string SystemMessage
            {
                get { return strSystemMessage; }
                set { strSystemMessage = value; }
            }

            public string SystemStackTrace
            {
                get { return strSystemStackTrace; }
                set { strSystemStackTrace = value; }
            }

            public string SystemInnerException
            {
                get { return strSystemInnerException; }
                set { strSystemInnerException = value; }
            }

            public string InterfaceResultCode
            {
                get { return strInterfaceResultCode; }
                set { strInterfaceResultCode = value; }
            }

            public string InterfaceResultMessage
            {
                get { return strInterfaceResultMessage; }
                set { strInterfaceResultMessage = value; }
            }

            public TransErrRtn()
            {
                intErrNum = 0;
                strErrMessage = "";
                //arrReturnValue = null;
                //arrReturnValue.Clear();
                strSystemMessage = "";
                strSystemStackTrace = "";
                strSystemInnerException = "";
                strInterfaceResultCode = "";
                strInterfaceResultMessage = "";
            }

            /// <summary>
            /// 리턴값 배열 초기화
            /// </summary>
            public void mfInitReturnValue()
            {
                arrReturnValue.Clear();
            }

            /// <summary>
            /// 리턴값 배열에 값을 추가
            /// </summary>
            /// <param name="strValue"></param>
            public void mfAddReturnValue(string strValue)
            {
                arrReturnValue.Add(strValue);
            }

            /// <summary>
            /// 리턴갑 배열에 값을 삭제
            /// </summary>
            /// <param name="intIndex"></param>
            public void mfDeleteReturnValue(int intIndex)
            {
                arrReturnValue.Remove(intIndex);
            }

            /// <summary>
            /// 리턴값 배열 얻기
            /// </summary>
            /// <returns></returns>
            public ArrayList mfGetReturnValue()
            {
                return arrReturnValue;
            }

            /// <summary>
            /// 리턴갑 배열중 특정값 얻기
            /// </summary>
            /// <param name="intIndex"></param>
            /// <returns></returns>
            public string mfGetReturnValue(int intIndex)
            {
                if (arrReturnValue.Count >= intIndex + 1)
                {
                    return arrReturnValue[intIndex].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// 에러메세지 구조체 정보를 문자열로 변환
            /// </summary>
            /// <param name="Err">트랜잭션처리정보 구조체</param>
            /// <returns>Encoding값</returns>
            public string mfEncodingErrMessage(TransErrRtn Err)
            {
                string strErr = "";
                string strErrSep = "<Err>";
                string strOutSep = "<OUT>";
                try
                {
                    strErr = Err.intErrNum.ToString() + strErrSep +
                             Err.strErrMessage + strErrSep +
                             Err.strSystemMessage + strErrSep +
                             Err.strSystemStackTrace + strErrSep +
                             Err.strSystemInnerException + strErrSep +
                             Err.strInterfaceResultCode + strErrSep +       //추가
                             Err.strInterfaceResultMessage + strErrSep;     //추가

                    if (Err.arrReturnValue.Count > 0)
                    {
                        //strErr = strErr + "OUTPUT";
                        for (int i = 0; i < Err.arrReturnValue.Count; i++)
                        {
                            strErr = strErr + Err.arrReturnValue[i].ToString() + strOutSep;
                        }
                    }
                    return strErr;

                }
                catch //(Exception ex)
                {
                    return strErr;
                }
                finally
                {
                }
            }

            /// <summary>
            /// 에러메시지 문자를 구조체로 변환
            /// </summary>
            /// <param name="strErr">트랜잭션처리정보 문자열</param>
            /// <returns>Decoding값</returns>
            public TransErrRtn mfDecodingErrMessage(string strErr)
            {
                TransErrRtn errMsg = new TransErrRtn();
                try
                {
                    string[] arrErrSep = { "<Err>" };
                    string[] arrOutSep = { "<OUT>" };

                    string[] arrErrMsg = strErr.Split(arrErrSep, StringSplitOptions.None);

                    errMsg.intErrNum = Convert.ToInt32(arrErrMsg[0]);
                    if (arrErrMsg.Length > 1)
                        errMsg.strErrMessage = arrErrMsg[1];

                    if (arrErrMsg.Length > 2)
                        errMsg.strSystemMessage = arrErrMsg[2];

                    if (arrErrMsg.Length > 3)
                        errMsg.strSystemStackTrace = arrErrMsg[3];

                    if (arrErrMsg.Length > 4)
                        errMsg.strSystemInnerException = arrErrMsg[4];

                    if (arrErrMsg.Length > 5)
                        errMsg.strInterfaceResultCode = arrErrMsg[5];       //추가

                    if (arrErrMsg.Length > 6)
                        errMsg.strInterfaceResultMessage = arrErrMsg[6];    //추가


                    if (strErr.Split(arrOutSep, StringSplitOptions.None).Length > 0)
                    {
                        string strtemp = strErr.Substring(strErr.LastIndexOf("<Err>") + arrErrSep[0].Length, strErr.Length - strErr.LastIndexOf("<Err>") - arrErrSep[0].Length);
                        string[] arrOutput = strtemp.Split(arrOutSep, StringSplitOptions.None);

                        for (int i = 0; i < arrOutput.Length - 1; i++)
                            errMsg.mfAddReturnValue(arrOutput[i]);
                    }
                    return errMsg;
                }
                catch //(Exception ex)
                {
                    return errMsg;
                }
                finally
                {
                }
            }
        }
        #endregion
    }
}
