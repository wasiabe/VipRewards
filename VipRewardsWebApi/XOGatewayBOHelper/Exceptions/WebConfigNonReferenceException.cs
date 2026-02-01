/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/04/18
 * 程式說明：自訂錯誤處理類別
 * 
 * 修正記錄：
******************************************************/

using System;

namespace Cardif.PWS.XOGatewayBOHelper.Exceptions
{
    /// <summary>
    /// 表示 Web.Config 組態檔設定錯誤或未設定相關組態的例外類別。
    /// </summary>
    public class WebConfigNonReferenceException : ApplicationException
    {
        public WebConfigNonReferenceException(string message)
            : base(message)
        {
        }

        public WebConfigNonReferenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}