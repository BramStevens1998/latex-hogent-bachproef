using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication
{
    /// <summary>
    /// Represents a SQL request.
    /// </summary>
    /// <typeparam name="T">Extension of <see cref="ISqlAction"/></typeparam>
    public interface ISqlRequest<T> : IDataAcceleratorRequest where T : ISqlAction
    {
        /// <summary>
        /// Implementation of <see cref="ISqlAction"/>.
        /// </summary>
        T Action { get; set; }

        /// <summary>
        /// Boolean representing whether or not to refresh <see cref="Action"/> from storage
        /// </summary>
        bool Refresh { get; set; }
    }
}