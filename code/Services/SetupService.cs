using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.MSSDK;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Services
{
    public class SetupService : ISetupService
    {
        #region Constructor

        protected readonly IMicrosoftCognitiveServicesApiKeys MSCSApiKeys;
        protected readonly ILuisService LuisService;
        protected readonly IIntelligentSearchSettings Settings;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IContentSearchWrapper ContentSearch;
        protected readonly IPublishWrapper PublishWrapper;
        protected readonly HttpContextBase Context;
        protected readonly ISCSDKSettings SCSDKSettings;

        public SetupService(
            IMicrosoftCognitiveServicesApiKeys mscsApiKeys,
            ILuisService luisService,
            IIntelligentSearchSettings settings,
            ISitecoreDataWrapper dataWrapper,
            IContentSearchWrapper contentSearch,
            IPublishWrapper publishWrapper,
            HttpContextBase context,
            ISCSDKSettings scsdkSettings)
        {
            MSCSApiKeys = mscsApiKeys;
            LuisService = luisService;
            Settings = settings;
            Context = context;
            DataWrapper = dataWrapper;
            ContentSearch = contentSearch;
            PublishWrapper = publishWrapper;
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

        public bool BackupApplication()
        {
            //var apps = LuisService.GetApplicationVersions(Settings.OleApplicationId).OrderByDescending(a => a.Version).ToList();
            //if (!apps.Any())
            //    return false;
            
            //var export = LuisService.ExportApplicationVersion(Settings.OleApplicationId, apps.First().Version);
            //if (export == null)
            //    return false;

            //Item folderItem = DataWrapper.GetDatabase(Settings.MasterDatabase).GetItem(Settings.SettingsFolderId);
            //if (folderItem == null)
            //    return false;

            //Settings.ApplicationIdFieldId

            //DataWrapper.UpdateFields(folderItem, new Dictionary<ID, string>
            //{
            //    { Settings.ApplicationBackupFieldId, JsonConvert.SerializeObject(export) }        
            //});

            return true;
        }

        public bool RestoreApplication(bool overwrite)
        {
            //var jsonText = Settings.ApplicationBackup;
            //var appDefinition = JsonConvert.DeserializeObject<ApplicationDefinition>(jsonText);

            //var infoResponse = LuisService.GetUserApplications().FirstOrDefault(a => a.Name.Equals(appDefinition.Name));
            //bool shouldOverwrite = infoResponse != null && overwrite;
            //bool isNoApp = infoResponse == null;
            //bool hasAppId = !string.IsNullOrWhiteSpace(infoResponse?.Id);
            //if (shouldOverwrite)
            //    LuisService.DeleteApplication(new Guid(infoResponse.Id));

            //Guid appId;
            //if (shouldOverwrite || isNoApp)
            //{
            //    var importResponse = LuisService.ImportApplication(appDefinition, appDefinition.Name);
            //    if (!Guid.TryParse(importResponse, out appId))
            //        return false;

            //    Settings.OleApplicationId = appId;
            //}
            //else if (Settings.OleApplicationId == Guid.Empty && hasAppId)
            //{
            //    appId = Guid.Parse(infoResponse.Id);
            //    Settings.OleApplicationId = appId;
            //}
            //else
            //{
            //    appId = Settings.OleApplicationId;
            //}
                    
            //LuisService.TrainApplicationVersion(appId, appDefinition.VersionId);
            //int trainCount = 1;
            //int loopCount = 0;
            //var hasResponse = false;
            //do
            //{
            //    System.Threading.Thread.Sleep(1000);
                    
            //    var trainResponse = LuisService.GetApplicationVersionTrainingStatus(appId, appDefinition.VersionId);
            //    var statusList = trainResponse.Select(a => a.Details.Status).ToList();
            //    var anyFailed = statusList.Any(a => a.Equals("Fail"));
            //    var anyInProgress = statusList.Any(b => b.Equals("InProgress"));
            //    if (anyFailed)
            //    {
            //        if (trainCount > 3)
            //            return false;

            //        LuisService.TrainApplicationVersion(appId, appDefinition.VersionId);
            //        trainCount++;
            //    }
            //    else if (!anyInProgress) { 
            //        hasResponse = true;
            //    }

            //    if (loopCount > 100)
            //        return false;

            //    loopCount++;
            //}
            //while (!hasResponse);

            //PublishRequest pr = new PublishRequest()
            //{
            //    VersionId = appDefinition.VersionId,
            //    IsStaging = false,
            //    EndpointRegion = Settings.LuisPublishResource
            //};
            //var publishResponse = LuisService.PublishApplication(appId, pr);
            
            return true;
        }

        public bool QueryApplication()
        {
            //var response = LuisService.Query(Settings.OleApplicationId, Settings.TestMessage);
            //if (response == null)
            //    return false;
            
            return true;
        }

        public void PublishApplicationContent()
        {
            ////start at templates folder for yourself and core, and publish scs root in modules
            //List<ID> itemGuids = new List<ID>() {
            //    Settings.SCSDKTemplatesFolderId,
            //    Settings..OleTemplatesFolderId,
            //    Settings.SCSModulesFolderId
            //};
            
            //Database fromDb = DataWrapper.GetDatabase(Settings.MasterDatabase);
            //Database toDb = DataWrapper.GetDatabase(Settings.WebDatabase);
            //foreach (var g in itemGuids)
            //{
            //    var folder = fromDb.GetItem(g);

            //    PublishWrapper.PublishItem(folder, new[] { toDb }, new[] { folder.Language }, true, false, false);
            //}

            //Sitecore.Globalization.Translate.ResetCache(true);
        }
    }
}