using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Activities;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;


namespace Dlw.Integration.DataAccelerator.EntryPoints
{
    /// <summary>
    /// Respresents a class of function entry points.
    /// </summary>
    public class EntryPointFunction
    {
        /// <summary>
        /// <see cref="IIOCContainer"/> being used by function calls.
        /// </summary>
        protected IIOCContainer IOCContainer { get; set; }

        /// <summary>
        /// Initializes a new instance of the EntryPointFunction class.
        /// </summary>
        /// <param name="container">A <see cref="IIOCContainer"/>.</param>
        public EntryPointFunction(IIOCContainer container)
        {
            IOCContainer = container;
        }

        /// <summary>
        /// Gets the header parameters from the request message and uses them to initialize an <see cref="IDataAcceleratorRequest"/>.
        /// </summary>
        /// <param name="req">A HTTP request message.</param>
        /// <param name="request">A <see cref="HttpRequestMessage"/>.</param>
        /// <typeparam name="T">A type implementing <see cref="IDataAcceleratorRequest"/>.</typeparam>
        /// <returns>A <see cref="IDataAcceleratorRequest"/>.</returns>
        public IDataAcceleratorRequest ProcessHeaders<T>(HttpRequestMessage req, ref T request) where T : IDataAcceleratorRequest
        {
            
            var headers = req.Headers.ToDictionary(x => x.Key, x => x.Value);

            IEnumerable<string> durable;
            if (headers.TryGetValue("durable", out durable))
            {
                request.Durable = bool.Parse(durable.First<string>());
            }

            IEnumerable<string> callbackUrl;
            if (headers.TryGetValue("callbackUrl", out callbackUrl))
            {
                request.CallbackUrl = callbackUrl.First<string>();
            }

            IEnumerable<string> environment;
            if (headers.TryGetValue("environment", out environment))
            {
                request.Environment = callbackUrl.First<string>();
            }

            return request;
        }

        /// <summary>
        /// Creates a <see cref="Task"/> that creates a SQL query request.
        /// </summary>
        /// <param name="req">A <see cref="HttpRequest"/>.</param>
        /// <param name="client">A <see cref="IDurableOrchestrationClient"/></param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [FunctionName("Query")]
        public async Task<HttpResponseMessage> Query(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {

            var requestMessage = new HttpRequestMessageFeature(req.HttpContext).HttpRequestMessage;
            var instanceId = Guid.NewGuid().ToString();
            var body = string.Empty;
            if (req.GetTypedHeaders().ContentType.MediaType.Equals("multipart/form-data"))
            {
                body = req.Form.FirstOrDefault().Value;
            }
            else
                body = await requestMessage.Content.ReadAsStringAsync();
            var queryRequest = JsonConvert.DeserializeObject<SqlQueryRequest>(body, new IPayloadConverter(req));
            ProcessHeaders<SqlQueryRequest>(requestMessage, ref queryRequest);

            if (queryRequest.Durable)
            {
                var result = await client.StartNewAsync("OrchestrateSqlQuery", instanceId, queryRequest);
                return (HttpResponseMessage)client.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                var result = await new SqlQueryFunction(IOCContainer).ExecuteAsync(queryRequest, log);
                return result;
            }
        }

        /// <summary>
        /// Creates a <see cref="Task"/> that creates an SQL stored procedure request.
        /// </summary>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="client">A <see cref="IDurableOrchestrationClient"/></param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [FunctionName("Execute")]
        public async Task<HttpResponseMessage> Execute(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var requestMessage = new HttpRequestMessageFeature(req.HttpContext).HttpRequestMessage;
            var instanceId = Guid.NewGuid().ToString();
            var body = string.Empty;
            if (req.GetTypedHeaders().ContentType.MediaType.Equals("multipart/form-data"))
            {
                body = req.Form.FirstOrDefault().Value;
            }
            else
                body = await requestMessage.Content.ReadAsStringAsync();
            var storedProcedureRequest = JsonConvert.DeserializeObject<SqlStoredProcedureRequest>(body, new IPayloadConverter(req));
            ProcessHeaders<SqlStoredProcedureRequest>(requestMessage, ref storedProcedureRequest);

            if (storedProcedureRequest.Durable)
            {
                var result = await client.StartNewAsync("OrchestrateSqlStoredProcedure", instanceId, storedProcedureRequest);
                return (HttpResponseMessage)client.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                var response = await new SqlStoredProcedureFunction(IOCContainer).ExecuteAsync(storedProcedureRequest, log);
                return response;
            }

        }

        /// <summary>
        /// Creates a <see cref="Task"/> that creates an XSLT transform request.
        /// </summary>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="client">A <see cref="IDurableOrchestrationClient"/></param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [FunctionName("Transform")]
        public async Task<HttpResponseMessage> Transform(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var requestMessage = new HttpRequestMessageFeature(req.HttpContext).HttpRequestMessage;
            var instanceId = Guid.NewGuid().ToString();
            var body = string.Empty;
            if (req.GetTypedHeaders().ContentType.MediaType.Equals("multipart/form-data"))
            {
                body = req.Form.FirstOrDefault().Value;
            }
            else
                body = await requestMessage.Content.ReadAsStringAsync();
            var xsltTransformRequest = JsonConvert.DeserializeObject<XsltTransformRequest>(body, new IPayloadConverter(req));
            ProcessHeaders<XsltTransformRequest>(requestMessage, ref xsltTransformRequest);

            if (xsltTransformRequest.Durable)
            {
                var result = await client.StartNewAsync("OrchestrateXsltTransform", instanceId, xsltTransformRequest);
                return (HttpResponseMessage)client.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                var result = await new XsltTransformFunction(IOCContainer).ExecuteAsync(xsltTransformRequest, log);
                return result;
            }

        }

        /// <summary>
        /// Creates a <see cref="Task"/> that creates a convert request.
        /// </summary>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="client">A <see cref="IDurableOrchestrationClient"/></param>
        /// <param name="log">Implementation of <see cref="ILogger"/></param>
        /// <returns>A <see cref="Task"/>.</returns>
        [FunctionName("Convert")]
        public async Task<HttpResponseMessage> Convert(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var requestMessage = new HttpRequestMessageFeature(req.HttpContext).HttpRequestMessage;
            var instanceId = Guid.NewGuid().ToString();
            var body = string.Empty;
            if (req.GetTypedHeaders().ContentType.MediaType.Equals("multipart/form-data"))
            {
                body = req.Form.FirstOrDefault().Value;
            }
            else
                body = await requestMessage.Content.ReadAsStringAsync();
            var convertRequest = JsonConvert.DeserializeObject<ConvertRequest>(body, new IPayloadConverter(req));
            ProcessHeaders<ConvertRequest>(requestMessage, ref convertRequest);
            
            if (convertRequest.Durable)
            {
                var result = await client.StartNewAsync("OrchestrateConvert", instanceId, convertRequest);
                return (HttpResponseMessage)client.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                var result = await new ConvertFunction(IOCContainer).ExecuteAsync(convertRequest, log);
                return result;
            }
        }
    }
}