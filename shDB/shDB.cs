using System;
using System.Collections.Generic;

namespace shDB
{
    #region Transaction class
    public class CTransactionErr
    {
        int nErrNum;
        string sErrMsg;
        string sException;
        List<string> arrOutput;

        const string sErrSep = "<Err>";
        const string sOutSep = "<Out>";

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
        /// <returns>Encoding value</returns>
        public string EncodingErrTransaction()
        {
            string sErr = "";

            try
            {
                sErr = nErrNum.ToString() + sErrSep +
                       sErrMsg + sErrSep +
                       sException + sErrSep;

                foreach (var item in arrOutput)
                {
                    sErr += item + sOutSep;
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
        public static CTransactionErr DecodingErrTransaction(string _sErr)
        {
            CTransactionErr ErrTrans = new CTransactionErr();

            string[] arrErrSep = { sErrSep };
            string[] arrOutSep = { sOutSep };

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