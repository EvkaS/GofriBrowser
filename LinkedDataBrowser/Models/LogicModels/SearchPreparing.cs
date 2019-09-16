using LinkedDataBrowser.Models.Exceptions;
using LinkedDataBrowser.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.LogicModels
{
    public static class SearchPreparing
    {
        public static SparqlRemoteEndpoint CreateEndpoint(string endpointIRI, int timeout, string defaultGraphIRI = "")
        {            
            SparqlRemoteEndpoint endpoint;
            if (defaultGraphIRI != "")
            {
                //throws only ArgumentNullException
                endpoint = new SparqlRemoteEndpoint(new Uri(endpointIRI), defaultGraphIRI);
            }
            else
            {
                //throws only ArgumentNullException
                endpoint = new SparqlRemoteEndpoint(new Uri(endpointIRI));
            }
            endpoint.Timeout = timeout;
            return endpoint;
        }

        public static List<SparqlRemoteEndpoint> CreateEndpoints(string[] endpointsIRIs, int timeout, string defaultGraph = "")
        {
            List<SparqlRemoteEndpoint> endpoints = new List<SparqlRemoteEndpoint>(capacity: endpointsIRIs.Length);
            foreach (var endpointIRI in endpointsIRIs)
            {
                if (endpointIRI == null)
                    continue;
                endpoints.Add(CreateEndpoint(endpointIRI, timeout, defaultGraph));                
            }
            return endpoints;
        }
    }
}
