using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents a convert request.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ConvertRequest : BaseDataAcceleratorRequest, IConvertRequest
    {
        /// <summary>
        /// Implementation of the incoming <see cref="MessageFormat"/>.
        /// </summary>
        [JsonProperty("inFormat")]
        public MessageFormat InFormat { get; set; }

        /// <summary>
        /// Implementation of the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        [JsonProperty("outFormat")]
        public MessageFormat OutFormat { get; set; }

        [JsonProperty("delimiter")]
        public string Delimiter { get; set; } = ";";

        /// <summary>
        /// Implementation of the <see cref="IPayload"/> of the incoming convert request.
        /// </summary>
        [JsonProperty("inMessage")]
        public IPayload InMessage { get; set; }
    }
}
