using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using Newtonsoft.Json;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Sql
{
    /// <summary>
    /// Represents a SQL stored procedure action
    /// </summary>
    public class SqlStoredProcedure : BaseSqlAction<SqlStoredProcedureParameter>, ISqlAction<SqlStoredProcedureParameter>
    {
        /// <summary>
        /// Name of this SQL stored procedure action
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// List of SQL stored procedure parameters
        /// </summary>
        public override IList<SqlStoredProcedureParameter> Parameters { get; set; } = new List<SqlStoredProcedureParameter>();

        /// <summary>
        /// Initializes a new instance of the SqlStoredProcedure class.
        /// </summary>
        public SqlStoredProcedure() { }

        /// <summary>
        /// Initializes a new instance of the SqlStoredProcedure class.
        /// </summary>
        /// <param name="name">Name of this SQL stored procedure action</param>
        public SqlStoredProcedure(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes this SQL stored procedure action
        /// </summary>
        /// <param name="environment">Implementation of <see cref="ISqlEnvironment"/></param>
        /// <param name="source">Implementation of <see cref="ISource"/></param>
        /// <param name="refresh">Boolean representing whether or not to refresh the SQL command from Azure storage</param>
        /// <param name="checkParameters">Boolean representing whether or not to check for SQL parameters</param>
        /// <returns></returns>
        public override async Task InitAsync(ISqlEnvironment environment, ISource source, bool refresh = false, bool checkParameters = false)
        {
            await base.InitAsync(environment, source, refresh, checkParameters);

            await CreateAsync(environment, source);
        }

        /// <summary>
        /// Initializes this SQL stored procedure action
        /// </summary>
        /// <param name="environment">Implementation of <see cref="ISqlEnvironment"/></param>
        /// <param name="source">Implementation of <see cref="ISource"/></param>
        /// <param name="refresh">Boolean representing whether or not to refresh the SQL command from Azure storage</param>
        /// <param name="update">Boolean representing whether or not to update the SQL stored procedure in the SQL database</param>
        /// <param name="checkParameters">Boolean representing whether or not to check for SQL parameters</param>
        /// <returns></returns>
        public async Task InitAsync(ISqlEnvironment environment, ISource source, bool refresh, bool update, bool checkParameters)
        {
            await base.InitAsync(environment, source, refresh, checkParameters);

            await CreateAsync(environment, source, update);
        }

        /// <summary>
        /// Creates or updates the SQL stored procedure in SQL database
        /// </summary>
        /// <param name="environment">Implementation of <see cref="ISqlEnvironment"/></param>
        /// <param name="source">Implementation of <see cref="ISource"/></param>
        /// <param name="update">Boolean representing whether or not to update the SQL stored procedure if it allready exists</param>
        /// <returns></returns>
        public async Task CreateAsync(ISqlEnvironment environment, ISource source, bool update = false)
        {
            using (var connection = new SqlConnection(Environment.ConnectionString))
            {
                await connection.OpenAsync();
                object objectId;
                using (var command = new SqlCommand($"SELECT OBJECT_ID('{Name}', 'P')", connection))
                {
                    objectId = await command.ExecuteScalarAsync();
                }
                if (objectId != DBNull.Value && update)
                {
                    using (var dropCommand = new SqlCommand($"DROP PROCEDURE {Name}", connection))
                    {
                        await dropCommand.ExecuteNonQueryAsync();
                    }
                }
                if (objectId == DBNull.Value || update)
                {
                    using (var command = new SqlCommand(CommandText, connection))
                    {
                        var result = await command.ExecuteNonQueryAsync();
                    }
                }
                await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Executes this SQL stored procedure action
        /// </summary>
        /// <returns>Result of the execution of this SQL stored procedure action</returns>
        public override async Task<string> ExecuteAsync()
        {
            string result;
            using (var connection = new SqlConnection(Environment.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(Name, connection) { CommandType = CommandType.StoredProcedure })
                {
                    SqlDataReader reader;
                    command.Parameters.AddRange(Parameters.Select(x => new SqlParameter($"@{x.Name}", x.Value.GetStringAsync().Result)).Union(MissingParameters.Select(x => new SqlParameter(x, DBNull.Value))).ToArray());
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    using (var dataTable = new DataTable())
                    {
                        dataTable.Load(reader);
                        await reader.CloseAsync();
                        result = JsonConvert.SerializeObject(dataTable);
                    }
                }
            }
            return result;
        }
    }
}