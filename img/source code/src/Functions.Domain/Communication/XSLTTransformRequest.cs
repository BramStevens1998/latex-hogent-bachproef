using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.Communication;
using Dlw.Integration.DataAccelerator.Functions.Domain.Xslt;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Communication
{
    /// <summary>
    /// Represents an XSLT transform request.
    /// </summary>
    public class XsltTransformRequest : BaseXsltRequest<XsltTransform>, IXsltTransformRequest<XsltTransform>
    {
        /// <summary>
        /// Implementation of XSLT mapping.
        /// </summary>
        public override XsltTransform Mapping { get; set; }
    }

}