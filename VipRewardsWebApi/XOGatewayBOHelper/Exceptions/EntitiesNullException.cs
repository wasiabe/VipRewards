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
    /// 表示BOHelper層的Entity物件為Null的例外情況。
    /// </summary>
    public class EntitiesNullException : ApplicationException
    {
        public EntitiesNullException(string message)
            : base(message)
        {
        }

        public EntitiesNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }

}