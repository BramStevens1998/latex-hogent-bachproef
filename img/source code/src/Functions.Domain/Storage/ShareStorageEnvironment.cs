using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using Azure.Storage.Files.Shares;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Storage
{
    /// <summary>
    /// Represents a share storage environment.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ShareStorageEnvironment : IStorageEnvironment
    {
        /// <summary>
        /// Represents a query versions lock.
        /// </summary>
        private readonly object queryVersionsLock = new object();

        /// <summary>
        /// Represents a stored procedure versions lock.
        /// </summary>
        private readonly object storedProcedureVersionsLock = new object();

        /// <summary>
        /// Represents a mapping versions lock.
        /// </summary>
        private readonly object mappingVersionsLock = new object();

        /// <summary>
        /// <see cref="ShareClient"/> of the share storage environment
        /// </summary>
        private ShareClient client;

        /// <summary>
        /// <see cref="IDictionary"/> of the query versions.
        /// </summary>
        private IDictionary<string, IDictionary<string, ISource>> queryVersions;

        /// <summary>
        /// <see cref="IDictionary"/> of the stored procedure versions.
        /// </summary>
        private IDictionary<string, IDictionary<string, ISource>> storedProcedureVersions;

        /// <summary>
        /// <see cref="IDictionary"/> of the mapping versions.
        /// </summary>
        private IDictionary<string, IDictionary<string, ISource>> mappingVersions;

        /// <summary>
        /// Name of the share storage environment.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Connectionstring of the share storage environment.
        /// </summary>
        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the share.
        /// </summary>
        [JsonProperty("ShareName")]
        public string ShareName { get; set; }

        /// <summary>
        /// Connects the <see cref="ShareClient"/>.
        /// </summary>
        public ShareClient Client
        {
            get
            {
                if (!IsConnected)
                    client = new ShareClient(ConnectionString, ShareName);
                return client;
            }
        }

        /// <summary>
        /// Gets the versions of the queries.
        /// </summary>
        public IDictionary<string, IDictionary<string, ISource>> QueryVersions
        {
            get
            {
                if (!IsInitialized)
                {
                    lock (queryVersionsLock)
                    {
                        queryVersions = new Dictionary<string, IDictionary<string, ISource>>();
                        var queryClient = Client.GetDirectoryClient("Queries");
                        // Get Original
                        GetVersion(queryVersions, "Original", queryClient);
                        // Get Versions
                        GetVersions(queryVersions, queryClient);
                    }
                }
                return queryVersions;
            }
        }

        /// <summary>
        /// Gets the versions of the stored procedures.
        /// </summary>
        public IDictionary<string, IDictionary<string, ISource>> StoredProcedureVersions
        {
            get
            {
                if (!IsInitialized)
                {
                    lock (storedProcedureVersionsLock)
                    {
                        storedProcedureVersions = new Dictionary<string, IDictionary<string, ISource>>();
                        var storedProcedureClient = Client.GetDirectoryClient("StoredProcedures");
                        // Get Original
                        GetVersion(storedProcedureVersions, "Original", storedProcedureClient);
                        // Get Versions
                        GetVersions(storedProcedureVersions, storedProcedureClient);
                    }
                }
                return storedProcedureVersions;
            }
        }

        /// <summary>
        /// Gets the versions of the mappings.
        /// </summary>
        public IDictionary<string, IDictionary<string, ISource>> MappingVersions
        {
            get
            {
                if (!IsInitialized)
                {
                    lock (mappingVersionsLock)
                    {
                        mappingVersions = new Dictionary<string, IDictionary<string, ISource>>();
                        var mappingClient = Client.GetDirectoryClient("StoredProcedures");
                        // Get Original
                        GetVersion(mappingVersions, "Original", mappingClient);
                        // Get Versions
                        GetVersions(mappingVersions, mappingClient);
                    }
                }
                return mappingVersions;
            }
        }

        /// <summary>
        /// Boolean representing whether or not the client is connected
        /// </summary>
        public bool IsConnected { get { return client != null; } }

        /// <summary>
        /// Boolean representing whether or not the versions are initialized
        /// </summary>
        public bool IsInitialized { get { return queryVersions != null && storedProcedureVersions != null; } }

        /// <summary>
        /// Initializes the ShareStorageEnvironment class.
        /// </summary>
        public ShareStorageEnvironment() { }

        /// <summary>
        /// Initializes the ShareStorageEnvironment class.
        /// </summary>
        /// <param name="name">Name of the share storage environment.</param>
        /// <param name="connectionString">The share storage environment connection string.</param>
        /// <param name="shareName">The share name.</param>
        public ShareStorageEnvironment(string name, string connectionString, string shareName) {
            Name = name;
            ConnectionString = connectionString;
            ShareName = shareName;
        }

        /// <summary>
        /// Gets the the dictionary of the versions.
        /// </summary>
        /// <param name="dictionary">Implements <see cref="IDictionary"/></param>
        /// <param name="client">The <see cref="ShareClient"/></param>
        public void GetVersions(IDictionary<string, IDictionary<string, ISource>> dictionary, ShareDirectoryClient client)
        {
            foreach (var item in client.GetFilesAndDirectories())
            {
                if (item.IsDirectory)
                    GetVersion(dictionary, item.Name, client.GetSubdirectoryClient(item.Name));
            }
        }

        /// <summary>
        /// Gets the version out of the dictionary.
        /// </summary>
        /// <param name="dictionary">Implements <see cref="IDictionary"/></param>
        /// <param name="version">The version of the share.</param>
        /// <param name="client">The <see cref="ShareClient"/></param>
        public void GetVersion(IDictionary<string, IDictionary<string, ISource>> dictionary, string version, ShareDirectoryClient client)
        {
            dictionary.Add(version, new Dictionary<string, ISource>());
            foreach (var item in client.GetFilesAndDirectories())
            {
                if (!item.IsDirectory)
                    dictionary[version].Add(new ShareSource(client, item.Name).GetKeyValuePair());
            }
        }
    }
}