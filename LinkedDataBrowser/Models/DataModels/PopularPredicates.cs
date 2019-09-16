using LinkedDataBrowser.Models.Exceptions;
using LinkedDataBrowser.Models.LogicModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.DataModels
{
    public static class PopularPredicates
    {
        private static Dictionary<string, int> popularPredicates = null;

        
        
        public static async Task<int> GetPopularityOf(string predicateIri)
        {
            if (predicateIri == null)
                throw new InternalErrorException("Predicate IRI cannot be empty!");
            if (popularPredicates == null)
            {
                await Initialize();
            }
            if (popularPredicates.TryGetValue(predicateIri, out int popularity))
            {
                return popularity;
            }
            else return -1;
        }

        private static async Task Initialize()
        {
            //TODO: check return StatusCode(503);
            if (File.Exists(Config.POPULAR_PREDICATES_FILE))
            {
                popularPredicates = ReadPopularPredicatesFromFile(Config.POPULAR_PREDICATES_FILE);                
            }
            else
            {
                popularPredicates = await WritePopularPredicatesToFile(Config.POPULAR_PREDICATES_FILE);
            }
        }

        private static Dictionary<string, int> ResultSetToDictionary(SparqlResultSet resultSet)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var row in resultSet)
            {
                string property = row[0].ToString();
                if (row[1] is ILiteralNode
                    && int.TryParse(((ILiteralNode)row[1]).Value, out int occurences))
                {
                    dictionary.Add(property, occurences);
                }
            }
            return (dictionary.Count != 0) ? dictionary : null;
        }
        private static async Task<Dictionary<string, int>> RetrievePopularPropertiesFromEndpoint()
        {
            string errorMessage = "";
            object results = null;
            try
            {
                SparqlQuery query = QueryParsing.ParseQuery(QueryCreating.GetQueryTemplateCopy(QueryTemplates.SelectPopularProperties));
                SparqlRemoteEndpoint endpoint = SearchPreparing.CreateEndpoint(Config.LOV_SPARQL_ENDPOINT,Config.LOV_PREDICATES_SEARCH_TIMEOUT);
                results = await QueryProcessing.ProcessQuery(query, endpoint);
            }
            catch (BrowserException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new BrowserException("An unexpected error occured while retrieving the list of popular predicates.", e);
            }

            if (results is SparqlResultSet)
            {
                return ResultSetToDictionary((SparqlResultSet)results);
            }
            else if (results is AsyncError)
            { /*TODO: remove exceptions */
                throw ((AsyncError)results).Error.InnerException;
                //errorMessage += ResultProcessing.ProcessAsyncError((AsyncError)results);
                //return null;
            }
            /*TODO: remove exceptions*/
            throw new Exception(errorMessage);

            //otherwise it should be null and errorMessage should be already set
            //return null;
        }

        private static async Task<Dictionary<string, int>> WritePopularPredicatesToFile(string fileName)
        {
            var predicates = await RetrievePopularPropertiesFromEndpoint();
            StreamWriter writer = new StreamWriter(fileName);
            
            foreach (var predicate in predicates)
            {
                writer.Write(predicate.Key);
                writer.Write('\t');
                writer.WriteLine(predicate.Value);
            }
            writer.Close();
            return predicates;
        }

        private static Dictionary<string, int> ReadPopularPredicatesFromFile(string fileName)
        {
            Dictionary<string, int> predicates = new Dictionary<string, int>();
            StreamReader reader;
            try
            {
                reader = new StreamReader(fileName);
                string line;
                while (true)
                {
                    if ((line = reader.ReadLine()) == null)
                        break;
                    var tokens = line.Split();
                    predicates.Add(tokens[0], int.Parse(tokens[1]));
                }
            }
            catch (Exception)
            {
                throw new InternalErrorException("Internal server error occured while processing.");
            }
            return predicates;            
        }

    }
}
