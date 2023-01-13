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
    /// Respresents a durable functions task that executes an XSLT transform.
    /// </summary>
    public class XsltTransformFunction : BaseFunction<XsltTransformRequest>, IFunction<XsltTransformRequest>
    {
        /// <summary>
        /// Initializes a new instance of the XsltTransfromFunction.
        /// </summary>
        /// <param name="container">Implementation of <see cref="IIOCContainer"/></param>
        public XsltTransformFunction(IIOCContainer container) : base(container) { }

        /// <summary>
        /// Executes the XSLT transform function configured in a <see cref="XsltTransformRequest"/> and returns the result
        /// </summary>
        /// <param name="request">The <see cref="XsltTransformRequest"/>.</param>
        /// <param name="logger">Implementation of <see cref="ILogger"/></param>
        /// <returns>The result of the query.</returns>
        [FunctionName("XsltTransformFunction")]
        public override async Task<HttpResponseMessage> ExecuteAsync([ActivityTrigger] XsltTransformRequest request, ILogger logger)
        {
            // Initialize Environment
            if (!IOCContainer.StorageEnvironments.ContainsKey(request.Environment))
                throw new InvalidOperationException($"Storage environment {request.Environment} is not configured");
            var storageEnvironment = IOCContainer.StorageEnvironments[request.Environment];

            // Initialize Version
            var version = "Original";
            if (!string.IsNullOrWhiteSpace(request.Version))
                version = request.Version;
            if (!storageEnvironment.MappingVersions.ContainsKey(version))
                throw new InvalidOperationException($"Version {version} is not defined in configuration");

            // Get content
            var action = request.Mapping.Name;
            if (!storageEnvironment.MappingVersions[version].ContainsKey(action))
                throw new InvalidOperationException($"Mapping {action} is not defined in configuration");
            var source = storageEnvironment.MappingVersions[version][action];

            // Initialize
            await request.Mapping.InitAsync(source, request.InFormat, request.OutFormat, request.InMessage, request.Refresh);

            // Execute query
            var result = await request.Mapping.ExecuteAsync();

            // CallBack if needed
            await CallBackAsync(request, result);

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
        }
    }
}