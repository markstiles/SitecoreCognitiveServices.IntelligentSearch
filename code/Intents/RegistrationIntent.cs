using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Security;
using Sitecore.Data.Managers;
using Sitecore.Security.Accounts;
using Sitecore.Security.Domains;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Intents.Parameters;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Statics;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Enums;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents
{
    public class RegistrationIntent : BaseIntelligentSearchIntent
    {
        public override string KeyName => "registration";

        public override string DisplayName => "";

        public override bool RequiresConfirmation => false;
        
        #region Local Properties

        protected string UsernameKey = "Username";
        protected string PasswordKey = "Password";

        #endregion

        public RegistrationIntent(
            IIntentInputFactory inputFactory,
            IConversationResponseFactory responseFactory,
            IParameterResultFactory resultFactory,
            IIntelligentSearchSettings settings) : base(inputFactory, responseFactory, settings)
        {
            ConversationParameters.Add(new UsernameParameter(UsernameKey, inputFactory, resultFactory));
            ConversationParameters.Add(new PasswordParameter(PasswordKey, inputFactory, resultFactory));
        }

        public override ConversationResponse Respond(LuisResult result, ItemContextParameters parameters, IConversation conversation)
        {
            var username = (string)conversation.Data[UsernameKey];
            var password = (string)conversation.Data[PasswordKey];
            //CreateUser(Sitecore.Context.Domain, username, password);
            
            return ConversationResponseFactory.Create(KeyName, Translator.Text("SearchForm.Intents.Registration.Success"));
        }

        public void CreateUser(Domain domain, string email, string password)
        {
            try
            {
                var domainUser = $"{domain}\\{email}";
                User sitecoreUser = User.Create(domainUser, password);
                MembershipUser membershipUser = Membership.GetUser(domainUser);
                if (sitecoreUser == null || membershipUser == null)
                    return;

                membershipUser.IsApproved = false;
                Membership.UpdateUser(membershipUser);

                sitecoreUser.Profile.Email = email;
                sitecoreUser.Profile.Save();
            }
            catch (MembershipCreateUserException ex) { }
        }
    }
}