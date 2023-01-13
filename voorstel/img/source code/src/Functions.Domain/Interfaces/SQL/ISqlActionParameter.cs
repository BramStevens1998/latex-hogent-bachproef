namespace Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql
{
    /// <summary>
    /// Represents a SQL action parameter.
    /// </summary>
    public interface ISqlActionParameter {

        /// <summary>
        /// Name of the SQL action.
        /// </summary>
        string Name {get;}
    }

    /// <summary>
    /// Represents a SQL action.
    /// </summary>
    /// <typeparam name="T">Implementation of <see cref="ISqlActionParameter"/></typeparam>
    public interface ISqlActionParameter<T> : ISqlActionParameter
    {
        /// <summary>
        /// Implements <see cref="ISqlActionParameter"/>.
        /// </summary>
        T Value { get; }
    }
}