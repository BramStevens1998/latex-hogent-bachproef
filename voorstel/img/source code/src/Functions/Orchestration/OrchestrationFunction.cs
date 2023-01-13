using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Dlw.Integration.DataAccelerator.Functions.Orchestration
{
    /// <summary>
    /// Respresents a .
    /// </summary>
    public class OrchestrationFunction
    {
        /// <summary>
        /// Converts a .
        /// </summary>
        /// <param name="context">The <see cref="IDurableOrchestrationContext"/>.</param>
        /// <param name="activity">The activity name.</param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <typeparam name="T">Implementation of <see cref="IDataAcceleratorRequest"/>.</typeparam>
        /// <returns>A <see cref="Task"/></returns>
        public async Task<string> Orchestrate<T>([OrchestrationTrigger] IDurableOrchestrationContext context, string activity, ILogger log) where T : IDataAcceleratorRequest
        {
            var data = context.GetInput<T>();
            var response = await context.CallActivityAsync<string>(activity, data);
            return response;
        }

        /// <summary>
        /// Creates a SQL query request from the request object.
        /// </summary>
        /// <param name="context">The <see cref="IDurableOrchestrationContext"/>.</param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [FunctionName("OrchestrateSqlQuery")]
        public async Task<string> OrchestrateSqlQuery([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            return await Orchestrate<SqlQueryRequest>(context, "SqlQueryFunction", log);
        }

        /// <summary>
        /// Creates a SQL stored procedure request from the request object.
        /// </summary>
        /// <param name="context">The <see cref="IDurableOrchestrationContext"/>.</param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [FunctionName("OrchestrateSqlStoredProcedure")]
        public async Task<string> OrchestrateSqlStoredProcedure([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            return await Orchestrate<SqlStoredProcedureRequest>(context, "SqlStoredProcedureFunction", log);
        }

        /// <summary>
        /// Creates a Xslt transform request from the request object.
        /// </summary>
        /// <param name="context">The <see cref="IDurableOrchestrationContext"/></param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/>.</returns>
        [FunctionName("OrchestrateXsltTransform")]
        public async Task<string> OrchestrateXsltTransform([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            return await Orchestrate<XsltTransformRequest>(context, "XsltTransformFunction", log);
        }
    }
}