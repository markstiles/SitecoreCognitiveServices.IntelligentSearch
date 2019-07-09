using System;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.MSSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch
{
    public class IntelligentSearchSettings : IIntelligentSearchSettings
    {
        protected readonly IMicrosoftCognitiveServicesApiKeys MSApiKeys;
        protected ISitecoreDataWrapper DataWrapper;

        public IntelligentSearchSettings(IMicrosoftCognitiveServicesApiKeys msApiKeys, ISitecoreDataWrapper dataWrapper)
        {
            MSApiKeys = msApiKeys;
            DataWrapper = dataWrapper;
        }
        
        #region Globally Shared Settings

        public virtual string CoreDatabase => Settings.GetSetting("CognitiveService.CoreDatabase");
        public virtual string MasterDatabase => Settings.GetSetting("CognitiveService.MasterDatabase");
        public virtual string WebDatabase => Settings.GetSetting("CognitiveService.WebDatabase");
        public virtual ID SCSDKTemplatesFolderId => new ID(Settings.GetSetting("CognitiveService.SCSDKTemplatesFolder"));
        public virtual ID SCSModulesFolderId => new ID(Settings.GetSetting("CognitiveService.SCSModulesFolder"));

        #endregion

        public virtual string DictionaryDomain => Settings.GetSetting("CognitiveService.IntelligentSearch.DictionaryDomain");
        public virtual Guid SearchProfileTemplateId => new Guid(Settings.GetSetting("CognitiveService.IntelligentSearch.SearchProfileTemplateId"));
        public virtual ID IntelligentSearchRootId => new ID(Settings.GetSetting("CognitiveService.IntelligentSearch.IntelligentSearchRootId"));
        public virtual ID IntelligentSearchItemId => new ID(Settings.GetSetting("CognitiveService.IntelligentSearch.IntelligentSearchItemId"));
        public virtual ID ApplicationIdFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentSearch.ApplicationIdFieldId"));
        public virtual ID ApplicationBackupFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentSearch.ApplicationBackupFieldId"));
        public virtual Guid ApplicationId(ID itemId)
        {
            var value = DataWrapper.GetItemById(itemId, MasterDatabase)?.Fields[ApplicationIdFieldId]?.Value;

            return string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out Guid g)
                ? Guid.Empty 
                : g;
        } 

        public bool HasNoValue(string str)
        {
            return string.IsNullOrWhiteSpace(str.Trim());
        }
        public bool MissingKeys()
        {
            if (MSApiKeys == null)
                return true;

            return HasNoValue(MSApiKeys.LuisEndpoint)
                   || HasNoValue(MSApiKeys.Luis)
                   || HasNoValue(MSApiKeys.TextAnalyticsEndpoint)
                   || HasNoValue(MSApiKeys.TextAnalytics);
        }
    }
}