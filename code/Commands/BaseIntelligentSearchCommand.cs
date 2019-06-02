using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Commands
{
    public class BaseIntelligentSearchCommand : BaseCommand
    {
        protected readonly IIntelligentSearchSettings Settings;
        
        public BaseIntelligentSearchCommand()
        {
            Settings = DependencyResolver.Current.GetService<IIntelligentSearchSettings>();
        }
    }
}