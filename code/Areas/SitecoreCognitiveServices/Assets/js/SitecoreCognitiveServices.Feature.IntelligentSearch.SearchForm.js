jQuery.noConflict();

//search form
jQuery(document).ready(function () {
    //search
    var hi = "hi";
    var searchForm = ".search-form";
    var searchChat = searchForm + " .search-chat";
    var searchInput = searchForm + " .search-input";
    var formSubmit = searchForm + " .search-submit";
    var searchState = searchForm + " .search-state";
    var dialogState = searchForm + " .dialog-state";
    var dialogWrap = searchForm + " .dialog-wrap";
    var showClass = "show";
    var progressIndicator = ".progress-indicator";
    var resultFailure = ".result-failure";
    var knowledgePanel = ".knowledge-panel";
    var knowledgePanelInner = ".knowledge-panel-inner";
    var answerBox = ".answer-box";
    var answerBoxInner = ".answer-box-inner";
    var resultWrap = ".result-wrap";
    var searchResults = ".search-results";
    var searchNoResults = ".search-no-results";
    var searchSuggest = ".search-suggest";
    var suggestTimer;
    
    //sends chat text on 'enter-press' on the form
    jQuery(formSubmit).click(function(event) {
        event.preventDefault();

        var queryValue = jQuery(searchInput).val();
        if (queryValue === "")
            return;

        jQuery(searchInput).val("");
        
        GetDialogResult(queryValue);
    });

    jQuery(searchInput).keydown(function (e)
    {
        ClearSuggestState();
        clearTimeout(suggestTimer);
        
        if (e.which !== 13)
        {
            suggestTimer = setTimeout(function () { GetSuggestions(jQuery(searchInput).val()); }, 500);
            
            return;
        }
        
        e.preventDefault();
        jQuery(formSubmit).click();
    });

    jQuery(searchForm).click(function (e) {
        ClearSuggestState();
        clearTimeout(suggestTimer);
    });
    
    jQuery(searchState + " .cancel").click(function (event) {
        event.preventDefault();

        ClearSearchState();

        jQuery(progressIndicator).hide();

        jQuery(searchInput).focus();
    });

    jQuery(dialogState + " .cancel").click(function (event) {
        event.preventDefault();

        jQuery(searchChat).html("");

        ClearDialogState();

        GetDialogResult("cancel");

        jQuery(searchInput).focus();
    });
    
    function GetDialogResult(queryValue)
    {
        var idValue = jQuery(searchForm + " #id").val();
        var dbValue = jQuery(searchForm + " #db").val();
        var langValue = jQuery(searchForm + " #language").val();

        jQuery(searchChat).css('opacity', '0');
        jQuery(searchInput).attr("type", "text");

        jQuery.post(
            jQuery(searchForm + " form").attr("dialog-action"),
            {
                id: idValue,
                db: dbValue,
                language: langValue,
                query: queryValue
            }
        ).done(function (r) {
            ProcessDialogResult(queryValue, r);
        });
    }

    function ProcessDialogResult(queryValue, dialogResult)
    {
        ClearDialogState();

        var hasResponse = dialogResult.Response !== undefined && dialogResult.Response !== null;
        var isOver = !hasResponse || (hasResponse && dialogResult.Response.Ended);
        var isQuit = dialogResult.Response.Intent.indexOf("quit") > -1;
        var hasNoIntent = dialogResult.Response.Intent.length < 1;
        var spellCorrectedValue = dialogResult.SpellCorrected !== null && dialogResult.SpellCorrected.length > 0 ? dialogResult.SpellCorrected : queryValue; 
        var searchValue = dialogResult.SearchPhrase.length > 0 ? dialogResult.SearchPhrase : spellCorrectedValue;
        if (isOver && hasNoIntent)
            GetSearchResult(searchValue);

        if (dialogResult.Failed) {
            jQuery(resultFailure).show();
            return;
        }
        
        jQuery(resultWrap).show();
        
        var hasKnowledgePanel = dialogResult.KnowledgePanel !== undefined && dialogResult.KnowledgePanel !== null;
        
        if (hasResponse) {        
            jQuery(searchChat).html(dialogResult.Response.Message);
            if (dialogResult.Response.Message.length > 0)
                jQuery(searchChat).css('opacity', '1');

            if (dialogResult.Response.Input !== null)
                HandleInput(dialogResult.Response.Input);

            if (dialogResult.Response.Ended)
                setTimeout(function () { jQuery(searchChat).css('opacity', '0'); }, 2500);

            if (!dialogResult.Response.Ended && !isQuit) {
                jQuery(dialogState + " .value").html(dialogResult.Response.Intent);
                jQuery(dialogState).addClass(showClass);
                jQuery(dialogWrap).addClass(showClass);
            }
        }
        if (hasKnowledgePanel) {
            var knowHtml = "<div class='images'><img src='" + dialogResult.KnowledgePanel.ImageUrl + "' /></div>";
            knowHtml += "<div class='text'>";
            knowHtml += "<div class='title'>" + dialogResult.KnowledgePanel.Title + "</div>";
            knowHtml += "<div class='value'>" + dialogResult.KnowledgePanel.Value + "</div>";
            knowHtml += "</div>";
            knowHtml += "<div class='link'>" + dialogResult.KnowledgePanel.LinkText + "</div>";
            jQuery(knowledgePanelInner).html(knowHtml);
            jQuery(knowledgePanel).show();
        }

        jQuery(searchInput).focus();
    }

    function HandleInput(formInput) {
        if (formInput.InputType === "Password")
            jQuery(searchInput).attr("type", "password")
    }

    function ClearDialogState() {
        jQuery(searchState).removeClass(showClass);
        jQuery(searchResults).html("");
        jQuery(searchNoResults).hide();
        jQuery(answerBoxInner).html("");
        jQuery(answerBox).hide();
        jQuery(knowledgePanelInner).html("");
        jQuery(knowledgePanel).hide();
        jQuery(resultWrap).hide();
        jQuery(searchInput).val("");
        jQuery(dialogState).removeClass(showClass);
        jQuery(dialogWrap).removeClass(showClass);
        jQuery(dialogState + " .value").html("");
    }

    function GetSearchResult(queryValue) {
        var isInDialog = jQuery(dialogState + " .value").text().length > 0;
        var isEmptyRequest = queryValue === "";
        var isInitText = queryValue === hi;
        if (isInDialog || isEmptyRequest || isInitText)
            return;

        var idValue = jQuery(searchForm + " #id").val();
        var dbValue = jQuery(searchForm + " #db").val();
        var langValue = jQuery(searchForm + " #language").val();

        ClearSearchState();

        jQuery(searchState).addClass(showClass);
        jQuery(searchState + " .value").html(queryValue);

        jQuery.post(
            jQuery(searchForm + " form").attr("action"),
            {
                id: idValue,
                db: dbValue,
                language: langValue,
                query: queryValue
            }
        ).done(function (searchResult) {
            ProcessSearchResult(searchResult);
        }).always(function () {
            jQuery(progressIndicator).hide();
        });
    }

    function ProcessSearchResult(result) {
        if (result.Failed) {
            jQuery(resultFailure).show();
        }
        else {
            jQuery(resultWrap).show();

            if (result.Items.length > 0) {
                var searchHtml = "";
                for (var d = 0; d < result.Items.length; d++) {
                    searchHtml += "<div class='result-item' data-path='" + result.Items[d].Url + "'>";
                    searchHtml += "<div class='title'>" + result.Items[d].Title + "</div>";
                    searchHtml += "<a class='link' target='_blank' href='" + result.Items[d].Url + "'>" + result.Items[d].Url + "</a>";
                    searchHtml += "<div class='description'>" + result.Items[d].Description + "</div>";
                    searchHtml += "</div>";
                }

                jQuery(searchResults).html(searchHtml);
            }
            else {
                jQuery(searchNoResults).show();
            }
        }
    }
    
    function ClearSearchState()
    {
        jQuery(searchResults).html("");
        jQuery(searchNoResults).hide();
        jQuery(resultWrap).hide();
        jQuery(searchInput).val("");
        jQuery(searchState).removeClass(showClass);
        jQuery(searchState + " .value").html("");
        jQuery(progressIndicator).show();
    }

    function GetSuggestions(queryValue)
    {
        var isInDialog = jQuery(dialogState + " .value").text().length > 0;
        var isEmptyRequest = queryValue === "";
        var isInitText = queryValue === hi;
        if (isInDialog || isEmptyRequest || isInitText)
            return;
        
        jQuery.post(
            jQuery(searchForm + " form").attr("suggestion-action"),
            {
                query: queryValue
            }
        ).done(function (suggestions)
        {
            ProcessSuggestionResult(queryValue, suggestions);
        }).always(function () {
            jQuery(progressIndicator).hide();
        });
    }

    function ProcessSuggestionResult(queryValue, suggestions)
    {
        jQuery(searchSuggest).addClass(showClass);
        if (suggestions === null || suggestions.length < 1)
            return;

        var markup = "";
        for (var d = 0; d < suggestions.length; d++) {
            var suggestPart = suggestions[d].DisplayText.trim();
            var knownPart = "";
            if (suggestPart === queryValue.trim())
                continue;

            if (suggestPart.indexOf(queryValue) > -1) {
                var parts = suggestPart.split(queryValue);
                suggestPart = parts.length > 0 ? parts[1] : "";
                knownPart = queryValue;
                if (suggestPart === "")
                    continue;
            }

            markup += "<div class='option' data-value='" + suggestions[d].Query + "'>" + knownPart + "<span class='suggestion'>" + suggestPart + "</span></div>";
        }
        jQuery(searchSuggest).html(markup);
        jQuery(searchSuggest + " .option").on("click", function () {
            var value = jQuery(this).data("value");
            jQuery(searchInput).val(value);
            ClearSuggestState();
            jQuery(formSubmit).click();
        });
    }

    function ClearSuggestState()
    {
        jQuery(searchSuggest).html("");
        jQuery(searchSuggest).removeClass(showClass);
    }

    GetDialogResult(hi);
    jQuery(searchInput).focus();
});