namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents an inline payload.
    /// </summary>
    public interface IInlinePayload : IPayload
    {
    }

    /// <summary>
    /// Represent an inline payload.
    /// </summary>
    public interface IInlinePayload<T> : IInlinePayload, IPayload<T>
    {
    }
}
