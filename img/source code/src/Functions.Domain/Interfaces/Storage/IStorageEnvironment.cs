using System.Collections.Generic;
using Azure.Storage.Files.Shares;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage
{
    /// <summary>
    /// Represents a storage environment on Azure.
    /// </summary>
    public interface IStorageEnvironment
    {
        /// <summary>
        /// Name of this storage environment.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// <see cref="ConnectionString"/> of this storage environment.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Key value pair representing the versions of the queries.
        /// </summary>
        IDictionary<string, IDictionary<string, ISource>> QueryVersions { get; }

        /// <summary>
        /// Key value pair representing the versions of the stored procedures.
        /// </summary>
        IDictionary<string, IDictionary<string, ISource>> StoredProcedureVersions { get; }
        
        /// <summary>
        /// Key value pair representing the versions of the mappings.
        /// </summary>
        IDictionary<string, IDictionary<string, ISource>> MappingVersions { get; }
    }
}