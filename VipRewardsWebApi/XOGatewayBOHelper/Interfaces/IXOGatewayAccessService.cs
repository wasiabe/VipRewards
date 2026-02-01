/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：XOGatewayAccessServiceX的Interface。
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * 修正命名空間為Cardif.PWS.XOGatewayBOHelper.Interfaces。
 * 
 * 2014/10/29 - Chris Su - SR14090198
 * a.同步CTI XO Interface
 * b.移除不需要的命名空間
******************************************************/

using Cardif.PWS.XOGatewayBOHelper.BusinessEntities;

namespace Cardif.PWS.XOGatewayBOHelper.Interfaces
{
    /// <summary>
    /// XOGatewayAccessServiceX的Interface。
    /// </summary>
    public interface IXOGatewayAccessService
    {
        /// <summary>
        /// 呼叫XOGatewayBO層，取得XOGateway資料查詢結果。
        /// </summary>
        /// <param name="xoINPARAM">XOGatewayBOHelper的XO輸入參數物件(XOINPARAMEntity)。</param>
        /// <returns>
        /// XOGatewayBOHelper的查詢結果物件(XOResultDataEntity)。
        /// </returns>
        XOResultDataEntity GetResultDataEntity(XOINPARAMEntity xoINPARAM);

        /// <summary>
        /// 呼叫XOGatewayBO層，取得XOGateway資料查詢結果。
        /// </summary>
        /// <param name="xoINPARAM">XOGatewayBOHelper的XO輸入參數物件(XOINPARAMEntity)。</param>
        /// <param name="parTimeout">指定Timeout時間(單位:秒數)</param>
        /// <returns>
        /// XOGatewayBOHelper的查詢結果物件(XOResultDataEntity)。
        /// </returns>
        XOResultDataEntity GetResultDataEntity(XOINPARAMEntity xoINPARAM, int parTimeout);



        /// <summary>
        /// 取得執行電文字串
        /// </summary>
        /// <param name="parXML">上行電文XML</param>
        /// <returns></returns>
        /// <remarks>
        /// 2012/03/01 Chris新增，給CTI呼叫Web Service用
        /// </remarks>
        string GetResultDataXML(string parXML);

        /// <summary>
        /// 取得執行電文字串
        /// </summary>
        /// <param name="parXML">上行電文XML</param>
        /// <param name="parTimeout">指定Timeout時間(單位:秒數)</param>
        /// <returns></returns>
        /// <remarks>
        /// 2012/03/01 Chris新增，給CTI呼叫Web Service用
        /// </remarks>
        string GetResultDataXML(string parXML, int parTimeout);

        /// <summary>
        /// 根據XML轉換成XO可識別的程式碼
        /// </summary>
        /// <param name="parXMLString">XML字串</param>
        /// <returns>
        /// 回傳XO Code
        /// </returns>
        string ParseXOCode(string parXMLString);

        /// <summary>
        /// 根據XML轉換成XO可識別的程式碼
        /// </summary>
        /// <param name="parXMLString">XML字串</param>
        /// <param name="parCompany">公司別</param>
        /// <returns>
        /// 回傳XO Code
        /// </returns>
        string ParseXOCode(string parXMLString, string parCompany);
    }
}
