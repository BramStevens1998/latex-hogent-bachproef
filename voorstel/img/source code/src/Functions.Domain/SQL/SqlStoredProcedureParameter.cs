using Newtonsoft.Json;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Sql
{
    /// <summary>
    /// Represents a SQL stored procedure parameter.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SqlStoredProcedureParameter : BaseSqlActionParameter, ISqlActionParameter, ISqlActionParameter<IPayload>
    {
        /// <summary>
        /// Name of the SQL stored procedure parameter.
        /// </summary>
        [JsonProperty("name")]
        public override string Name { get; set; }

        /// <summary>
        /// <see cref="IPayload"/> of the SQL stored procedure parameter.
        /// </summary>
        [JsonProperty("value")]
        public override IPayload Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the SqlStoredProcedureParameter.
        /// </summary>
        /// <param name="name">Name of the SQL stored procedure parameter.</param>
        /// <param name="value"><see cref="IPayload"/> of the SQL stored procedure parameter.</param>
        public SqlStoredProcedureParameter(string name, IPayload value) : base(name, value) { }
    }
}