/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：XO輸入參數欄位類別。
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * a.修正命名空間為Cardif.PWS.XOGatewayBOHelper.BusinessEntities。
 * 
 * 2008/04/23 - Amy Hung
 * a.增加FieldProperty變數。用以表示為一般欄位或是起迄日期查詢欄位。
 * 
 * 2017/07/11 - Chris Su - CR-229129
 * a.修改檔案編碼為UTF8
 * b.移除非必要的namespace
******************************************************/

namespace Cardif.PWS.XOGatewayBOHelper.BusinessEntities
{

    /// <summary>
    /// XO輸入參數欄位。
    /// </summary>
    internal struct XOFieldEntity
    {

        /// <summary>
        /// 欄位名稱。
        /// </summary>
        public string FieldName;

        /// <summary>
        /// 欄位值。
        /// </summary>
        public string FieldValue;

        /// <summary>
        /// 欄位的屬性。
        /// </summary>
        /// <remarks>N：表示一般的查詢欄位。D:表示是起迄日期的查詢欄位。</remarks>
        public string FieldProperty;

    }

}
