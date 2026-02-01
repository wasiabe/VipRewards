/// <summary>
/// XOINPARAM 資料表對應實體
/// </summary>
public class XoInParam
{
    public string SysId { get; set; } = string.Empty;
    public string TranId { get; set; } = string.Empty;
    public string XoInput { get; set; } = string.Empty;
    public string XoOutput { get; set; } = string.Empty;

    public string? XoFnType { get; set; }
    public string? XoClass { get; set; }
    public string? XoPackage { get; set; }
    public string? XoMethod { get; set; }
    public string? UiInput { get; set; }
    public string? UiOutput { get; set; }
    public string? UiOutputHash { get; set; }
    public string? ChineseFieldName { get; set; }
}
