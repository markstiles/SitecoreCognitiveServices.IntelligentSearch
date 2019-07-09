using System.Collections.Generic;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class QueryApplicationViewModel
    {
        public string AppId { get; set; }

        public QueryApplicationViewModel(string appId)
        {
            AppId = appId;
        }
    }
}