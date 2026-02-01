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
    /// 表示呼叫XOGatewayBO層時發生的例外情況。
    /// </summary>
    class CallXOGatewayBOException : ApplicationException
    {

        public CallXOGatewayBOException(string message)
            : base(message)
        {
        }

        public CallXOGatewayBOException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
