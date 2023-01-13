using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using Dlw.Integration.DataAccelerator.Functions.Domain.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Xslt
{
    /// <summary>
    /// Respresents an XSLT transform.
    /// </summary>
    public interface IXsltTransform
    {
        /// <summary>
        /// The XSLT transform name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The XSLT source.
        /// </summary>
        ISource Source { get; }

        /// <summary>
        /// Implements the incoming <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat InFormat { get; }

        /// <summary>
        /// Implements the outgoing <see cref="MessageFormat"/>.
        /// </summary>
        MessageFormat OutFormat { get; }

        /// <summary>
        /// Implements the incoming <see cref="IPayload"/>.
        /// </summary>
        IPayload InMessage { get; }

        /// <summary>
        /// Initializes an XLST transform task.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inFormat">Implements the <see cref="MessageFormat"/>.</param>
        /// <param name="outFormat">Implements the <see cref="MessageFormat"/>.</param>
        /// <param name="inMessage">Implements the <see cref="IPayload"/></param>.
        /// <param name="refresh">Boolean representing whether or not to refresh <see cref="ISource"/>.</param>
        Task InitAsync(ISource source, MessageFormat inFormat, MessageFormat outFormat, IPayload inMessage, bool refresh);
        
        /// <summary>
        /// Executes this XSLT transform.
        /// </summary>
        Task<string> ExecuteAsync();
    }
}