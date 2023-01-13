using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Sql
{
    /// <summary>
    /// Types of SQL queries
    /// </summary>
    public enum SqlQueryType
    {
        Reader = 0,
        Scalar = 1,
        NonQuery = 2
    }

    /// <summary>
    /// Represents a SQL query action
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SqlQuery : BaseSqlAction<SqlQueryParameter>, ISqlAction<SqlQueryParameter>
    {
        /// <summary>
        /// Name of this SQL query action
        /// </summary>
        [JsonProperty("name")]
        public override string Name { get; set; }

        /// <summary>
        /// List of SQL query parameters
        /// </summary>
        [JsonProperty("parameters")]
        public override IList<SqlQueryParameter> Parameters { get; set; } = new List<SqlQueryParameter>();

        /// <summary>
        /// <see cref="SqlQueryType"/> of this SQL query action
        /// </summary>
        [JsonProperty("type")]
        public SqlQueryType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the SqlQuery class.
        /// </summary>
        public SqlQuery() { }

        /// <summary>
        /// Initializes a new instance of the SqlQuery class.
        /// </summary>
        /// <param name="name">Name of this SQL query action</param>
        /// <param name="type"><see cref="SqlQueryType"/> of this SQL query action</param>
        public SqlQuery(string name, SqlQueryType type = default(SqlQueryType))
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Executes this SQL query action
        /// </summary>
        /// <returns>Result of this SQL query action</returns>
        public override async Task<string> ExecuteAsync()
        {
            string result;
            using (var connection = new SqlConnection(Environment.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(CommandText, connection))
                {
                    SqlDataReader reader;
                    command.Parameters.AddRange(Parameters.Select(x => new SqlParameter($"@{x.Name}", x.Value.GetStringAsync().Result)).Union(MissingParameters.Select(x => new SqlParameter(x, DBNull.Value))).ToArray());
                    switch (Type)
                    {
                        case SqlQueryType.NonQuery:
                            result = (await command.ExecuteNonQueryAsync()).ToString();
                            break;
                        case SqlQueryType.Scalar:
                            result = JsonConvert.SerializeObject(await command.ExecuteScalarAsync());
                            break;
                        default:
                            reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                            using (var dataTable = new DataTable())
                            {
                                dataTable.Load(reader);
                                await reader.CloseAsync();
                                result = JsonConvert.SerializeObject(dataTable);
                            }
                            break;
                    }
                }
            }
            return result;
        }
    }
}