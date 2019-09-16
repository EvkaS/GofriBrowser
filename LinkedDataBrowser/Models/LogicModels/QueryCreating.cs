using LinkedDataBrowser.Models.Extensions;
using LinkedDataBrowser.Models.ViewModels;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.LogicModels
{
    public enum QueryTemplates
    {
        //SelectPredicateObjectOfSubject,
        SelectDistinctPredicateOfSubject,
        SelectDistinctPredicateAndObjectCount,
        SelectPopularProperties,
        SelectFilterPredicatesFilterLanguage,
        SelectObjectsOfPredicate,
        SelectObjectsOfPredicateWithLimit
    }

    public class QueryCreating
    {
        public static SparqlParameterizedString GetQueryTemplateCopy(QueryTemplates template)
        {
            switch (template)
            {
                //case QueryTemplates.SelectPredicateObjectOfSubject:
                    //return new SparqlParameterizedString(selectPredicateObjectOfSubject);
                case QueryTemplates.SelectDistinctPredicateOfSubject:
                    return new SparqlParameterizedString(selectDistinctPredicateOfSubject);
                case QueryTemplates.SelectDistinctPredicateAndObjectCount:
                    return new SparqlParameterizedString(selectDistinctPredicateAndObjectCount);
                case QueryTemplates.SelectPopularProperties:
                    return new SparqlParameterizedString(selectPopularProperties);
                //case QueryTemplates.SelectFilterPredicatesFilterLanguage:
                    //return new SparqlParameterizedString(selectFilterPredicatesFilterLanguage);
                case QueryTemplates.SelectObjectsOfPredicate:
                    return new SparqlParameterizedString(selectObjectsOfPredicate);
                case QueryTemplates.SelectObjectsOfPredicateWithLimit:
                    return new SparqlParameterizedString(selectObjectsOfPredicateWithLimit);
                default:
                    return null;
            }
        }
        /*
        public static SparqlQuery CreateQuery(QueryTemplates template, string subjectIri, PredicateResultWrapperList predicateObjectWrapperList, string language)
        {
            SparqlParameterizedString queryParamString = GetQueryTemplateCopy(template);
            queryParamString.SetSubject(subjectIri);
            queryParamString.SetLanguage(language);
            string predicates = predicateObjectWrapperList.GetPredicateListString();
            queryParamString.SetPart("predicates", predicates);
            return QueryParsing.ParseQuery(queryParamString);
        }*/

        public static SparqlQuery CreateQuery(QueryTemplates template, string subjectIri, string language)
        {
            SparqlParameterizedString queryParamString = GetQueryTemplateCopy(template);
            queryParamString.SetSubject(subjectIri);
            queryParamString.SetLanguage(language);
            return QueryParsing.ParseQuery(queryParamString);
        }

        public static SparqlQuery CreateQuery(QueryTemplates template, string subjectIri)
        {
            SparqlParameterizedString queryParamString = GetQueryTemplateCopy(template);
            queryParamString.SetSubject(subjectIri);
            return QueryParsing.ParseQuery(queryParamString);
        }

        //private static readonly string selectPredicateObjectOfSubject = $"SELECT ?predicate ?object WHERE {{ @{Config.SUBJECT_KEYWORD} ?predicate ?object }}";

        private static readonly string selectDistinctPredicateOfSubject = $"SELECT DISTINCT ?predicate WHERE {{ @{Config.SUBJECT_KEYWORD} ?predicate ?object }}";

        private static readonly string selectDistinctPredicateAndObjectCount = $"SELECT DISTINCT ?predicate COUNT(?object) AS ?countObj WHERE {{ @{Config.SUBJECT_KEYWORD} ?predicate ?object . FILTER((!ISLITERAL(?object)) || (LANG(?object) = \"\" || LANG(?object) = \"@language\")) }} GROUP BY ?predicate ORDER BY DESC(?countObj)";

        private static readonly string selectPopularProperties = $"SELECT DISTINCT ?propIRI ?occurence WHERE {{ GRAPH ?g {{ ?propIRI a <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property>. }} GRAPH <{ Config.LOV_GRAPH }> {{ ?propIRI <http://purl.org/vocommons/voaf#occurrencesInDatasets> ?occurence. }} }} ORDER BY DESC(?occurence)";

        //private static readonly string selectFilterPredicatesFilterLanguage = $"SELECT ?predicate ?object WHERE{{ @{Config.SUBJECT_KEYWORD} ?predicate ?object . FILTER(?predicate IN ( @predicates )) FILTER((!ISLITERAL(?object)) || (LANG(?object) = \"\" || LANG(?object) = \"@language\")) }}";

        private static readonly string selectObjectsOfPredicate = $"SELECT ?object WHERE{{ @{Config.SUBJECT_KEYWORD} @{Config.PREDICATE_KEYWORD} ?object . FILTER((!ISLITERAL(?object)) || (LANG(?object) = \"\" || LANG(?object) = \"@{Config.LANG_VAL_KEYWORD}\")) }}";

        private static readonly string selectObjectsOfPredicateWithLimit = $"SELECT ?object WHERE{{ @{Config.SUBJECT_KEYWORD} @{Config.PREDICATE_KEYWORD} ?object . FILTER((!ISLITERAL(?object)) || (LANG(?object) = \"\" || LANG(?object) = \"@{Config.LANG_VAL_KEYWORD}\")) }} LIMIT @{Config.LIMIT_VAL_KEYWORD}";
    }
}
