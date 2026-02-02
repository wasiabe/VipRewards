using System.Globalization;
using System.Reflection;
using Cardif.PWS.XOGatewayBOHelper.BusinessEntities;
using Cardif.PWS.XOGatewayBOHelper.Services;
using Newtonsoft.Json;

namespace VipRewardsWebApi.Services;

public sealed class VipRewardService
{
    private readonly XoInParamRepository _xoInParamRepository;
    private readonly XOGatewayAccessService _xoGatewayAccessService;
    private readonly ILogger<VipRewardService> _logger;

    public VipRewardService(
        XoInParamRepository xoInParamRepository,
        XOGatewayAccessService xoGatewayAccessService,
        ILogger<VipRewardService> logger)
    {
        _xoInParamRepository = xoInParamRepository;
        _xoGatewayAccessService = xoGatewayAccessService;
        _logger = logger;
    }

    public async Task<object> GetXODataAsync<TModel>(string tranId, TModel model, CancellationToken ct = default)
    {
        _ = ct;

        XoInParam? xoInParam = await _xoInParamRepository.GetXoInParam(tranId);
        if (xoInParam is null)
        {
            throw new InvalidOperationException($"XOINPARAM not found for tranId: {tranId}");
        }

        XOINPARAMEntity entity = CreateXoInParamEntity(xoInParam);
        SetXOField(entity, model);

        var result = _xoGatewayAccessService.GetResultDataEntity(entity);
        return result;
    }

    private static XOINPARAMEntity CreateXoInParamEntity(XoInParam source)
    {
        return new XOINPARAMEntity
        {
            SYSID = source.SysId ?? string.Empty,
            TRANID = source.TranId ?? string.Empty,
            XO_INPUT = source.XoInput ?? string.Empty,
            XO_OUTPUT = source.XoOutput ?? string.Empty,
            XO_FNTYPE = source.XoFnType ?? string.Empty,
            XO_CLASS = source.XoClass ?? string.Empty,
            XO_PACKAGE = source.XoPackage ?? string.Empty,
            XO_METHOD = source.XoMethod ?? string.Empty,
            UI_INPUT = source.UiInput ?? string.Empty,
            UI_OUTPUT = source.UiOutput ?? string.Empty,
            UI_OUTPUT_HASH = source.UiOutputHash ?? string.Empty,
            ChineseFieldName = source.ChineseFieldName ?? string.Empty
        };
    }

    private static void SetXOField<T>(XOINPARAMEntity entity, T model)
    {
        if (entity is null || model is null)
        {
            return;
        }

        string uiInput = entity.UI_INPUT ?? string.Empty;
        if (string.IsNullOrWhiteSpace(uiInput))
        {
            return;
        }

        Type modelType = model.GetType();
        foreach (PropertyInfo property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            if (!IsSupportedSimpleType(property.PropertyType))
            {
                continue;
            }

            JsonPropertyAttribute? jsonAttr = property.GetCustomAttribute<JsonPropertyAttribute>();
            string fieldName = jsonAttr?.PropertyName ?? property.Name;
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                continue;
            }

            if (uiInput.IndexOf(fieldName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            object? fieldValue = property.GetValue(model);
            if (fieldValue is null)
            {
                continue;
            }

            entity.AppendXOField(fieldName, Convert.ToString(fieldValue, CultureInfo.InvariantCulture) ?? string.Empty);
        }
    }

    private static bool IsSupportedSimpleType(Type type)
    {
        if (type == typeof(string))
        {
            return true;
        }

        Type? underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is not null)
        {
            type = underlyingType;
        }

        if (type.IsEnum || type.IsPrimitive)
        {
            return true;
        }

        if (type == typeof(DateTime) || type == typeof(decimal))
        {
            return true;
        }

        return false;
    }
}
