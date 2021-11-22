using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace shDB.MSAccess
{
    public class CSQL : IDisposable
    {
        private readonly string m_sConnectionString;
        private static OleDbConnection m_SqlCon;
        private static OleDbTransaction m_Trans;

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
        //        MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return false;
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return null;
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
                return null;
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
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
                MessageOut($"[{System.Reflection.MethodBase.GetCurrentMethod().Name}] {ex}");
            }
            //finally
            //{
            //}
        }

        #endregion
    }
}
