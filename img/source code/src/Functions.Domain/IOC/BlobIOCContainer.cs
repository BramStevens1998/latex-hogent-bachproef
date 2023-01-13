using System.Collections.Generic;
using System.Linq;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using Dlw.Integration.DataAccelerator.Functions.Domain.Sql;
using Dlw.Integration.DataAccelerator.Functions.Domain.Storage;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.IOC
{
    /// <summary>
    /// Represents a blob IOCContainer
    /// </summary>
    public class BlobIOCContainer : IIOCContainer<BlobStorageEnvironment>, IIOCContainer
    {
        /// <summary>
        /// <see cref="IDictionary"/> of SQL environments.
        /// </summary>
        public IDictionary<string, SqlEnvironment> SqlEnvironments { get; set; }

        /// <summary>
        /// <see cref="IDictionary"/> of storage environments.
        /// </summary>
        public IDictionary<string, BlobStorageEnvironment> StorageEnvironments { get; set; }

        /// <summary>
        /// Method to cast to the correct interface.
        /// </summary>
        IDictionary<string, IStorageEnvironment> IIOCContainer<IStorageEnvironment>.StorageEnvironments => StorageEnvironments.Select
        (x => new KeyValuePair<string, IStorageEnvironment>(x.Key, x.Value)).ToDictionary(x => x.Key, y => y.Value);
    }
}