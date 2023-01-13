using System.Collections.Generic;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using Dlw.Integration.DataAccelerator.Functions.Domain.Sql;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC
{
    /// <summary>
    /// Represents an IOCContainer
    /// </summary>
    /// <typeparam name="T">Implementation of <see cref="IStorageEnvironment"/>.</typeparam>
    public interface IIOCContainer<T> where T : IStorageEnvironment
    {
        // AppSettings
        IDictionary<string, SqlEnvironment> SqlEnvironments { get; }
        IDictionary<string, T> StorageEnvironments { get; }
    }

    /// <summary>
    /// Represents an IOCContainer
    /// </summary>
    public interface IIOCContainer : IIOCContainer<IStorageEnvironment> { }
}