using SitecoreCognitiveServices.Feature.IntelligentSearch.Statics;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Enums;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Intents.Parameters
{
    public class EmailParameter : IRequiredParameter
    {
        #region Constructor

        public string ParamName { get; set; }
        protected string ParamMessage { get; set; }
        public string GetParamMessage(IConversation conversation) => ParamMessage;

        public IIntentInputFactory IntentInputFactory { get; set; }
        public IParameterResultFactory ResultFactory { get; set; }

        public EmailParameter(
            string paramName, 
            IIntentInputFactory inputFactory,
            IParameterResultFactory resultFactory)
        {
            ParamName = paramName;
            ParamMessage = Translator.Text("SearchForm.Parameters.EmailRequest");
            IntentInputFactory = inputFactory;
            ResultFactory = resultFactory;
        }

        #endregion

        public IParameterResult GetParameter(string paramValue, IConversationContext context)
        {
            try
            {
                MailAddress m = new MailAddress(paramValue);

                return ResultFactory.GetSuccess(paramValue, paramValue);
            }
            catch (FormatException) { }

            return ResultFactory.GetFailure(Translator.Text("SearchForm.Parameters.EmailValidationError"));
        }

        public IntentInput GetInput(ItemContextParameters parameters, IConversation conversation)
        {
            return IntentInputFactory.Create(IntentInputType.None);
        }
    }
}