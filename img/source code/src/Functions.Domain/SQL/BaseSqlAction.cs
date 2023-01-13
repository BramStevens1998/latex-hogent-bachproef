using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Storage;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Sql
{
    /// <summary>
    /// Represents a base SQL action
    /// </summary>
    /// <typeparam name="T">Extension of <see cref="BaseSqlActionParameter"/></typeparam>
    public abstract class BaseSqlAction<T> : ISqlAction<T> where T : BaseSqlActionParameter
    {

        /// <summary>
        /// Implementation of <see cref="ISqlEnvironment"/>
        /// </summary>
        public ISqlEnvironment Environment { get; set; }

        /// <summary>
        /// Implementation of <see cref="ISource"/>
        /// </summary>
        public ISource Source { get; set; }

        /// <summary>
        /// Name of this base SQL action
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Command text that will be issued against a SQL database
        /// </summary>
        public string CommandText { get; private set; }

        /// <summary>
        /// List of all SQL parameters found in <see cref="CommandText"/>
        /// </summary>
        public IList<string> ExpectedParameters { get; private set; }

        /// <summary>
        /// List of SQL parameters
        /// </summary>
        public virtual IList<T> Parameters { get; set; }

        /// <summary>
        /// List of SQL parameters that are present in <see cref="ExpectedParameters"/> but not present in <see cref="Parameters"/>
        /// </summary>
        public IList<string> MissingParameters { get; set; } = new List<string>();

        /// <summary>
        /// Initializes this base SQL action
        /// </summary>
        /// <param name="environment">Implementation of <see cref="ISqlEnvironment"/></param>
        /// <param name="source">Implementation of <see cref="ISource"/></param>
        /// <param name="refresh">Boolean representing whether or not to refresh the SQL command from Azure storage</param>
        /// <param name="checkParameters">Boolean representing whether or not to check for SQL parameters</param>
        public virtual async Task InitAsync(ISqlEnvironment environment, ISource source, bool refresh = false, bool checkParameters = true)
        {
            // Set environment and source
            Environment = environment;
            Source = source;

            if(refresh) Source.Refresh();

            // Set commandtext
            using (StreamReader sr = new StreamReader(Source.Content))
            {
                CommandText = await sr.ReadToEndAsync();
            }

            if (checkParameters)
            {
                // Determine expected parameters
                var r = new Regex("\\@([\\w.$]+|\"[^\"]+\"|'[^']+')");
                ExpectedParameters = r.Matches(CommandText).Select(x => x.Value).Distinct().ToList();
                MissingParameters = ExpectedParameters.Where(x => !Parameters.Any(y => string.Equals($"@{y.Name}", x, StringComparison.InvariantCultureIgnoreCase))).ToList();
            }
        }

        /// <summary>
        /// Executes this SQL action
        /// </summary>
        /// <returns>Result of the execution of this SQL action</returns>
        public abstract Task<string> ExecuteAsync();
    }
}