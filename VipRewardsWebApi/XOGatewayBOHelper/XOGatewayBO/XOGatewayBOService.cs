using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Cardif.PWS.XOGatewayBOHelper.XOGatewayBO
{
    public sealed class XOGatewayBOService
    {
        private static readonly XNamespace SoapNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly XNamespace ServiceNamespace = "http://tempuri.org/";
        private readonly HttpClient httpClient;

        public XOGatewayBOService()
            : this(new HttpClient())
        {
        }

        public XOGatewayBOService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Timeout = 100000;
        }

        public Credentials CredentialsValue { get; set; }

        public string Url { get; set; }

        public int Timeout { get; set; }

        public XOResultDataEntity GetXOGatewayResult(XOINPARAMEntity xoEntity)
        {
            if (xoEntity == null)
            {
                throw new ArgumentNullException(nameof(xoEntity));
            }

            XElement parameter = SerializeToElement("xoEntity", xoEntity);
            XDocument response = SendSoapRequest("GetXOGatewayResult", parameter);
            XElement resultElement = FindResultElement(response, "GetXOGatewayResultResult");

            return DeserializeXOResultDataEntity(resultElement);
        }

        public string GetXOGatewayResultXML(string parXMLString)
        {
            XElement parameter = new XElement(ServiceNamespace + "parXMLString", parXMLString ?? string.Empty);
            XDocument response = SendSoapRequest("GetXOGatewayResultXML", parameter);
            XElement resultElement = FindResultElement(response, "GetXOGatewayResultXMLResult");
            return resultElement?.Value ?? string.Empty;
        }

        public string ParseXOCode(string parXMLString)
        {
            XElement parameter = new XElement(ServiceNamespace + "parXMLString", parXMLString ?? string.Empty);
            XDocument response = SendSoapRequest("ParseXOCode", parameter);
            XElement resultElement = FindResultElement(response, "ParseXOCodeResult");
            return resultElement?.Value ?? string.Empty;
        }

        private XDocument SendSoapRequest(string actionName, XElement parameter)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                throw new InvalidOperationException("XOGatewayBOService Url must be set before calling.");
            }

            XDocument envelope = BuildSoapEnvelope(actionName, parameter);
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url)
            {
                Content = new StringContent(envelope.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "text/xml")
            };
            request.Headers.Add("SOAPAction", "\"" + ServiceNamespace + actionName + "\"");

            using var cts = Timeout > 0 ? new System.Threading.CancellationTokenSource(Timeout) : null;
            System.Threading.Tasks.Task<HttpResponseMessage> sendTask = httpClient.SendAsync(request, cts?.Token ?? System.Threading.CancellationToken.None);
            HttpResponseMessage response = sendTask.GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            string responseXml = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return XDocument.Parse(responseXml);
        }

        private XDocument BuildSoapEnvelope(string actionName, XElement parameter)
        {
            XElement header = null;
            if (CredentialsValue != null)
            {
                header = new XElement(
                    SoapNamespace + "Header",
                    SerializeToElement("Credentials", CredentialsValue));
            }

            XElement body = new XElement(
                SoapNamespace + "Body",
                new XElement(ServiceNamespace + actionName, parameter));

            XElement envelope = new XElement(SoapNamespace + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", SoapNamespace),
                new XAttribute(XNamespace.Xmlns + "tem", ServiceNamespace));

            if (header != null)
            {
                envelope.Add(header);
            }

            envelope.Add(body);

            return new XDocument(envelope);
        }

        private static XElement SerializeToElement<T>(string elementName, T value)
        {
            XmlRootAttribute root = new XmlRootAttribute(elementName)
            {
                Namespace = ServiceNamespace.NamespaceName,
                IsNullable = false
            };

            XmlSerializer serializer = new XmlSerializer(typeof(T), root);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, ServiceNamespace.NamespaceName);

            using MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, namespaces);
            }

            stream.Position = 0;
            return XElement.Load(stream);
        }

        private static XElement FindResultElement(XDocument response, string elementName)
        {
            if (response?.Root == null)
            {
                return null;
            }

            foreach (XElement element in response.Descendants())
            {
                if (element.Name.LocalName == elementName)
                {
                    return element;
                }
            }

            return null;
        }

        private static XOResultDataEntity DeserializeXOResultDataEntity(XElement resultElement)
        {
            XOResultDataEntity result = new XOResultDataEntity();
            if (resultElement == null)
            {
                return result;
            }

            result.RetCode = GetElementValue(resultElement, "RetCode");
            result.Reason = GetElementValue(resultElement, "Reason");
            result.ErrMsg = GetElementValue(resultElement, "ErrMsg");
            result.RetCount = GetElementValue(resultElement, "RetCount");

            XElement resultSetElement = GetChildElement(resultElement, "ResultSet");
            if (resultSetElement != null)
            {
                DataSet dataSet = new DataSet
                {
                    Locale = CultureInfo.InvariantCulture
                };

                using XmlReader reader = resultSetElement.CreateReader();
                dataSet.ReadXml(reader, XmlReadMode.ReadSchema);
                result.ResultSet = dataSet;
            }
            else
            {
                result.ResultSet = new DataSet();
            }

            return result;
        }

        private static string GetElementValue(XElement parent, string elementName)
        {
            XElement element = GetChildElement(parent, elementName);
            return element?.Value;
        }

        private static XElement GetChildElement(XElement parent, string elementName)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (XElement element in parent.Elements())
            {
                if (element.Name.LocalName == elementName)
                {
                    return element;
                }
            }

            return null;
        }
    }
}
