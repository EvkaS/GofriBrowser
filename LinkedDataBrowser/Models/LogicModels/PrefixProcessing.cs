using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace LinkedDataBrowser.Models.LogicModels
{
    public static class PrefixProcessing
    {
        /// <summary>
        /// Converts IRI to prefixed expression or returns original text representation of a node if it is not IRI or the prefix is unknown
        /// </summary>
        public static string TryConvertToPrefixedExpression(string wholeIri)
        {            
            EnsureFreshPrefixDictionary();
            if(!dictionaryIsValid)
                return wholeIri;
        //finding where to cut
        int last = wholeIri.LastIndexOf("#");
            if (last == -1)
                last = wholeIri.LastIndexOf("/");
            if (last == -1)
                return wholeIri;
            //cut
            char[] pathArray = new char[last + 1];
            char[] sufixArray = new char[wholeIri.Length - (last + 1)];
            wholeIri.CopyTo(0, pathArray, 0, pathArray.Length);
            wholeIri.CopyTo(last + 1, sufixArray, 0, sufixArray.Length);
            string path = new string(pathArray);
            string sufix = new string(sufixArray);

            string prefix;
            bool ok = prefixMapping.TryGetValue(path, out prefix);
            if (ok)
                return prefix + ':' + WebUtility.UrlDecode(sufix);
            else
                return WebUtility.UrlDecode(wholeIri);
        }

        private static void EnsureFreshPrefixDictionary()
        {
            if (DateTime.Now - lastDownload > TimeSpan.FromDays(Config.PREFIXES_UPDATE_INTERVAL_DAYS))
            {
                DownloadPrefixDictionary();
            }
        }

        private static void DownloadPrefixDictionary()
        {
            WebClient client = new WebClient();
            Stream stream = null;
            try
            {
                stream = client.OpenRead(Config.LIST_OF_PREFIXES_URL);
            }
            catch (WebException)
            {
                return;
            }
            StreamReader reader = new StreamReader(stream);
            var newDictionary = new Dictionary<string, string>();
            string line;
            while (true)
            {
                try
                {
                    line = reader.ReadLine();
                }
                catch (Exception) //TODO: dlhodobu OutOfMemoryException by trebalo nejak riesit
                {
                    return;
                }

                if (line == null) break;
                var tokens = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);

                newDictionary.TryAdd(tokens[1], tokens[0]);
            }
            prefixMapping = newDictionary;
            dictionaryIsValid = true;
            lastDownload = DateTime.Now;
        }

        private static Dictionary<string, string> prefixMapping;
        private static bool dictionaryIsValid = false;
        private static DateTime lastDownload = new DateTime(0);
    }
}
