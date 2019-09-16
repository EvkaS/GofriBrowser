using LinkedDataBrowser.Models.Exceptions;
using System.Net;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.LogicModels
{
    public class ErrorProcessing
    {
        public static string ProcessAsyncError(AsyncError asyncError)
        {
            var error = asyncError.Error.InnerException;
            if (error is RdfQueryException
                || error is RdfParseException //TODO: should it be here?
                || error is WebException
                || error is BrowserException)
            {
                return error.Message;
            }
            else
            {
                string ret = "An unexpected error occured while processing the query. ";
                /*TODO remove*/
                ret += error.Message;
                return ret;
            }
        }
    }
}
