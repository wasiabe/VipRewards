using System;
using System.Data;
using System.Xml.Serialization;

namespace Cardif.PWS.XOGatewayBOHelper.XOGatewayBO
{
    [Serializable]
    [XmlType(Namespace = "http://tempuri.org/")]
    [XmlRoot(Namespace = "http://tempuri.org/", IsNullable = false)]
    public sealed class Credentials
    {
        public string Token { get; set; }

        [XmlAnyAttribute]
        public System.Xml.XmlAttribute[] AnyAttr { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://tempuri.org/")]
    public sealed class XOResultDataEntity
    {
        public string RetCode { get; set; }

        public string Reason { get; set; }

        public string ErrMsg { get; set; }

        public string RetCount { get; set; }

        public DataSet ResultSet { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://tempuri.org/")]
    public sealed class XOFieldEntity
    {
        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public string FieldProperty { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "http://tempuri.org/")]
    public sealed class XOINPARAMEntity
    {
        public string RequestUniqueKey { get; set; }

        public string SYSID { get; set; }

        public string TRANID { get; set; }

        public string XO_INPUT { get; set; }

        public string XO_OUTPUT { get; set; }

        public string XO_FNTYPE { get; set; }

        public string XO_CLASS { get; set; }

        public string XO_PACKAGE { get; set; }

        public string XO_METHOD { get; set; }

        public string XO_OUTPUT_HASH { get; set; }

        public string ChineseFieldName { get; set; }

        public string SortStruct { get; set; }

        public string SortField { get; set; }

        public string SortStyle { get; set; }

        public int PerPageRecord { get; set; }

        public int ShowPageIndex { get; set; }

        [XmlArrayItem(IsNullable = false)]
        public XOFieldEntity[] XoFieldList { get; set; }

        public string Company { get; set; }
    }
}
