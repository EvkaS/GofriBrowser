using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VDS.RDF.Query;
using VDS.RDF.Parsing;
using VDS.RDF;
using LinkedDataBrowser.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkedDataBrowser.Models.ViewModels
{
    
    public class SearchInfo
    {        
        [Required(ErrorMessage = "You must enter the endpoint.")]
        [DataType(DataType.Url)]
        //public string Endpoints { get; set; }

        public string[] Endpoints { get; set; }

        [Display(Name = "Default graph")]
        public string DefaultGraph { get; set; }

        [DataType(DataType.Time)]
        public int Timeout { get; set; }
        //TODO: what if the GET request contains a value bigger than int?

        [Required]
        [Display(Name = "Results on page")]
        public int ResultsOnPage { get; set; }

        public int PageNumber { get; set; }

        /// <summary>
        /// If negative, take all objects, otherwise use LIMIT in query
        /// </summary>
        public int MaxObjects { get; set; }

        public string Language { get; set; }

        public bool validUris = false;

        public virtual void Validate()
        {
            if(Endpoints == null || Endpoints[0] == null || Endpoints[0] == "")
            {
                throw new BrowserException("You must set at least one endpoint IRI!");
            }
            List<string> newEndpoints = new List<string>();
            foreach (var endpoint in Endpoints)
            {
                if (endpoint != null)
                {
                    Config.AssertAbsoluteUri(endpoint);
                    newEndpoints.Add(endpoint);
                }                
            }
            Endpoints = newEndpoints.ToArray();

            DefaultGraph = (DefaultGraph != null) ? DefaultGraph.Trim() : "";
            //TODO: somehow replace by Config.Assert
            if (DefaultGraph != ""
                && !Uri.IsWellFormedUriString(DefaultGraph, UriKind.Absolute))
            {
                throw new BrowserException("Invalid default graph URI!");
            }

            if (Timeout < Config.MIN_SEARCH_TIME)
            {
                Timeout = Config.MIN_SEARCH_TIME;
            }

            if (ResultsOnPage < Config.MIN_RESULTS_ON_PAGE)
            {
                ResultsOnPage = Config.MIN_RESULTS_ON_PAGE;
            }

            validUris = true;
        }
    }

    public class QueryInfo : SearchInfo
    {        
        [Required]
        [Display(Name = "Query")]
        public string QueryString { get; set; }
    }

    public class EntityInfo : SearchInfo
    {
        /// <summary>
        /// The entity we are describing, usually the subject of a triple
        /// </summary>
        [Required]
        public string EntityIri { get; set; }

        public override void Validate()
        {
            base.Validate();
            EntityIri = EntityIri?.Trim();

            if(EntityIri == null || EntityIri == "")
                throw new BrowserException("Entity URI cannot be empty!");

            Config.AssertAbsoluteUri(EntityIri, "Invalid entity URI!");
        }
    }

    public class OnePredicateInfo : EntityInfo
    {
        [Required]
        public string PredicateIri { get; set; }

        public int ExpectedObjects { get; set; }

        public override void Validate()
        {
            base.Validate();
            PredicateIri = PredicateIri?.Trim();

            if (PredicateIri == null || PredicateIri == "")
                throw new BrowserException("Predicate URI cannot be empty!");

            Config.AssertAbsoluteUri(PredicateIri, "Invalid predicate URI!");
        }
    }
}