using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Storage
{
    /// <summary>
    /// In-memory representation of a file on a Azure storage share
    /// </summary>
    public class ShareSource : ISource
    {
        private readonly object contentLock = new object();

        private Task downloadTask
        {
            get
            {
                var t = new Task(() =>
                {
                    Client.DownloadAsync().ContinueWith((t) =>
                    {
                        lock (contentLock)
                        {
                            var result = t.Result.Value;
                            result.Content.CopyTo(content);
                            content.Seek(0, 0);
                        }
                    });
                });
                return t;
            }
        }

        private MemoryStream content;

        /// <summary>
        /// Name of this source
        /// </summary>
        public string Name { get; private set; }

        private ShareFileClient Client { get; set; }

        /// <summary>
        /// In-memory content of this Azure share file
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
        /// Boolean representing whether or not this source is downloaded from a Azure storage share
        /// </summary>
        public bool IsDownloaded
        {
            get
            {
                return content != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ShareSource class.
        /// </summary>
        /// <param name="directoryClient">ShareDirectoryClient that connects to a Azure storage share</param>
        /// <param name="fileName">The name of the client file.</param>
        public ShareSource(ShareDirectoryClient directoryClient, string fileName)
        {
            Name = fileName.Substring(0, fileName.LastIndexOf('.'));
            Client = directoryClient.GetFileClient(fileName);
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