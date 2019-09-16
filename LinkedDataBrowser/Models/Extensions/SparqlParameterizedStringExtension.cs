using System;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.Extensions
{
    public static class SparqlParameterizedStringExtension
    {
        public static void SetSubject(this SparqlParameterizedString paramString, string subjectIri)
        {
            if (!paramString.CommandText.Contains('@' + Config.SUBJECT_KEYWORD))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            if (subjectIri == null
                || subjectIri == ""
                || !Uri.IsWellFormedUriString(subjectIri.Trim(), UriKind.Absolute))
            {
                throw new ArgumentException("Subject IRI is not valid!");
            }
            paramString.SetUri(Config.SUBJECT_KEYWORD, new Uri(subjectIri));
        }

        public static void UnsetSubject(this SparqlParameterizedString paramString)
        {
            if (!paramString.CommandText.Contains('@' + Config.SUBJECT_KEYWORD))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            paramString.UnsetParameter(Config.SUBJECT_KEYWORD);
        }

        public static void SetPredicate(this SparqlParameterizedString paramString, string predicateIri)
        {
            if (!paramString.CommandText.Contains('@' + Config.PREDICATE_KEYWORD))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            if (predicateIri == null
                || predicateIri == ""
                || !Uri.IsWellFormedUriString(predicateIri.Trim(), UriKind.Absolute))
            {
                throw new ArgumentException("Predicate IRI is not valid!");
            }
            paramString.SetUri(Config.PREDICATE_KEYWORD, new Uri(predicateIri));
        }

        public static void UnsetPredicate(this SparqlParameterizedString paramString)
        {
            if (!paramString.CommandText.Contains('@' + Config.PREDICATE_KEYWORD))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            paramString.UnsetParameter(Config.PREDICATE_KEYWORD);
        }

        public static void SetLanguage(this SparqlParameterizedString paramString, string language)
        {
            if (!paramString.CommandText.Contains('@' + Config.LANG_VAL_KEYWORD))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            if (language == null || language == "")
            {
                throw new ArgumentException("Language is not valid!");
            }
            paramString.CommandText = paramString.CommandText.Replace('@' + Config.LANG_VAL_KEYWORD, language);
        }

        public static void SetLimit(this SparqlParameterizedString paramString, int limit)
        {
            if (!paramString.CommandText.Contains('@' + Config.LIMIT_VAL_KEYWORD))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            if (limit < 0)
            {
                throw new ArgumentException("Limit is not valid!");
            }
            paramString.CommandText = paramString.CommandText.Replace('@' + Config.LIMIT_VAL_KEYWORD, limit.ToString());
        }

        public static void SetPart(this SparqlParameterizedString paramString, string nameOfPart, string value)
        {
            if (!paramString.CommandText.Contains('@' + nameOfPart))
            {
                throw new InvalidOperationException("SparqlParameterizedString has an invalid form.");
            }
            var val = value ?? " ";
            paramString.CommandText = paramString.CommandText.Replace('@' + nameOfPart, val);
        }

    }
}
