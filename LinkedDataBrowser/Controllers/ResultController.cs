using LinkedDataBrowser.Models.ViewModels;
using LinkedDataBrowser.Models.LogicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDS.RDF;
using VDS.RDF.Query;
using System.Net;
using VDS.RDF.Parsing;
using LinkedDataBrowser.Models.Exceptions;

namespace LinkedDataBrowser.Controllers
{
    public class ResultController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> EntityResultMultipleEndpoints(EntityInfo entityInfo)
        {
            if (entityInfo == null)
            {
                return BadRequest();
            }            
            var queryResultWrappertList = new PredicateResultWrapperList();
            try
            {
                /*** validation ***/
                entityInfo.Validate();
                ViewData[nameof(SearchInfo)] = entityInfo;

                /*** search preparation ***/
                List<SparqlRemoteEndpoint> endpoints = SearchPreparing.CreateEndpoints(entityInfo.Endpoints, entityInfo.Timeout);

                /*** getting all the predicates ***/
                await ResultRetrieving.RetrievePredicatesAndObjectCounts(queryResultWrappertList, entityInfo.EntityIri, entityInfo.Language, endpoints);

                queryResultWrappertList.ShortenToNthSubset(entityInfo.ResultsOnPage, entityInfo.PageNumber);

                /*** getting objects ***/
                await ResultRetrieving.RetrieveObjects(queryResultWrappertList, entityInfo.EntityIri, entityInfo.Language, entityInfo.MaxObjects);

                //ViewData["ExecutionTime"] = query.QueryExecutionTime;
                ViewData[nameof(EntityInfo.EntityIri)] = entityInfo.EntityIri;
            }
            catch (BrowserException e)
            {
                return View("QueryResultError", e.Message);
            }
            catch (InternalErrorException)
            {
                return StatusCode(503);
            }

            return View("EntityResultOnlyFive", queryResultWrappertList);

            //return View("EntityResultMultipleEndpoints", queryResultWrappertList);
        }

        [HttpGet]
        public async Task<ActionResult> EntityResultOnePredicate(OnePredicateInfo predicateInfo)
        {
            if(predicateInfo == null)
            {
                return BadRequest();
            }
            ViewData[nameof(SearchInfo)] = predicateInfo;
            var queryResultWrappertList = new PredicateResultWrapperList();
            try
            {
                /*** validation ***/
                predicateInfo.Validate();

                /*** search preparation ***/
                SparqlRemoteEndpoint endpoint = SearchPreparing.CreateEndpoint(predicateInfo.Endpoints[0], predicateInfo.Timeout, predicateInfo.DefaultGraph);

                /*** preparing the predicate ***/
                queryResultWrappertList.InsertPredicateWrapper(predicateInfo.EntityIri, predicateInfo.PredicateIri, endpoint, predicateInfo.ExpectedObjects);

                /*** getting objects ***/
                await ResultRetrieving.RetrieveObjects(queryResultWrappertList,predicateInfo.EntityIri, predicateInfo.Language);

                //ViewData["ExecutionTime"] = query.QueryExecutionTime;
                ViewData[nameof(OnePredicateInfo.EntityIri)] = predicateInfo.EntityIri;
                
            }
            catch (BrowserException e)
            {
                return View("QueryResultError", e.Message);
            }
            catch (InternalErrorException)
            {
                return StatusCode(503);
            }
            catch (Exception)
            {
                //TODO: better text
                return View("QueryResultError", "Unknown error...");
            }

            return View("EntityResultOnePredicate", queryResultWrappertList);
        }

        [HttpGet]
        public async Task<ActionResult> EntityResultOnlyFive(EntityInfo entityInfo)
        {
            if (entityInfo == null)
            {
                return BadRequest();
            }
            ViewData[nameof(SearchInfo)] = entityInfo;
            var queryResultWrappertList = new PredicateResultWrapperList();
            try
            {
                /*** validation ***/
                entityInfo.Validate();

                /*** search preparation ***/
                SparqlRemoteEndpoint endpoint = SearchPreparing.CreateEndpoint(entityInfo.Endpoints[0],entityInfo.Timeout,entityInfo.DefaultGraph);

                /*** getting all the predicates ***/
                await ResultRetrieving.RetrievePredicatesAndObjectCounts(queryResultWrappertList,entityInfo.EntityIri, entityInfo.Language, endpoint);
                
                queryResultWrappertList.ShortenToNthSubset(entityInfo.ResultsOnPage, entityInfo.PageNumber);

                /*** getting objects ***/
                await ResultRetrieving.RetrieveObjects(queryResultWrappertList,entityInfo.EntityIri, entityInfo.Language, entityInfo.MaxObjects);

                //ViewData["ExecutionTime"] = query.QueryExecutionTime;
                ViewData[nameof(EntityInfo.EntityIri)] = entityInfo.EntityIri;
            }
            catch (BrowserException e)
            {
                return View("QueryResultError", e.Message);
            }
            catch (InternalErrorException)
            {
                return StatusCode(503);
            }

            return View("EntityResultOnlyFive", queryResultWrappertList);            
        }
                
        [HttpGet]
        public async Task<ActionResult> QueryResult(QueryInfo queryInfo)
        {
            if (queryInfo == null)
            {
                return BadRequest();
            }
            ViewData[nameof(SearchInfo)] = queryInfo;
            object results;
            try
            {
                /*** validation ***/
                queryInfo.Validate();

                /*** query preparation ***/
                SparqlRemoteEndpoint endpoint = SearchPreparing.CreateEndpoint(queryInfo.Endpoints[0],queryInfo.Timeout,queryInfo.DefaultGraph);
                SparqlQuery query = QueryParsing.ParseQuery(queryInfo.QueryString);

                /*** query processing ***/
                results = await QueryProcessing.ProcessQuery(query, endpoint);                
                //ViewData["ExecutionTime"] = query.QueryExecutionTime;
            }
            catch (BrowserException e)
            {
                return View("QueryResultError", e.Message);
            }
            catch(Exception)
            {
                //TODO: better text
                return View("QueryResultError", "Unknown error...");
            }

            /*** result processing ***/
            if (results is SparqlResultSet)
            {
                return View("QueryResultSet", (SparqlResultSet)results);
            }
            else if (results is IGraph)
            {
                return View("QueryResultGraph", (IGraph)results);
            }
            else if (results is AsyncError)
            {
                return View("QueryResultError", ErrorProcessing.ProcessAsyncError((AsyncError)results));
            }
            else
            {
                //TODO: better text
                return View("QueryResultError", "Unknown error...");
            }
        }

        [HttpGet]
        public ActionResult EntityResultNoSparql(string EntityIri)
        {
            ViewData[nameof(EntityInfo.EntityIri)] = EntityIri;
            if (!Uri.IsWellFormedUriString(EntityIri, UriKind.Absolute))
            {
                return View("QueryResultError", "EntityIri is not valid.");
            }
            
            var client = new WebClient();
            client.Headers.Set("Accept", "application/rdf+xml, text/rdf+n3, application/rdf+turtle, application/x-turtle, application/turtle, application/xml, text/turtle");
            string entityDataString;
            IGraph g = new Graph();
            try
            {
                entityDataString = client.DownloadString(EntityIri);
                StringParser.Parse(g, entityDataString);
            }
            catch (RdfParseException)
            {
                return View("QueryResultError", "Error parsing response from server.");
            }
            catch (Exception e)
            {
                return View("QueryResultError", e.Message);
            }
            return View("QueryResultGraph", g);
        }
    }

}