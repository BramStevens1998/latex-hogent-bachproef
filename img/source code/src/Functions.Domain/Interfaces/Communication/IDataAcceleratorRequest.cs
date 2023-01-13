namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents a Data Accelerator request.
    /// </summary>
    public interface IDataAcceleratorRequest : IRequest
    {
        /// <summary>
        /// Boolean representing if the azure function called with this request should operate
        ///  in a <see href="https://learn.microsoft.com/en-us/azure/azure-functions/durable/">durable</see> or classic way.
        /// </summary>
        bool Durable { get; set; }

        /// <summary>
        /// URL to be called with the response of the azure function that was called with this dataaccelerator request
        /// </summary>
        string CallbackUrl { get; set; }

        /// <summary>
        /// Environment to be initialized that was called with this dataaccelerator request.
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// The version to be initalized that was called with this dataaccelerator request.
        /// </summary>
        string Version { get; set; }
    }
}