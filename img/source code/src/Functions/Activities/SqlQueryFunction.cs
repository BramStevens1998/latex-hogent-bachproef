using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Activities;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Dlw.Integration.DataAccelerator.Functions.Activities
{
    /// <summary>
    /// Respresents a durable functions task that executes a SQL query
    /// </summary>
    public class SqlQueryFunction : BaseFunction<SqlQueryRequest>, IFunction<SqlQueryRequest>
    {
        /// <summary>
        /// Initializes a new instance of the SqlQueryFunction
        /// </summary>
        /// <param name="container">Implementation of <see cref="IIOCContainer"/></param>
        public SqlQueryFunction(IIOCContainer container) : base(container) { }

        /// <summary>
        /// Executes the SQL query configured in a <see cref="SqlQueryRequest"/> and returns the result
        /// </summary>
        /// <param name="request">A <see cref="SqlQueryRequest"/></param>
        /// <param name="logger">Implementation of <see cref="ILogger"/></param>
        /// <returns>The result of the query.</returns>
        [FunctionName("SqlQueryFunction")]
        public override async Task<HttpResponseMessage> ExecuteAsync([ActivityTrigger] SqlQueryRequest request, ILogger logger)
        {
            // Initialize Environment
            if (!IOCContainer.StorageEnvironments.ContainsKey(request.Environment))
                throw new InvalidOperationException($"Storage environment {request.Environment} is not configured");
            var storageEnvironment = IOCContainer.StorageEnvironments[request.Environment];
            if (!IOCContainer.SqlEnvironments.ContainsKey(request.Environment))
                throw new InvalidOperationException($"Sql environment {request.Environment} is not configured");
            var sqlEnvironment = IOCContainer.SqlEnvironments[request.Environment];

            // Initialize Version
            var version = "Original";
            if (!string.IsNullOrWhiteSpace(request.Version))
                version = request.Version;
            if (!storageEnvironment.QueryVersions.ContainsKey(version))
                throw new InvalidOperationException($"Version {version} is not defined in configuration");

            // Get SqlQuery
            var action = request.Action.Name;
            if (!storageEnvironment.QueryVersions[version].ContainsKey(action))
                throw new InvalidOperationException($"Query {action} is not defined in configuration");
            var source = storageEnvironment.QueryVersions[version][action];

            // Initialize
            await request.Action.InitAsync(sqlEnvironment, source, request.Refresh);

            // Execute query
            var result = await request.Action.ExecuteAsync();

            // CallBack if needed
            await CallBackAsync(request, result);

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
        }
    }
}