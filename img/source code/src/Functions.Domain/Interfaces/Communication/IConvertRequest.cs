using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents a convert request.
    /// </summary>
    public interface IConvertRequest
    {
        /// <summary>
        /// Implentation of the incoming <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat InFormat { get; set; }

        /// <summary>
        /// Implementation of the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat OutFormat { get; set; }

        string Delimiter {get;set;}

        /// <summary>
        /// Implementation of the incoming message <see cref="IPayload"/>.
        /// </summary>
        IPayload InMessage { get; set; }
    }
}
