using System.IO;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents a form file payload.
    /// </summary>
    public class FormFilePayload : IFormFilePayload
    {
        /// <summary>
        /// Form file content <see cref="IFormFile"/>.
        /// </summary>
        public IFormFile _value { get; set; }

        /// <summary>
        /// Initializes a new instance of the FormFilePayload class.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <param name="index">The reference to the 1st (zero-based) 
        /// file contained in the form data of the request.</param>
        public FormFilePayload(HttpRequest request, int index)
        {
            _value = request.Form.Files[index];
        }

        /// <summary>
        /// Gets a string value.
        /// </summary>
        public async Task<string> GetStringAsync()
        {
            var result = string.Empty;
            using (var reader = new StreamReader(_value.OpenReadStream()))
            {
                result = await reader.ReadToEndAsync();
            }
            return result;
        }

        /// <summary>
        /// Gets a byte array from form file payload.
        /// </summary>
        public async Task<byte[]> GetBytesAsync()
        {
            using var ms = new MemoryStream();
            await this._value.CopyToAsync(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Makes a <see cref="Stream"/> containing form file content.
        /// </summary>
        public async Task<Stream> GetStreamAsync()
        {
            return await Task.Run(() =>
            {
                return _value.OpenReadStream();
            });
        }

        /// <summary>
        /// Sets the form file payload to the content of a string
        /// </summary>
        /// <param name="content">The string to be set as content.</param>
        public async Task SetContent(string content)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(content);
                    await writer.FlushAsync();
                }
                stream.Position = 0;

                _value = new FormFile(stream, 0, content.Length, null, null);
            }
        }

        /// <summary>
        /// Sets the form file payload to the content of a <see cref="IFormFile"/> object.
        /// </summary>
        /// <param name="content">The <see cref="IFormFile"/> object to be set as content.</param>
        public void SetContent(IFormFile content)
        {
            _value = content;
        }
    }
}