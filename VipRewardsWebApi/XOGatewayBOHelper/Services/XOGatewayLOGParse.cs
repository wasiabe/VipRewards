/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/06/09
 * 程式說明：提供對查詢XOGateway時，所記錄下來的LOG進行解析以取得所需的資料。
 * 
 * 修正記錄：
******************************************************/

using Cardif.PWS.XOGatewayBOHelper.Interfaces;
using System;
using System.Xml;

namespace Cardif.PWS.XOGatewayBOHelper.Services
{
    public class XOGatewayLOGParse : IXOGatewayLOGParse
    {
        #region IXOGatewayLOGParse 成員

        /// <summary>
        /// 取得GatewayLog的查詢條件。
        /// </summary>
        /// <param name="xoInputData">InputData欄位值。</param>
        /// <returns>查詢條件(查詢欄位及查詢值)。</returns>
        string IXOGatewayLOGParse.GetSearchConditions(string xoInputData)
        {

            XmlDocument xdcInput = new XmlDocument();
            string SearchConditions = "";

            try
            {

                xdcInput.LoadXml(xoInputData);

                XmlNodeList xndlParam1 = xdcInput.SelectSingleNode("//InputData/Param1").ChildNodes;

                foreach (XmlNode xndNode in xndlParam1)
                {

                    string NodeName = xndNode.Name;

                    if (NodeName.EndsWith("prod_type"))
                    {
                        SearchConditions += ((XmlElement)xndNode).GetAttribute("ChineseName") + "=" + xndNode.InnerText + "+";
                    }
                    else
                    {

                        if (NodeName.EndsWith("_para") | NodeName.EndsWith("_key"))
                        {

                            foreach (XmlNode xndSub in xndNode.ChildNodes)
                            {
                                if (xndSub.NodeType == XmlNodeType.Element)
                                {

                                    if (NodeName.Equals("query_date_para"))
                                    {
                                        SearchConditions += ((XmlElement)xndSub).GetAttribute("ChineseName") + "=" + xndSub.InnerText.Replace(',', '~') + "+";
                                    }
                                    else
                                    {
                                        SearchConditions += ((XmlElement)xndSub).GetAttribute("ChineseName") + "=" + xndSub.InnerText + "+";
                                    }

                                }
                            }

                        }

                    }

                }

                // 過濾掉最後一個「+」
                if (SearchConditions != "")
                {
                    SearchConditions = SearchConditions.Substring(0, SearchConditions.Length - 1);
                }
            }
            catch (Exception ex)
            {
                SearchConditions = ex.ToString();
            }

            return SearchConditions;

        }

        #endregion
    }
}
