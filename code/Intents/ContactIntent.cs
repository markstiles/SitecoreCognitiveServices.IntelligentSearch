using System;
using System.Collections.Generic;
using System.Net.Mail;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Intents.Parameters;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Statics;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents
{
    public class ContactIntent : BaseIntelligentSearchIntent
    {
        public override string KeyName => "contact";

        public override string DisplayName => "";

        public override bool RequiresConfirmation => false;
        
        #region Local Properties

        protected string NameKey = "Name";
        protected string EmailKey = "Email";
        protected string MessageKey = "Message";

        #endregion

        public ContactIntent(
            IIntentInputFactory inputFactory,
            IConversationResponseFactory responseFactory,
            IParameterResultFactory resultFactory,
            IIntelligentSearchSettings settings) : base(inputFactory, responseFactory, settings)
        {
            ConversationParameters.Add(new NameParameter(NameKey, inputFactory, resultFactory));
            ConversationParameters.Add(new EmailParameter(EmailKey, inputFactory, resultFactory));
            ConversationParameters.Add(new MessageParameter(MessageKey, inputFactory, resultFactory));
        }

        public override ConversationResponse Respond(LuisResult result, ItemContextParameters parameters, IConversation conversation)
        {
            var name = (string)conversation.Data[NameKey];
            var email = (string)conversation.Data[EmailKey];
            var message = (string)conversation.Data[MessageKey];
            //send email

            return ConversationResponseFactory.Create(KeyName, Translator.Text("SearchForm.Intents.Contact.Success"));
        }
    }
}