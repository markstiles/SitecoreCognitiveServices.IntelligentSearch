namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class SetupInformation
    {
        public string LuisApiKey { get; set; }
        public string LuisApiEndpoint { get; set; }

        public SetupInformation(string luisApiKey, string luisApiEndpoint)
        {
            LuisApiKey = luisApiKey;
            LuisApiEndpoint = luisApiEndpoint;
        }
    }
}