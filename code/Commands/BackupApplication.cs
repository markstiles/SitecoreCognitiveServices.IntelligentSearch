using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Commands
{
    [Serializable]
    public class BackupApplication : BaseIntelligentSearchCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentSearch/BackupApplicatoin?id={Id}&language={Language}&db={Db}")
            {
                Header = "Backup Application",
                Height = "200",
                Width = "410",
                Message = "Backup Application",
                Response = true
            };
            SheerResponse.ShowModalDialog(mdo);
            args.WaitForPostBack();
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Settings.MissingKeys())
                return CommandState.Disabled;

            Item ctxItem = DataWrapper?.ExtractItem(context);
            if (ctxItem == null || ctxItem.ID.Guid != Settings.SearchProfileTemplateId)
                return CommandState.Hidden;

            return CommandState.Enabled;
        }
    }
}