using System;
using Sitecore.Data;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch {
    public interface IIntelligentSearchSettings
    {
        string CoreDatabase { get; }
        string MasterDatabase { get; }
        string WebDatabase { get; }
        ID SCSDKTemplatesFolderId { get; }
        ID SCSModulesFolderId { get; }
        Guid SearchProfileTemplateId { get; }
        ID IntelligentSearchItemId { get; }
        ID ApplicationIdFieldId { get; }
        Guid ApplicationId(ID itemId);
        bool HasNoValue(string str);
        bool MissingKeys();
    }
}