using ChoETL;
using ClosedXML.Excel;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Conversion;
using ExcelDataReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Conversion
{
    /// <summary>
    /// Represents a conversion between data types.
    /// </summary>
    public class Converter : IConverter
    {
        /// <summary>
        /// Implements the incoming payload message.
        /// </summary>
        public IPayload InMessage { get; set; }

        /// <summary>
        /// Implements the incoming <see cref="MessageFormat"/>.
        /// </summary>
        public MessageFormat InFormat { get; set; }

        /// <summary>
        /// Implements the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        public MessageFormat OutFormat { get; set; }

        public string Delimiter { get; set; } = ";";

        /// <summary>
        /// Name of this file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Initializes a new instance of the Converter class.
        /// </summary>
        /// <param name="inMessage">Implementation of <see cref="IPayload"/>.</param>
        /// <param name="inFormat">Implementation of <see cref="MessageFormat"/>.</param>
        /// <param name="outFormat">Implementation of <see cref="MessageFormat"/></param>
        /// <param name="delimiter">Implementation of a delimiter.</param>
        public Converter(IPayload inMessage, MessageFormat inFormat, MessageFormat outFormat, string delimiter = ";")
        {
            InMessage = inMessage;
            InFormat = inFormat;
            OutFormat = outFormat;
            Delimiter = delimiter;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Initializes this conversion.
        /// </summary>
        /// <param name="inMessage">Implementation of <see cref="IPayload"/>.</param>
        /// <param name="inFormat">Implementation of <see cref="MessageFormat"/>.</param>
        /// <param name="outFormat">Implementation of <see cref="MessageFormat"/>.</param>
        /// <param name="delimiter">Implementation of a delimiter.</param>
        public virtual async Task InitAsync(IPayload inMessage, MessageFormat inFormat, MessageFormat outFormat, string delimiter = ";")
        {
            await Task.Run(() =>
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                InMessage = inMessage;
                InFormat = inFormat;
                OutFormat = outFormat;
                Delimiter = delimiter;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                this.FileName = $"convertedfile_{DateTime.Now:yyyyMMddHHmmssfff}.{OutFormat}";
            });
        }

        /// <summary>
        /// Executes this conversion.
        /// </summary>
        /// <returns>Result of the execution of this conversion.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync()
            => (InFormat, OutFormat) switch
            {
                (MessageFormat.Xls, MessageFormat.Json) => await ExcelToJson(),
                (MessageFormat.Xlsx, MessageFormat.Json) => await ExcelToJson(),
                (MessageFormat.Csv, MessageFormat.Json) => await CSVToJson(),
                (MessageFormat.Json, MessageFormat.Xlsx) => await JsonToExcel(),
                (MessageFormat.Json, MessageFormat.Xls) => await JsonToExcel(),
                (MessageFormat.Json, MessageFormat.Csv) => await JsonToCsv(),
                (MessageFormat.Json, MessageFormat.Xml) => await JsonToXml(),
                (MessageFormat.Xml, MessageFormat.Json) => await XmlToJson(),
                (_, _) => null
            };

        /// <summary>
        /// Converts a JSON to an XML
        /// </summary>
        /// <returns>A HTTP response message</returns>
        private async Task<HttpResponseMessage> JsonToXml()
        {
            var xml = JsonConvert.DeserializeXNode(await InMessage.GetStringAsync(), "root");
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(xml.ToString()) };
            if (response.Content.Headers.Contains("Content-Type")) response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.Add("Content-Type", "text/xml");
            return response;
        }

        /// <summary>
        /// Converts an XML to JSON
        /// </summary>
        /// <returns>A HTTP response message</returns>
        private async Task<HttpResponseMessage> XmlToJson()
        {
            var xml = new XmlDocument();
            xml.LoadXml(await InMessage.GetStringAsync());
            var json = JsonConvert.SerializeXmlNode(xml.DocumentElement, Newtonsoft.Json.Formatting.None, true);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            if (response.Content.Headers.Contains("Content-Type")) response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.Add("Content-Type", "application/json");
            return response;
        }

        /// <summary>
        /// Converts a CSV to JSON.
        /// </summary>
        /// <returns>A HTTP response message.</returns>
        private async Task<HttpResponseMessage> CSVToJson()
        {
            string csvString = await InMessage.GetStringAsync();
            string result = string.Empty;
            result = ToJSON(csvString);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
            if (response.Content.Headers.Contains("Content-Type")) response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.Add("Content-Type", "application/json");
            return response;
        }

        /// <summary>
        /// Converts an Excel to JSON.
        /// </summary>
        /// <returns>A HTTP response message.</returns>
        private async Task<HttpResponseMessage> ExcelToJson()
        {
            var stream = await InMessage.GetStreamAsync();
            string result = string.Empty;
            DataSet ds = ExcelToDataSet(data: stream, hasHeader: true);
            result = ToJSON(ToCSV(ds.Tables[0]));
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
            if (response.Content.Headers.Contains("Content-Type")) response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.Add("Content-Type", "application/json");
            return response;
        }

        /// <summary>
        /// Converts a given data set to CSV.
        /// </summary>
        /// <param name="dataTable">Implementation of <see cref="DataTable"/>.</param>
        /// <returns>A CSV.</returns>
        public string ToCSV(DataTable dataTable)
        {
            try
            {
                StringBuilder sbData = new StringBuilder();

                // Only return Null if there is no structure.
                if (dataTable.Columns.Count == 0)
                    return null;

                foreach (var col in dataTable.Columns)
                {
                    if (col == null)
                        sbData.Append(Delimiter);
                    else
                        sbData.Append("\"" + col.ToString().Replace("\"", "\"\"") + $"\"{Delimiter}");
                }

                sbData.Replace(Delimiter, System.Environment.NewLine, sbData.Length - 1, 1);

                foreach (DataRow dr in dataTable.Rows)
                {
                    foreach (var column in dr.ItemArray)
                    {
                        if (column == null)
                            sbData.Append(Delimiter);
                        else
                            sbData.Append("\"" + column.ToString().Replace("\"", "\"\"") + $"\"{Delimiter}");
                    }
                    sbData.Replace(Delimiter, System.Environment.NewLine, sbData.Length - 1, 1);
                }

                return sbData.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a string to a JSON.
        /// </summary>
        /// <param name="source">A string.</param>
        /// <returns>A JSON.</returns>
        public string ToJSON(string source)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                using var w = new ChoJSONWriter(sb);

                // Sets up the conversion schema.
                // Assumes that the first row is a header
                foreach (dynamic rec in ChoCSVReader
                    .LoadText(source)
                    .Configure(c =>
                    {
                        c.Delimiter = Delimiter;
                        c.AutoDiscoverColumns = true;
                        c.AutoDiscoverFieldTypes = true;
                        c.ThrowAndStopOnMissingField = false;
                        c.MayContainEOLInData = true;
                        c.NullValueHandling = ChoNullValueHandling.Empty;
                        c.QuoteAllFields = true;
                    })
                    .WithFirstLineHeader())
                {
                    w.Write(rec);
                }

                w.Close();

                return sb.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts an Excel to a data set.
        /// </summary>
        /// <param name="data">The Excel data.</param>
        /// <param name="hasHeader">Boolean representing wether or not an Excel has a header.</param>
        /// <returns>A dataset containing Excel data.</returns>
        private DataSet ExcelToDataSet(Stream data, bool hasHeader = false)
        {
            try
            {
                using var reader = ExcelReaderFactory.CreateReader(data);
                return reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                    {
                        EmptyColumnNamePrefix = "Column",
                        UseHeaderRow = hasHeader,
                    },
                    UseColumnDataType = true
                });
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a JSON to an Excel.
        /// </summary>
        /// <returns>A HTTP response message.</returns>
        private async Task<HttpResponseMessage> JsonToExcel()
        {
            var dT = JsonToDataTable(await InMessage.GetStringAsync());
            var xslt = ToExcel(dT);
            var response = FileToHttpResponse(xslt, this.FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return response;
        }

        /// <summary>
        /// Converts a JSON to a CSV.
        /// </summary>
        /// <returns>A HTTP response message.</returns>
        private async Task<HttpResponseMessage> JsonToCsv()
        {
            var dT = JsonToDataTable(await InMessage.GetStringAsync());
            var csv = ToCSV(dT);
            var respone = FileToHttpResponse(new ByteArrayContent(Encoding.UTF8.GetBytes(csv)), this.FileName, "application/octet-stream");
            return respone;
        }

        /// <summary>
        /// Converts a JSON to a data table.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>A data table.</returns>
        public static DataTable JsonToDataTable(string json)
        {
            try
            {
                var jsonLinq = JObject.Parse(json);

                // Find the first array using Linq
                var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
                var trgArray = new JArray();
                foreach (JObject row in srcArray.Children<JObject>())
                {
                    var cleanRow = new JObject();
                    foreach (JProperty column in row.Properties())
                    {
                        // Only include JValue types
                        if (column.Value is JValue)
                        {
                            cleanRow.Add(column.Name, column.Value);
                        }
                    }

                    trgArray.Add(cleanRow);
                }
                var result = JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a data table to a byte array.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable"/>.</param>
        /// <returns>A byte array content.</returns>
        public static ByteArrayContent ToExcel(DataTable dt)
        {
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Sheet1");
                int iCol = 0;
                foreach (DataColumn c in dt.Columns)
                {
                    iCol++;
                    worksheet.Cell(1, iCol).Value = tryConvert(c.ColumnName.ToString());
                }

                int iRow = 0;
                foreach (DataRow r in dt.Rows)
                {
                    iRow++;
                    // add each row's cell data...
                    iCol = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        iCol++;
                        worksheet.Cell(iRow + 1, iCol).Value = tryConvert(r[c.ColumnName].ToString());
                    }
                }

                MemoryStream memorystream = new MemoryStream();
                workbook.SaveAs(memorystream);
                return new ByteArrayContent(memorystream.ToArray());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a file to a HTTP response message.
        /// </summary>
        /// <param name="fileBytes">Implements <see cref="HttpContent"/>.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="mediaTypeHeader">The media type used in a Content-Type header</param>
        /// <returns>A HTTP response message.</returns>
        public static HttpResponseMessage FileToHttpResponse(HttpContent fileBytes, string fileName, string mediaTypeHeader)
        {
            try
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = fileBytes;
                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment") { FileName = $"{fileName}" };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue($"{mediaTypeHeader}");
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a given string value to the corresponding type.
        /// </summary>
        /// <param name="value">The string value to be converted.</param>
        /// <returns>The converted value.</returns>
        public static object tryConvert(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return (string.Empty);
            }

            double doubleValue = 0;
            if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out doubleValue))
            {
                return (doubleValue);
            }

            float floatValue = 0;
            if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out floatValue))
            {
                return (floatValue);
            }

            long longValue = 0;
            if (long.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out longValue))
            {
                return (longValue);
            }

            int intValue = 0;
            if (int.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out intValue))
            {
                return (intValue);
            }

            bool boolValue = false;
            if (bool.TryParse(value, out boolValue))
            {
                return (boolValue);
            }

            DateTime dateTimeValue = DateTime.MinValue;
            if (DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTimeValue))
            {
                return (dateTimeValue);
            }

            return (value);
        }
    }
}

