using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents an inline string payload.
    /// </summary>
    public class InlineStringPayload : IInlinePayload<string>
    {
        /// <summary>
        /// Inline string content.
        /// </summary>
        public string _value { get; set; }

        /// <summary>
        /// Initializes a new instance of the InlineStringPayload class.
        /// </summary>
        /// <param name="value">A string.</param>
        public InlineStringPayload(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Converts an <see cref="InlineStringPayload"/> to a string value.
        /// </summary>
        /// <param name="isp">An <see cref="InlineStringPayload"/>.</param>
        public static implicit operator string(InlineStringPayload isp)
        {
            return isp._value;
        }

        /// <summary>
        /// Creates an <see cref="InlineStringPayload"/> from a given string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public static implicit operator InlineStringPayload(string value)
        {
            return new InlineStringPayload(value);
        }

        /// <summary>
        /// Gets a string value.
        /// </summary>
        public async Task<string> GetStringAsync()
        {
            return await Task.Run(() =>
            {
                return _value;
            });
        }

        /// <summary>
        /// Gets a byte array in UTF8 encoding from inline string payload.
        /// </summary>
        public async Task<byte[]> GetBytesAsync() {
            return await Task.Run(() => {
                return Encoding.UTF8.GetBytes(_value);
            });
        }

        /// <summary>
        /// Makes a memorystreamcontaining string content.
        /// </summary>
        /// <returns>A <see cref="Stream"/>.</returns>
        public async Task<Stream> GetStreamAsync() {
            Stream stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(_value);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
            
        }

        /// <summary>
        /// Sets the inline string payload to the content of a string
        /// </summary>
        /// <param name="content">The string to be set as content.</param>
        public Task SetContent(string content)
        {
            return Task.Run(() =>
            {
                _value = content;
            });
        }

        /// <summary>
        /// Sets the inline string payload to the content of a string object.
        /// </summary>
        /// <param name="content">The string object to be set as content.</param>
        void IPayload<string>.SetContent(string content)
        {
            _value = content;
        }
    }
}