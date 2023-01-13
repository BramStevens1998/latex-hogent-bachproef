using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Sql;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents a SQL stored procedure request
    /// </summary>
    public class SqlStoredProcedureRequest : BaseSqlRequest<SqlStoredProcedure>, ISqlRequest<SqlStoredProcedure>
    {
        /// <summary>
        /// Implementation of <see cref="ISqlAction"/>
        /// </summary>
        public override SqlStoredProcedure Action { get; set; }

        /// <summary>
        /// Boolean representing whether or not to update <see cref="Action"/> in SQL database
        /// </summary>
        [JsonProperty("update")]
        public virtual bool Update { get; set; } = false;

    }

}