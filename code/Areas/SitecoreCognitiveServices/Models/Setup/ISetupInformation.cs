namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Setup
{
    public interface ISetupInformation
    {
        string LuisApiKey { get; set; }
        string LuisApiEndpoint { get; set; }
        string TextAnalyticsApiKey { get; set; }
        string TextAnalyticsApiEndpoint { get; set; }
    }
}