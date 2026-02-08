/******************************************************
 * 建立者　：Amy Hung
 * 建立日期：2008/03/25
 * 程式說明：提供對XOGatewayBO層進行資料查詢服務的輔助服務。
 * 
 * 修正記錄：
 * 2008/04/15 - Amy Hung
 * 修正命名空間為Cardif.PWS.XOGatewayBOHelper.Services。
 * 
 * 2014/10/29 - Chris Su - SR14090198
 * a.同步CTI XO Interface
 * b.移除不需要的命名空間
 * 
 * 2017/07/03 - Chris Su - CR-229129
 * a.增加寫入屬性RecordCount
 * b.若XO回應0筆資料的話，會將內容清空避免回傳一列空為底線的資料。
******************************************************/

using Cardif.PWS.XOGatewayBOHelper.BusinessEntities;
using Cardif.PWS.XOGatewayBOHelper.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Cardif.PWS.XOGatewayBOHelper.Services
{
    /// <summary>
    /// 提供XOGatewayBOHelper與XOGatewayBO之間的Entity物件的轉換服務。
    /// </summary>
    class XOGatewayBOMappingService : IXOGatewayBOMappingService
    {
        #region IXOGatewayBOMappingService 成員

        #region 轉換XOINPARAMEntity

        /// <summary>
        /// 轉換XOGatewayBOHelper的XOINPARAMEntity物件為XOGatewayBO的XOINPARAMEntity物件。
        /// </summary>
        /// <param name="xoINPARAM">XOGatewayBOHelper的XOResultDataEntity物件。</param>
        /// <returns>XOGatewayBO層的XOINPARAMEntity物件。</returns>
        XOGatewayBO.XOINPARAMEntity IXOGatewayBOMappingService.TransforINPARAM(XOINPARAMEntity xoSourceINPARAM)
        {

            // 若傳入的XOINPARAMEntity為null，則直接回傳null。
            if (xoSourceINPARAM == null)
            {
                return null;
            }

            // 建立BO層的XOINPARAMEntity物件實體。
            XOGatewayBO.XOINPARAMEntity xoINPARAM = new XOGatewayBO.XOINPARAMEntity();

            // 以下將BOHelper的XOINPARAMEntity物件的各屬性值，設定到BO的XOINPARAMEntity物件的相對屬性。
            // ================================================================
            xoINPARAM.XoFieldList = this.TransforXoFieldList(xoSourceINPARAM.XoFieldList
                                                            , xoSourceINPARAM.UI_INPUT
                                                            , xoSourceINPARAM.XO_INPUT);

            xoINPARAM.PerPageRecord = xoSourceINPARAM.PerPageRecord;
            xoINPARAM.ShowPageIndex = xoSourceINPARAM.ShowPageIndex;
            xoINPARAM.Company = xoSourceINPARAM.Company;
            xoINPARAM.TRANID = xoSourceINPARAM.TRANID;
            xoINPARAM.XO_CLASS = xoSourceINPARAM.XO_CLASS;
            xoINPARAM.XO_FNTYPE = xoSourceINPARAM.XO_FNTYPE;
            xoINPARAM.XO_INPUT = xoSourceINPARAM.XO_INPUT;
            xoINPARAM.XO_METHOD = xoSourceINPARAM.XO_METHOD;
            xoINPARAM.XO_OUTPUT = xoSourceINPARAM.XO_OUTPUT;
            xoINPARAM.XO_PACKAGE = xoSourceINPARAM.XO_PACKAGE;
            xoINPARAM.ChineseFieldName = xoSourceINPARAM.ChineseFieldName;
            xoINPARAM.RequestUniqueKey = xoSourceINPARAM.RequestUniqueKey;
            xoINPARAM.SortStruct = this.GetSortStruct(xoSourceINPARAM.UI_OUTPUT);
            xoINPARAM.SortField = this.GetSortField(xoSourceINPARAM.UI_OUTPUT);
            xoINPARAM.SortStyle = this.GetSortStyle(xoSourceINPARAM.UI_OUTPUT);

            // 將UI層OUTPUT的欄位名稱轉換為XO的欄位名稱。
            xoINPARAM.XO_OUTPUT_HASH = this.TransforUIOutputHash(xoSourceINPARAM.UI_OUTPUT_HASH
                                                                , xoSourceINPARAM.UI_OUTPUT
                                                                , xoSourceINPARAM.XO_OUTPUT);

            // ================================================================

            return xoINPARAM;

        }

        #endregion

        #region 轉換XOResultDataEntity

        /// <summary>
        /// 轉換XOGatewayBO的XOResultDataEntity物件為XOGatewayBOHelper的XOResultDataEntity物件。
        /// </summary>
        /// <param name="xoResult">XOGatewayBO層的XOResultDataEntity物件。</param>
        /// <returns>XOGatewayBOHelper的XOResultDataEntity物件。</returns>
        /// <remarks>
        /// 除了轉換為BOHelper層的物件之外，
        /// 還會處理XO主機欄位與UI欄位的轉換。
        /// </remarks>
        XOResultDataEntity IXOGatewayBOMappingService.TransforResultDataEntity(XOGatewayBO.XOResultDataEntity xoSourceResult
                                                                                , XOINPARAMEntity xoBoINPARAM)
        {

            // 若傳入的XOResultDataEntity為null，則直接回傳null。
            if (xoSourceResult == null)
            {
                return null;
            }

            // 建立BOHelper的XOResultDataEntity物件實體。
            XOResultDataEntity xoResult = new XOResultDataEntity();

            // 將UI層的欄位及XO主機的欄位依「,」分割為陣列，供後續轉換時使用。
            string[] UIFieldName = xoBoINPARAM.UI_OUTPUT.Split(',');
            string[] XOFieldName = xoBoINPARAM.XO_OUTPUT.Split(',');

            // @start: 將查詢回來的BO層的XOResultDataEntity物件中的資料，填到BOHelper的XOResultDataEntity物件中。
            // ================================================================
            xoResult.ErrMsg = xoSourceResult.ErrMsg;
            xoResult.Reason = xoSourceResult.Reason;
            xoResult.ResultSet = xoSourceResult.ResultSet;
            xoResult.RetCode = xoSourceResult.RetCode;
            xoResult.RetCount = xoSourceResult.RetCount;

            int RecordCount = 0;
            int.TryParse(xoSourceResult.RetCount, out RecordCount);
            xoResult.RecordCount = RecordCount;

            xoResult.ResultTable = xoSourceResult.ResultSet.Tables[0];
            xoResult.ResultRows = xoSourceResult.ResultSet.Tables[0].Rows;
            if (RecordCount <= 0)
            {
                //若為0筆資料的話，會將回傳結果的內容強制清空，避免回傳一列全部為底線的資料
                xoSourceResult.ResultSet.Tables[0].Rows.Clear();

                xoResult.FirstRow = null;
            }
            else
            {
                xoResult.FirstRow = xoSourceResult.ResultSet.Tables[0].Rows[0];
            }

            // ================================================================
            // @end

            // @start: 將XO主機的欄位名稱改為UI的欄位名稱。

            // PS:由於查詢回來的資料結果，其欄位名稱為XO主機的名稱，要再轉換為UI欄位的名稱。

            DataSet dsResult = xoResult.ResultSet;

            // 逐一將DataSet中的欄位名稱改為UI欄位名稱。
            for (int i = 0; i < dsResult.Tables[0].Columns.Count; i++)
            {
                dsResult.Tables[0].Columns[i].ColumnName = this.XOOutputMappingToUIOutput(
                                                       dsResult.Tables[0].Columns[i].ColumnName
                                                       , UIFieldName, XOFieldName);
            }
            // @end

            // 回傳。
            return xoResult;

        }

        #endregion

        #endregion

        #region 私有方法

        #region Helper List 轉 BO

        /// <summary>
        /// 轉換XOGatewayBOHelper的List物件(泛型為XOFieldEntity)為XOGatewayBO層的陣列(型別為XOFieldEntity)。
        /// </summary>
        /// <param name="helperXOFieldList"></param>
        /// <param name="uiFieldName">UI 層的輸入欄位名稱。(以「,」區隔多個欄位)</param>
        /// <param name="xoFieldName">對應 XO 主機的欄位名稱。(以「,」區隔多個欄位)</param>
        /// <returns>己完成轉換的XOGatewayBO層陣列。</returns>
        /// <remarks>除了轉換為BO層的型別之外，還會進行UI層及XO英文欄位對應轉換的處理。</remarks>
        private XOGatewayBO.XOFieldEntity[] TransforXoFieldList(List<XOFieldEntity> helperXOFieldList, string uiInput, string xoInput)
        {

            // 建立XOGatewayBO.XOFieldEntity型別的陣列。
            // 陣列長度為BOHelper的XoFieldList的元素個數。
            XOGatewayBO.XOFieldEntity[] aryXoFieldList = new Cardif.PWS.XOGatewayBOHelper.XOGatewayBO.XOFieldEntity[helperXOFieldList.Count];

            // 將UI層的欄位及XO主機的欄位依「,」分割為陣列，供後續轉換時使用。
            string[] UIFieldName = uiInput.Split(',');
            string[] XOFieldName = xoInput.Split(',');

            // 用來做為放置XOFieldEntity物件到陣列(aryXoFieldList)中的索引鍵。
            int i = 0;

            // 逐一將BOHelper裡的List<XOFieldEntity>中的XOFieldEntity，加到XOGatewayBO.XOFieldEntity型別的陣列中。
            foreach (XOFieldEntity HelperXOField in helperXOFieldList)
            {

                // 以下建立BO的XOFieldEntity物件，指定欄位名稱、欄位值。

                XOGatewayBO.XOFieldEntity BoXOField = new XOGatewayBO.XOFieldEntity();

                // 必須將UI層的欄位名稱轉換為XO主機的欄位名稱再放入FieldName中。
                BoXOField.FieldName = this.UIInputMappingToXOInput(HelperXOField.FieldName.Trim()
                                                                    , UIFieldName, XOFieldName);

                BoXOField.FieldValue = HelperXOField.FieldValue.Trim();
                BoXOField.FieldProperty = HelperXOField.FieldProperty;

                // 將建立完成的XOFieldEntity物件放到陣列中。
                aryXoFieldList[i] = BoXOField;

                i++; // 索引加1。

            }

            return aryXoFieldList;

        }

        #endregion

        #region UI加密欄位轉換為XO欄位

        /// <summary>
        /// 轉換UI_OUTOUT_HASH中的UI OUTOUT欄位為XO的OUTOUT欄位。
        /// </summary>
        /// <param name="uiHashOutput">UI層要加密的輸出欄位。</param>
        /// <param name="uiOutput">UI 層的輸出欄位名稱。(以「,」區隔多個欄位)</param>
        /// <param name="xoOutput">對應 XO 主機的欄位名稱。(以「,」區隔多個欄位)</param>
        /// <returns>回傳已轉換為XO欄位名稱的字串。</returns>
        private string TransforUIOutputHash(string uiHashOutput, string uiOutput, string xoOutput)
        {

            // UI_OUTPUT_HASH 的格式為：「欄位名稱$加密方式」。以「,」區隔每個要加密的欄位。
            // 例如：p_person_para.identity$IDT,p_person_para.address$BKA

            // 如果為空表示沒有要加密的欄位，不用再進行後面的轉換。
            if (uiHashOutput.Trim().Length == 0)
            {
                return "";
            }

            // 將UI層的欄位及XO主機的欄位依「,」分割為陣列，供後續轉換時使用。
            string[] UIFieldName = uiOutput.Split(',');
            string[] XOFieldName = xoOutput.Split(',');

            // 依「,」切割取得要加密的欄位陣列。
            string[] HashOutput = uiHashOutput.Split(',');

            // 存放轉換為XO欄位名稱的字串。
            string XoHashOutput = "";

            for (int i = 0; i < HashOutput.Length; i++)
            {

                string[] TempHash = HashOutput[i].Split('$');
                string HashResult = "";

                // 判斷是否補「,」號。
                if (i > 0)
                {
                    XoHashOutput = XoHashOutput + ",";
                }

                // 轉換取得XO的欄位名稱(回傳值格式示例: pos_field_lst.pos_nmbr)。
                HashResult = this.UIOutputMappingToXOOutput(TempHash[0], UIFieldName, XOFieldName);

                // 由於回傳的XO欄位名稱會以「.」區隔field_list名稱跟欄位名稱(pos_field_lst.pos_nmbr)。
                // 「.」的後面才是真正的欄位名稱。
                if (HashResult.Split('.').Length > 1)
                {
                    // 索引值1的位置為欄位名稱。
                    XoHashOutput = XoHashOutput + HashResult.Split('.')[1] + "$";
                }
                else
                {
                    // 長度沒有大於1，可能在設定資料時沒有用「.」區隔。
                    // 所以直接以回傳的值為欄位名稱。
                    XoHashOutput = XoHashOutput + HashResult + "$";
                }

                if (TempHash.Length > 1)
                {
                    XoHashOutput = XoHashOutput + TempHash[1];
                }
                else
                {
                    XoHashOutput = XoHashOutput + "IDT";
                }

            }

            return XoHashOutput;

        }

        #endregion

        #region UI層欄位和XO欄位之間的轉換

        /// <summary>
        /// UI的Input欄位轉XO的Input欄位。
        /// </summary>
        /// <param name="findFieldName">要轉換的UI欄位名稱。</param>
        /// <param name="uiInput">UI欄位陣列。</param>
        /// <param name="xoInput">XO欄位陣列。</param>
        /// <returns>XO欄位名稱。</returns>
        private string UIInputMappingToXOInput(string findFieldName, string[] uiInput, string[] xoInput)
        {

            string XOField = "";
            int index = -1;

            for (int i = 0; i < uiInput.Length; i++)
            {

                if (findFieldName.ToLower().Equals(uiInput[i].ToLower()))
                {
                    index = i;
                    break;
                }

            }

            if (index != -1)
            {
                if (index <= xoInput.Length - 1)
                {
                    XOField = xoInput[index];
                }
            }

            return XOField;

        }

        /// <summary>
        /// UI的Output欄位轉XO的Output欄位。
        /// </summary>
        /// <param name="findFieldName">要轉換的UI欄位名稱。</param>
        /// <param name="uiInput">UI欄位陣列。</param>
        /// <param name="xoInput">XO欄位陣列。</param>
        /// <returns>XO欄位名稱。</returns>
        private string UIOutputMappingToXOOutput(string findFieldName, string[] uiOutput, string[] xoOutput)
        {

            string XOField = "";
            int index = -1;

            for (int i = 0; i < uiOutput.Length; i++)
            {

                string[] TempField = uiOutput[i].Split('$');

                if (findFieldName.Equals(TempField[0]))
                {
                    index = i;
                    break;
                }

            }

            if (index != -1)
            {
                if (index <= xoOutput.Length - 1)
                {
                    XOField = xoOutput[index];
                }
            }

            return XOField;

        }

        /// <summary>
        /// XO的Output欄位轉UI的Output欄位。
        /// </summary>
        /// <param name="findFieldName">要轉換的XO欄位名稱。</param>
        /// <param name="uiInput">UI欄位陣列。</param>
        /// <param name="xoInput">XO欄位陣列。</param>
        /// <returns>UI欄位名稱。</returns>
        private string XOOutputMappingToUIOutput(string findFieldName, string[] uiOutput, string[] xoOutput)
        {

            string XOField = "";
            int index = -1;

            for (int i = 0; i < xoOutput.Length; i++)
            {

                string[] TempField = xoOutput[i].Split('.');
                string SearchFlag = "";

                if (TempField.Length > 1)
                {
                    SearchFlag = TempField[1];
                }
                else
                {
                    SearchFlag = TempField[0];
                }

                if (findFieldName.Equals(SearchFlag))
                {
                    index = i;
                    break;
                }

            }

            if (index != -1)
            {
                if (index <= uiOutput.Length - 1)
                {
                    string[] TempField = uiOutput[index].Split('$');
                    XOField = TempField[0];
                }
            }

            return XOField;

        }

        #endregion

        #region 排序欄位資料格式處理

        /// <summary>
        /// 取得排序欄位的結構。
        /// </summary>
        /// <param name="uiOutput">UI Output欄位。</param>
        /// <returns>類似「F0,F1,F2....」格式的字串。</returns>
        private string GetSortStruct(string uiOutput)
        {

            string[] XoOutputField = uiOutput.Split(',');
            string SortStruct = "";

            for (int i = 0; i < XoOutputField.Length; i++)
            {
                SortStruct += "F" + (i + 1) + ",";
            }

            // 去除最後一個「,」號。
            if (SortStruct != "")
            {
                SortStruct = SortStruct.Substring(0, SortStruct.Length - 1);
            }

            return SortStruct;
        }

        /// <summary>
        /// 取得要排序之欄位的索引位置。
        /// </summary>
        /// <param name="uiOutput">UI Output欄位。</param>
        /// <returns>類似「0,1,2....」格式的字串。</returns>
        private string GetSortField(string uiOutput)
        {

            string SortField = "";
            string[] UiOutputField = uiOutput.Split(',');

            for (int i = 0; i < UiOutputField.Length; i++)
            {
                if (UiOutputField[i].IndexOf('$') != -1)
                {
                    // 索引位置從「1」開始，所以要加1。
                    SortField += (i + 1) + ",";
                }
            }

            // 去除最後一個「,」號。
            if (SortField != "")
            {
                SortField = SortField.Substring(0, SortField.Length - 1);
            }

            return SortField;

        }

        /// <summary>
        /// 取得要排序之欄位的排序方式。
        /// </summary>
        /// <param name="uiOutput">UI Output欄位。</param>
        /// <returns>類似「asc,desc....」格式的字串。</returns>
        private string GetSortStyle(string uiOutput)
        {

            string SortStyle = "";
            string[] UiOutputField = uiOutput.Split(',');

            for (int i = 0; i < UiOutputField.Length; i++)
            {
                if (UiOutputField[i].IndexOf('$') != -1)
                {

                    string[] TempSplit = UiOutputField[i].Split('$');
                    string TempStyle = "";

                    switch (TempSplit[1].ToUpper())
                    {
                        case "A":

                            TempStyle = "asc";
                            break;

                        case "D":

                            TempStyle = "desc";
                            break;

                        default:

                            TempStyle = "desc";
                            break;
                    }

                    SortStyle += TempStyle + ",";

                }
            }

            // 去除最後一個「,」號。
            if (SortStyle != "")
            {
                SortStyle = SortStyle.Substring(0, SortStyle.Length - 1);
            }

            return SortStyle;

        }

        #endregion

        #endregion

    }
}
