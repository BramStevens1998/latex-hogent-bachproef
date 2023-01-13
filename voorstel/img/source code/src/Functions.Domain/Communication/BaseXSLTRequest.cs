using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Xslt;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents a base XSLT request.
    /// </summary>
    /// <typeparam name="T">Type of a <see cref="IXsltTransform"/></typeparam>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseXsltRequest<T> : BaseDataAcceleratorRequest, IXsltTransformRequest<T> where T : IXsltTransform
    {
        /// <summary>
        /// Implementation of <see cref="IXsltTransform"/>.
        /// </summary>
        [JsonProperty("mapping")]
        public abstract T Mapping { get; set; }

        /// <summary>
        /// Implementation of the incoming <see cref="MessageFormat"/>.
        /// </summary>
        [JsonProperty("inFormat")]
        public virtual MessageFormat InFormat { get; set; } = MessageFormat.Json;

        /// <summary>
        /// Implementation of the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        [JsonProperty("outFormat")]
        public virtual MessageFormat OutFormat { get; set; } = MessageFormat.Json;

        /// <summary>
        /// Implementation of the <see cref="IPayload"/> of the incoming XLST request.
        /// </summary>
        [JsonProperty("inMessage")]
        public virtual IPayload InMessage { get; set; }

        /// <summary>
        /// Boolean representing whether or not to refresh <see cref="Mapping"/> from storage
        /// </summary>
        [JsonProperty("refresh")]
        public virtual bool Refresh { get; set; } = false;
    }
}