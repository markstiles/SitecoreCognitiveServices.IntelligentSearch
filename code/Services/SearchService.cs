using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.Utilities;
using SitecoreCognitiveServices.Foundation.MSSDK.Bing.Models.ImageSearch;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Services
{
    public class SearchService : ISearchService
    {
        #region Constructor 

        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentSearchSettings Settings;
        protected readonly IContentSearchWrapper ContentSearch;

        public SearchService(
            ISitecoreDataWrapper dataWrapper,
            IIntelligentSearchSettings settings,
            IContentSearchWrapper contentSearch)
        {
            DataWrapper = dataWrapper;
            Settings = settings;
            ContentSearch = contentSearch;
        }

        #endregion

        public virtual List<SearchResultItem> GetResults(string query)
        {
            var indexName = ContentSearch.GetSitecoreIndexName("master");
            var index = ContentSearch.GetIndex(indexName);
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                var contentPredicate = PredicateBuilder.False<SearchResultItem>();

                if (query.Contains(" "))
                {
                    var words = query.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    words.ForEach(w => contentPredicate = contentPredicate
                        .Or(item => item.Content.Contains(w)));
                }
                else
                {
                    contentPredicate = contentPredicate.Or(item => item.Content.Contains(query));
                }

                return context.GetQueryable<SearchResultItem>()
                    .Where(a => a.Paths.Contains(Sitecore.ItemIDs.ContentRoot))
                    .Where(contentPredicate)
                    .Take(10)
                    .ToList();
            }
        }
    }
}