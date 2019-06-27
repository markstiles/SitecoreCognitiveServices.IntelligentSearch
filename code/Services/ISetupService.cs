using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Services
{
    public interface ISetupService
    {
        void SaveKeys(string luisApi, string luisApiEndpoint);
        void UpdateKey(ID fieldId, string value);
        bool BackupApplication();
        bool RestoreApplication(bool overwrite);
        bool QueryApplication();
        void PublishApplicationContent();
    }
}