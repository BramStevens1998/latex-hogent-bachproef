using System.IO;
using System.Threading.Tasks;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage
{
    /// <summary>
    /// In-memory representation of a file on Azure storage
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Name of this source
        /// </summary>
        string Name { get; }

        /// <summary>
        /// <see cref="MemoryStream"/> containing file data
        /// </summary>
        MemoryStream Content { get; }

        /// <summary>
        /// Boolean representing whether or not this source is downloaded from Azure storage
        /// </summary>
        bool IsDownloaded { get; }

        /// <summary>
        /// Refreshes the contents of this source
        /// </summary>
        Task Refresh();
    }
}