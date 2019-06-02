using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents
{
    public abstract class BaseIntelligentSearchIntent : IIntent
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract bool RequiresConfirmation { get; }
        
        public abstract ConversationResponse Respond(LuisResult result, ItemContextParameters parameters, IConversation conversation);

        #region Base Intent

        protected readonly IIntelligentSearchSettings Settings;
        protected readonly IConversationResponseFactory ConversationResponseFactory;
        protected readonly IIntentInputFactory IntentInputFactory;

        public virtual Guid ApplicationId => Settings.ApplicationId(Settings.IntelligentSearchItemId);

        public virtual List<IRequiredConversationParameter> ConversationParameters { get; }
        
        protected BaseIntelligentSearchIntent(
            IIntentInputFactory inputFactory,
            IConversationResponseFactory responseFactory,
            IIntelligentSearchSettings settings)
        {
            Settings = settings;
            ConversationResponseFactory = responseFactory;
            IntentInputFactory = inputFactory;
            ConversationParameters = new List<IRequiredConversationParameter>();
        }
        
        #endregion
    }
}