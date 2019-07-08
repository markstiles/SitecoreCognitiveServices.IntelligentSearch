using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Services
{
    public interface ISetupService
    {
        void SaveKeys(string luisApi, string luisApiEndpoint);
        void UpdateKey(ID fieldId, string value);
        List<UserApplication> GetApplications();
        bool BackupApplication();
        bool RestoreApplication(bool overwrite);
        bool QueryApplication();
        void PublishApplicationContent();
    }
}