using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Statics;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents
{
    public class QuitIntent : BaseIntelligentSearchIntent
    {
        public override string Name => "quit";

        public override string Description => "";

        public override bool RequiresConfirmation => false;

        public QuitIntent(
            IIntentInputFactory inputFactory,
            IConversationResponseFactory responseFactory,
            IIntelligentSearchSettings settings) : base(inputFactory, responseFactory, settings)
        {
        }

        public override ConversationResponse Respond(LuisResult result, ItemContextParameters parameters, IConversation conversation)
        {
            return ConversationResponseFactory.Create(Name, Translator.Text("SearchForm.Intents.Quit.Response"));
        }
    }
}