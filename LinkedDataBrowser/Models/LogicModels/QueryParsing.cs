using LinkedDataBrowser.Models.Exceptions;
using System;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.LogicModels
{


    public static class QueryParsing
    {        
        /// <summary>
        /// Parses a SPARQL Query from a String or returns null if it is not a valid Query String
        /// </summary>
        /// <returns>SparqlQuery or null</returns>
        public static SparqlQuery ParseQuery(string queryString)
        {
            return ParseQuery(queryString, new SparqlQueryParser());
        }

        public static SparqlQuery ParseQuery(SparqlParameterizedString queryString)
        {
            return ParseQuery(queryString.ToString(), new SparqlQueryParser());
        }

        /// <summary>
        /// Parses a SPARQL Query from a String using the given parser or returns null if it is not a valid Query String
        /// </summary>        
        /// <param name="parser">A Parser used for parsing</param>
        /// <returns>SparqlQuery or null</returns>
        public static SparqlQuery ParseQuery(string queryString, SparqlQueryParser parser)
        {
            if (queryString == null || queryString.Trim() == "")
            {
                throw new BrowserException("Cannot parse an empty query!");
            }
            else
            {
                SparqlQuery query = null;
                try
                {
                    query = parser.ParseFromString(queryString);
                }
                catch (Exception e) when (e is RdfParseException || e is RdfQueryException)
                {
                    throw new BrowserException(e.Message);
                }
                catch (RdfException e)
                {
                    throw new BrowserException(e.Message);
                }
                catch (Exception)
                {
                    throw new BrowserException("An unexpected error occured while parsing the query.");
                }
                return query;
            }            
        }
    }

    

}


