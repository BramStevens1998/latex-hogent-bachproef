using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Sql;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Sql
{
    /// <summary>
    /// Represents a base SQL action parameter.
    /// </summary>
    public abstract class BaseSqlActionParameter : ISqlActionParameter, ISqlActionParameter<IPayload>
    {
        /// <summary>
        /// Name of the base SQL action parameter.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// <see cref="IPayload"/> of the base SQL action parameter.
        /// </summary>
        public abstract IPayload Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the BaseSqlActionParameter class.
        /// </summary>
        /// <param name="name">Name of the base SQL action parameter.</param>
        /// <param name="value">Value of the <see cref="IPayload"/>.</param>
        public BaseSqlActionParameter(string name, IPayload value)
        {
            Name = name;
            Value = value;
        }
    }
}