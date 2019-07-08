using System.Collections.Generic;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class SyncApplications
    {
        public Dictionary<string, string> Applications { get; set; }

        public SyncApplications(Dictionary<string, string> applications)
        {
            Applications = applications;
        }
    }
}