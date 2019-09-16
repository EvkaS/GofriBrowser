using LinkedDataBrowser.Models.DataModels;
using LinkedDataBrowser.Models.Exceptions;
using LinkedDataBrowser.Models.Extensions;
using LinkedDataBrowser.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.LogicModels
{
    public class ResultRetrieving
    {
        /// <summary>
        /// Get all the predicates from the triples with given subject IRI from multiple endpoints, sorted according to the popularity of predicates
        /// </summary>
        public static async Task RetrievePredicatesAndObjectCounts(PredicateResultWrapperList list, string subjectIri, string language, List<SparqlRemoteEndpoint> endpoints)
        {
            Config.AssertAbsoluteUri(subjectIri);

            SparqlQuery query = QueryCreating.CreateQuery(QueryTemplates.SelectDistinctPredicateAndObjectCount, subjectIri, language);

            /*** create tasks ***/
            List<Task<ResultHolder>> tasks = new List<Task<ResultHolder>>(capacity: endpoints.Count);
            try
            {
                foreach (var endpoint in endpoints)
                {
                    tasks.Add(RetrieveResultHolder(query, endpoint));
                }
            }
            catch (Exception)
            {
                throw;
            }
            /*** process results ***/
            try
            {
                foreach (var task in tasks)
                {
                    var endpoint = task.Result.QueryEndpoint;
                    foreach (var result in task.Result.QueryResultSet)
                    {
                        var predicate = await CreatePredicateNodeFromSparqlResult(subjectIri, endpoint, result);
                        list.InsertPredicateWrapper(predicate);
                    }
                }
            }
            catch (AggregateException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

            list.Sort();
            //TODO: handle duplicates

        }

        /// <summary>
        /// Get all the predicates from the triples with given subject IRI, sorted according to the popularity of predicates
        /// </summary>
        public static async Task RetrievePredicatesAndObjectCounts(PredicateResultWrapperList list, string subjectIri, string language, SparqlRemoteEndpoint endpoint)
        {
            Config.AssertAbsoluteUri(subjectIri);

            SparqlQuery query = QueryCreating.CreateQuery(QueryTemplates.SelectDistinctPredicateAndObjectCount, subjectIri, language);

            var holder = await RetrieveResultHolder(query, endpoint);
            foreach (var result in holder.QueryResultSet)
            {
                var predicate = await CreatePredicateNodeFromSparqlResult(subjectIri, holder.QueryEndpoint, result);
                list.InsertPredicateWrapper(predicate);
            }
            list.Sort();
        }

        /// <summary>
        /// Retrieve objects of the predicates of the entity and store them to particular lists
        /// </summary>
        /// <param name="subjectIri">An IRI of the entity we are describing</param>
        /// <param name="language">The language of the search</param>
        /// <param name="endpoint">An endpoint where to search</param>
        /// <param name="count">If positive, set limit according to this value; if negative, retrieve all objects</param>
        public static Task RetrieveObjects(PredicateResultWrapperList predicateWrapperList, string subjectIri, string language, int count = -1)
        {
            Config.AssertAbsoluteUri(subjectIri);

            if (!predicateWrapperList.HasPredicates) return Task.CompletedTask;
            SparqlParameterizedString template;
            if (count < 0)
            {
                template = QueryCreating.GetQueryTemplateCopy(QueryTemplates.SelectObjectsOfPredicate);
            }
            else
            {
                template = QueryCreating.GetQueryTemplateCopy(QueryTemplates.SelectObjectsOfPredicateWithLimit);
                template.SetLimit(count);
            }
            template.SetLanguage(language);
            template.SetSubject(subjectIri);
            List<Task> tasks = new List<Task>();
            foreach (var predicate in predicateWrapperList)
            {
                var task = RetrieveObjectsOfPredicate(predicate, template);
                tasks.Add(task);

            }
            return Task.WhenAll(tasks);
            //var x = Parallel.ForEach(list, async predicate => await RetrieveObjectsOfPredicate(predicate, template, endpoint));            
        }

        private static async Task<PredicateResultWrapper> CreatePredicateNodeFromSparqlResult(string subjectIri, SparqlRemoteEndpoint endpoint, SparqlResult result)
        {
            var predicate = new PredicateResultWrapper(result[0], endpoint);
            predicate.SubjectIri = subjectIri;
            predicate.ExpectedObjectCount = int.Parse(((ILiteralNode)result[1]).Value);
            predicate.Popularity = await PopularPredicates.GetPopularityOf(predicate.ToString());
            return predicate;
        }

        /// <summary>
        /// Get the SparqlResultSet together with the endpoint it came from
        /// </summary>
        private static async Task<ResultHolder> RetrieveResultHolder(SparqlQuery query, SparqlRemoteEndpoint endpoint)
        {
            object results = await QueryProcessing.ProcessQuery(query, endpoint);
            if (results is SparqlResultSet)
            {
                SparqlResultSet set = (SparqlResultSet)results;
                return new ResultHolder(endpoint, set);

            }
            else if (results is AsyncError)
            {
                throw new BrowserException(ErrorProcessing.ProcessAsyncError((AsyncError)results));
            }
            else
            {
                throw new BrowserException("An unexpected error occured while processing the query.");
            }
        }

        /// <summary>
        /// Retrieve objects of the given predicate; number of objects is already set in the template query
        /// </summary>
        private static async Task RetrieveObjectsOfPredicate(PredicateResultWrapper predicate, SparqlParameterizedString template)
        {
            var queryText = new SparqlParameterizedString(template.ToString());
            queryText.SetPredicate(predicate.ToString());
            var query = QueryParsing.ParseQuery(queryText);
            var results = await QueryProcessing.ProcessQuery(query, predicate.Endpoint);
            if (results is SparqlResultSet)
            {
                SparqlResultSet set = (SparqlResultSet)results;
                foreach (var obj in set)
                {
                    predicate.AddObject(new ObjectWrapper(obj[0], predicate.Endpoint));
                }
            }
            else if (results is AsyncError)
            {
                throw new BrowserException(ErrorProcessing.ProcessAsyncError((AsyncError)results));
            }
            else
            {
                throw new BrowserException("An unexpected error occured while processing the query.");
            }
            //template.UnsetPredicate(); //no more reusing
        }

        class ResultHolder
        {
            public ResultHolder(SparqlRemoteEndpoint endpoint, SparqlResultSet set)
            {
                QueryEndpoint = endpoint;
                QueryResultSet = set;
            }
            public SparqlRemoteEndpoint QueryEndpoint { get; private set; }
            public SparqlResultSet QueryResultSet { get; private set; }
        }
    }
}
