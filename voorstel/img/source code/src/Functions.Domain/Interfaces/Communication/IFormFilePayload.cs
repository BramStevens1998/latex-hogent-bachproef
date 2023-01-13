using Microsoft.AspNetCore.Http;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents a form file payload.
    /// </summary>
    public interface IFormFilePayload : IPayload<IFormFile>
    {
    }
}
