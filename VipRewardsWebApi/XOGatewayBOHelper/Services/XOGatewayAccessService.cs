/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：提供對XOGatewayBO層進行資料查詢服務的輔助服務。
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * 修正命名空間為Cardif.PWS.XOGatewayBOHelper.Services。
 * 
 * 2008/06/24 - Amy Hung
 * 修正GetResultDataEntity()方法，當查詢發生錯誤時，不直接回丟錯誤，
 * 改為回傳ResultDataEntity物件，將錯誤訊息及錯誤編號設定到物件的相對屬性中供PWS判斷。
 * 
 * 2012/02/24 - Chris Su
 * 增加公司別區隔
 * 增加Timeout動動態設定方法
 * 
 * 2014/10/29 - Chris Su
 * a.固定化XOGatewayToken屬性
 * 
 * 2017/07/11 - Chris Su - CR-229129
 * a.修正尋找WebService的判斷邏輯
******************************************************/

using Cardif.PWS.XOGatewayBOHelper.BusinessEntities;
using Cardif.PWS.XOGatewayBOHelper.Exceptions;
using Cardif.PWS.XOGatewayBOHelper.Interfaces;
using System;
using System.Configuration;
using System.Net.Http;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace Cardif.PWS.XOGatewayBOHelper.Services
{
    /// <summary>
    /// 提供對XOGatewayBO層進行資料查詢服務的輔助服務。
    /// </summary>
    public class XOGatewayAccessService : IXOGatewayAccessService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public XOGatewayAccessService()
        {
        }

        public XOGatewayAccessService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        #region 私有方法

        /// <summary>
        /// 檢查XOINPARAMEntity物件是否為null及Web.config組態檔設定。
        /// </summary>
        /// <param name="xoEntity">XOINPARAMEntity物件</param>
        private void RequestCheck(XOINPARAMEntity xoEntity)
        {

            // 傳入之Entity不能為null。
            if (xoEntity == null)
            {
                EntitiesNullException ex = new EntitiesNullException("傳入之 XOINPARAMEntity 物件為 null。");
                throw ex;
            }

            xoEntity.Company = _configuration.GetSection($"XOGatewayBO:Company").Value ?? string.Empty;
            string ConfigKeyName = string.Format("{0}XOGatewayBOWebService", (xoEntity.Company == "") ? "" : xoEntity.Company + "_");

            bool hasConfigUrl = _configuration.GetSection($"XOGatewayBO:BaseUrl").Value != null;
            bool hasBaseAddress = _httpClient?.BaseAddress != null;

            if (!hasConfigUrl && !hasBaseAddress)
            {
                WebConfigNonReferenceException ex = new WebConfigNonReferenceException(
                                                "Web.config檔中沒有指定參數" + ConfigKeyName + "。");

                throw ex;
            }



            //2012/04/23 Chris 新增參數，加強Web Service的安全性            
            if (_configuration.GetSection($"XOGatewayBO:XOGatewayToken").Value == null)
            {
                WebConfigNonReferenceException ex = new WebConfigNonReferenceException(
                                                "Web.config檔中沒有指定參數XOGatewayToken。");

                throw ex;
            }

            if (xoEntity.PerPageRecord == -1 || xoEntity.ShowPageIndex == -1)
            {
                throw new Exception("未指定正確的頁碼，請檢查參數PerPageRecord與ShowPageIndex是否有進行設定!");
            }
        }

        /// <summary>
        /// 發生錯誤時取得回傳給PWS用的ResultEntity物件。
        /// </summary>
        /// <param name="ex">錯誤物件。</param>
        /// <returns>錯誤的查詢結果物件。</returns>
        private XOResultDataEntity GetErrorResultEntity(Exception ex)
        {

            XOResultDataEntity ErrorXOResult = new XOResultDataEntity();

            ErrorXOResult.ErrMsg = ex.Message;
            ErrorXOResult.RetCode = "999";

            return ErrorXOResult;

        }

        /// <summary>
        /// 建立XOGatewayBO的程序
        /// </summary>
        /// <param name="parCompany">公司別</param>
        /// <returns></returns>
        private XOGatewayBO.XOGatewayBOService GetXOGatewayBOService(string parCompany)
        {
            // 建立XOGatewayBO層的服務實體。
            XOGatewayBO.XOGatewayBOService xoService = _httpClient == null
                ? new XOGatewayBO.XOGatewayBOService()
                : new XOGatewayBO.XOGatewayBOService(_httpClient);

            XOGatewayBO.Credentials _Credentials = new XOGatewayBO.Credentials();
            _Credentials.Token = _configuration.GetSection("XOGatewayBO:XOGatewayToken").Value.ToString() ?? string.Empty;

            xoService.CredentialsValue = _Credentials;

            xoService.Url = _configuration.GetSection("XOGatewayBO:Url").Value.ToString() ?? string.Empty
                ?? _httpClient?.BaseAddress?.ToString();

            return xoService;
        }

        #endregion

        #region IXOGatewayAccessService 成員

        /// <summary>
        /// 呼叫XOGatewayBO層，取得XOGateway資料查詢結果。
        /// </summary>
        /// <param name="xoINPARAM">XOGatewayBOHelper的XO輸入參數物件(XOINPARAMEntity)。</param>
        /// <returns>
        /// XOGatewayBOHelper的查詢結果物件(XOResultDataEntity)。
        /// </returns>
        public XOResultDataEntity GetResultDataEntity(XOINPARAMEntity xoINPARAM)
        {
            return GetResultDataEntity(xoINPARAM, -1);
        }

        /// <summary>
        /// 呼叫XOGatewayBO層，取得XOGateway資料查詢結果。
        /// </summary>
        /// <param name="xoINPARAM">XOGatewayBOHelper的XO輸入參數物件(XOINPARAMEntity)。</param>
        /// <param name="parTimeout">指定Timeout時間</param>
        /// <returns>
        /// XOGatewayBOHelper的查詢結果物件(XOResultDataEntity)。
        /// </returns>
        public XOResultDataEntity GetResultDataEntity(XOINPARAMEntity helperXOINPARAM, int parTimeout)
        {
            // 檢查組態設定及物件。
            this.RequestCheck(helperXOINPARAM);

            XOGatewayBO.XOINPARAMEntity BoXOINPARAM = null; // BO層的XOINPARAMEntity。
            XOGatewayBO.XOResultDataEntity BoXOResult = null; // BO層的XOResultDataEntity。
            XOResultDataEntity HelperXOResult = null;  // Helper層的XOResultDataEntity

            // 建立BOHelper和BO層之間轉換物件服務的實體。
            IXOGatewayBOMappingService xoMapping = new XOGatewayBOMappingService();

            // 轉換XOGatewayBOHelper的XO輸入參數物件(XOINPARAMEntity)為XOGatewayBO的XOINPARAMEntity。
            try
            {
                BoXOINPARAM = xoMapping.TransforINPARAM(helperXOINPARAM);
            }
            catch (Exception ex)
            {

                EntityFormatterException CustomException = new EntityFormatterException(
                                            "轉換 XOGatewayBOHelper 的 XOINPARAMEntity 物件時發生錯誤", ex);

                return this.GetErrorResultEntity(CustomException);

            }

            // 建立XOGatewayBO層的服務實體。
            XOGatewayBO.XOGatewayBOService xoService = GetXOGatewayBOService(helperXOINPARAM.Company);

            //假如指定的Timeout時間不為-1
            //表示可以指定Timeout時效
            if (parTimeout != -1)
            {
                xoService.Timeout = parTimeout * 1000;
            }


            // 用try抓取連接XOGatewayBO的Web Service可能發生的錯誤。
            // 並拋出自訂的例外訊息，提示較明確的錯誤原因。
            try
            {
                // 呼叫XOGatewayBO層取得查詢結果。
                BoXOResult = xoService.GetXOGatewayResult(BoXOINPARAM);
            }
            catch (System.Net.WebException ex)
            {

                CallXOGatewayBOException CustomException
                            = new CallXOGatewayBOException("連接 XOGatewayBO WebService 時發生錯誤，"
                              + "可能的原因為設定的 WebService 網址有誤、"
                              + "目標站台未啟動，或是連線逾時。", ex);

                return this.GetErrorResultEntity(CustomException);

            }
            catch (Exception ex)
            {

                CallXOGatewayBOException CustomException
                        = new CallXOGatewayBOException("查詢 Gateway 時發生錯誤，資料查詢失敗！", ex);

                return this.GetErrorResultEntity(CustomException);

            }

            // 將XOGatewayBO層傳回之查詢結果轉換為XOGatewayBOHelper的查詢結果物件(XOResultDataEntity)。
            try
            {
                HelperXOResult = xoMapping.TransforResultDataEntity(BoXOResult, helperXOINPARAM);
            }
            catch (Exception ex)
            {

                EntityFormatterException CustomException
                        = new EntityFormatterException("轉換 XOGatewayBO 的 XOResultDataEntity 物件時發生錯誤", ex);

                return this.GetErrorResultEntity(ex);

            }

            // 回傳。
            return HelperXOResult;

        }


        /// <summary>
        /// 取得執行電文字串
        /// </summary>
        /// <param name="parXML">上行電文XML</param>
        /// <returns></returns>
        public string GetResultDataXML(string parXML)
        {
            return GetResultDataXML(parXML, -1);
        }

        /// <summary>
        /// 取得執行電文字串
        /// </summary>
        /// <param name="parXML">上行電文XML</param>
        /// <param name="parTimeout">指定Timeout時間(單位:秒數)</param>
        /// <returns></returns>
        public string GetResultDataXML(string parXML, int parTimeout)
        {

            XmlDocument _XmlDocument = new XmlDocument();

            _XmlDocument.LoadXml(parXML);

            string Company = _XmlDocument.SelectSingleNode("//InputData//Company").InnerText;

            // 建立XOGatewayBO層的服務實體。
            // 設定XOGatewayBO的Web Service網址。
            // 2012/02/13 - Chris - 根據公司別決定參數
            // 建立XOGatewayBO層的服務實體。
            XOGatewayBO.XOGatewayBOService xoService = GetXOGatewayBOService(Company);

            //假如指定的Timeout時間不為-1
            //表示可以指定Timeout時效
            if (parTimeout != -1)
            {
                xoService.Timeout = parTimeout * 1000;
            }

            string XOResultXML = "";
            // 用try抓取連接XOGatewayBO的Web Service可能發生的錯誤。
            // 並拋出自訂的例外訊息，提示較明確的錯誤原因。
            try
            {
                // 呼叫XOGatewayBO層取得查詢結果。
                XOResultXML = xoService.GetXOGatewayResultXML(parXML);
            }
            catch
            {
                throw;

            }


            // 回傳。
            return XOResultXML;
        }

        /// <summary>
        /// 根據XML轉換成XO可識別的程式碼
        /// </summary>
        /// <param name="parXMLString">XML字串</param>
        /// <returns>
        /// 回傳XO Code
        /// </returns>
        public string ParseXOCode(string parXMLString)
        {
            return ParseXOCode(parXMLString, "");
        }

        /// <summary>
        /// 根據XML轉換成XO可識別的程式碼
        /// </summary>
        /// <param name="parXMLString">XML字串</param>
        /// <param name="parCompany">公司別</param>
        /// <returns>
        /// 回傳XO Code
        /// </returns>
        public string ParseXOCode(string parXMLString, string parCompany)
        {
            // 建立XOGatewayBO層的服務實體。
            XOGatewayBO.XOGatewayBOService xoService = GetXOGatewayBOService(parCompany);

            return xoService.ParseXOCode(parXMLString);
        }

        #endregion
    }
}
