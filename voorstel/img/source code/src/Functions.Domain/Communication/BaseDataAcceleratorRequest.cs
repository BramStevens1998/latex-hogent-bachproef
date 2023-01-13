using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Respresents a base Data Accelerator request.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseDataAcceleratorRequest : IDataAcceleratorRequest
    {
        /// <summary>
        /// Boolean representing if the azure function called with this request should operate
        ///  in a <see href="https://learn.microsoft.com/en-us/azure/azure-functions/durable/">durable</see> or classic way.
        /// </summary>
        [JsonProperty("durable")]
        public virtual bool Durable { get; set; } = false;
        
        /// <summary>
        ///  URL to be called with the response of the azure function that was called with this dataaccelerator request
        /// </summary>
        [JsonProperty("callbackurl")]
        public virtual string CallbackUrl { get; set; }
        
        /// <summary>
        /// Environment to be initialized that was called with this dataaccelerator request.
        /// </summary>
        [JsonProperty("environment")]
        public virtual string Environment { get; set; }
        
        /// <summary>
        /// The version to be initalized that was called with this dataaccelerator request.
        /// </summary>
        [JsonProperty("version")]
        public virtual string Version { get; set; }
    }
}