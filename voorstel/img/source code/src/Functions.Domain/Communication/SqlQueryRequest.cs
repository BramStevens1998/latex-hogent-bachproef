using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Sql;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents a SQL query request.
    /// </summary>
    public class SqlQueryRequest : BaseSqlRequest<SqlQuery>, ISqlRequest<SqlQuery>
    {
        /// <summary>
        /// Implementation of <see cref="ISqlAction"/>
        /// </summary>
        public override SqlQuery Action { get; set; }
    }

}