namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class SetupInformation : ISetupInformation
    {
        public string LuisApiKey { get; set; }
        public string LuisApiEndpoint { get; set; }
        public string TextAnalyticsApiKey { get; set; }
        public string TextAnalyticsApiEndpoint { get; set; }
    }
}