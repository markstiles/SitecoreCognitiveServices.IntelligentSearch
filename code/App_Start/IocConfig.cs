using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Controllers;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Intents;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch
{
    public class IocConfig : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            //system
            serviceCollection.AddTransient<IIntelligentSearchSettings, IntelligentSearchSettings>();

            //services
            serviceCollection.AddTransient<ISearchService, SearchService>();

            //intents
            serviceCollection.AddTransient<IIntent, DefaultIntent>();
            serviceCollection.AddTransient<IIntent, FrustratedUserIntent>();
            serviceCollection.AddTransient<IIntent, RegistrationIntent>();
            serviceCollection.AddTransient<IIntent, ContactIntent>();
            serviceCollection.AddTransient<IIntent, QuitIntent>();

            //models
            //serviceCollection.AddTransient<ISetupInformation, SetupInformation>();

            //setup
            serviceCollection.AddTransient<ISetupService, SetupService>();

            serviceCollection.AddTransient(typeof(IntelligentSearchController));
        }
    }
}