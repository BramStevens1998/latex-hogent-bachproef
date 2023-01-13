using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Respresents a base SQL request
    /// </summary>
    /// <typeparam name="T">Type of a <see cref="ISqlAction"/> implementation</typeparam>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseSqlRequest<T> : BaseDataAcceleratorRequest, ISqlRequest<T> where T : ISqlAction
    {
        /// <summary>
        /// Implementation of <see cref="ISqlAction"/>.
        /// </summary>
        [JsonProperty("action")]
        public abstract T Action { get; set; }

        /// <summary>
        /// Boolean representing whether or not to refresh <see cref="Action"/> from storage
        /// </summary>
        [JsonProperty("refresh")]
        public virtual bool Refresh { get; set; } = false;
    }
}