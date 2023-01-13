using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using System.Net.Http;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Conversion
{
    /// <summary>
    /// Represents a converter class.
    /// </summary>
    public interface IConverter
    {

        /// <summary>
        /// Implements the incoming <see cref="IPayload"/>.
        /// </summary>
        IPayload InMessage { get; set; }

        /// <summary>
        /// Implements the incoming <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat InFormat { get; set; }

        /// <summary>
        /// Implements the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat OutFormat { get; set; }

        string Delimiter { get; set; }

        /// <summary>
        /// Initializes this conversion to be able to execute.
        /// </summary>
        /// <param name="inMessage">Implementation of <see cref="IPayload"/></param>
        /// <param name="inFormat">Implementation of <see cref="MessageFormat"/></param>
        /// <param name="outFormat">Implementation of <see cref="MessageFormat"/></param>
        /// <param name="Delimiter">The delimiter of the file.</param>
        Task InitAsync(IPayload inMessage, MessageFormat inFormat, MessageFormat outFormat, string Delimiter = ";");

        /// <summary>
        /// Executes this conversion.
        /// </summary>
        /// <returns>Result of the execution of this conversion.</returns>
        Task<HttpResponseMessage> ExecuteAsync();

    }
}