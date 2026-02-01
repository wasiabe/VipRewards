/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：XO輸入參數類別。
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * 修正命名空間為Cardif.PWS.XOGatewayBOHelper.BusinessEntities。
 * 
 * 2008/04/23 - Amy Hung
 *  增加AppendDateField方法。若查詢欄位為起迄日期的查詢，則要呼叫此方法。
 * 
 * 2008/06/09 - Amy Hung
 * 增加ChineseFieldName(Input查詢欄位的中文名稱)屬性。
 * 為提供報表程式可以取得中文名稱的查詢欄位，故而修改程式將中文的欄位
 * 名稱，也傳入後端供報表程式利用。
 * 
 * 2014/10/29 - Chris Su - SR14090198
 * a.同步CTI XO Interface
 * b.移除不需要的命名空間
 * 
 * 2017/07/11 - Chris Su - CR-229129
 * a.修改查詢的分頁預設值
******************************************************/

using Cardif.PWS.XOGatewayBOHelper.Exceptions;
using System;
using System.Collections.Generic;

namespace Cardif.PWS.XOGatewayBOHelper.BusinessEntities
{

    /// <summary>
    /// XO輸入參數物件。
    /// </summary>
    public class XOINPARAMEntity
    {

        #region 私有成員宣告

        /// <summary>
        /// 請求該次交易查詢的唯一編號。
        /// </summary>
        private string _RequestUniqueKey = "";

        /// <summary>
        /// 系統代號
        /// </summary>
        private string _SYSID = "PWS";

        /// <summary>
        /// 交易ID
        /// </summary>
        private string _TRANID = "";

        /// <summary>
        /// XO輸入參數
        /// </summary>
        private string _XO_INPUT = "";

        /// <summary>
        /// XO輸出參數
        /// </summary>
        private string _XO_OUTPUT = "";

        /// <summary>
        /// XO功能別
        /// </summary>
        private string _XO_FNTYPE = "";

        /// <summary>
        /// XO Class
        /// </summary>
        private string _XO_CLASS = "p_xo_servercom";

        /// <summary>
        /// XO Package
        /// </summary>
        private string _XO_PACKAGE = "tools";

        /// <summary>
        /// XO_Method
        /// </summary>
        private string _XO_METHOD = "servercom_trigger";

        /// <summary>
        /// UI_INPUT
        /// </summary>
        private string _UI_INPUT = "";

        /// <summary>
        /// UI_OUTPUT
        /// </summary>
        private string _UI_OUTPUT = "";

        /// <summary>
        /// UI_OUTPUT_HASH
        /// </summary>
        private string _UI_OUTPUT_HASH = "";

        /// <summary>
        /// 查詢欄位的中文名稱。
        /// </summary>
        /// <remarks>
        /// Amy Hung 2008/06/09：新增。
        /// </remarks>
        private string _ChineseFieldName = "";

        /// <summary>
        /// 每頁要顯示的記錄筆數。
        /// </summary>
        private int _PerPageRecord = 0;

        /// <summary>
        /// 要顯示的頁數。
        /// </summary>
        private int _ShowPageIndex = 0;

        /// <summary>
        /// XO輸入參數查詢條件的List。
        /// 用來存放查詢條件的欄位名稱及欄位值
        /// </summary>
        private List<XOFieldEntity> _XoFieldList;

        /// <summary>
        /// 公司別
        /// </summary>
        /// <remarks>
        /// Chris 2011/11/03 新增
        /// </remarks>
        private string _Company = "";

        #endregion

        #region 建構子
        public XOINPARAMEntity()
        {
            _XoFieldList = new List<XOFieldEntity>();
            _RequestUniqueKey = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

            _PerPageRecord = 1;
            _ShowPageIndex = 1;
        }
        #endregion

        #region 存取子宣告

        public string RequestUniqueKey
        {
            get { return _RequestUniqueKey; }
        }

        public string SYSID
        {
            get { return _SYSID; }
            set { _SYSID = value; }
        }

        public string TRANID
        {
            get { return _TRANID; }
            set { _TRANID = value; }
        }

        public string XO_INPUT
        {
            get { return _XO_INPUT; }
            set { _XO_INPUT = value; }
        }

        public string XO_OUTPUT
        {
            get { return _XO_OUTPUT; }
            set { _XO_OUTPUT = value; }
        }

        public string XO_FNTYPE
        {
            get { return _XO_FNTYPE; }
            set { _XO_FNTYPE = value; }
        }

        public string XO_CLASS
        {
            get { return _XO_CLASS; }
            set { _XO_CLASS = value; }
        }

        public string XO_PACKAGE
        {
            get { return _XO_PACKAGE; }
            set { _XO_PACKAGE = value; }
        }

        public string XO_METHOD
        {
            get { return _XO_METHOD; }
            set { _XO_METHOD = value; }
        }

        public string UI_INPUT
        {
            get { return _UI_INPUT; }
            set { _UI_INPUT = value; }
        }

        public string UI_OUTPUT
        {
            get { return _UI_OUTPUT; }
            set { _UI_OUTPUT = value; }
        }

        public string UI_OUTPUT_HASH
        {
            get { return _UI_OUTPUT_HASH; }
            set { _UI_OUTPUT_HASH = value; }
        }

        /// <remarks>
        /// Amy Hung - 2008/06/09：新增。
        /// </remarks>
        public string ChineseFieldName
        {
            get { return _ChineseFieldName; }
            set { _ChineseFieldName = value; }
        }

        public int PerPageRecord
        {
            get { return _PerPageRecord; }
            set { _PerPageRecord = value; }
        }

        public int ShowPageIndex
        {
            get { return _ShowPageIndex; }
            set { _ShowPageIndex = value; }
        }

        internal List<XOFieldEntity> XoFieldList
        {
            get { return _XoFieldList; }
        }

        /// <remarks>
        /// Chris 2011/11/03 新增
        /// </remarks>
        public string Company
        {
            get { return _Company; }
            set { _Company = value; }
        }

        #endregion

        #region 方法成員宣告

        /// <summary>
        /// 加入一般查詢欄位的XO輸入參數條件。
        /// </summary>
        /// <param name="FieldName">查詢條件的欄位名稱。</param>
        /// <param name="FieldValue">查詢條件的欄位值。</param>
        public void AppendXOField(string FieldName, string FieldValue)
        {
            XOFieldEntity xoItem;
            xoItem.FieldName = FieldName;
            xoItem.FieldValue = FieldValue.Trim();
            xoItem.FieldProperty = "N"; // N 表示為一般的查詢欄位條件。
            _XoFieldList.Add(xoItem);
        }

        /// <summary>
        /// 加入起迄日期查詢欄位的XO輸入參數條件。
        /// </summary>
        /// <param name="FieldName">查詢條件的欄位名稱。</param>
        /// <param name="startDate">起始日期。</param>
        /// <param name="endDate">結束日期。</param>
        public void AppendDateRangeField(string FieldName, string startDate, string endDate)
        {

            DateTime StartDate;
            DateTime EndDate;

            // 起始日期及結束日期若要輸入，則必須兩者都輸入，不可以只輸入一個。
            if ((startDate != "" & endDate == "") || (startDate == "" & endDate != ""))
            {
                AppendDateFieldException ex = new AppendDateFieldException(
                                                   "呼叫 AppendDateField 時發生錯誤，"
                                                   + "起始日期及結束日期必須同時輸入。");
                throw ex;
            }

            // 判斷起始日期是否為正確的日期格式。
            if (startDate != "")
            {
                if (!DateTime.TryParse(startDate, out StartDate))
                {
                    AppendDateFieldException ex = new AppendDateFieldException(
                                                    "呼叫 AppendDateField 時發生錯誤，"
                                                    + " 傳入的起始日期 \" " + endDate + " \" 不是正確的日期格式。");
                    throw ex;
                }
            }

            // 判斷結束日期是否為正確的日期格式。
            if (endDate != "")
            {
                if (!DateTime.TryParse(endDate, out EndDate))
                {
                    AppendDateFieldException ex = new AppendDateFieldException(
                                                    "呼叫 AppendDateField 時發生錯誤，"
                                                    + " 傳入的結束日期 \" " + endDate + " \" 不是正確的日期格式。");
                    throw ex;
                }
            }

            // 判斷起始日期不可以大於結束日期。
            if (startDate != "" & endDate != "")
            {
                if (DateTime.Parse(startDate) > DateTime.Parse(endDate))
                {
                    AppendDateFieldException ex = new AppendDateFieldException(
                                                        "呼叫 AppendDateField 時發生錯誤，"
                                                        + "起始日期不可以大於結束日期。");
                    throw ex;
                }
            }

            XOFieldEntity xoItem;
            xoItem.FieldName = FieldName.Trim();
            xoItem.FieldValue = startDate + "," + endDate;
            xoItem.FieldProperty = "D"; // D 表示為有起迄日期的查詢條件。
            _XoFieldList.Add(xoItem);

        }

        #endregion

    }

}
