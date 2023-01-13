using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents an inline JSON payload.
    /// </summary>
    public class InlineJsonPayload : IInlinePayload<JRaw>
    {
        /// <summary>
        /// Inline JSON content <see cref="JRaw"/>.
        /// </summary>
        public JRaw _value { get; set; }

        /// <summary>
        /// Initializes a new instance of the InlineJsonPayload class.
        /// </summary>
        /// <param name="value">A <see cref="JRaw"/>.</param>
        public InlineJsonPayload(JRaw value)
        {
            _value = value;
        }

        /// <summary>
        /// Converts an <see cref="InlineJsonPayload"/> to a string.
        /// </summary>
        /// <param name="isp">An <see cref="InlineJsonPayload"/>.</param>
        public static implicit operator string(InlineJsonPayload isp)
        {
            return isp._value.ToString();
        }

        /// <summary>
        /// Creates an <see cref="InlineJsonPayload"/> from a given string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public static implicit operator InlineJsonPayload(string value)
        {
            return new InlineJsonPayload(JRaw.Create(new JsonTextReader(new StringReader(value))));
        }

        /// <summary>
        /// Gets a string value.
        /// </summary>
        public async Task<string> GetStringAsync()
        {
            return await Task.Run(() =>
            {
                return _value.ToString();
            });
        }

        /// <summary>
        /// Gets a byte array in UTF8 encoding from Inline JSON payload.
        /// </summary>
        public async Task<byte[]> GetBytesAsync()
        {
            return await Task.Run(() =>
            {
                return Encoding.UTF8.GetBytes(_value.ToString());
            });
        }

        /// <summary>
        /// Makes a memorystream containing Raw JSON content.
        /// </summary>
        /// <returns>A <see cref="Stream"/>.</returns>
        public async Task<Stream> GetStreamAsync()
        {
            Stream stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(_value.ToString());
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Sets the Inline JSON payload to the content of a string.
        /// </summary>
        /// <param name="content">The string to be set as content.</param>
        public async Task SetContent(string content)
        {
            _value = await JRaw.CreateAsync(new JsonTextReader(new StringReader(content)));
        }

        /// <summary>
        /// Sets the Inline JSON payload to the content of a <see cref="JRaw"/> object.
        /// </summary>
        /// <param name="content">The <see cref="JRaw"/> object to be set as content.</param>
        public void SetContent(JRaw content)
        {
            _value = content;
        }
    }
}