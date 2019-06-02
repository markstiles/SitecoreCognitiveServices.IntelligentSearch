using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents
{
    public class FrustratedUserIntent : BaseIntelligentSearchIntent
    {
        public override string Name => "frustrated user";

        public override string Description => "";

        public override bool RequiresConfirmation => false;

        public FrustratedUserIntent(
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