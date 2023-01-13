namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql
{
    /// <summary>
    /// Represents a SQL environment.
    /// </summary>
    public interface ISqlEnvironment
    {
        /// <summary>
        /// The SQL environment name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the SQL environment Connectionstring.
        /// </summary>
        string ConnectionString { get; }
    }
}