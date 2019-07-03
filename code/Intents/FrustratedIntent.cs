using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents
{
    public class FrustratedIntent : BaseIntelligentSearchIntent
    {
        public override string KeyName => "frustrated";

        public override string DisplayName => "";

        public override bool RequiresConfirmation => false;

        public FrustratedIntent(
            IIntentInputFactory inputFactory,
            IConversationResponseFactory responseFactory,
            IIntelligentSearchSettings settings) : base(inputFactory, responseFactory, settings)
        {
        }

        public override ConversationResponse Respond(LuisResult result, ItemContextParameters parameters, IConversation conversation)
        {
            return ConversationResponseFactory.Create();
        }
    }
}