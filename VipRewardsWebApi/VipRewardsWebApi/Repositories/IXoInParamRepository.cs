/// <summary>
/// XOINPARAM 資料存取介面
/// </summary>
public interface IXoInParamRepository
{
    Task<XoInParam> GetXoInParam(string tranId);
}
