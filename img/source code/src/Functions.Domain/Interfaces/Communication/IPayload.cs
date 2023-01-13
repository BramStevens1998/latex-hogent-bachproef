using System.IO;
using System.Threading.Tasks;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents a payload.
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Respresent an asynchronous task that returns a string.
        /// </summary>
        Task<string> GetStringAsync();

        /// <summary>
        /// Represents an asynchronous task that returns a byte array.
        /// </summary>
        Task<byte[]> GetBytesAsync();

        /// <summary>
        /// Represents an asynchronous task that returns a stream.
        /// </summary>
        Task<Stream> GetStreamAsync();

        /// <summary>
        /// Sets the content to a given value.
        /// </summary>
        /// <param name="content">The content value.</param>
        Task SetContent(string content);
    }

    /// <summary>
    /// Represents a payload.
    /// </summary>
    public interface IPayload<T> : IPayload
    {
        /// <summary>
        /// Implementation of <see cref="IPayload"/>.
        /// </summary>
        T _value { get; set; }

        /// <summary>
        /// Sets the content of the payload.
        /// </summary>
        /// <param name="content">The content of the payload.</param>
        void SetContent(T content);
    }
}
