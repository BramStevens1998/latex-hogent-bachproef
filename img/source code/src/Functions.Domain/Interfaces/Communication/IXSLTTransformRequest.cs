using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Xslt;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents an XSLT transform request.
    /// </summary>
    /// <typeparam name="T">Extension of <see cref="IXsltTransform"/></typeparam>
    public interface IXsltTransformRequest<T> : IDataAcceleratorRequest where T : IXsltTransform
    {
        /// <summary>
        /// Implementation of <see cref="IXsltTransform"/>.
        /// </summary>
        T Mapping { get; set; }

        /// <summary>
        /// Implementation of the incoming <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat InFormat { get; set; }

        /// <summary>
        /// Implementation of the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat OutFormat { get; set; }

        /// <summary>
        /// Implementation of <see cref="IPayload"/>.
        /// </summary>
        IPayload InMessage { get; set; }

        /// <summary>
        /// Boolean representing whether or not to refresh the source.
        /// </summary>
        bool Refresh { get; set; }
    }
}