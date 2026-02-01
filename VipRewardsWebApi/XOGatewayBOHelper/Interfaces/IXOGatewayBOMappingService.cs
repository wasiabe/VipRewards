/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：XOGatewayBOMappingService的Interface
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * a.修正命名空間為Cardif.PWS.XOGatewayBOHelper.Interfaces。
******************************************************/

using Cardif.PWS.XOGatewayBOHelper.BusinessEntities;

namespace Cardif.PWS.XOGatewayBOHelper.Interfaces
{
    /// <summary>
    /// XOGatewayBOMappingService的Interface。
    /// </summary>
    interface IXOGatewayBOMappingService
    {

        /// <summary>
        /// 轉換XOGatewayBOHelper的XOINPARAMEntity物件為XOGatewayBO的XOINPARAMEntity物件。
        /// </summary>
        /// <param name="xoINPARAM">XOGatewayBOHelper的XOINPARAMEntity物件。</param>
        /// <returns>XOGatewayBO層的XOINPARAMEntity物件。</returns>
        XOGatewayBO.XOINPARAMEntity TransforINPARAM(XOINPARAMEntity xoINPARAM);

        /// <summary>
        /// 轉換XOGatewayBO的XOResultDataEntity物件為XOGatewayBOHelper的XOResultDataEntity物件。
        /// </summary>
        /// <param name="xoResult">XOGatewayBO層的XOResultDataEntity物件。</param>
        /// <returns>XOGatewayBOHelper的XOResultDataEntity物件。</returns>
        XOResultDataEntity TransforResultDataEntity(XOGatewayBO.XOResultDataEntity xoSourceResult, XOINPARAMEntity xoBoINPARAM);

    }
}
