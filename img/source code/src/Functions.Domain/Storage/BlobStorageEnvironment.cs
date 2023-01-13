using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;
using Newtonsoft.Json;
using System.Collections.Generic;
using Azure.Storage.Blobs;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Storage
{
    /// <summary>
    /// Represents a blob storage environment.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BlobStorageEnvironment : IStorageEnvironment
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
        private readonly object mapppingVersionsLock = new object();

        /// <summary>
        /// <see cref="BlobServiceClient"/> of the blob storage environment.
        /// </summary>
        private BlobServiceClient client;

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
        /// Name of the blob storage environment.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Connectionstring of the blob storage environment.
        /// </summary>
        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Query container of the blob storage environment.
        /// </summary>
        [JsonProperty("QueryContainer")]
        public string QueryContainer { get; set; }

        /// <summary>
        /// Stored procedure container of the blob storage environment.
        /// </summary>
        [JsonProperty("StoredProcedureContainer")]
        public string StoredProcedureContainer { get; set; }

        /// <summary>
        /// Mapping container of the blob storage environment.
        /// </summary>
        [JsonProperty("MappingContainer")]
        public string MappingContainer { get; set; }

        /// <summary>
        /// Connects the <see cref="BlobServiceClient"/>.
        /// </summary>
        private BlobServiceClient Client
        {
            get
            {
                if (!IsConnected)
                    client = new BlobServiceClient(ConnectionString);
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
                        var queryClient = Client.GetBlobContainerClient(QueryContainer);
                        // Get versions
                        GetVersions(ref queryVersions, queryClient);
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
                        var storedProcedureClient = Client.GetBlobContainerClient(StoredProcedureContainer);
                        // Get versions
                        GetVersions(ref storedProcedureVersions, storedProcedureClient);
                    }
                }
                return storedProcedureVersions;
            }
        }

        /// <summary>
        /// Gets the versions of the mapping.
        /// </summary>
        public IDictionary<string, IDictionary<string, ISource>> MappingVersions
        {
            get
            {
                if (!IsInitialized)
                {
                    lock (mapppingVersionsLock)
                    {
                        mappingVersions = new Dictionary<string, IDictionary<string, ISource>>();
                        var mappingClient = Client.GetBlobContainerClient(MappingContainer);
                        // Get versions
                        GetVersions(ref mappingVersions, mappingClient);
                    }
                }
                return mappingVersions;
            }
        }
        
        /// <summary>
        /// Boolean representing whether or not the client is connected.
        /// </summary>
        public bool IsConnected { get { return client != null; } }

        /// <summary>
        /// Boolean representing whether or not the versions are initialized
        /// </summary>
        public bool IsInitialized { get { return queryVersions != null && storedProcedureVersions != null && mappingVersions != null; } }

        /// <summary>
        /// Initializes the BlobStorageEnvironment class.
        /// </summary>
        public BlobStorageEnvironment() { }

        /// <summary>
        /// Initializes the BlobStorageEnvironment class.
        /// </summary>
        /// <param name="name">The blob storage environment name.</param>
        /// <param name="connectionString">The blob storage environment connection string.</param>
        /// <param name="queryContainer">The blob storage environment query container.</param>
        /// <param name="storedProcedureContainer">The blob storage environment stored procedure container.</param>
        /// <param name="mappingContainer">The blob storage environment mapping container.</param>
        public BlobStorageEnvironment(string name, string connectionString, string queryContainer, string storedProcedureContainer, string mappingContainer)
        {
            Name = name;
            ConnectionString = connectionString;
            QueryContainer = queryContainer;
            StoredProcedureContainer = storedProcedureContainer;
            MappingContainer = mappingContainer;
        }

        /// <summary>
        /// Adds the version of a blob source to a dictionary.
        /// </summary>
        /// <param name="dictionary">Implements <see cref="IDictionary"/></param>
        /// <param name="client">The <see cref="BlobContainerClient"/></param>
        public void GetVersions(ref IDictionary<string, IDictionary<string, ISource>> dictionary, BlobContainerClient client)
        {
            dictionary = new Dictionary<string, IDictionary<string, ISource>>();
            dictionary.Add("Original", new Dictionary<string, ISource>());
            foreach (var blob in client.GetBlobs())
            {
                if (blob.Name.Contains('/'))
                {
                    var version = blob.Name.Substring(0, blob.Name.IndexOf('/'));
                    if (!dictionary.ContainsKey(version))
                        dictionary.Add(version, new Dictionary<string, ISource>());
                    dictionary[version].Add(new BlobSource(client.GetBlobClient(blob.Name)).GetKeyValuePair());
                }
                else
                    dictionary["Original"].Add(new BlobSource(client.GetBlobClient(blob.Name)).GetKeyValuePair());
            }
        }
    }
}