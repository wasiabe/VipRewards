/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：XOGateway查詢結果類別。
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * 修正命名空間為Cardif.PWS.XOGatewayBOHelper.Services。
 * 
 * 2017/07/03 - Chris Su - CR-229129
 * a.擴充屬性RecordCount、ResultRows
******************************************************/

using System.Data;

namespace Cardif.PWS.XOGatewayBOHelper.BusinessEntities
{

    /// <summary>
    /// XOGateway查詢結果物件。
    /// </summary>
    public class XOResultDataEntity
    {

        #region 私有成員宣告

        /// <summary>
        /// 錯誤代碼。
        /// </summary>
        private string _RetCode = "";

        /// <summary>
        /// 錯誤原因。
        /// </summary>
        private string _Reason = "";

        /// <summary>
        /// 錯誤訊息。
        /// </summary>
        private string _ErrMsg = "";

        /// <summary>
        /// 查詢結果總記錄筆數。
        /// </summary>
        private string _RetCount = "";

        /// <summary>
        /// 查詢結果資料(ResultSet)。
        /// </summary>
        private DataSet _ResultSet;

        #endregion

        #region 建構子
        public XOResultDataEntity()
        {
            _ResultSet = new DataSet();
        }
        #endregion

        #region 存取子宣告

        /// <summary>
        /// 錯誤代碼。
        /// </summary>
        public string RetCode
        {
            get { return _RetCode; }
            set { _RetCode = value; }
        }

        /// <summary>
        /// 錯誤原因。
        /// </summary>
        public string Reason
        {
            get { return _Reason; }
            set { _Reason = value; }
        }

        /// <summary>
        /// 錯誤訊息。
        /// </summary>
        public string ErrMsg
        {
            get { return _ErrMsg; }
            set { _ErrMsg = value; }
        }

        /// <summary>
        /// 查詢結果總記錄筆數。
        /// </summary>
        public string RetCount
        {
            get { return _RetCount; }
            set { _RetCount = value; }
        }

        /// <summary>
        /// 查詢結果總記錄筆數(int型態)
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 查詢結果資料(ResultSet)。
        /// </summary>
        public DataSet ResultSet
        {
            get { return _ResultSet; }
            set { _ResultSet = value; }
        }

        /// <summary>
        /// 查詢結果資料(DataTable)
        /// </summary>
        public DataTable ResultTable { get; set; }

        /// <summary>
        /// 資料集合DataRows
        /// </summary>
        public DataRowCollection ResultRows { get; set; }

        /// <summary>
        /// XO查詢的第一筆查詢結果，若無資料的情況會回傳null值
        /// </summary>
        public DataRow FirstRow { get; set; }

        #endregion

    }

}
