using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Models.Sample;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Services;
using SitecoreCognitiveServices.Feature.IntelligentSearch.Statics;
using SitecoreCognitiveServices.Foundation.MSSDK.Bing.Models.AutoSuggest;
using SitecoreCognitiveServices.Foundation.MSSDK.Enums;
using SitecoreCognitiveServices.Foundation.MSSDK.Language.Models.Luis;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Bing;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Language.Models;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Speech;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentSearch.Areas.SitecoreCognitiveServices.Controllers
{
    public class IntelligentSearchController : Controller
    {
        #region Constructor

        protected readonly IWebUtilWrapper WebUtil;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentSearchSettings Settings;
        protected readonly ISearchService Searcher;
        protected readonly ILuisService LuisService;
        protected readonly ILuisConversationService LuisConversationService;
        protected readonly IAutoSuggestService AutoSuggestService;
        protected readonly ISpeechService SpeechService;
        protected readonly IWebSearchService WebSearchService;
        protected readonly IConversationContextFactory ConversationContextFactory;
        protected readonly ISetupService SetupService;

        public IntelligentSearchController (
            IWebUtilWrapper webUtil,
            ISitecoreDataWrapper dataWrapper,
            IIntelligentSearchSettings settings,
            ISearchService searcher,
            ILuisService luisService,
            ILuisConversationService luisConversationService,
            IAutoSuggestService autoSuggestService,
            ISpeechService speechService,
            IWebSearchService webSearchService,
            IConversationContextFactory conversationContextFactory,
            ISetupService setupService)
        {
            WebUtil = webUtil;
            DataWrapper = dataWrapper;
            Settings = settings;
            Searcher = searcher;
            LuisService = luisService;
            LuisConversationService = luisConversationService;
            AutoSuggestService = autoSuggestService;
            SpeechService = speechService;
            WebSearchService = webSearchService;
            ConversationContextFactory = conversationContextFactory;
            SetupService = setupService;
        }

        #endregion
        
        #region Search Form
        
        public ActionResult SearchFormPost(string id, string db, string language, string query)
        {
            var results = WebSearchService.WebSearch(query, 10, "en-US", SafeSearchOptions.Strict);
            var returnList = results?.WebPages?.Value?.Select(a => new SearchResult { Title = a.Name, Url = a.DisplayUrl, Description = a.Snippet }) ?? new List<SearchResult>();

            return Json(new
            {
                Failed = false,
                Items = returnList
            });
        }

        public ActionResult SearchFormLuisPost(string id, string db, string language, string query)
        {
            var appId = Settings.ApplicationId(Settings.IntelligentSearchItemId);
            if (appId == Guid.Empty)
                return Json(new { Failed = false });

            //call luis
            var luisResult = !string.IsNullOrWhiteSpace(query) ? LuisService.Query(appId, query, true) : null;
            var dialogPhrase = GetPhrase(luisResult, new List<string> { "Noun", "Adjective", "Verb", "Adverb", "Preposition" });
            var knowledgePanel = BuildKnowledgePanel(luisResult, dialogPhrase);
            var contextParams = new ItemContextParameters
            {
                Database = db,
                Id = id,
                Language = language
            };

            var conversationContext = ConversationContextFactory.Create(
                    appId,
                    Translator.Text("Chat.Accept"),
                    Translator.Text("Chat.Clear"),
                    Translator.Text("Chat.ConfirmMessage"),
                    "frustrated user",
                    query,
                    contextParams,
                    luisResult,
                    "quit");

            var response = LuisConversationService.HandleMessage(conversationContext);

            //return result
            return Json(new
            {
                Failed = false,
                KnowledgePanel = knowledgePanel,
                Response = response,
                SearchPhrase = GetPhrase(luisResult, new List<string> { "Noun", "Adjective" }),
                SpellCorrected = luisResult.AlteredQuery
            });
        }

        public string GetPhrase(LuisResult luisResult, List<string> types)
        {
            var filteredEntities = FilterEntityRecommendations(luisResult.Entities);

            var entities = filteredEntities
                .Where(a => types.Contains(a.Type))
                .Select(b => new SortEntity { Name = b.Entity, SortValue = GetSortValue(b.Type), StartIndex = b.StartIndex.Value })
                .OrderBy(c => c.SortValue)
                .ThenBy(d => d.StartIndex)
                .Select(e => e.Name);

            return string.Join(" ", entities).Replace(" ' ", "'");
        }

        public List<EntityRecommendation> FilterEntityRecommendations(IList<EntityRecommendation> entities)
        {
            var filteredEntities = new Dictionary<string, EntityRecommendation>();
            foreach (var e in entities)
            {
                if (filteredEntities.ContainsKey(e.Entity))
                {
                    if (filteredEntities[e.Entity].Score < e.Score)
                        filteredEntities[e.Entity] = e;
                }
                else
                {
                    filteredEntities.Add(e.Entity, e);
                }
            }

            return filteredEntities.Values.ToList();
        }

        public int GetSortValue(string type)
        {
            if (type.Equals("Adverb"))
                return 1;
            if (type.Equals("Verb"))
                return 2;
            if (type.Equals("Adjective"))
                return 3;
            if (type.Equals("Noun"))
                return 4;
            if (type.Equals("Preposition"))
                return 4;

            return 5;
        }

        public KnowledgePanelModel BuildKnowledgePanel(LuisResult luisResult, string phrase)
        {
            if (luisResult?.TopScoringIntent?.Score == null)
                return null;

            if (luisResult.TopScoringIntent.Score < 0.75)
                return null;

            var intent = luisResult.TopScoringIntent.Intent.ToLower();
            var panel = new KnowledgePanelModel();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var title = string.IsNullOrWhiteSpace(phrase) ? intent : phrase;

            if (intent == "information")
            {
                panel.Title = title;
                panel.Value = $"Here's the information you wanted to know about {title}";
                panel.LinkText = "Learn More";
                panel.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f6/Wikipedia-logo-v2-wordmark.svg/1200px-Wikipedia-logo-v2-wordmark.svg.png";
            }
            else if (intent == "location")
            {
                panel.Title = textInfo.ToTitleCase(title);
                panel.Value = $"Here's where to find {title} near you";
                panel.LinkText = "View on a Map";
                panel.ImageUrl = "http://cdn.ndtv.com/tech/images/gadgets/google-maps-sena-bhawan-635.jpg";
            }
            else if (intent == "event")
            {
                panel.Title = textInfo.ToTitleCase(title);
                panel.Value = $"Here's when the {title} is";
                panel.LinkText = "Add to Calendar";
                panel.ImageUrl = "https://cdn2.techadvisor.co.uk/cmsdata/features/3634463/how-to-open-ics-vcs-files-in-gcal-header_thumb800.png";
            }
            else if (intent == "purchase")
            {
                panel.Title = textInfo.ToTitleCase(title);
                panel.Value = $"Here's the {title} you wanted to purchase";
                panel.LinkText = "Add to Cart";
                panel.ImageUrl = "http://icons.iconarchive.com/icons/graphicloads/100-flat/256/cart-add-icon.png";
            }
            else
            {
                return null;
            }

            return panel;
        }
        
        public ActionResult GetSuggestions(string query)
        {
            var suggestions = AutoSuggestService.GetSuggestions(query);

            return Json(suggestions?.SuggestionGroups[0]?.SearchSuggestions ?? new List<AutoSuggestEntry>());
        }


        public class SortEntity
        {
            public string Name { get; set; }
            public int SortValue { get; set; }
            public int StartIndex { get; set; }
        }

        public class SearchResult
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
        }

        #endregion

        #region Intents

        public ActionResult SyncProfiles()
        {
            // first open setup, input form, save and sync
            // one modal with three tabs for adding profiles, adding phrases 
            // with sample search and an intent selector
            // and sync/train/publish
            Item curItem = null;

            // get anything
            var apps = LuisService.GetUserApplications();
            if (!apps.Any())
                return new EmptyResult();

            Guid appId = new Guid(apps.First().Id); // new Guid(curItem.Fields[Settings.ApplicationIdFieldId].Value);
            
            // get any information
            var appVersions = LuisService.GetApplicationVersions(appId).OrderByDescending(a => a.Version).ToList();
            
            // export what you found
            var export = LuisService.ExportApplicationVersion(appId, appVersions.First().Version);
            
            // delete profile
            LuisService.DeleteApplication(appId);

            // create profile
            var appRequest = new AddApplicationRequest()
            {
                Culture = "",
                Description = "",
                Domain = "",
                InitialVersionId = "",
                Name = "",
                UsageScenario = ""
            };
            var appResponse = LuisService.AddApplication(appRequest);
            appId = new Guid(appResponse);
            
            // get profile information
            //LuisService.GetApplicationInfo();

            // create intent
            var neRequest = new NamedEntityRequest()
            {
                Name = ""
            };
            Guid intentId = LuisService.CreateIntentClassifier(appId, "", neRequest);

            // get intent info
            var intentInfo = LuisService.GetIntentInfo(appId, "", intentId);
            var intents = LuisService.GetApplicationVersionIntentInfos(appId, "");

            // gets queries to improve entities
            var entityQueries = LuisService.SuggestEndpointQueriesForEntities(appId, "", intentId);
            // gets queries to improve intents
            var intentQueries = LuisService.SuggestEndpointQueriesForIntents(appId, "", intentId);
            // get all query history
            var queryLogs = LuisService.DownloadApplicationQueryLogs(appId);
            // order by lowest rated confidence and deal with those first
            // also flatten to dictionary then you can filter through known examples
            var examples = LuisService.ReviewLabeledExamples(appId, "");

            // add phrases
            var request = new AddLabelRequest()
            {
                EntityLabels = new[]
                {
                    new ApplicationLabel()
                    {
                        EndCharIndex = 0,
                        EntityName = "",
                        StartCharIndex = 0
                    },
                },
                IntentName = "",
                Text = ""
            };
            LuisService.AddLabel(appId, "", request);
            
            return new EmptyResult();
        }

        #endregion

        //#region Setup

        //public ActionResult Setup()
        //{
        //    if (!IsSitecoreUser())
        //        return LoginPage();

        //    var db = Sitecore.Configuration.Factory.GetDatabase(Settings.MasterDatabase);
        //    using (new DatabaseSwitcher(db))
        //    {
        //        ISetupInformation info = SetupFactory.Create();

        //        return View("Setup", info);
        //    }
        //}

        //public ActionResult SetupSubmit(bool overwriteOption, string luisApi, string luisApiEndpoint, string textAnalyticsApi, string textAnalyticsApiEndpoint)
        //{
        //    if (!IsSitecoreUser())
        //        return LoginPage();

        //    List<string> items = new List<string>();

        //    SetupService.SaveKeys(luisApi, luisApiEndpoint, textAnalyticsApi, textAnalyticsApiEndpoint);

        //    var restoreResult = SetupService.RestoreSampleSearch(overwriteOption);
        //    if(!restoreResult)
        //        items.Add("Restore Sample Search");

        //    var queryResult = SetupService.QuerySampleSearch();
        //    if(!queryResult)
        //        items.Add("Query Sample Search");

        //    SetupService.PublishSampleSearchContent();

        //    return Json(new
        //    {
        //        Failed = items.Count > 0,
        //        Items = string.Join(",", items)
        //    });
        //}

        //#endregion
        
        #region Backup

        public ActionResult BackupApplication()
        {
            var result = SetupService.BackupApplication();

            return View("BackupApplication", model: result);
        }

        #endregion

        #region Restore

        public ActionResult RestoreApplication()
        {
            var result = SetupService.RestoreApplication(true);

            return View("RestoreApplication", model: result);
        }

        #endregion
        
        #region Shared

        public bool IsSitecoreUser()
        {
            return DataWrapper.ContextUser.IsAuthenticated
                   && DataWrapper.ContextUser.Domain.Name.ToLower().Equals("sitecore");
        }

        public ActionResult LoginPage()
        {
            return new RedirectResult("/sitecore/login");
        }

        #endregion
    }
}