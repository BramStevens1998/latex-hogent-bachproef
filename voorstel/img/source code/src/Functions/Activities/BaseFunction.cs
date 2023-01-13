using System.Net.Http;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Activities;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Dlw.Integration.DataAccelerator.Functions.Activities
{
    /// <summary>
    /// Contains activities of the base function.
    /// </summary>
    /// <typeparam name="T">Extension of <see cref="IDataAcceleratorRequest"/></typeparam>
    public abstract class BaseFunction<T> : IFunction<T> where T : IDataAcceleratorRequest
    {
        /// <summary>
        /// Implementation of <see cref="IOCContainer"/>
        /// </summary>
        protected IIOCContainer IOCContainer { get; set; }

        /// <summary>
        /// Initializes a new instance of the BaseFunction class.
        /// </summary>
        /// <param name="container">The <see cref="IOCContainer"/>.</param>
        public BaseFunction(IIOCContainer container)
        {
            IOCContainer = container;
        }

        /// <summary>
        /// Calls an endpoint that is specified in the request with the result.
        /// </summary>
        /// <param name="request">An <see cref="IDataAcceleratorRequest"/></param>
        /// <param name="result">Content of the request.</param>
        public async Task CallBackAsync(T request, string result)
        {
            if (!string.IsNullOrWhiteSpace(request.CallbackUrl))
            {
                var callBackRequest = new HttpRequestMessage(HttpMethod.Post, request.CallbackUrl);
                callBackRequest.Content = new StringContent(result);
                await new HttpClient().SendAsync(callBackRequest);
            }
        }

        /// <summary>
        /// Calls an endpoint that is specified in the request with the result.
        /// </summary>
        /// <param name="request">An <see cref="IDataAcceleratorRequest"/></param>
        /// <param name="result">The <see cref="HttpResponseMessage"/>.</param>
        public async Task CallBackAsync(T request, HttpResponseMessage result) {
            if (!string.IsNullOrWhiteSpace(request.CallbackUrl))
            {
                var callBackRequest = new HttpRequestMessage(HttpMethod.Post, request.CallbackUrl);
                callBackRequest.Content = result.Content;
                foreach(var header in result.Headers)
                {
                    callBackRequest.Headers.Add(header.Key, header.Value);
                }
                await new HttpClient().SendAsync(callBackRequest);
            }
        }

        /// <summary>
        /// Executes this base function.
        /// </summary>
        /// <param name="request">A <see cref="IDataAcceleratorRequest"/></param>
        /// <param name="logger">Implementation of <see cref="ILogger"/></param>
        /// <returns>Result of the execution of this base function.</returns>
        public abstract Task<HttpResponseMessage> ExecuteAsync([ActivityTrigger] T request, ILogger logger);
    }
}