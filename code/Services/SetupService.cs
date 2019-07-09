using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Data;
using SitecoreCognitiveServices.Foundation.MSSDK;
using SitecoreCognitiveServices.Foundation.SCSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Services
{
    public class SetupService : ISetupService
    {
        #region Constructor

        protected readonly IMicrosoftCognitiveServicesApiKeys MSCSApiKeys;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly ISCSDKSettings SCSDKSettings;

        public SetupService(
            IMicrosoftCognitiveServicesApiKeys mscsApiKeys,
            ISitecoreDataWrapper dataWrapper,
            ISCSDKSettings scsdkSettings)
        {
            MSCSApiKeys = mscsApiKeys;
            DataWrapper = dataWrapper;
            SCSDKSettings = scsdkSettings;
        }

        #endregion

        public void SaveKeys(string luisApi, string luisApiEndpoint)
        {
            //save items to fields
            if (MSCSApiKeys.Luis != luisApi)
                UpdateKey(SCSDKSettings.MSSDK_LuisFieldId, luisApi);
            if (MSCSApiKeys.LuisEndpoint != luisApiEndpoint)
                UpdateKey(SCSDKSettings.MSSDK_LuisEndpointFieldId, luisApiEndpoint);
        }

        public void UpdateKey(ID fieldId, string value)
        {
            var keyItem = DataWrapper?
                .GetDatabase(SCSDKSettings.MasterDatabase)
                .GetItem(SCSDKSettings.MSSDKId);
            DataWrapper.UpdateFields(keyItem, new Dictionary<ID, string>
            {
                { fieldId, value }
            });
        }
    }
}