using LinkedDataBrowser.Models.Exceptions;
using LinkedDataBrowser.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.LogicModels
{
    public static class QueryProcessing
    {
        /// <summary>
        /// Processes a SPARQL Query and retrieves data from a SPARQL endpoint returning either an IGraph instance or SparqlResultSet, or AsyncError if the processing failed
        /// </summary>
        /// <param name="endpoint">IGraph or SparqlResultSet or null</param>
        /// <returns></returns>
        public static Task<object> ProcessQuery(SparqlQuery query, SparqlRemoteEndpoint endpoint)
        {
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endpoint);
            return ProcessQuery(query, processor);
        }

        /// <summary>
        /// Processes a SPARQL Query using given SPARQL procesor returning either an IGraph instance or SparqlResultSet, or AsyncError if the processing failed
        /// </summary>
        /// <returns>IGraph or SparqlResultSet or AsyncError or null</returns>
        public static Task<object> ProcessQuery(SparqlQuery query, ISparqlQueryProcessor processor)
        {
            TaskCompletionSource<object> taskComplSource = new TaskCompletionSource<object>();
            if(query == null || processor == null)
            {
                var asyncError = new AsyncError(new BrowserException("An internal error occured while processing the query."), null);
                taskComplSource.SetResult(asyncError);
            }
            else
            {
                //state will contain AsyncError
                //catches all exceptions inside, putting them into AsyncError
                processor.ProcessQuery(
                    query,
                    (IGraph g, object state) => { taskComplSource.SetResult(g ?? state); },
                    (SparqlResultSet results, object state) => { taskComplSource.SetResult(results ?? state); },
                    null
                );
            }
            return taskComplSource.Task;
        }
        /**/
        public static object ProcessQuery(SparqlQuery query, SparqlRemoteEndpoint endpoint, ISparqlQueryProcessor processor)
        {
            if (query == null || processor == null)
            {
                throw new BrowserException("An internal error occured when processing the query.");
            }
            object result;
            try
            {
                result = processor.ProcessQuery(query);
            }
            catch (Exception e)
            {
                result = new AsyncError(e, null);
            }
            return result;
        }/**/

    }
}
