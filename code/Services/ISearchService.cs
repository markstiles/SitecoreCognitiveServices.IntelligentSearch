using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch.SearchTypes;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Services
{
    public interface ISearchService
    {
        List<SearchResultItem> GetResults(string query);
    }
}