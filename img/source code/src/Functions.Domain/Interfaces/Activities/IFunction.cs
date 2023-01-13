using System.Net.Http;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Activities
{
    /// <summary>
    /// Represents a Function.
    /// </summary>
    /// <typeparam name="T">Implementation of <see cref="IDataAcceleratorRequest"/></typeparam>
    public interface IFunction<T> where T : IDataAcceleratorRequest
    {
        /// <summary>
        /// Creates a callback for a task.
        /// </summary>
        /// <param name="request">Implementation of the <see cref="IDataAcceleratorRequest"/>.</param>
        /// <param name="response">The callback response.</param>
        Task CallBackAsync(T request, string response);

        /// <summary>
        /// Creates a callback for a task.
        /// </summary>
        /// <param name="request">Implements the <see cref="IDataAcceleratorRequest"/>.</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
        Task CallBackAsync(T request, HttpResponseMessage response);

        /// <summary>
        /// Executes a task.
        /// </summary>
        /// <param name="request">Implements the <see cref="IDataAcceleratorRequest"/>.</param>
        /// <param name="logger">Implements the <see cref="ILogger"/>.</param>
        Task<HttpResponseMessage> ExecuteAsync([ActivityTrigger] T request, ILogger logger);
    }
}