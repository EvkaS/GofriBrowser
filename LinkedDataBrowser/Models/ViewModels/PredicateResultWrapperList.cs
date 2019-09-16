using LinkedDataBrowser.Models.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.ViewModels
{
    public class PredicateResultWrapperList : IEnumerable<PredicateResultWrapper>
    {        
        private List<PredicateResultWrapper> list = new List<PredicateResultWrapper>();
        public int PredicatesCount
        {
            get
            { 
                return list?.Count ?? 0; /* if the list is null, returns 0 */
            }
        }

        /// <summary>
        /// True if at least one predicate has at least one object
        /// </summary>
        public bool HasObjects => HasPredicates && list.Any(predicate => predicate.ObjectCount != 0);

        public bool HasNextPage { get; private set; } = false;

        public int ObjectsCount()
        {
            if (!HasPredicates)
                return 0;
            else return list.Sum(wrapper => wrapper.ObjectCount);
        }
        
        /// <summary>
        /// True if the list contains at least one predicate with or without objects
        /// </summary>
        public bool HasPredicates => list != null && list.Count != 0;

        public void ShortenToNthSubset(int resultsOnPage, int pageNumber)
        {
            try
            {
                var newList = list.GetNthSubset(resultsOnPage, pageNumber, out int indexOfSubset);
                if (list.Count - indexOfSubset > resultsOnPage)
                    HasNextPage = true;
                list = newList;
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public void InsertPredicateWrapper(PredicateResultWrapper predicate)
        {
            //TODO: check if exists
            list.Add(predicate);
        }

        public void InsertPredicateWrapper(string subjectIri, string predicateIri, SparqlRemoteEndpoint endpoint, int expectedObjects = 0)
        {
            Config.AssertAbsoluteUri(subjectIri);
            Config.AssertAbsoluteUri(predicateIri);

            IUriNode node = new Graph().CreateUriNode(new Uri(predicateIri));
            var wrapper = new PredicateResultWrapper(node, endpoint);
            wrapper.ExpectedObjectCount = expectedObjects;
            wrapper.SubjectIri = subjectIri;
            list.Add(wrapper);
        }

        public void Sort()
        {
            list.Sort();
        }
        


        /*
        public string GetPredicateListString()
        {

            if (!HasPredicates) return "";
            List<string> param = new List<string>(list.Count);
            foreach (var predicate in list)
            {
                param.Add("<" + predicate.ToString() + ">");
            }
            return string.Join(',', param);
        }*/

        /*
        public bool MergeResultSetWithResultWrapper(SparqlResultSet resultSet)
        {
            if (resultSet == null) return false;
            switch (resultSet.ResultsType)
            {
                case SparqlResultsType.VariableBindings:
                    return MergeSetWithWrapper(resultSet);
                case SparqlResultsType.Boolean:
                    return false;
                case SparqlResultsType.Unknown:
                    return false;
                default:
                    return false;
            }
        }*/
        /*
        private bool MergeSetWithWrapper(SparqlResultSet resultSet)
        {
            foreach (var result in resultSet)
            {
                PredicateResultWrapper currentWrapper = null;

                if (result.HasValue(Config.PREDICATE_VAR)) // "predicate"
                {
                    currentWrapper = this.list.First(predicate => (predicate.ToString() == result[Config.PREDICATE_VAR].ToString()));
                }

                if (currentWrapper == null)
                    continue;

                if (result.HasValue(Config.OBJECT_VAR))
                {
                    ObjectWrapper objectWrapper = new ObjectWrapper(result[Config.OBJECT_VAR]);
                    currentWrapper.AddObject(objectWrapper);
                }
            }
            return true;
        }
        */
        public IEnumerator<PredicateResultWrapper> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }        
    }
}
