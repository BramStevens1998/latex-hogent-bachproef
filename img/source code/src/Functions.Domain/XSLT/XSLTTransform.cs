
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Xslt;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Xslt
{
    /// <summary>
    /// Represents an XSLT transform.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class XsltTransform : IXsltTransform
    {
        /// <summary>
        /// Name of the XSLT transform.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// <see cref="ISource"/> of the XSLT transform.
        /// </summary>
        public ISource Source { get; set; }

        /// <summary>
        /// Incoming <see cref="MessageFormat"/> of the XSLT transform.
        /// </summary>
        [JsonProperty("inFormat")]
        public MessageFormat InFormat { get; set; }

        /// <summary>
        /// Outgoing <see cref="MessageFormat"/> of the XSLT transform.
        /// </summary>
        [JsonProperty("outFormat")]
        public MessageFormat OutFormat { get; set; }

        /// <summary>
        /// Incoming <see cref="IPayload"/> of the XSLT transform.
        /// </summary>
        [JsonProperty("inMessage")]
        public IPayload InMessage { get; set; }

        /// <summary>
        /// Initializes the XlstTransform class.
        /// </summary>
        /// <param name="source">The <see cref="ISource"/>.</param>
        /// <param name="inFormat">The incoming <see cref="MessageFormat"/>.</param>
        /// <param name="outFormat">The outgoing <see cref="MessageFormat"/>.</param>
        /// <param name="inMessage">The incoming <see cref="IPayload"/>.</param>
        public XsltTransform(ISource source, MessageFormat inFormat, MessageFormat outFormat, IPayload inMessage)
        {
            Source = source;
            InFormat = inFormat;
            OutFormat = outFormat;
            InMessage = inMessage;
        }

        /// <summary>
        /// Initializes this XSLT transform.
        /// </summary>
        /// <param name="source">The <see cref="ISource"/>.</param>
        /// <param name="inFormat">The incoming <see cref="MessageFormat"/>.</param>
        /// <param name="outFormat">The outgoing <see cref="MessageFormat"/>.</param>
        /// <param name="inMessage">The incoming <see cref="IPayload"/>.</param>
        /// <param name="refresh">Boolean representing whether or not to refresh <see cref="ISource"/>.</param>
        public async Task InitAsync(ISource source, MessageFormat inFormat, MessageFormat outFormat, IPayload inMessage, bool refresh = false)
        {
            Source = source;
            InFormat = inFormat;
            OutFormat = outFormat;
            InMessage = inMessage;
            if (refresh)
            {
                await Source.Refresh();
            }
        }

        /// <summary>
        /// Executes this XLST transform.
        /// </summary>
        /// <returns>Result of the execution of this XSLT transform.</returns>
        public async Task<string> ExecuteAsync()
        {
            if (InFormat == MessageFormat.Json && OutFormat == MessageFormat.Json) return await JsonToJson(InMessage);
            if (InFormat == MessageFormat.Xml && OutFormat == MessageFormat.Xml) return await XmlToXml(InMessage);
            if (InFormat == MessageFormat.Json && OutFormat == MessageFormat.Xml) return await JsonToXml(InMessage);
            if (InFormat == MessageFormat.Xml && OutFormat == MessageFormat.Json) return await XmlToJson(InMessage);

            return null;
        }

        /// <summary>
        /// XSLT transforms a JSON to a JSON.
        /// </summary>
        /// <param name="inMessage">The incoming <see cref="IPayload"/></param>
        /// <returns>A JSON.</returns>
        public async Task<string> JsonToJson(IPayload inMessage)
        {
            var jsonOut = "{}";
            var xmlIn = JsonConvert.DeserializeXNode(await inMessage.GetStringAsync(), "root");
            jsonOut = JsonConvert.SerializeXmlNode(Transform(xmlIn), Newtonsoft.Json.Formatting.None, false);
            jsonOut = ApplyJsonTypes(jsonOut);
            return jsonOut;
        }

        /// <summary>
        /// XSLT transforms an XML to an XML.
        /// </summary>
        /// <param name="inMessage">The incoming <see cref="IPayload"/></param>
        /// <returns>An XML.</returns>
        public async Task<string> XmlToXml(IPayload inMessage)
        {
            var xmlIn = XDocument.Parse(await inMessage.GetStringAsync());
            return Transform(xmlIn).OuterXml;
        }

        /// <summary>
        /// XSLT transforms a JSON to an XML.
        /// </summary>
        /// <param name="inMessage">The incoming <see cref="IPayload"/></param>
        /// <returns>An XML.</returns>
        public async Task<string> JsonToXml(IPayload inMessage)
        {
            var xmlIn = JsonConvert.DeserializeXNode(await inMessage.GetStringAsync(), "root");
            return Transform(xmlIn).OuterXml;
        }

        /// <summary>
        /// XSLT transforms an XML to a JSON.
        /// </summary>
        /// <param name="inMessage">The incoming <see cref="IPayload"/></param>
        /// <returns>A JSON.</returns>
        public async Task<string> XmlToJson(IPayload inMessage)
        {
            var jsonOut = "{}";
            var xmlIn = XDocument.Parse(await inMessage.GetStringAsync());
            jsonOut = JsonConvert.SerializeXmlNode(Transform(xmlIn), Newtonsoft.Json.Formatting.None, true);
            jsonOut = ApplyJsonTypes(jsonOut);
            return jsonOut;
        }

        /// <summary>
        /// Transforms an <see cref="XDocument"/> to an <see cref="XmlDocument"/>
        /// </summary>
        /// <param name="xmlIn">The incoming <see cref="XDocument"/>.</param>
        /// <returns>An <see cref="XmlDocument"/>.</returns>
        private XmlDocument Transform(XDocument xmlIn)
        {
            var xmlOut = new XDocument();
            var settings = new XsltSettings(true, true);
            var transformer = new XslCompiledTransform();
            using (XmlReader xslReader = XmlReader.Create(Source.Content))
            {
                transformer.Load(xslReader);
            }

            using (XmlReader inReader = xmlIn.CreateReader())
            {
                using (XmlWriter outWriter = xmlOut.CreateWriter())
                {
                    transformer.Transform(inReader, outWriter);
                }
            }

            var xmlDocument = new XmlDocument();
            using (var xmlReader = xmlOut.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Makes sure the correct typing is applied.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>The JSON for this token.</returns>
        private string ApplyJsonTypes(string json)
        {
            var matchEvaluatorWithValue = new MatchEvaluator(delegate (Match match)
            {
                if (match.Groups.Count == 6)
                {
                    var t = match.Groups[2].Value;
                    var v = match.Groups[4].Value;
                    switch (t)
                    {
                        case "string":
                        case "number":
                        case "boolean":
                        case "null":
                            return v;
                    }
                }
                return match.Value;
            });
            var matchEvaluatorWithoutValue = new MatchEvaluator(delegate (Match match)
            {
                if (match.Groups.Count == 4)
                {
                    var t = match.Groups[2].Value;
                    switch (t)
                    {
                        case "string":
                        case "number":
                        case "boolean":
                        case "null":
                            return "null";
                    }
                }
                return match.Value;
            });
            var matchEvaluatorAction = new MatchEvaluator(delegate (Match match)
            {
                return string.Empty;
            });

            var linearized = JToken.Parse(json).ToString(Newtonsoft.Json.Formatting.None);
            var applied = Regex.Replace(linearized, "(\\,?\"[^\"]*\":{[^}]*\"json:action\":\"omit\"[^}]*})", matchEvaluatorAction, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            applied = Regex.Replace(applied, "(\\{\"@json:type\":\")([^\"]*)(\",\"@xmlns:json\":\"http:\\/\\/json\",\"\\#text\":\")([^\"]*)(\"})", matchEvaluatorWithValue, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            applied = Regex.Replace(applied, "(\\{\"@json:type\":\")([^\"]*)(\",\"@xmlns:json\":\"http:\\/\\/json\"})", matchEvaluatorWithoutValue, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            applied = applied.Replace("_AT_", "@");
            return JToken.Parse(applied).ToString(Newtonsoft.Json.Formatting.Indented);
        }
    }
}