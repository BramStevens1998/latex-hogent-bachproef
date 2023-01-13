using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Conversion;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Activities;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dlw.Integration.DataAccelerator.Functions.Activities
{
    /// <summary>
    /// Contains activities of the conversion function.
    /// </summary>
    public class ConvertFunction : BaseFunction<ConvertRequest>, IFunction<ConvertRequest>
    {
        /// <summary>
        /// Initializes a new instance of the ConvertFunction class.
        /// </summary>
        /// <param name="container">Implementation of <see cref="IIOCContainer"/>.</param>
        public ConvertFunction (IIOCContainer container) : base(container) { }

        /// <summary>
        /// Executes the conversion configured in a <see cref="ConvertRequest"/> and returns the result
        /// </summary>
        /// <param name="request">A <see cref="ConvertRequest"/></param>
        /// <param name="logger">Implementation of <see cref="ILogger"/></param>
        /// <returns>The result of this conversion activity.</returns>
        public override async Task<HttpResponseMessage> ExecuteAsync([ActivityTrigger] ConvertRequest request, ILogger logger)
        {
            var action = new Converter(request.InMessage, request.InFormat, request.OutFormat);
            // Execute convert
            var result = await action.ExecuteAsync();

            // CallBack if needed
            await CallBackAsync(request, result);

            return result;
        }
    }
}
