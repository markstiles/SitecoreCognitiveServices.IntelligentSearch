namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class SetupInformationViewModel
    {
        public string LuisApiKey { get; set; }
        public string LuisApiEndpoint { get; set; }

        public SetupInformationViewModel(string luisApiKey, string luisApiEndpoint)
        {
            LuisApiKey = luisApiKey;
            LuisApiEndpoint = luisApiEndpoint;
        }
    }
}