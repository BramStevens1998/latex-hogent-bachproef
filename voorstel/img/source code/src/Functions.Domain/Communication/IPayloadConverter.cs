
using System;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents a payload converter.
    /// </summary>
    public class IPayloadConverter : JsonConverter<IPayload>
    {
        /// <summary>
        /// Implementation of the <see cref="HttpRequest"/>.
        /// </summary>
        private HttpRequest _request { get; set; }

        /// <summary>
        /// Initializes a new instance of the IPayloadConverter class.
        /// </summary>
        public IPayloadConverter() { }

        /// <summary>
        /// Initializes a new instance of the IPayloadConvert class.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        public IPayloadConverter(HttpRequest request)
        {
            _request = request;
        }

        /// <summary>
        /// Writes the Json data.
        /// </summary>
        /// <param name="writer">Implementation of <see cref="JsonWriter"/>.</param>
        /// <param name="payload">Implementation of <see cref="IPayload"/>.</param>
        /// <param name="serializer">Implementation of <see cref="JsonSerializer"/>.</param>
        public override void WriteJson(JsonWriter writer, IPayload payload, JsonSerializer serializer)
        {
            writer.WriteValue(payload.GetStringAsync().Result);
        }

        /// <summary>
        /// Creates a payload from JSON.
        /// </summary>
        /// <param name="reader">Implementation of <see cref="JsonReader"/>.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="existingPayload">The <see cref="IPayload"/> that already exists.</param>
        /// <param name="hasExistingValue">Boolean representing whether or not there already exists a value.</param>
        /// <param name="serializer">Implementation of <see cref="JsonSerializer"/>.</param>
        /// <returns>The <see cref="IPayload"/>.</returns>
        public override IPayload ReadJson(JsonReader reader, Type objectType, IPayload existingPayload, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray)
                return new InlineJsonPayload(JRaw.Create(reader));
            else if (((string)reader.Value).StartsWith("FORM[")) {
                var value = (string)reader.Value;
                var idx = value.LastIndexOf(']');
                var index = int.Parse(value.Substring(5, idx - 5));
                return new FormFilePayload(_request, index);
            }
            else
                return new InlineStringPayload((string)reader.Value);
        }
    }
}