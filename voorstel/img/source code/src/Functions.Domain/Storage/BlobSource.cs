using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Storage
{
    /// <summary>
    /// In-memory representation of a file on Azure blob storage
    /// </summary>
    public class BlobSource : ISource
    {
        private readonly object contentLock = new object();

        private Task downloadTask
        {
            get
            {
                var t = new Task(() =>
                {
                    lock(contentLock) { Client.DownloadTo(content); }
                });
                return t;
            }
        }

        /// <summary>
        /// <see cref="MemoryStream"/> containing file data
        /// </summary>
        private MemoryStream content;

        /// <summary>
        /// Name of this source
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// <see cref="BlobClient"/> of this source.
        /// </summary>
        /// <value></value>
        private BlobClient Client { get; set; }

        /// <summary>
        /// In-memory content of this Azure blob
        /// </summary>
        public MemoryStream Content
        {
            get
            {
                if (!IsDownloaded)
                {
                    downloadTask.Wait();
                }
                var temp = new MemoryStream();
                lock (contentLock)
                {
                    content.Seek(0, 0);
                    content.CopyTo(temp);
                }
                temp.Seek(0, 0);
                return temp;
            }
        }

        /// <summary>
        /// Boolean representing whether or not this source is downloaded from Azure blob storage
        /// </summary>
        public bool IsDownloaded
        {
            get
            {
                return content != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BlobSource class.
        /// </summary>
        /// <param name="client">BlobClient that connects to Azure blob storage</param>
        public BlobSource(BlobClient client)
        {
            Name = client.Name.Substring(client.Name.LastIndexOf('/') + 1);
            Name = Name.Substring(0, Name.LastIndexOf('.'));
            Client = client;
            content = new MemoryStream();
            downloadTask.Start();
        }

        public async Task Refresh() {
            await Task.Run(() => {
                content = new MemoryStream();
                downloadTask.Start();
            });
        }

        /// <summary>
        /// Produces a <see cref="KeyValuePair"/> of the name of this source and this source itself
        /// </summary>
        /// <returns><see cref="KeyValuePair"/> of the name of this source and this source itself</returns>
        public KeyValuePair<string, ISource> GetKeyValuePair()
        {
            return KeyValuePair.Create<string, ISource>(Name, this);
        }
    }
}