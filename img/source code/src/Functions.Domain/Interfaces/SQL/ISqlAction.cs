using System.Collections.Generic;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql
{
    /// <summary>
    /// Represents a SQL action.
    /// </summary>
    public interface ISqlAction {

        /// <summary>
        /// Implementation of <see cref="ISqlEnvironment"/>
        /// </summary>
        ISqlEnvironment Environment { get; }

        /// <summary>
        /// Implementation of <see cref="ISource"/>
        /// </summary>
        ISource Source { get; }

        /// <summary>
        /// Name of this SQL action
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Initializes this action to be able to execute
        /// </summary>
        /// <param name="environment">Implementation of <see cref="ISqlEnvironment"/></param>
        /// <param name="source">Implementation of <see cref="ISource"/></param>
        /// <param name="checkParameters">Boolean representing whether or not to check for SQL parameters</param>
        /// <param name="refresh">Boolean representing whether or not to refresh <see cref="ISource"/>.</param>
        Task InitAsync(ISqlEnvironment environment, ISource source, bool refresh, bool checkParameters);

        /// <summary>
        /// Executes this SQL action
        /// </summary>
        /// <returns>Result of the execution of this SQL action</returns>
        Task<string> ExecuteAsync();

    }

    /// <summary>
    /// Represents a SQL action.
    /// </summary>
    /// <typeparam name="T">Implementation of <see cref="ISqlActionParameter"/></typeparam>
    public interface ISqlAction<T> : ISqlAction where T: ISqlActionParameter
    {
        /// <summary>
        /// List of <see cref="ISqlActionParameter"/>s
        /// </summary>
        IList<T> Parameters { get; }
    }
}