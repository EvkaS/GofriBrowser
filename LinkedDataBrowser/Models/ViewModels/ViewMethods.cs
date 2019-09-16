using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkedDataBrowser.Models.LogicModels;

namespace LinkedDataBrowser.Models.ViewModels
{
    public class ViewMethods
    {
        /// <summary>
        /// Converts IRI to prefixed expression or returns original text representation of a node if it is not IRI or the prefix is unknown
        /// </summary>
        public static string TryConvertToPrefixedExpression(string wholeIri)
        {
            return PrefixProcessing.TryConvertToPrefixedExpression(wholeIri);
        }


        public static Dictionary<string, string> CreateRouteData(SearchInfo searchInfo, string entityIri, int pageNumber)
        {
            if (searchInfo == null || entityIri == null || pageNumber < 1) return new Dictionary<string, string>();
            else
            {
                var routeParam = new Dictionary<string, string>
                {
                    { nameof(SearchInfo.DefaultGraph), searchInfo.DefaultGraph },
                    { nameof(SearchInfo.Timeout), searchInfo.Timeout.ToString() },
                    { nameof(SearchInfo.Language), searchInfo.Language },
                    { nameof(SearchInfo.MaxObjects), searchInfo.MaxObjects.ToString() },
                    { nameof(SearchInfo.ResultsOnPage), searchInfo.ResultsOnPage.ToString() },
                    { nameof(SearchInfo.PageNumber), pageNumber.ToString() },
                    { nameof(EntityInfo.EntityIri), entityIri }
                };
                var i = 0;
                foreach (var endpoint in searchInfo.Endpoints)
                {
                    if (endpoint != null)
                    {
                        routeParam.Add(nameof(SearchInfo.Endpoints) + $"[{i++}]", endpoint);
                    }
                }
                return routeParam;
            }
        }

        public static Dictionary<string, string> CreateRouteData(SearchInfo searchInfo, string entityIri, string predicateIri, int expectedObjects)
        {
            //TODO: how about 0 expected objects?
            if (searchInfo == null || entityIri == null || predicateIri == null || expectedObjects < 0) return new Dictionary<string, string>();
            else
            {
                var routeParam = new Dictionary<string, string>
                {
                    { nameof(SearchInfo.DefaultGraph), searchInfo.DefaultGraph },
                    { nameof(SearchInfo.Timeout), searchInfo.Timeout.ToString() },
                    { nameof(SearchInfo.Language), searchInfo.Language },
                    { nameof(SearchInfo.MaxObjects), searchInfo.MaxObjects.ToString() },
                    { nameof(SearchInfo.ResultsOnPage), searchInfo.ResultsOnPage.ToString() },
                    { nameof(SearchInfo.PageNumber), "1" },
                    { nameof(EntityInfo.EntityIri), entityIri },
                    { nameof(OnePredicateInfo.PredicateIri), predicateIri },
                    { nameof(OnePredicateInfo.ExpectedObjects), expectedObjects.ToString() }
                };
                var i = 0; //TODO: does asp.net support other way?
                foreach (var endpoint in searchInfo.Endpoints)
                {
                    if (endpoint != null)
                    {
                        routeParam.Add(nameof(SearchInfo.Endpoints) + $"[{i++}]", endpoint);
                    }
                }
                return routeParam;
                //we want to keep MaxObjects and ResultsOnPage for future queries
            }
        }
    }
}
