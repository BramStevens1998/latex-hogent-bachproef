using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Sql
{
    /// <summary>
    /// Represents a SQL environment.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SqlEnvironment : ISqlEnvironment
    {
        /// <summary>
        /// Name of the SQL environment.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// <see cref="ConnectionString"/> of the SQL environment.
        /// </summary>
        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the SqlEnvironment class.
        /// </summary>
        public SqlEnvironment() {}
        
        /// <summary>
        /// Initializes a new instance of the SqlEnvironment class.
        /// </summary>
        /// <param name="name">Name of the SQL environment.</param>
        /// <param name="connectionString"><see cref="ConnectionString"/> of the SQL environment.</param>
        public SqlEnvironment(string name, string connectionString) {
            Name = name;
            ConnectionString = connectionString;
        }
    }
}