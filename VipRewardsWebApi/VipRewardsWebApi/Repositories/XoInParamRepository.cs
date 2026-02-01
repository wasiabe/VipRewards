using Microsoft.EntityFrameworkCore;

/// <summary>
/// XOINPARAM 資料存取實作
/// </summary>
public class XoInParamRepository : IXoInParamRepository
{
    private readonly AppDbContext _db;

    public XoInParamRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 依 TRANID 取得資料
    /// </summary>
    /// <param name="tranId"></param>
    /// <returns></returns>
    public async Task<XoInParam> GetXoInParam(string tranId)
    {
        XoInParam? xoInParam = await _db.XoInParams
            .AsNoTracking()
            .Where(x => x.TranId == tranId)
            .FirstOrDefaultAsync();
        return xoInParam;

    }
}
