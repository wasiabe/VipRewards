/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/06/09
 * 程式說明：XOGatewayLOGParse的介面。
 * 
 * 修正記錄：
******************************************************/

namespace Cardif.PWS.XOGatewayBOHelper.Interfaces
{
    public interface IXOGatewayLOGParse
    {
        /// <summary>
        /// 取得GatewayLog的查詢條件。
        /// </summary>
        /// <param name="xoInputData">InputData欄位值。</param>
        /// <returns>查詢條件(查詢欄位及查詢值)。</returns>
        string GetSearchConditions(string xoInputData);
    }
}
