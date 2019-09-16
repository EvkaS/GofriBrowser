using LinkedDataBrowser.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedDataBrowser
{
    public static class Config
    {
        public const string LIST_OF_PREFIXES_URL = "http://prefix.cc/popular/all.file.txt";
        public const string LOV_SPARQL_ENDPOINT = "https://lov.linkeddata.es/dataset/lov/sparql";
        public const string LOV_GRAPH = "https://lov.linkeddata.es/dataset/lov";
        public const string POPULAR_PREDICATES_FILE = "popular_predicates.txt";
        public const int LOV_PREDICATES_SEARCH_TIMEOUT = 20000;
        public const int MIN_SEARCH_TIME = 500;
        public const int MIN_RESULTS_ON_PAGE = 10;
        public const int PREFIXES_UPDATE_INTERVAL_DAYS = 5;
        public const int RESULT_OBJECTS_NUMBER = 5;
        public const string PREFERED_EN_LANGUAGE = "en";
        public const string SUBJECT_KEYWORD = "subject";
        public const string PREDICATE_KEYWORD = "predicate";
        public const string OBJECT_KEYWORD = "object";
        public const string SUBJECT_VAR = "subject";
        public const string PREDICATE_VAR = "predicate";
        public const string OBJECT_VAR = "object";
        public const string LIMIT_VAL_KEYWORD = "number";
        public const string LANG_VAL_KEYWORD = "language";

        public static void AssertNonEmptyUri(string uri, string errorMessage)
        {
            if(uri == null || uri == "")
            {
                throw new BrowserException(errorMessage);
            }
        }

        public static void AssertAbsoluteUri(string uri, string errorMessage = "")
        {
            if(uri == null)
            {
                throw new ArgumentNullException("Uri cannot be null!");
            }

            if(!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                if (errorMessage == "")
                    throw new BrowserException($"\"{uri}\" is not a valid IRI!");
                else
                    throw new BrowserException(errorMessage);
            }
        }
    }
}

