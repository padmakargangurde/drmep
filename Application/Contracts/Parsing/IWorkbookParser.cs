using System.Collections.Generic;
using System.IO;

namespace Application.Contracts.Parsing
{
    /// <summary>
    /// Declared here, implemented in Infrastructure with ClosedXML — keeps the
    /// Excel library out of Application, same seam as the repository contracts.
    /// </summary>
    public interface IWorkbookParser
    {
        IReadOnlyList<ParsedCategory> Parse(Stream xlsxStream);
    }
}
